using System;
using System.Collections;
using System.Threading.Tasks;
using Camera;
using DG.Tweening;
using PlayerMovement.PlayerData;
using UnityEngine;

namespace PlayerMovement
{
    [RequireComponent(typeof(PlayerController), typeof(PlayerInput), typeof(PlayerCamera))]
    public class Player : MonoBehaviour
    {
        public event Action<float> OnGravityChanged;

        [SerializeField] private PlayerDataSO playerData;

        private float _gravity;
        private float _maxJumpVelocity;
        private float _minJumpVelocity;
        private float _velocityXSmoothing;
        private float _velocitySlideXSmoothing;
        private float _jumpBufferCurrentTime;
        private float _coyoteTimeCounter;

        private Vector2 _velocity;
        private Vector2 _input;

        private bool _wasJumpPressed;
        private bool _wasJumpReleased;
        private bool _startBufferTimer;
        private bool _isClimbing;
        private bool _wasGrounded;
        
        private PlayerController _playerController;
        private PlayerInput _playerInput;
        private PlayerCamera _playerCamera;
        private Sequence _climbSequence;

        public Vector2 Velocity => _velocity;

        public bool WasGrounded
        {
            get => _wasGrounded;
            set => _wasGrounded = value;
        }

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerInput = GetComponent<PlayerInput>();
            _playerCamera = GetComponent<PlayerCamera>();

            _playerController.OnLedgeDetected += HandleLedgeClimb;

            JumpVariableSetup();
        }

        private void Update()
        {         
            if (!_isClimbing)
            {
                _input = _playerInput.MoveInput;
            }
            
            if (_playerInput.WasJumpPressed)
            {
                // if(_playerInput.WasJumpPressed && !_playerController.GetCollisionInfo.Bottom)
                //
                _startBufferTimer = true;
                _jumpBufferCurrentTime = 0f;

                _wasJumpPressed = true;
                _playerInput.OnPlayerConsumeJump();
            }

            if (_playerInput.WasJumpReleased)
            {
                _wasJumpReleased = true;
                _playerInput.OnPlayerConsumeJumpRelease();
            }

            if (_startBufferTimer)
            {
                _jumpBufferCurrentTime += Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            HandleCoyoteTime();
            HandleJumpLogic();
            OnPlayerMovement(_input);
            ClampVelocity();
            
            _wasGrounded = _playerController.GetCollisionInfo.Bottom || _playerController.GetCollisionInfo.Top;
        }

        private void LateUpdate()
        {
            HandleCameraLookingUpAndDown(_input);
        }

        private void OnDestroy()
        {
            if (_playerController)
            {
                _playerController.OnLedgeDetected -= HandleLedgeClimb;
            }
            
            _climbSequence?.Kill();
        }

        private void OnMaxJump()
        {
            bool canJump = _coyoteTimeCounter > 0.02f;
            
            if (_wasJumpPressed && !_wasJumpReleased && canJump)
            {
                _velocity.y = _maxJumpVelocity;
            }
        }

        //Maybe add coyote time to this too? 
        private void OnMaxJumpAfterBuffer()
        {
            _velocity.y = _maxJumpVelocity;
        }

        private void OnVariableJump()
        {
            if (_wasJumpReleased && !_wasJumpPressed)
            {
                if (_velocity.y > _minJumpVelocity)
                {
                    _velocity.y = _minJumpVelocity;
                }
            }
        }

        // private IEnumerator JumpBufferCoroutine()
        // {
        //     while (!_playerController.GetCollisionInfo.Bottom && )
        //     {
        //         
        //     }
        // }

        private void HandleLedgeClimb(float height, float distance)
        {
            if (!_isClimbing && !_wasGrounded)
            {
                _ = ClimbLedgeAsync(height, distance);
            }
        }
        
        private async Task ClimbLedgeAsync(float targetHeight, float targetDistance)
        {
            _isClimbing = true;
            
            float previousHorizontalVelocity =  _velocity.x;
            _velocity.y = 0f;
            
            _climbSequence?.Kill();

            Vector2 startPos = transform.position;
            Vector2 verticalTarget = startPos + Vector2.up * targetHeight;
            Vector2 finalTarget = verticalTarget + Vector2.right * targetDistance;

            try
            {
                _climbSequence = DOTween.Sequence();

                _climbSequence.Append(transform.DOMoveY(verticalTarget.y, playerData.climbVerticalDuration).SetEase(playerData.climbVerticalEase));

                float horizontalDelay = playerData.climbVerticalDuration * playerData.horizontalStartPercent;
                _climbSequence.Insert(horizontalDelay,transform.DOMoveX(finalTarget.x, playerData.climbHorizontalDuration).SetEase(playerData.climbHorizontalEase));
                
                while (_climbSequence != null && _climbSequence.IsActive() && !_climbSequence.IsComplete())
                {
                    await Task.Yield();
                }

                transform.position = finalTarget;
                _velocity.x = previousHorizontalVelocity;
            }
            finally
            {
                _climbSequence = null;
                _isClimbing = false;
            }
        }
        
        private void HandleCoyoteTime()
        {
            bool isGrounded = _playerController.GetCollisionInfo.Bottom;

            if (isGrounded)
            {
                _coyoteTimeCounter = playerData.coyoteTime; 
            }
            else
            {
                _coyoteTimeCounter = Mathf.Max(0, _coyoteTimeCounter - Time.fixedDeltaTime);
            }
        }

        private void HandleJumpLogic()
        {
            // Player landed on ground
            if (_playerController.GetCollisionInfo.Bottom || _playerController.GetCollisionInfo.Top)
            {
                _velocity.y = 0;
                
                //check if there is a jump buffered
                if (_startBufferTimer && _jumpBufferCurrentTime <= playerData.jumpBufferMaxTime)
                {                    
                    //Debug.Log($"player clicked space {jumpBufferCurrentTime} seconds before landing");
                    
                    OnMaxJumpAfterBuffer();
                    OnVariableJump();
                    _wasJumpPressed = false;
                }

                _startBufferTimer = false;
                _jumpBufferCurrentTime = 0f;
            }

            OnMaxJump();
            OnVariableJump();

            _wasJumpPressed = false;
            _wasJumpReleased = false;
        }

        private void HandleCameraLookingUpAndDown(Vector2 input)
        {
            if (Mathf.Approximately(Mathf.Sign(input.y), 1) && input.y >= 1)
            {
                _playerCamera.MoveCameraUp();
            }
            else if (Mathf.Approximately(Mathf.Sign(input.y), -1))
            {
                _playerCamera.MoveCameraDown();
            }
            else
            {
                _playerCamera.ReturnCameraToOriginalPosition();
            }
        }

        private void ClampVelocity()
        {
            _velocity.y = Mathf.Clamp(_velocity.y,-playerData.maxFallSpeed, playerData.maxFallSpeed );
            _velocity.x = Mathf.Clamp(_velocity.x,-playerData.maxMoveSpeed, playerData.maxMoveSpeed );
        }

        private void OnPlayerMovement(Vector2 input)
        {
            if (_isClimbing)
            {
                return;
            }
            
            float targetVelocityX = input.x * playerData.moveSpeed;
            float atApexPosition = 0;
            
            if (!_playerController.GetCollisionInfo.Bottom)
            {
                atApexPosition = Mathf.InverseLerp(0, playerData.apexThreshold, Mathf.Abs(_velocity.y));
                float atApexVelocityBonus = 1f + (atApexPosition * playerData.atApexSpeedBonus);
                targetVelocityX *= atApexVelocityBonus;
            }

            float accelerateOrDecelerate;
                
            if (_playerController.GetCollisionInfo.Bottom)
            {
                accelerateOrDecelerate = (Mathf.Abs(input.x) > 0.01f) ? playerData.accelerationOnGround : playerData.decelerationOnGround;
                _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerateOrDecelerate);
            }
            else
            {
                accelerateOrDecelerate = (Mathf.Abs(input.x) > 0.01f) ? playerData.accelerationInAir : playerData.decelerationInAir;
                _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerateOrDecelerate);
            }
            
            float currentGravity = _gravity;
            if (!_playerController.GetCollisionInfo.Bottom)
            {
                currentGravity = Mathf.Lerp(_gravity * playerData.apexGravityMultiplier, _gravity, atApexPosition);
            }

            OnGravityChanged?.Invoke(currentGravity);

            _velocity.y += currentGravity * Time.fixedDeltaTime;
            
            // if(_velocity.y <= maxFallSpeed)
            //     _velocity.y += currentGravity * Time.fixedDeltaTime; 
            // else
            //     _velocity.y = maxFallSpeed;
            
            _playerController.Displacement(_velocity * Time.fixedDeltaTime);
        }

        private void JumpVariableSetup()
        {
            _gravity = -(2 * playerData.maxJumpHeight) / Mathf.Pow(playerData.timeToJumpApex, 2);
            _maxJumpVelocity = Mathf.Abs(_gravity) * playerData.timeToJumpApex;
            _minJumpVelocity = Mathf.Sqrt(2 *  Mathf.Abs(_gravity) * playerData.minJumpHeight);
        }

        private void OnValidate()
        {
            JumpVariableSetup();
        }

        private void OnDrawGizmos()
        {
            //Draw Velocity indicator line
            
            Gizmos.color = Color.green;
            
            Vector2 start = transform.position;
            Vector2 end = start + _velocity * 0.2f;
            
            Gizmos.DrawLine(start, end);
        }
    }
}


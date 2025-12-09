using System;
using System.Collections;
using System.Threading.Tasks;
using PlayerMovement.PlayerData;
using UnityEngine;

namespace PlayerMovement
{
    [RequireComponent(typeof(PlayerController), typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        public event Action<float> OnGravityChanged;

        [SerializeField] private PlayerDataSO playerData;

        private float _gravity;
        private float _maxJumpVelocity;
        private float _minJumpVelocity;
        private float _velocityXSmoothing;
        private float _jumpBufferCurrentTime;

        private Vector2 _velocity;

        private bool _wasJumpPressed;
        private bool _wasJumpReleased;
        private bool _startBufferTimer;

        private PlayerController _playerController;
        private PlayerInput _playerInput;

        public Vector2 Velocity => _velocity;

        void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerInput = GetComponent<PlayerInput>();

            JumpVariableSetup();
        }

        private void Update()
        {
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
            Vector2 input = _playerInput.MoveInput;

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

            OnPlayerMovement(input);
        }

        private void OnMaxJump()
        {
            if (_wasJumpPressed && !_wasJumpReleased && _playerController.GetCollisionInfo.Bottom)
            {
                _velocity.y = _maxJumpVelocity;
            }
        }

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

        private void OnPlayerMovement(Vector2 input)
        {            
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
            _velocity.y = Mathf.Clamp(_velocity.y,-playerData.maxFallSpeed, playerData.maxFallSpeed );
            
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
    }
}


using System;
using UnityEngine;

namespace PlayerMovement
{
    [RequireComponent( typeof(PlayerController), typeof(PlayerInput) )]
    public class Player : MonoBehaviour
    {
        [Header("[Jump]")]
        [Range(0f, 100f)]
        [SerializeField] private float maxJumpHeight = 4f;
        [Range(0f, 100f)]
        [SerializeField] private float minJumpHeight = 1f;
        [Range(0f, 10f)]
        [SerializeField] private float timeToJumpApex = 0.4f;
        [Header("[Movement]")]
        [Range(0f, 100f)]
        [SerializeField] private float moveSpeed = 5f;
        [Header("[Acceleration/Deceleration]")]
        [Range(0f, 100f)]
        [SerializeField] private float accelerationOnGround = 0.1f;
        [Range(0f, 100f)]
        [SerializeField] private float accelerationInAir = 0.2f;
        [Range(0f, 100f)]
        [SerializeField] private float decelerationOnGround = 0.1f;
        [Range(0f, 100f)]
        [SerializeField] private float decelerationInAir = 0.2f;

        private float _gravity;
        private float _maxJumpVelocity;
        private float _minJumpVelocity;
        private float _velocityXSmoothing;
        
        private Vector2 _velocity;
        
        private bool _wasJumpPressed;
        private bool _wasJumpReleased;
        
        private PlayerController _playerController;
        private PlayerInput _playerInput;
        
        void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerInput =  GetComponent<PlayerInput>();

            JumpVariableSetup();
        }

        private void Update()
        {
            if (_playerInput.WasJumpPressed)
            {
                _wasJumpPressed =  true;
                _playerInput.OnPlayerConsumeJump();
            }

            if (_playerInput.WasJumpReleased)
            {
                _wasJumpReleased  = true;
                _playerInput.OnPlayerConsumeJumpRelease();
            }
        }

        private void FixedUpdate()
        {
            Vector2 input = _playerInput.MoveInput;

            //No idea how to call that
            if (_playerController.GetCollisionInfo.Bottom || _playerController.GetCollisionInfo.Top)
            {
                _velocity.y = 0;
            }

            OnMaxJump();
            OnVariableJump();
            
            _wasJumpPressed = false;
            _wasJumpReleased  = false;

            OnPlayerMovement(input);
        }

        private void OnMaxJump()
        {
            if(_wasJumpPressed && !_wasJumpReleased && _playerController.GetCollisionInfo.Bottom)
            {
                _velocity.y = _maxJumpVelocity;
            }
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

        private void OnPlayerMovement(Vector2 input)
        {
            float targetVelocityX = input.x * moveSpeed;
            float accelerateOrDecelerate;

            if (_playerController.GetCollisionInfo.Bottom)
            {
                accelerateOrDecelerate = (Mathf.Abs(input.x) > 0.01f) ? accelerationOnGround : decelerationOnGround;
                _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerateOrDecelerate);
            }
            else
            {
                accelerateOrDecelerate = (Mathf.Abs(input.x) > 0.01f) ? accelerationInAir : decelerationInAir;
                _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, accelerateOrDecelerate);
            }

            _velocity.y += _gravity * Time.fixedDeltaTime; 
            
            _playerController.Displacement(_velocity * Time.fixedDeltaTime);
        }

        private void JumpVariableSetup()
        {
            _gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            _maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
            _minJumpVelocity = Mathf.Sqrt(2 *  Mathf.Abs(_gravity) * minJumpHeight);
        }
    }
}


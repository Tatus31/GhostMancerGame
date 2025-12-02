using System;
using UnityEngine;

namespace PlayerMovement
{
    [RequireComponent( typeof(PlayerController), typeof(PlayerInput) )]
    public class Player : MonoBehaviour
    {
        [Header("Jump")]
        [Range(0f, 100f)]
        [SerializeField] private float jumpHeight = 4f;
        [Range(0f, 10f)]
        [SerializeField] private float timeToJumpApex = 0.4f;
        [Header("Movement")]
        [Range(0f, 100f)]
        [SerializeField] private float moveSpeed = 5f;
        [Range(0f, 100f)]
        [SerializeField] private float accelerationOnGround = 0.1f;
        [Range(0f, 100f)]
        [SerializeField] private float accelerationInAir = 0.2f;

        private float _gravity;
        private float _jumpVelocity;
        private float _velocityXsmoothing;
        
        private Vector2 _velocity;
        
        private bool _wasJumpPressed;
        
        private PlayerController _playerController;
        private PlayerInput _playerInput;
        
        void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerInput =  GetComponent<PlayerInput>();
            
            _gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            _jumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
        }

        private void Update()
        {
            if (_playerInput.WasJumpPressed)
            {
                _wasJumpPressed =  true;
                _playerInput.OnPlayerConsumeJump();
            }
        }

        private void FixedUpdate()
        {
            Vector2 input = _playerInput.MoveInput;

            if (_playerController.GetCollisionInfo.Bottom ||  _playerController.GetCollisionInfo.Top)
            {
                _velocity.y = 0;
            }
            
            if(_wasJumpPressed && _playerController.GetCollisionInfo.Bottom)
            {
                _velocity.y = _jumpVelocity;
            }
            _wasJumpPressed = false;
            
            float targetVelocityX = input.x * moveSpeed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXsmoothing, _playerController.GetCollisionInfo.Bottom ? accelerationOnGround : accelerationInAir);
            _velocity.y += _gravity * Time.fixedDeltaTime; 
            
            _playerController.Displacement(_velocity * Time.fixedDeltaTime);
        }
    }
}


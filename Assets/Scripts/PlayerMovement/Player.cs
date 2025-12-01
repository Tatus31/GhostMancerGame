using System;
using UnityEngine;

namespace PlayerMovement
{
    [RequireComponent( typeof(PlayerController) )]
    public class Player : MonoBehaviour
    {
        [Range(0f, 100f)]
        [SerializeField] private float gravity = 20f;
        [Range(0f, 100f)]
        [SerializeField] private float moveSpeed = 5f;
        
        private Vector2 _velocity;
        
        PlayerController _playerController;
        void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }

        private void FixedUpdate()
        {            
            Vector2 input = PlayerInput.GetMovementInput();
            
            _velocity.x = input.x * moveSpeed;
            _velocity.y += -gravity * Time.deltaTime; 
            
            _playerController.Displacement(_velocity * Time.deltaTime);
        }
    }
}


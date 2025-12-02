using System;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerMovement
{
    public class PlayerInput : MonoBehaviour
    {        
        public bool WasJumpPressed { get; private set; }
        public Vector2 MoveInput { get; private set; }
        
        private InputSystem_Actions _inputSystemActions;
        
        private void Awake()
        {
            _inputSystemActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputSystemActions ??= new InputSystem_Actions();
            _inputSystemActions.Enable();

            _inputSystemActions.Player.Move.performed += OnPlayerMove;
            _inputSystemActions.Player.Move.canceled += OnPlayerMove;
            _inputSystemActions.Player.Jump.performed += OnPlayerJump;
        }
        
        private void OnDisable()
        {
            _inputSystemActions ??= new InputSystem_Actions();
            _inputSystemActions.Disable();
            
            _inputSystemActions.Player.Move.performed -= OnPlayerMove;
            _inputSystemActions.Player.Move.canceled -= OnPlayerMove;
            _inputSystemActions.Player.Jump.performed -= OnPlayerJump;
        }

        public void OnPlayerMove(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }        
        
        private void OnPlayerJump(InputAction.CallbackContext obj)
        {
            WasJumpPressed = true;
        }

        public void OnPlayerConsumeJump()
        {
            WasJumpPressed = false;
        }
    }
}

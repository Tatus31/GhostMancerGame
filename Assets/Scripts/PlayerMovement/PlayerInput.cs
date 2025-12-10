using System;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerMovement
{
    public class PlayerInput : MonoBehaviour
    {        
        public bool WasJumpPressed { get; private set; }
        public bool WasJumpReleased { get; private set; }
        public bool WasCrouchPressed { get; private set; }
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
            _inputSystemActions.Player.Jump.started += OnPlayerJump;
            _inputSystemActions.Player.Jump.canceled += OnPlayerJumpRelease;
            _inputSystemActions.Player.Crouch.performed += OnPlayerCrouch;
            _inputSystemActions.Player.Crouch.canceled += OnPlayerCrouchRelease;
        }
        
        private void OnDisable()
        {
            _inputSystemActions ??= new InputSystem_Actions();
            _inputSystemActions.Disable();
            
            _inputSystemActions.Player.Move.performed -= OnPlayerMove;
            _inputSystemActions.Player.Move.canceled -= OnPlayerMove;
            _inputSystemActions.Player.Jump.performed -= OnPlayerJump;
            _inputSystemActions.Player.Jump.canceled -= OnPlayerJumpRelease;
            _inputSystemActions.Player.Crouch.performed -= OnPlayerCrouch;
        }

        private void OnPlayerMove(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }        
        
        private void OnPlayerJump(InputAction.CallbackContext obj)
        {
            WasJumpPressed = true;
        }

        private void OnPlayerJumpRelease(InputAction.CallbackContext ctx)
        {
            WasJumpReleased  = true;
        }
        
        private void OnPlayerCrouch(InputAction.CallbackContext ctx)
        {
            WasCrouchPressed = true;
        }

        private void OnPlayerCrouchRelease(InputAction.CallbackContext ctx)
        {
            WasCrouchPressed = false;
        }
        
        public void OnPlayerConsumeJump()
        {
            WasJumpPressed = false;
        }

        public void OnPlayerConsumeJumpRelease()
        {
            WasJumpReleased = false;
        }
    }
}

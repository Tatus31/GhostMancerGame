using System;
using Game.Input;
using UnityEngine;

namespace PlayerMovement
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance;
        
        private InputSystem_Actions _inputSystemActions;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            _inputSystemActions ??= new InputSystem_Actions();
            _inputSystemActions.Enable();
        }

        private void OnDisable()
        {
            _inputSystemActions ??= new InputSystem_Actions();
            _inputSystemActions.Disable();
        }

        public static Vector2 GetMovementInput()
        {
            return Instance._inputSystemActions.Player.Move.ReadValue<Vector2>();
        }
    }
}

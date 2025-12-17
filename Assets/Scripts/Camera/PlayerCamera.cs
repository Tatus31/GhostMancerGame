using System;
using PlayerMovement;
using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(PlayerController),typeof(Player))]
    public class PlayerCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachinePositionComposer cinemachinePositionComposer;

        [Header("Camera Settings")] 
        [Range(-10f, 0f)]
        [SerializeField] private float minCameraOffset;
        [Range(0f, 10f)]
        [SerializeField] private float maxCameraOffset;
        [Range(0f, 10f)]
        [SerializeField] private float cameraOffsetSpeed;

        private float _originalTargetOffsetY;
        
        private void Start()
        {
            _originalTargetOffsetY = cinemachinePositionComposer.TargetOffset.y;
        }

        public void MoveCameraUp()
        {
            cinemachinePositionComposer.TargetOffset.y = Mathf.MoveTowards(cinemachinePositionComposer.TargetOffset.y, maxCameraOffset,  Time.fixedDeltaTime * cameraOffsetSpeed);
        }

        public void ReturnCameraToOriginalPosition()
        {
            cinemachinePositionComposer.TargetOffset.y = Mathf.MoveTowards(cinemachinePositionComposer.TargetOffset.y, _originalTargetOffsetY,  Time.fixedDeltaTime * cameraOffsetSpeed);
        }

        public void MoveCameraDown()
        {
            cinemachinePositionComposer.TargetOffset.y = Mathf.MoveTowards(cinemachinePositionComposer.TargetOffset.y, minCameraOffset,  Time.fixedDeltaTime * cameraOffsetSpeed);
        }
    }
}

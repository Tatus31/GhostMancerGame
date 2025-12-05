using UnityEngine;

namespace PlayerMovement.PlayerData
{
    [CreateAssetMenu(fileName = "New Player Stats", menuName = "Player Stats/Create New Player Stats", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        [Header("[Jump]")]
        [Range(0f, 100f)]
        public float maxJumpHeight = 4f;
        [Range(0f, 100f)]
        public float minJumpHeight = 1f;
        [Range(0f, 10f)]
        public float timeToJumpApex = 0.4f;
        [Range(0f, 100f)]
        public float apexThreshold = 1f;
        [Range(0f, 10f)]
        public float apexGravityMultiplier = 0.4f;
        [Header("[Movement]")]
        [Range(0f, 100f)]
        public float moveSpeed = 5f;
        [Range(0f, 1f)]
        public float atApexSpeedBonus = 0.2f;
        [Header("[Acceleration/Deceleration]")]
        [Range(0f, 100f)]
        public float accelerationOnGround = 0.1f;
        [Range(0f, 100f)]
        public float accelerationInAir = 0.2f;
        [Range(0f, 100f)]
        public float decelerationOnGround = 0.1f;
        [Range(0f, 100f)]
        public float decelerationInAir = 0.2f;
        [Header("[Movement Limits]")]
        [Range(0f, 100f)]
        public float maxFallSpeed = 10f;
    }
}

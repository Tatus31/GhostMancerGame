using UnityEngine;

namespace PlayerMovement.PlayerData
{
    [CreateAssetMenu(fileName = "New Player Stats", menuName = "Player Stats/Create New Player Stats", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        [Header("[Jump]")]
        [Tooltip("How high the player can jump")]
        [Range(0f, 100f)]
        public float maxJumpHeight = 4f;
        [Tooltip("How small can the player jump height be")]
        [Range(0f, 100f)]
        public float minJumpHeight = 1f;
        [Tooltip("How long it takes for the player to reach the apex of his jump")]
        [Range(0f, 10f)]
        public float timeToJumpApex = 0.4f;
        [Tooltip("Determines when player is considered to have reached the apex of his jump")]
        [Range(0f, 100f)]
        public float apexThreshold = 1f;
        [Tooltip("How much the gravity affecting the player will change at the apex")]
        [Range(0f, 10f)]
        public float apexGravityMultiplier = 0.4f;
        [Tooltip("For how long will a jump buffer even if the player clicks the jump button without touching the ground")]
        [Range(0f, 1f)]
        public float jumpBufferMaxTime = 0.1f;

        [Header("[Movement]")]
        [Tooltip("The maximum speed the player can achieve")]
        [Range(0f, 100f)]
        public float moveSpeed = 5f;
        [Tooltip("How much faster the player becomes at the apex")]
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
        [Tooltip("How fast can the player fall")]
        [Range(0f, 100f)]
        public float maxFallSpeed = 10f;
    }
}

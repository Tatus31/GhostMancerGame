using System;
using System.Collections;
using UnityEngine;

namespace PlayerMovement.PlayerData
{
    [CreateAssetMenu(fileName = "New Speed Boost Effect", menuName = "Talisman Effects/Create New Speed Boost Effect", order = 2)]
    public class SpeedBoostCombinationSO  : TalismanEffect
    {
        [Header("Speed Modifiers")]
        [SerializeField, Range(1f, 5f)] 
        private float speedMultiplier = 2f;
        
        [Header("Ground Movement Modifiers")]
        [SerializeField, Range(0.1f, 10f)] 
        private float groundAccelerationMultiplier = 0.1f;
        [SerializeField, Range(0.1f, 10f)] 
        private float groundDecelerationMultiplier = 0.1f;
        
        [Header("Air Movement Modifiers")]
        [SerializeField, Range(0.1f, 10f)] 
        private float airAccelerationMultiplier = 0.1f;
        [SerializeField, Range(0.1f, 10f)] 
        private float airDecelerationMultiplier = 0.1f;
        
        [Header("Effect Settings")]
        [SerializeField, Min(0f)] 
        private float duration = 5f;

        private Coroutine _activeEffectCoroutine;
        public PlayerMovementStatsForBoost _originalPlayerStats;
        public int _activationCount = 0;

        public override void ActivateTalisman(Player player)
        {
            if (_activationCount == 0)
            {
                _originalPlayerStats = GetPlayerMovementStats(player);
            }

            _activationCount++;
            
            if (_activeEffectCoroutine != null)
            {
                player.StopCoroutine(_activeEffectCoroutine);
            }

            _activeEffectCoroutine = player.StartCoroutine(ApplySpeedBoostEffect(player));
        }

        private IEnumerator ApplySpeedBoostEffect(Player player)
        {
            // _originalPlayerStats = GetPlayerMovementStats(player);
            ApplyMoveBoostModifiers(player);
            
            yield  return new WaitForSeconds(duration);
            
            _activationCount = 0;

            RestoreOriginalPlayerStats(player, _originalPlayerStats);
            _activeEffectCoroutine = null;
        }

        public override void DeactivateTalisman(Player player)
        {
            var originalPlayerStats = GetPlayerMovementStats(player);
            
            RestoreOriginalPlayerStats(player, originalPlayerStats);
            _activeEffectCoroutine = null;
        }

        private void ApplyMoveBoostModifiers(Player player)
        {
            var playerData = player.PlayerData;
            
            playerData.moveSpeed *= speedMultiplier;
            playerData.accelerationOnGround *= groundAccelerationMultiplier;
            playerData.accelerationInAir *= airAccelerationMultiplier;
            playerData.decelerationOnGround *= groundDecelerationMultiplier;
            playerData.decelerationInAir *= airDecelerationMultiplier;
            
            // Debug.Log($"applied stats: playerData.moveSpeed [{playerData.moveSpeed}] | playerData.accelerationOnGround [{playerData.moveSpeed}] " +
            //           $"| playerData.accelerationInAir [{playerData.accelerationInAir}] | playerData.decelerationOnGround [{playerData.decelerationOnGround}] " +
            //           $"| playerData.decelerationInAir  [{playerData.decelerationInAir}] ");
        }

        private void RestoreOriginalPlayerStats(Player player, PlayerMovementStatsForBoost  playerStats)
        {
            if (!playerStats.IsValid())
            {
#if UNITY_EDITOR
                Debug.LogWarning("the playerStats values are 0 or below");
#endif
                return;
            }
            
            var playerData = player.PlayerData;

            playerData.moveSpeed = playerStats.MoveSpeed;
            playerData.accelerationOnGround = playerStats.AccelerationOnGround;
            playerData.accelerationInAir = playerStats.AccelerationInAir;
            playerData.decelerationOnGround = playerStats.DecelerationOnGround;
            playerData.decelerationInAir = playerStats.DecelerationInAir;
        }

        private PlayerMovementStatsForBoost GetPlayerMovementStats(Player player)
        {
            var playerData  = player.PlayerData;
            
            return new PlayerMovementStatsForBoost
            {
                MoveSpeed =  playerData.moveSpeed,
                AccelerationOnGround =  playerData.accelerationOnGround,
                AccelerationInAir =  playerData.accelerationInAir,
                DecelerationOnGround =  playerData.decelerationOnGround,
                DecelerationInAir =  playerData.decelerationInAir,
            };
        }

        [Serializable]
        public struct PlayerMovementStatsForBoost
        {
            public float MoveSpeed;
            public float AccelerationOnGround;
            public float AccelerationInAir;
            public float DecelerationOnGround;
            public float DecelerationInAir;
            
            public bool IsValid()
            {
                return MoveSpeed > 0 || AccelerationOnGround > 0 || AccelerationInAir > 0 
                       || DecelerationOnGround > 0 || DecelerationInAir > 0;
            }
        }
    }
}
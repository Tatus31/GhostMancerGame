using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerMovement.PlayerData
{
    [CreateAssetMenu(fileName = "New Talisman Combination", menuName = "Talismans/Create New Talisman")]
    public class TalismanCombinationSO : ScriptableObject
    {
        public string talismanName;
        public List<Player.TalismanInputs> talismanInputs;
        public Sprite talismanSprite;
        public Sprite talismanInputSprite;
    
        [SerializeField] private List<TalismanEffect> effects;
    
        public void ActivateTalisman(Player player)
        {
            foreach (var effect in effects)
            {
                effect.ActivateTalisman(player);
            }
        }
    }
    
    public abstract class TalismanEffect : ScriptableObject
    {
        public abstract void ActivateTalisman(Player player);
        public abstract void DeactivateTalisman(Player player);
    }
    
    [CreateAssetMenu(fileName = "New Sprint Effect", menuName = "Talismans/Create New Sprint Effect", order = 1)]
    public class SpeedBoostCombinationSo  : TalismanEffect
    {
        public float speedMultiplier = 2f;
        
        public float groundAccelerationMultiplier = 0.1f;
        public float groundDecelerationMultiplier = 0.1f;
        
        public float airAccelerationMultiplier = 0.1f;
        public float airDecelerationMultiplier = 0.1f;
        
        public float duration = 5f;

        public override void ActivateTalisman(Player player)
        {
            player.StartCoroutine(ApplySpeedBoostEffect(player));
        }

        private IEnumerator ApplySpeedBoostEffect(Player player)
        {
            float originalSpeed = player.PlayerData.moveSpeed;
            float originalAccelOnGround = player.PlayerData.accelerationOnGround;
            float originalAccelInAir = player.PlayerData.accelerationInAir;
            float originalDecelerationOnGround = player.PlayerData.decelerationOnGround;
            float originalDecelerationInAir = player.PlayerData.decelerationInAir;
            
            player.PlayerData.moveSpeed *= speedMultiplier;
            player.PlayerData.accelerationOnGround *= groundAccelerationMultiplier;
            player.PlayerData.accelerationInAir *= airAccelerationMultiplier;
            player.PlayerData.decelerationOnGround *= groundDecelerationMultiplier;
            player.PlayerData.decelerationInAir *= airDecelerationMultiplier;
            
            yield  return new WaitForSeconds(duration);
            
            player.PlayerData.moveSpeed = originalSpeed;
            player.PlayerData.accelerationOnGround = originalAccelOnGround;
            player.PlayerData.accelerationInAir = originalAccelInAir;
            player.PlayerData.decelerationOnGround = originalDecelerationOnGround;
            player.PlayerData.decelerationInAir = originalDecelerationInAir;
        }

        public override void DeactivateTalisman(Player player)
        {

        }
    }

}

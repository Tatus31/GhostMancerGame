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

        public void DeactivateTalisman(Player player)
        {
            foreach (var effect in effects)
            {
                effect.DeactivateTalisman(player);
            }
        }
    }
    
    public abstract class TalismanEffect : ScriptableObject
    {
        public abstract void ActivateTalisman(Player player);
        public abstract void DeactivateTalisman(Player player);
    }
}

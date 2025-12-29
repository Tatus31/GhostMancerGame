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

        private readonly Dictionary<System.Type, int> _effectCount = new Dictionary<System.Type, int>();

        public void ActivateTalisman(Player player)
        {
            _effectCount.Clear();

            CountEffectsWithSameType();
            ActivateIfAllowedEffectCount(player);
        }

        private void ActivateIfAllowedEffectCount(Player player)
        {
            foreach (var effect in effects)
            {
                var effectType = effect.GetType();

                if (_effectCount[effectType] <= effect.GetMaxEffects())
                {
                    effect.ActivateTalisman(player);
                }

#if UNITY_EDITOR
                else if (_effectCount[effectType] == effect.GetMaxEffects() + 1)
                {
                    Debug.LogWarning($"Too many effects of type {effectType.Name}. Found {_effectCount[effectType]}, but max allowed is {effect.GetMaxEffects()}. Skipping activation.");
                }
#endif
            }
        }

        private void CountEffectsWithSameType()
        {
            foreach (var talismanEffect in effects)
            {
                var effectType = talismanEffect.GetType();

                if (!_effectCount.TryAdd(effectType, 1))
                {
                    _effectCount[effectType]++;
                }
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
        public abstract int GetMaxEffects();
    }
}



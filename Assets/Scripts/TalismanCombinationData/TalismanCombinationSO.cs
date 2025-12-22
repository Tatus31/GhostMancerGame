using System.Collections.Generic;
using UnityEngine;

namespace PlayerMovement.PlayerData
{
    [CreateAssetMenu(fileName = "New Talisman Combination", menuName = "Talismans/Create New Talisman", order = 1)]
    public class TalismanCombinationSO : ScriptableObject
    {
        public string talismanName;
        public List<Player.TalismanInputs> talismanInputs;
        public Sprite talismanSprite;
        public Sprite talismanInputSprite;
    }

}

using System;
using PlayerMovement;
using PlayerMovement.PlayerData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
public class DebugMenu : MonoBehaviour
{
    [Header("[References]")]
    [SerializeField] private TextMeshProUGUI gravityText;
    [SerializeField] private TextMeshProUGUI velocityText;
    [SerializeField] private TextMeshProUGUI groundedText;
    [SerializeField] private TextMeshProUGUI talismanText;
    [Header("[Talismans]")]
    [SerializeField] private RawImage upTalisman;
    [SerializeField] private RawImage leftTalisman;
    [SerializeField] private RawImage rightTalisman;
    [SerializeField] private RawImage downTalisman;
    [Header("[Talisman Actions]")]
    [SerializeField] private Image talismanImageFirst;
    [SerializeField] private Image talismanImageSecond;
    [SerializeField] private Image talismanImageThird;
    
    private RawImage[] _talismanImages;
    private Image[] _talismanActivationImages;
    
    private TalismanCombinationSO[] _equippedTalismans;

    private float _gravity;
    
    private Player _player;
    
    private void Start()
    {
        _talismanImages = new[] { upTalisman, leftTalisman, rightTalisman, downTalisman };
        _talismanActivationImages = new []{talismanImageFirst,  talismanImageSecond, talismanImageThird};

        foreach (var talismanImage in _talismanImages)
        {
            if (!talismanImage)
            {
                Debug.LogWarning($"No talisman image found in the scene");
            }
        }
        
        var go = GameObject.FindGameObjectsWithTag("Player");
        _player = go[0].GetComponent<Player>();

        if (!_player)
        {
            Debug.LogError("No player found in the scene");
        }

        _equippedTalismans = _player.EquippedTalismans;
        
        _player.OnGravityChangedDebug += gravity => _gravity = gravity;
        _player.OnTalismanInputsDebug += UpdateTalismanActions;
        _player.OnTalismanResetDebug += ResetTalismanActions;
        _player.OnTalismanCorrectInput += UpdateTalismanName;

        UpdateTalismanActivationHelper();
    }

    private void OnDestroy()
    {
        _player.OnTalismanInputsDebug -= UpdateTalismanActions;
        _player.OnTalismanResetDebug -= ResetTalismanActions;
        _player.OnTalismanCorrectInput -= UpdateTalismanName;
    }

    private void UpdateTalismanName(string talismanName)
    {
        talismanText.text = talismanName;
    }

    private void UpdateTalismanActivationHelper()
    {
        for (int i = 0; i < _talismanActivationImages.Length; i++)
        {
            //Wrap around if needed
            int talismanIndex = i % _equippedTalismans.Length;
            _talismanActivationImages[i].sprite = _equippedTalismans[talismanIndex].talismanInputSprite;
        }
    }

    private void UpdatePlayerValues()
    {
        if (!_player)
        {
            Debug.LogError("No player found in the scene");
            return;
        }

        gravityText.text = _gravity.ToString("F2");
        velocityText.text = _player.Velocity.ToString();
    }

    private void UpdateGroundedState()
    {
        if (!_player)
        {
            Debug.LogError("No player found in the scene");
        }

        groundedText.text = _player.WasGrounded.ToString();
    }

    private void UpdateTalismanActions(Player.TalismanInputs talismanDirection)
    {
        var image = SelectTalismanDebugImage(talismanDirection);
        if (image)
        {
            image.color = Color.red;
        }
    }
    
    private void ResetTalismanActions()
    {
        foreach (var image in _talismanImages)
        {
            if (image)
            {
                image.color = Color.white;
            }
        }
        
        //talismanText.text = "Empty";
    }

    private RawImage SelectTalismanDebugImage(Player.TalismanInputs talismanDirection)
    {
        return talismanDirection switch
        {
            Player.TalismanInputs.Up => upTalisman,
            Player.TalismanInputs.Down => downTalisman,
            Player.TalismanInputs.Left => leftTalisman,
            Player.TalismanInputs.Right => rightTalisman,
            _ => null
        };
    }

    private void Update()
    {
        UpdatePlayerValues();
        UpdateGroundedState();
    }

}
#endif
using System;
using PlayerMovement;
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
    [Header("[Talismans]")]
    [SerializeField] private RawImage upTalisman;
    [SerializeField] private RawImage leftTalisman;
    [SerializeField] private RawImage rightTalisman;
    [SerializeField] private RawImage downTalisman;
    
    private RawImage[] _talismanImages;

    private float _gravity;
    
    private Player _player;
    
    private void Start()
    {
        _talismanImages = new[] { upTalisman, leftTalisman, rightTalisman, downTalisman };
        
        var go = GameObject.FindGameObjectsWithTag("Player");
        _player = go[0].GetComponent<Player>();

        if (!_player)
        {
            Debug.LogError("No player found in the scene");
        }

        _player.OnGravityChangedDebug += gravity => _gravity = gravity;
        _player.OnTalismanInputsDebug += UpdateTalismanActions;
        _player.OnTalismanResetDebug += ResetTalismanActions;
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
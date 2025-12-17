using System;
using PlayerMovement;
using TMPro;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    [Header("[References]")]
    [SerializeField] private TextMeshProUGUI gravityText;
    [SerializeField] private TextMeshProUGUI velocityText;
    [SerializeField] private TextMeshProUGUI groundedText;

    private float _gravity;
    
    private Player _player;
    
    private void Start()
    {
        var go = GameObject.FindGameObjectsWithTag("Player");
        _player = go[0].GetComponent<Player>();

        if (!_player)
        {
            Debug.LogError("No player found in the scene");
        }

        _player.OnGravityChanged += gravity => _gravity = gravity;
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

    private void Update()
    {
        UpdatePlayerValues();
        UpdateGroundedState();
    }
}

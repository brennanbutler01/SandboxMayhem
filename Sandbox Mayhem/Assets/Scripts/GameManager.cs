using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<PlayerController> _players;
    private PlayerController _humanPlayer;
    [SerializeField] public Text pointsText;

    private void Start()
    {
        pointsText.text = "Points: 0";
        _players = new List<PlayerController>(FindObjectsOfType<PlayerController>());

        var humanPlayers = _players.Where(player => player.isHuman).ToList();

        switch (humanPlayers.Count)
        {
            case 0:
                Debug.LogError("No PlayerController marked with IsHuman=True found.");
                return;
            case > 1:
                Debug.LogError("Multiple PlayerControllers marked with IsHuman=True found.");
                return;
            default:
                _humanPlayer = humanPlayers.First();
                break;
        }
        
        _humanPlayer.onPointsChanged.AddListener(UpdatePointsUI);
    }

    private void UpdatePointsUI()
    {
        Debug.Log("Updating points...");
        pointsText.text = "Points: " + _humanPlayer.Coins;
    }
}
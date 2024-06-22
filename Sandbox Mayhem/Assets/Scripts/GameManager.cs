using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<PlayerController> _players;
    public static List<PositionWaypoint> Waypoints;
    private PlayerController _humanPlayer;
    public static int TotalLaps = 3;
    public static int HalfwayTriggerIndex;
    public Text lapsText,  wrongWayText,  rankingText,  pointsText;
    public bool  gameOver;
    public Text leaderboardText;
    private void Start()
    {
        pointsText.text = "Points: 0";
        gameOver = false;
        Waypoints = FindObjectsOfType<PositionWaypoint>().ToList();
        _players = FindObjectsOfType<PlayerController>().ToList();
        var humanPlayers = _players.Where(player => player.isHuman).ToList();

        // we do some auditing here for counts and states required for the game to function.
    
        // we need at least one human player
        switch (humanPlayers.Count)
        {
            case 0:
                Debug.LogError("No PlayerController marked with IsHuman=True found.");
                return;
            case > 1:
                Debug.LogError("Multiple PlayerControllers marked with IsHuman=True found.");
                return;
        }

        _humanPlayer = humanPlayers.First();

        // We need waypoint prefabs to be able to track race progress
        if (Waypoints.Count == 0)
        {
            Debug.LogError("Add Waypoints to the track to race!");
            return;
        }
        
        Debug.Log($"Race initialized with {Waypoints.Count} Waypoints ");

        // should just have one component marked as halfway trigger - this activates 'laps'  
        var halfwayTriggers = Waypoints.Where(x => x.isHalfwayTrigger).ToList();
        switch (halfwayTriggers.Count)
        {
            case 0:
                Debug.LogError("At least one waypoint should be marked with IsHalfwayTrigger=True so the lapping mechanisms can work.");
                return;
            case > 1:
                Debug.LogError("There should only be one waypoint marked with IsHalfwayTrigger=True");
                return;
        }

        Debug.Log($"Halfway is: {halfwayTriggers[0].waypointIndex}");
        HalfwayTriggerIndex = halfwayTriggers[0].waypointIndex;
        lapsText.text = $"Lap: {_humanPlayer.currentLap}/{TotalLaps}";
        wrongWayText.text = "";
        leaderboardText.text = "";
        rankingText.text = "";
        InvokeRepeating(nameof(UpdateRanking), 1f, 0.1f);
    }

    private void UpdateRanking()
    {
        var finishedPlayers = _players.Where(x => x.hasFinished).OrderBy(x => x.ranking).ToList();
        var unfinishedPlayers = _players.Where(x => !x.hasFinished).ToList();

        var startRank = finishedPlayers.Count + 1;

        var rankedUnfinishedPlayers =
            unfinishedPlayers
                .OrderByDescending(x => x.currentLap)
                .ThenByDescending(x => x.currentWaypoint)
                .ThenBy(x => x.GetDistanceToNextWaypoint(x.currentWaypoint))
                .ToList();

        for (var i = 0; i < rankedUnfinishedPlayers.Count; i++)
        {
            rankedUnfinishedPlayers[i].ranking = startRank + i;
        }
        
        if (finishedPlayers.Count == _players.Count)
        {
            gameOver = true;
        }

        // Update the ranking text for the human player
        rankingText.text = $"Ranked: {_humanPlayer.ranking}/{_players.Count}";
    }
    
    private void Update()
    {
        pointsText.text = "Points: " + _humanPlayer.coins;
        lapsText.text = $"Lap: {_humanPlayer.currentLap}/{TotalLaps}";
        wrongWayText.text = _humanPlayer.isGoingBackward ? "Going backwards! \n Turn around." : "";
        
        if (gameOver)
        { 
            leaderboardText.text =  _players.OrderBy(x => x.ranking)
                .Aggregate("", (current, player) => current + $"{player.ranking} - {(player.isHuman ? "Human" : "AI")}\n");
        }
    }
}
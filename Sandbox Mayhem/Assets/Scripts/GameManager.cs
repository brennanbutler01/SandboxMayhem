using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

struct Behavior
{
    public float speed;
    public float rotationSpeed;
    public float minimumSpeed;
    public float maxAngle;
    public float brakeForce;
    public float maxSpeed;
    public float accelerationForce;
}

public class GameManager : MonoBehaviour
{
    private List<PlayerController> _players;
    public static List<PositionWaypoint> Waypoints;
    private PlayerController _humanPlayer;
    public static int TotalLaps = 3;
    public static int HalfwayTriggerIndex;
    public Text lapsText,  wrongWayText,  rankingText,  pointsText;
    public bool isGameOver;
    public Text leaderboardText;
    public GameObject leaderboard;
    public Text finishedText;
    public bool debug_TriggerRaceEnd = false;

    public GameObject pauseScreen;
    private CanvasGroup canvasGroup;

    public Text countdownText;
    public CarController playerCarController;
    public CarController[] aiCarController;
    private int countdownInt = 3;
    private float countdownElapsedTime = 0;
    private bool isCountDownInProgress = false;
    private float COUNTDOWN_BEEP_DURATION_IN_SECONDS = 0.6f; // Depends on the chosen countdown audio
    private float? carTiltedElapsedTime = null;
    
    private List<Behavior> behaviors;
    //private readonly Random random;

    private void Start()
    {
        pointsText.text = "Points: 0";
        isGameOver = false;
        Waypoints = FindObjectsOfType<PositionWaypoint>().ToList();
        _players = FindObjectsOfType<PlayerController>().ToList();
        var humanPlayers = _players.Where(player => player.isHuman).ToList();

        canvasGroup = pauseScreen.GetComponent<CanvasGroup>();
        pauseScreen.SetActive(false);

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
        leaderboard.SetActive(false);
        finishedText.enabled = false;
        InvokeRepeating(nameof(UpdateRanking), 1f, 0.1f);

        enableAllCars(false);
        startCountdown();
        setBehaviors();
        applyBehaviors();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            pointsText.text = "Points: " + _humanPlayer.coins;
            lapsText.text = $"Lap: {_humanPlayer.currentLap}/{TotalLaps}";
            wrongWayText.text = _humanPlayer.isGoingBackward ? "Going backwards! \n Turn around." : "";
           
            if (isCountDownInProgress)
            {
                updateCountdown();
            }

        }
        if (debug_TriggerRaceEnd)
        {
            endRace();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (canvasGroup.interactable)
            {
                pauseScreen.SetActive(false);
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
                Time.timeScale = 1f;
            }
            else
            {
                pauseScreen.SetActive(true);
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                Time.timeScale = 0f;
            }
        }
        checkIfPlayerCarIsTilted();
    }

    private void UpdateRanking()
    {
        if (isGameOver)
        {
            return;
        }
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
        

        // Update the ranking text for the human player
        rankingText.text = $"Rank: {_humanPlayer.ranking}/{_players.Count}";
        finishedText.text = "Finished " + positionToText(_humanPlayer.ranking);

        if (_humanPlayer.hasFinished)
        {
            endRace();
        }
    }

    private string positionToText(int pos)
    {
        string result = pos.ToString();
        switch (pos)
        {
            case 1:
                result += "st";
                break;
            case 2:
                result += "nd";
                break;
            case 3:
                result += "rd";
                break;
            default:
                result += "th";
                break;
        }
        return result;
    }

    private void startCountdown()
    {
        // Trigger countdown sound
        EventManager.TriggerEvent<RaceCountdownEvent, GameObject>(gameObject);

        // Trigger UI countdown update
        isCountDownInProgress = true;
    }

    private void updateCountdown()
    {
        countdownElapsedTime += Time.deltaTime;
        for (int countdown = 3; countdown >= 0; countdown--)
        {
            if ((countdownInt == countdown) && (countdownElapsedTime >= (4 - countdown) * COUNTDOWN_BEEP_DURATION_IN_SECONDS))
            {
                countdownInt -= 1;
                if (countdownInt > 0)
                {
                    countdownText.text = countdownInt.ToString();
                }
                else if (countdownInt == 0)
                {
                    // Finished countdown. Start the race.
                    isCountDownInProgress = false;
                    countdownText.text = "";
                    startRace();
                }
            }
        }
    }
    private void startRaceMusic()
    {
        //This separate method is needed to allow a delayed music start using Invoke (otherwise it overlaps with the last beep of race countdown)
        EventManager.TriggerEvent<RaceMusicEvent, GameObject>(gameObject);
    }

    private void enableAllCars(bool enable)
    {
        playerCarController.isMovementEnabled = enable;

        foreach (CarController aiCar in aiCarController)
        {
            aiCar.isMovementEnabled = enable;
        }
    }

    // Applies acceleration and braking values to AI opponents
    private void applyBehaviors()
    {
        int behaviorIndex = 0;

        foreach (CarController aiCar in aiCarController)
        {
            FollowWP followWP = aiCar.gameObject.GetComponent<FollowWP>();
            Behavior behavior = behaviors[behaviorIndex];
            followWP.accelerationForce = behavior.accelerationForce;
            followWP.brakeForce = behavior.brakeForce;
            followWP.maxSpeed = behavior.maxSpeed;
            followWP.maxAngle = behavior.maxAngle;
            followWP.minimumSpeed = behavior.minimumSpeed;
            behaviorIndex = (behaviorIndex + 1) % 3;
        }
    }

    private void startRace()
    {
        // Starting the race after the countdown is finished

        // Music starts a little delayed to avoid overlapping with race start beep
        Invoke("startRaceMusic", 2);

        // Hide countdown text
        countdownText.enabled = false;

        // Enabling all cars to move
        enableAllCars(true);
    }

    private void endRace()
    {
        isGameOver = true;
        finishedText.enabled = true;

        Invoke("showLeaderboardAndFreezeGame", 3);
    }

    private void showLeaderboardAndFreezeGame()
    {
        finishedText.text = "";
        finishedText.enabled = false;

        // Show leaderboard
        leaderboard.SetActive(true);
        leaderboardText.text = _players.OrderBy(x => x.ranking)
                .Aggregate("", (current, player) => current + $"{player.ranking} - {(player.isHuman ? "Human" : "AI")}\n");

        Time.timeScale = 0f;
    }

    // Create acceleration and braking values for AI opponents
    private void setBehaviors()
    {
        behaviors = new List<Behavior>();

        Behavior aggressive = new Behavior();
        aggressive.accelerationForce = .09f;
        aggressive.brakeForce = .9f;
        aggressive.maxSpeed = 30f;
        aggressive.maxAngle = 20f;
        aggressive.minimumSpeed = 5f;

        behaviors.Add(aggressive);

        Behavior passive = new Behavior();
        passive.accelerationForce = .05f;
        passive.brakeForce = 1f;
        passive.maxSpeed = 28f;
        passive.maxAngle = 22f;
        passive.minimumSpeed = 5f;

        behaviors.Add(passive);

        Behavior intermediate = new Behavior();
        intermediate.accelerationForce = .075f;
        intermediate.brakeForce = .95f;
        intermediate.maxSpeed = 29f;
        intermediate.maxAngle = 21f;
        intermediate.minimumSpeed = 5f;

        behaviors.Add(intermediate);
    }

    private void checkIfPlayerCarIsTilted()
    {
        if (_humanPlayer.Car.isCarTilted())
        {
            if (carTiltedElapsedTime is not null)
            {
                carTiltedElapsedTime += Time.deltaTime;
                
                // Reset the car position if it has been tilted for X seconds
                if (carTiltedElapsedTime > 3)
                {
                    print("Triggered car tilted/upside down reset");
                    resetCarPositionToWaypoint();
                    carTiltedElapsedTime = 0;
                }

            } else
            {
                carTiltedElapsedTime = 0;
            }
        } else if (carTiltedElapsedTime is not null)
        {
            carTiltedElapsedTime = null;
        }
    }

    // Resets car position to the last waypoint
    public void resetCarPositionToWaypoint()
    {
        PositionWaypoint waypoint = Waypoints.Find(x => x.waypointIndex == _humanPlayer.currentWaypoint);
        Debug.Log("Resetting player position to waypoint " + waypoint.waypointIndex);
        
        _humanPlayer.Car.resetCarPosition(waypoint.transform.position);
    }
}
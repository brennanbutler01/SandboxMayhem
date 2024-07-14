using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public string Id { private set; get; }
    public CarController Car { get; set; }
    public int currentLap, currentWaypoint, lastWaypoint, coins, ranking;
    private int _trailingWaypoint;
    public bool isGoingBackward, isHalfway, isHuman, lapPenalty, hasFinished;
    private GameObject[] uiCoins;
    private GameObject coinBoostActiveText;
    private bool alreadyBoosting = false;
    private bool allCoinsShowing = false;
    private float _bestLapTime = float.MaxValue;
    private float _currentLapStartTime;
    private GhostCarManager _ghostCarManager;

    private void Start()
    {
        coins = 0;
        currentLap = 0;
        currentWaypoint = 0;
        isHalfway = false;
        hasFinished = false;
        isGoingBackward = false;
        lapPenalty = false;
        Id = Guid.NewGuid().ToString();
        Car = GetComponent<CarController>();
        if (Car == null)
        {
            Debug.LogError("PlayerController requires a car controller");
        }
        uiCoins = GameObject.FindGameObjectsWithTag("UICoin").OrderBy(x => x.name).ToArray();
        coinBoostActiveText = GameObject.FindGameObjectWithTag("CoinBoostActiveText");
        _ghostCarManager = GetComponent<GhostCarManager>();
        if (isHuman && _ghostCarManager == null)
        {
            Debug.LogError("GhostCarManager not found on player object");
        }
    }

    public void StartNewLap()
    {
        if (!isHuman || currentLap == 0) return;
        if (currentLap == 1)
        {
            _ghostCarManager.StartRecording(transform);
            Debug.Log("Starting first lap, recording ghost data");
        }
        else
        {
            _ghostCarManager.StopRecording();
            _ghostCarManager.SaveBestLap();
            _ghostCarManager.StartPlayback();
            _ghostCarManager.StartRecording(transform);
            Debug.Log($"Starting lap {currentLap}, ghost car should be visible");
        }
    }

    public void CompleteLap()
    {
        if (!isHuman) return;
        var lapTime = Time.time - _currentLapStartTime;
        _ghostCarManager.StopRecording();

        if (lapTime < _bestLapTime)
        {
            _bestLapTime = lapTime;
            _ghostCarManager.SaveBestLap();
        }
        
        Debug.Log($"Completed Lap {currentLap} in {lapTime} seconds. Best {_bestLapTime}");
    }

    public void FinishRace()
    {
        hasFinished = true;
        if (!isHuman) return;
        _ghostCarManager.StopPlayback();
        _ghostCarManager.StopRecording();
        Debug.Log($"Player {Id} finished - rank is {ranking}");
    }
    
    public float GetDistanceToNextWaypoint(int index)
    {
        // Debug.LogError($"index - {index} - count - {GameManager.Waypoints.Count}");
        var nextWaypoint = index + 1 == GameManager.Waypoints.Count ? 0 : index + 1;
        var wp = GameManager.Waypoints.Find(x => x.waypointIndex == nextWaypoint);
        return Vector3.Distance(transform.position,
            wp?.transform.position ?? Vector3.zero);
    }

    public void Update()
    {
        // if we started going forward
        if (currentWaypoint > lastWaypoint)
        {
            StartCoroutine(_setTrailingWaypoint());
        }
    
        // if our last is in front of the trailing (it should be), we are going forward
        if (lastWaypoint > _trailingWaypoint)
        {
            isGoingBackward = false;
        }
    
        // if our last is behind the trailing, we are going backwards
        if (lastWaypoint < _trailingWaypoint)
        {
            isGoingBackward = true;
        }

        // Show/hide coins in UI depending on each coin collected
        // Only run if collector is human and all coins are not already showing
        if (isHuman && !allCoinsShowing)
        {
            coinBoostActiveText.SetActive(false);
            if (coins % 5 != 0 || coins == 0)
            {
                alreadyBoosting = false;
                for (int i = 0; i < 5; i++)
                {
                    if (i < (coins % 5))
                    {
                        uiCoins[i].SetActive(true);
                    }
                    else
                    {
                        uiCoins[i].SetActive(false);
                    }

                }
            }
            // If player has 5 coins and isn't in a coin boost state
            // Show all coins and coin boost text and enter a coin boost state
            else if (!alreadyBoosting)
            {
                allCoinsShowing = true;
                coinBoostActiveText.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    uiCoins[i].SetActive(true);
                }
                StartCoroutine(_hideUICoins());
                alreadyBoosting = true;
            }                    
        }
    }

    private IEnumerator _setTrailingWaypoint()
    {
        // after a second, we will set our trailing wp
        yield return new WaitForSeconds(1);
        _trailingWaypoint = lastWaypoint;
    }

    // Show coins for 3 seconds, the duration of the coin boost
    // At the end, leave the coin boost state and hide coin boost text
    private IEnumerator _hideUICoins()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < 5; i++)
        {
            uiCoins[i].SetActive(false);
        }
        StopCoroutine(_hideUICoins());
        allCoinsShowing = false;
        coinBoostActiveText.SetActive(false);
    }
}
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public string Id { private set; get; }
    public CarController Car { get; set; }
    public int currentLap, currentWaypoint, lastWaypoint, coins, ranking, boostCoins;
    private int _trailingWaypoint;
    public bool isGoingBackward, isHalfway, isHuman, lapPenalty, hasFinished, hasEnoughCoinsForBoost;
    private GameObject[] uiCoins;
    private GameObject speedBoostActiveText;
    private GameObject speedBoostAvailableText;
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
        speedBoostActiveText = GameObject.FindGameObjectWithTag("CoinBoostActiveText");
        speedBoostAvailableText = GameObject.FindGameObjectWithTag("SpeedBoostAvailableText");
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
        if (isHuman)
        {
            if (!allCoinsShowing)
            {
                speedBoostActiveText.SetActive(false);
                speedBoostAvailableText.SetActive(false);
                if (boostCoins < 5)
                {
                    alreadyBoosting = false;
                    for (int i = 0; i < 5; i++)
                    {
                        if (i < (boostCoins % 5))
                        {
                            uiCoins[i].SetActive(true);
                        }
                        else
                        {
                            uiCoins[i].SetActive(false);
                        }

                    }
                }
            }

            // If player has 5 coins and isn't in a coin boost state
            // Show all coins and available boost text and enable available boost  
            if (hasEnoughCoinsForBoost)
            {
                if (!alreadyBoosting)
                {
                    allCoinsShowing = true;
                    speedBoostAvailableText.SetActive(true);
                    for (int i = 0; i < 5; i++)
                    {
                        uiCoins[i].SetActive(true);
                    }
                }
            }

            // If player has enough coins to boost and they press the boost key
            // Activate speed boost and activate speed boost active text
            if (hasEnoughCoinsForBoost && Input.GetKeyUp(KeyCode.E))
            {
                Car.ActivateConsumableSpeedBoost(1.5f, 3f);
                hasEnoughCoinsForBoost = false;
                if (!alreadyBoosting)
                {
                    allCoinsShowing = true;
                    speedBoostAvailableText.SetActive(false);
                    speedBoostActiveText.SetActive(true);
                    for (int i = 0; i < 5; i++)
                    {
                        uiCoins[i].SetActive(true);
                    }
                    StartCoroutine(_hideUICoins());
                    alreadyBoosting = true;
                }
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
    // At the end, leave the coin boost state and hide speed boost text
    private IEnumerator _hideUICoins()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < 5; i++)
        {
            uiCoins[i].SetActive(false);
        }
        StopCoroutine(_hideUICoins());
        allCoinsShowing = false;
        speedBoostActiveText.SetActive(false);
        boostCoins = 0;
        hasEnoughCoinsForBoost = false;
    }
}
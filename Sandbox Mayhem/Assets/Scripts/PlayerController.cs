using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public enum Weapon
    {
        None, 
        Dart
    }
    public string Id { private set; get; }
    public CarController Car { get; set; }
    public int currentLap, currentWaypoint, lastWaypoint, coins, ranking;
    private int _trailingWaypoint;
    public bool isGoingBackward, isHalfway, isHuman, lapPenalty, hasFinished;
    public Transform weaponSpawn;
    public Weapon currentWeapon;
    private GameObject[] uiCoins;
    private GameObject coinBoostActiveText;
    private bool alreadyBoosting = false;
    private bool allCoinsShowing = false;
    
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
    }

    public float GetDistanceToNextWaypoint(int index)
    {
        // Debug.LogError($"index - {index} - count - {GameManager.Waypoints.Count}");
        var nextWaypoint = index + 1 == GameManager.Waypoints.Count ? 0 : index + 1;
        var wp = GameManager.Waypoints.Find(x => x.waypointIndex == nextWaypoint);
        return Vector3.Distance(transform.position,
            wp?.transform.position ?? Vector3.zero);
    }

    public void SetDart()
    {
        currentWeapon = Weapon.Dart;
    }

    public void RemoveWeapon()
    {
        currentWeapon = Weapon.None;
    }

    public void Update()
    {
        HandleWeaponsInput();
        
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

    private void HandleWeaponsInput()
    {
        if (Input.GetButtonDown("Fire") && isHuman)
        {
            if (currentWeapon == Weapon.Dart)
            {
                EventManager.TriggerEvent<FireDartEvent, PlayerController>(this);
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
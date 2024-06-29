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
    }

    private IEnumerator _setTrailingWaypoint()
    {
        // after a second, we will set our trailing wp
        yield return new WaitForSeconds(1);
        _trailingWaypoint = lastWaypoint;
    }
}
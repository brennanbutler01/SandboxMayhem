using System;
using UnityEngine;

public class PositionWaypoint : MonoBehaviour
{
    public int waypointIndex;
    public bool isHalfwayTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var player = other.GetComponent<PlayerController>();
        if (player == null || player.hasFinished) return;

        // at the home waypoint and either is first lap or we have gone halfway already
        if (waypointIndex == 0 && (player.isHalfway || player.currentLap == 0 || player.lapPenalty))
        {
            player.currentWaypoint = 0;
            player.isHalfway = false;
            player.lapPenalty = false;
            player.isGoingBackward = false;
            if (player.currentLap == GameManager.TotalLaps)
            {
                player.hasFinished = true;
                Debug.Log($"Player {player.Id} finished - rank is {player.ranking}");
                return;
            }
            player.currentLap++;
            Debug.Log("Increased lap");
        }
        
        // if we have not yet gone halfway, but we are showing a waypoint like 58 for example, that means we went backwards. we should subtract a lap here
        else if (waypointIndex > GameManager.HalfwayTriggerIndex && !player.isHalfway && !player.lapPenalty)
        {
            // we are cheating
            player.lapPenalty = true;
            player.currentLap =  Math.Max(0, player.currentLap - 1); // Ensure lap doesn't go below 0
            Debug.Log("Player has gone backwards...");
        }

        if (isHalfwayTrigger)
        {
            Debug.Log("Player gone halfway");
            player.isHalfway = true;
        }
        // going forward! do the second part to check if we are at the end of waypoints
        if (player.currentWaypoint < waypointIndex || player.currentWaypoint == GameManager.Waypoints.Count)
        {
            player.currentWaypoint = waypointIndex;
        }

        if (player.currentWaypoint > waypointIndex)
        {
            player.lastWaypoint = waypointIndex;
        }
    }
}

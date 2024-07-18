using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrapAudioController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController && playerController.isHuman)
        {
            EventManager.TriggerEvent<FloorTrapCollisionEvent, Vector3>(gameObject.transform.position);
            CarController carController = other.gameObject.GetComponent<CarController>();
            if (carController != null)
            {
                carController.triggerSpeedChange();
            }
        }
    }
}

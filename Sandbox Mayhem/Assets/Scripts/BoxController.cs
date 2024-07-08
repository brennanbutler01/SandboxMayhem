using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class BoxController : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.SetDart();
            EventManager.TriggerEvent<BoxBreakEvent, GameObject>(gameObject);
            Destroy(gameObject);
        }
    }
}

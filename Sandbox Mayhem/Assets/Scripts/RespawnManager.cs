using System.Collections;
using UnityEngine;

// we need this class because objects that are collected cannot respawn themselves
public class RespawnManager : MonoBehaviour
{
    private static RespawnManager _instance;

    public static RespawnManager Instance
    {
        //singleton
        get
        {
            // if we already have a reference
            if (_instance != null) return _instance;

            // if we have one in the scene
            _instance = FindObjectOfType<RespawnManager>();
            if (_instance != null) return _instance;
            
            // else we are going to make a new one
            var respawnManager = new GameObject("RespawnManager");
            _instance = respawnManager.AddComponent<RespawnManager>();
            return _instance;
        }
    }

    public void ScheduleRespawn(GameObject go, float respawnTime) =>
        StartCoroutine(_respawnCoroutine(go, respawnTime));

    private IEnumerator _respawnCoroutine(GameObject go, float respawnTime)
    {
        Debug.Log($"Inactivating - {go.name} for {respawnTime}s");
        go.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        go.SetActive(true);
        Debug.Log($"Respawning {go.name}");
    }
}
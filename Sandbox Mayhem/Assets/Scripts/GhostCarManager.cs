using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GhostCarManager : MonoBehaviour
{
    public class GhostFrame
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float Time;
    }

    private List<GhostFrame> _bestLapFrames = new List<GhostFrame>();
    private readonly List<GhostFrame> _currentLapFrames = new List<GhostFrame>();
    private float _lapStartTime;
    private bool _isRecording = false;

    public GameObject ghostCarPrefab;
    private GhostCar _ghostCar;

    private void Start()
    {
        var ghostCarObject = Instantiate(ghostCarPrefab, Vector3.zero, Quaternion.identity);
        _ghostCar = ghostCarObject.GetComponent<GhostCar>();
        ghostCarObject.SetActive(false);
    }

    public void StartRecording(Transform playerTransform)
    {
        _currentLapFrames.Clear();
        _lapStartTime = Time.time;
        _isRecording = true;
        Debug.Log("Started recording ghost data");
    }

    public void StopRecording()
    {
        _isRecording = false;
        Debug.Log("Stopped recording ghost data");
    }

    public void SaveBestLap()
    {
        _bestLapFrames = new List<GhostFrame>(_currentLapFrames);
        Debug.Log("Saved best lap ghost data");
    }

    public void StartPlayback()
    {
        if (_bestLapFrames.Count <= 0)
        {
            Debug.Log("No ghost data available for playback");
            return;
        }
        _ghostCar.StartPlayback(_bestLapFrames);
        Debug.Log("Started ghost car playback");
    }

    public void StopPlayback()
    {
        _ghostCar.StopPlayback();
        Debug.Log("Stopped ghost car playback");
    }

    private void Update()
    {
        if (_isRecording)
        {
            RecordFrame(transform);
        }
    }

    private void RecordFrame(Transform playerTransform)
    {
        _currentLapFrames.Add(new GhostFrame
        {
            Position = playerTransform.position,
            Rotation = playerTransform.rotation,
            Time = Time.time - _lapStartTime
        });
    }
}
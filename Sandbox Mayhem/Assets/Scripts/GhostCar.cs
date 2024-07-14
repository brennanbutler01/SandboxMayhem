using System.Collections.Generic;
using UnityEngine;

public class GhostCar : MonoBehaviour
{
    private List<GhostCarManager.GhostFrame> _frames;
    private int _currentFrameIndex = 0;
    private float _lapStartTime;
    private bool _isPlaying = false;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _endPosition;
    private Quaternion _endRotation;
    private float _startTime;
    private float _endTime;

    public void StartPlayback(List<GhostCarManager.GhostFrame> frames)
    {
        _frames = frames;
        _currentFrameIndex = 0;
        _lapStartTime = Time.time;
        _isPlaying = true;
        gameObject.SetActive(true);
        Debug.Log("Ghost car activated and starting playback");

        if (_frames.Count > 0)
        {
            SetInitialFrameData();
        }
    }

    public void StopPlayback()
    {
        _isPlaying = false;
        gameObject.SetActive(false);
        Debug.Log("Ghost car deactivated");
    }

    private void Update()
    {
        if (_isPlaying)
        {
            UpdateGhostPosition();
        }
    }

    private void SetInitialFrameData()
    {
        _startPosition = _frames[0].Position;
        _startRotation = _frames[0].Rotation;
        _startTime = 0f;

        if (_frames.Count > 1)
        {
            _endPosition = _frames[1].Position;
            _endRotation = _frames[1].Rotation;
            _endTime = _frames[1].Time;
        }
        else
        {
            _endPosition = _startPosition;
            _endRotation = _startRotation;
            _endTime = _startTime;
        }
    }

    private void UpdateGhostPosition()
    {
        var currentLapTime = Time.time - _lapStartTime;

        while (_currentFrameIndex < _frames.Count - 1 && 
               _frames[_currentFrameIndex + 1].Time < currentLapTime)
        {
            _currentFrameIndex++;
            UpdateFrameData();
        }

        if (_currentFrameIndex < _frames.Count - 1)
        {
            var t = Mathf.InverseLerp(_startTime, _endTime, currentLapTime);
            transform.position = Vector3.Lerp(_startPosition, _endPosition, t);
            transform.rotation = Quaternion.Slerp(_startRotation, _endRotation, t);
        }
        else
        {
            StopPlayback();
        }
    }

    private void UpdateFrameData()
    {
        _startPosition = _frames[_currentFrameIndex].Position;
        _startRotation = _frames[_currentFrameIndex].Rotation;
        _startTime = _frames[_currentFrameIndex].Time;

        if (_currentFrameIndex < _frames.Count - 1)
        {
            _endPosition = _frames[_currentFrameIndex + 1].Position;
            _endRotation = _frames[_currentFrameIndex + 1].Rotation;
            _endTime = _frames[_currentFrameIndex + 1].Time;
        }
        else
        {
            _endPosition = _startPosition;
            _endRotation = _startRotation;
            _endTime = _startTime;
        }
    }
}
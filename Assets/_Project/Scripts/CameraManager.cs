using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraAngles
{
    Front,
    Candidates,
    Top,
    Host
}

[Serializable]
public class CameraPosition {
    public CameraAngles Angle;
    public Vector3 Position;
    public Vector3 Rotation;
    public float Duration;
    public Ease Ease;
}

public class CameraManager : Singleton<CameraManager>
{
    private CameraAngles _currentCameraAngle;

    [SerializeField] private Camera _camera;
    [SerializeField] private List<CameraPosition> _cameraPositions;

    public void ChangeCameraAngle(CameraAngles cameraAngle)
    {
        if (_currentCameraAngle == cameraAngle) { return; }
        _currentCameraAngle = cameraAngle;

        CameraPosition cameraPosition = _cameraPositions.Find(x => x.Angle == cameraAngle);
        if (cameraPosition == null) {
            throw new KeyNotFoundException($"Camera position for {cameraAngle} not found");
        }

        _camera.transform.DOMove(cameraPosition.Position, cameraPosition.Duration);
        _camera.transform.DORotate(cameraPosition.Rotation, cameraPosition.Duration);
    }

    private void OnValidate()
    {
        if (_cameraPositions.Count != Enum.GetNames(typeof(CameraAngles)).Length)
        {
            Debug.LogWarning("Camera positions count has been updated! Please check..");

            // Check if the angle is in the list but is not in the enum, if so then add it
            foreach (var cameraPosition in _cameraPositions)
            {
                if (!Enum.IsDefined(typeof(CameraAngles), cameraPosition.Angle))
                {
                    _cameraPositions.Remove(cameraPosition);
                    continue;
                }
            }

            // Check if the angle is in the enum but is not in the list, if so then add it
            foreach (var cameraAngle in Enum.GetNames(typeof(CameraAngles)))
            {
                if (!_cameraPositions.Exists(x => x.Angle.ToString() == cameraAngle))
                {
                    _cameraPositions.Add(new CameraPosition() { Angle = (CameraAngles)Enum.Parse(typeof(CameraAngles), cameraAngle) });
                    continue;
                }
            }
        }
    }
}

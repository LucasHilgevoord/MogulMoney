using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StagePresets
{
    Front,
    Contestants,
    Host
}

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private StagePresets _currentView = StagePresets.Front;

    [Header("Components")]
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private LightManager _lightManager;

    public void ChangeView(StagePresets stagePreset)
    {
        switch (stagePreset)
        {
            case StagePresets.Front:
                _cameraManager.ChangeCameraAngle(CameraAngles.Front);
                _lightManager.ChangeLighting(LightingGroup.Front);
                break;
            case StagePresets.Contestants:
                _cameraManager.ChangeCameraAngle(CameraAngles.Contestants);
                _lightManager.ChangeLighting(LightingGroup.Contestants);
                break;
            case StagePresets.Host:
                _cameraManager.ChangeCameraAngle(CameraAngles.Host);
                _lightManager.ChangeLighting(LightingGroup.Host);
                break;
            default:
                break;
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            ChangeView(_currentView);
    }
}

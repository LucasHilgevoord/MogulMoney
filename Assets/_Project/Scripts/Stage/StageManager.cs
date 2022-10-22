using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StagePresets
{
    Front,
    Contestants,
    SingleContestant,
    Host,
}

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private StagePresets _currentView;

    [Header("Components")]
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private LightManager _lightManager;

    private void Start()
    {
        ChangeView(_currentView);
    }

    public void ChangeView(StagePresets stagePreset, object[] args = null)
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
            case StagePresets.SingleContestant:
                _cameraManager.ChangeCameraAngle(CameraAngles.Contestants);
                _lightManager.ChangeLighting(LightingGroup.SingleContestant);
                break;
            default:
                break;
        }
    }

    public void ChangeLight(LightingGroup group, float duration = 0)
    {
        _lightManager.ChangeLighting(group, duration);
    }

    private void OnValidate()
    {
        ChangeView(_currentView);
    }
}

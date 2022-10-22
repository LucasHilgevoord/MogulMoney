using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LightPreset
{
    public LightingGroup Group;
    public Light[] Lights;
    internal float[] Intensities;
}

public enum LightingGroup
{
    Front,
    Contestants,
    SingleContestant,
    Host
}

public class LightManager : MonoBehaviour
{
    [SerializeField] private List<LightPreset> _lightPresets;
    private LightPreset _currentPreset;

    private void SetupIntensities()
    {
        // Set the initial lighting intensity
        foreach (LightPreset preset in _lightPresets)
        {
            if (preset.Intensities != null && preset.Intensities.Length == preset.Lights.Length) { continue; }
            
            preset.Intensities = new float[preset.Lights.Length];
            for (int i = 0; i < preset.Lights.Length; i++)
            {
                preset.Intensities[i] = preset.Lights[i].intensity;
            }
        }
    }

    internal void ChangeLighting(LightingGroup newGroup, float duration = 0)
    {
        // Check if we have a preset for this group
        LightPreset newPreset = _lightPresets.Find(x => x.Group == newGroup);
        if (newPreset == null)
            throw new KeyNotFoundException($"Lighting preset for {newGroup} not found");

        // Check if this group is already enabled
        if (_currentPreset != null && newGroup == _currentPreset.Group) { return; }

        // QUICKFIX, Intensities are not set on start
        if (newPreset.Intensities == null)
            SetupIntensities();

        // Disable all the active lights
        if (_currentPreset != null)
        {
            foreach (Light light in _currentPreset.Lights)
            {
                // Check if we still need already active lights in the new preset
                if (Array.Exists(newPreset.Lights, x => x == light))
                    continue;

                // Fade out the light if we have a duration
                if (duration != 0)
                {
                    light.DOIntensity(0, duration).OnComplete(() => {
                        light.gameObject.SetActive(false);
                    });
                }
                else
                {
                    light.gameObject.SetActive(false);
                }
            }
        }

        // Enable all the lights
        for (int i = 0; i < newPreset.Lights.Length; i++)
        {
            Light light = newPreset.Lights[i];
            float intensity = newPreset.Intensities[i];

            // No need to do something with this light because it's already on
            if (light.isActiveAndEnabled) { continue; }
            light.gameObject.SetActive(true);

            // Fade in the light if we have a duration
            if (duration != 0)
            {
                light.DOIntensity(intensity, duration);
            }
            else
            {
                light.intensity = intensity;
            }
        }

        // Change the current preset to the new preset
        _currentPreset = newPreset;
    }

    private void OnValidate()
    {
        if (!Application.isEditor || Application.isPlaying) { return; }

        if (_lightPresets.Count != Enum.GetNames(typeof(LightingGroup)).Length)
        {
            Debug.LogWarning("Lighting group count has been updated! Please check..");

            // Check if the angle is in the list but is not in the enum, if so then add it
            foreach (var preset in _lightPresets)
            {
                if (!Enum.IsDefined(typeof(LightingGroup), preset.Group))
                {
                    _lightPresets.Remove(preset);
                    continue;
                }
            }

            // Check if the angle is in the enum but is not in the list, if so then add it
            foreach (var lightGroup in Enum.GetNames(typeof(LightingGroup)))
            {
                if (!_lightPresets.Exists(x => x.Group.ToString() == lightGroup))
                {
                    _lightPresets.Add(new LightPreset() { Group = (LightingGroup)Enum.Parse(typeof(LightingGroup), lightGroup) });
                    continue;
                }
            }
        }

        SetupIntensities();
    }
}

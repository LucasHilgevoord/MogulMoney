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
    private List<Light> _activeLights = new List<Light>();
    [SerializeField] private List<LightPreset> _lightPresets;
    private LightingGroup _currentLightingGroup;

    private void SetupIntensity()
    {
        // Set the initial lighting intensity
        foreach (LightPreset preset in _lightPresets)
        {
            preset.Intensities = new float[preset.Lights.Length];
            for (int i = 0; i < preset.Lights.Length; i++)
            {
                preset.Intensities[i] = preset.Lights[i].intensity;
            }
        }
    }

    internal void ChangeLighting(LightingGroup group, float duration = 0)
    {
        // Check if we have a preset for this group
        LightPreset lightPreset = _lightPresets.Find(x => x.Group == group);
        if (lightPreset == null)
            throw new KeyNotFoundException($"Lighting preset for {group} not found");

        // Check if this group is already enabled
        if (group == _currentLightingGroup) { return; }
        _currentLightingGroup = lightPreset.Group;

        // Clone the list of active lights so we can alter it
        List<Light> activeLights = new List<Light>(_activeLights);

        // Disable all the lights
        foreach (Light light in activeLights)
        {
            // Check if we still need already active lights in the new preset
            if (Array.Exists(lightPreset.Lights, x => x == light))
                continue;

            // Fade out the light if we have a duration
            if (duration != 0)
            {
                light.DOIntensity(0, duration).OnComplete(() => { 
                    light.gameObject.SetActive(false);
                });
            } else
            {
                light.gameObject.SetActive(false);
            }

            // Remove the light from the list
            _activeLights.Remove(light);
        }

        // Enable all the lights
        for (int i = 0; i < lightPreset.Lights.Length; i++)
        {
            Light light = lightPreset.Lights[i];
            float intensity = lightPreset.Intensities[i];

            // No need to do something with this light because it's already on
            if (_activeLights.Find(x => x == light) != null) { continue; }

            // Fade in the light if we have a duration
            if (duration != 0)
            {
                light.DOIntensity(intensity, duration);
            }
            else
            {
                light.intensity = intensity;
                light.gameObject.SetActive(true);
            }
            
            _activeLights.Add(light);
        }
    }

    private void OnValidate()
    {
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

        SetupIntensity();
    }
}

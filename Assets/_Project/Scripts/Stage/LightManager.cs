using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LightPreset
{
    public LightingGroup Group;
    public Light[] Lights;
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

    internal void ChangeLighting(LightingGroup group)
    {
        // Check if we have a preset for this group
        LightPreset lightPreset = _lightPresets.Find(x => x.Group == group);
        if (lightPreset == null)
            throw new KeyNotFoundException($"Lighting preset for {group} not found");
        _currentLightingGroup = lightPreset.Group;

        // Disable all the lights
        foreach (Light light in _activeLights)
        {
            // TODO: check if we this light is still needed
            light.gameObject.SetActive(false);
        }
        _activeLights.Clear();

        // Enable all the lights
        foreach (Light light in lightPreset.Lights)
        {
            light.gameObject.SetActive(true);
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
    }
}

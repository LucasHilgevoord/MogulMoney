using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private List<Contestant> _contestants = new List<Contestant>();

    internal void Initialize()
    {
        // TODO: Load in players
        foreach (Contestant c in _contestants)
        {
            c.Display.SetupDisplay(c);
        }
    }

    internal void EnableBuzzer(int contestant)
    {
        _contestants[contestant].Display.ToggleBuzzer(true);
    }
}

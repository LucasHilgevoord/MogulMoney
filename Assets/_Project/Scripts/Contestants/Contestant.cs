using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Contestant
{
    public int ID = -1;
    public string Name = "Default";
    public int Score = 0;

    public ContestantDisplay Display;
}

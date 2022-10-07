using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriviaData
{
    public string answerPrefix;
    public TriviaCategory[] categories;

    public TriviaData() { }
}

[Serializable]
public class TriviaCategory
{
    public string title;
    public TriviaQuestion[] questions;

    public TriviaCategory() { }
}


[Serializable]
public class TriviaQuestion
{
    public string question;
    public string[] answer;
    public int points;

    public TriviaQuestion() { }
}

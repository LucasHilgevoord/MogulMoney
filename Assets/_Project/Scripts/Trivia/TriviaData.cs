using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriviaData
{
    public string answerPrefix;
    public TriviaCategoryData[] categories;

    public TriviaData() { }
}

[Serializable]
public class TriviaCategoryData
{
    public string title;
    public TriviaQuestionData[] questions;

    public TriviaCategoryData() { }
}


[Serializable]
public class TriviaQuestionData
{
    public string question;
    public string[] answer;
    public int points;

    public TriviaQuestionData() { }
}

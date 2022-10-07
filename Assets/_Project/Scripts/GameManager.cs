using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TriviaManager _triviaManager;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _triviaManager.StartGame();
        }
    }
}

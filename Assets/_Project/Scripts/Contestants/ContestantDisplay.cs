using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContestantDisplay : MonoBehaviour
{
    private Contestant _contestant;
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private Image _buzzer;

    private float _buzzerAlphaDuration = 0.5f;

    internal void SetupDisplay(Contestant contestant)
    {
        _contestant = contestant;
        _nameLabel.text = contestant.Name;
        UpdateScore();
    }

    internal void UpdateScore()
    {
        _scoreLabel.text = _contestant.Score.ToString();
    }

    internal void ToggleBuzzer(bool active)
    {
        // Start the alpha with 0 if we want it to go active
        float _startAlpha = active ? 0 : 1;
        float _endAlpha = active ? 1 : 0;

        if (_buzzer.color.a != _startAlpha)
        {
            Color c = _buzzer.color;
            c.a = _startAlpha;

            _buzzer.color = c;
        }

        _buzzer.gameObject.SetActive(active);
        _buzzer.DOFade(_endAlpha, _buzzerAlphaDuration).SetEase(Ease.InOutSine);
    }
}

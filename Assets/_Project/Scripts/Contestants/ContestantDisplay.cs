using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContestantDisplay : MonoBehaviour
{
    private Contestant _contestant;
    [SerializeField] private Image _background;
    [SerializeField] private Image _logo;
    
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private Image _buzzer;

    [SerializeField] private TextMeshProUGUI _answerTitleLabel;
    [SerializeField] private TextMeshProUGUI _answerLabel;
    [SerializeField] private Color _correctColor, _incorrectColor;

    [SerializeField] private Color _activeColor, _inactiveColor;

    private float _panelActivityFadeDuration = 0.5f;
    private float _buzzerAlphaDuration = 0.5f;
    private float _panelSwitchDuration = 0.5f;
    
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

    internal void TogglePanelActivity(bool active, bool snap = false)
    {
        // Make everything a bit blacker
        Color color = active ? _activeColor : _inactiveColor;
        float duration = snap ? 0 : _panelActivityFadeDuration;

        _background.DOColor(color, duration);
        _logo.DOColor(color, duration);
        _nameLabel.DOColor(color, duration);
        _scoreLabel.DOColor(color, duration);
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

    internal void EnableAnswerPanel(string answer)
    {
        _answerLabel.text = answer;
        Color alpha = Color.white;
        alpha.a = 0;

        _answerTitleLabel.color = alpha;
        _answerLabel.color = alpha;
        _answerTitleLabel.gameObject.SetActive(true);
        _answerLabel.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_nameLabel.DOFade(0, _panelSwitchDuration).SetEase(Ease.InOutSine));
        sequence.Join(_scoreLabel.DOFade(0, _panelSwitchDuration).SetEase(Ease.InOutSine));
        sequence.Join(_buzzer.DOFade(0, _panelSwitchDuration).SetEase(Ease.InOutSine));
        sequence.Append(_answerTitleLabel.DOFade(1, _panelSwitchDuration).SetEase(Ease.InOutSine));
        sequence.Join(_answerLabel.DOFade(1, _panelSwitchDuration).SetEase(Ease.InOutSine));

        sequence.Play();
    }

    internal void EnablePointsPanel()
    {
        
    }

    internal void ShowCorrectColor(bool isCorrect, float delay = 0)
    {
        Color color = isCorrect ? _correctColor : _incorrectColor;
        _background.DOColor(color, 1f).SetDelay(delay);
    }
}

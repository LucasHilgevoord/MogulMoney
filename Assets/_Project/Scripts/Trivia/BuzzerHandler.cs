using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuzzerHandler : MonoBehaviour
{
    public static event Action<int> ButtonPressed;

    [SerializeField] private RectTransform _panel;
    [SerializeField] private TextMeshProUGUI _questionLabel;

    [SerializeField] private Image _backgroundImage;
    [SerializeField] private RectTransform _backgroundRect;
    [SerializeField] private Image _clockBar;

    [SerializeField] private float _clockShowDelay = 0.3f;
    [SerializeField] private Color _normalColor, _warningColor;

    private bool _enableTimer;
    private float _startTime;
    private float _currentTime;

    private float _warningTimeInSeconds = 5f;
    private bool _enableWarning;

    internal void SetupStage(string questionText)
    {
        _questionLabel.text = questionText;
        _panel.anchoredPosition = Vector2.up * _panel.rect.height;
    }

    internal void StartBuzzer(float time)
    {
        _panel.gameObject.SetActive(true);
        _panel.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutSine);

        SetupTimer(time);
    }

    private void SetupTimer(float time)
    {
        _startTime = time;
        _currentTime = _startTime;

        _clockBar.fillAmount = 1;
        _backgroundRect.localScale = Vector3.zero;
        _clockBar.transform.localScale = Vector3.zero;
        _backgroundImage.color = _normalColor;

        Sequence sequence = DOTween.Sequence();
        sequence.SetDelay(_clockShowDelay);
        sequence.Append(_backgroundRect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutSine));
        sequence.Append(_clockBar.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutSine));
        //equence.Join(_clockBar.DOFillAmount(1, 0.5f));

        sequence.OnComplete(() => {
            _backgroundImage.DOColor(_warningColor, _currentTime);
            _enableTimer = true;
        });
        sequence.Play();
    }

    private void Update()
    {
        if (_enableTimer) 
        {

            // TESTING
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                HitButton(1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HitButton(2);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                HitButton(3);
                return;
            }

            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        _currentTime -= Time.deltaTime;
        _clockBar.fillAmount = _currentTime / _startTime;

        //if (_currentTime <= _warningTimeInSeconds && !_enableWarning)
        //{
        //    _enableWarning = true;
        //    _backgroundRect.DOShakeAnchorPos(_warningTimeInSeconds, 5, 5, 50, false).SetSpeedBased();
        //    _backgroundImage.DOColor(_warningColor, _warningTimeInSeconds);
        //}

        if (_currentTime <= 0)
            TimeUp();
    }

    private void HitButton(int candidate)
    {
        _enableTimer = false;
        DOTween.Kill(_backgroundImage);
        ButtonPressed?.Invoke(candidate);

        //HidePanel();
    }

    private void TimeUp()
    {
        _enableTimer = false;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_backgroundImage.DOColor(_normalColor, 1f));
        sequence.Join(_backgroundRect.DOShakeAnchorPos(1, 10, 10, 50, false));
        sequence.OnComplete(HidePanel);
        sequence.Play();
    }

    private void HidePanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_backgroundRect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InSine));
        sequence.Append(_panel.DOAnchorPosY(_panel.rect.height, 0.5f).SetEase(Ease.InSine));
        sequence.Play();
    }
}

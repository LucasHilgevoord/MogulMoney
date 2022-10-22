using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionInputHandler : MonoBehaviour
{
    public static event Action<string, bool> QuestionAnswered;

    [SerializeField] private GameObject _panel;

    [SerializeField] private RectTransform _inputPanel;
    [SerializeField] private TextMeshProUGUI _categoryLabel;
    [SerializeField] private TextMeshProUGUI _questionLabel;
    [SerializeField] private TMP_InputField _answerInput;

    [SerializeField] private Button _confirmButton, _passButton;
    private RectTransform _confirmRect, _passRect;
    private float _buttonHideHeight;

    [SerializeField] private Image _overlay;
    private float _overlayAlpha = 0.5f;
    
    [SerializeField] private RectTransform _timer;
    [SerializeField] private TextMeshProUGUI _timerLabel;
    [SerializeField] private Image _timerBar;
    private TriviaQuestionData _questionData;

    private bool _allowInput;
    private float _startTime, _currentTime;

    private void Start()
    {
        _confirmRect = _confirmButton.GetComponent<RectTransform>();
        _passRect = _passButton.GetComponent<RectTransform>();
        _buttonHideHeight = -_confirmRect.parent.GetComponent<RectTransform>().anchoredPosition.y;

        _answerInput.onValueChanged.AddListener(OnAnswerValueChanged);
    }

    private void ResetPanels()
    {
        
    }

    internal void SetupPanel(TriviaQuestionData questionData, float time)
    {
        _questionData = questionData;

        // Set the catogory label to match the selected question
        _categoryLabel.text = _questionData.category;

        // Set the question label to match the selected question
        _questionLabel.text = _questionData.question;

        _startTime = time;

        ResetPanel();
    }

    private void ResetPanel()
    {
        _allowInput = false;

        // Reset the overlay
        Color c = _overlay.color;
        c.a = 0;
        _overlay.color = c;

        // Reset the timer
        _timerBar.fillAmount = 1;
        _timer.anchoredPosition = _timer.anchoredPosition + Vector2.up * _timer.rect.height;

        // Reset the input panel
        _inputPanel.anchoredPosition = Vector2.down * (_inputPanel.rect.height * 2);

        // Hide the buttons behind the panel
        _confirmRect.anchoredPosition = Vector2.up * _buttonHideHeight;
        _passRect.anchoredPosition = Vector2.up * _buttonHideHeight;

        // Set the confirm button to not be interactable until we have an answer
        _confirmButton.interactable = false;

        _currentTime = _startTime;
        _timerLabel.text = string.Format("{0:00}:{1:00}", ((int)_currentTime / 60), ((int)_currentTime % 60));
    }

    internal void ShowPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_overlay.DOFade(_overlayAlpha, 0.5f));
        sequence.Append(_inputPanel.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutSine));
        sequence.Append(_timer.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutSine));
        sequence.Append(_confirmRect.DOAnchorPosY(0, 0.2f).SetEase((Ease.OutSine)));
        sequence.Join(_passRect.DOAnchorPosY(0, 0.2f).SetEase((Ease.OutSine)));
        sequence.Play();

        sequence.OnComplete(StartInput);

        _panel.SetActive(true);
    }

    internal void HidePanel(float delay = 0, Action OnComplete = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delay);
        sequence.Append(_confirmRect.DOAnchorPosY(_buttonHideHeight, 0.2f).SetEase((Ease.InSine)));
        sequence.Join(_passRect.DOAnchorPosY(_buttonHideHeight, 0.2f).SetEase((Ease.InSine)));
        sequence.Append(_timer.DOAnchorPosY(_timer.rect.height, 0.5f).SetEase(Ease.InSine));
        sequence.Append(_inputPanel.DOAnchorPosY(-_inputPanel.rect.height * 2, 0.5f).SetEase(Ease.InSine));
        sequence.Append(_overlay.DOFade(0, 0.5f));
        sequence.Play();

        sequence.OnComplete(() =>
        {
            _panel.SetActive(false);
            OnComplete?.Invoke();
        });

    }

    private void Update()
    {
        if (_allowInput)
        {
            if (Input.GetKeyDown(KeyCode.Return) && _confirmButton.interactable)
            {
                OnConfirmButtonClicked();
                return;
            }

            UpdateTimer();
        }
    }
    private void StartInput()
    {
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        _passButton.onClick.AddListener(OnPassButtonClicked);
        _allowInput = true;

        _answerInput.Select();
    }

    private void OnConfirmButtonClicked()
    {
        ToggleInteraction(false);
        ShowAnswer();
    }

    private void OnPassButtonClicked()
    {
        ToggleInteraction(false);
        _answerInput.text = "";
        ShowAnswer();
    }

    private void OnAnswerValueChanged(string value)
    {
        bool hasInput = !string.IsNullOrEmpty(value);

        if (hasInput == _confirmButton.interactable)
            return;
        
        _confirmButton.interactable = hasInput;
    }

    private bool CheckAnswer()
    {
        foreach (string possibleAnswer in _questionData.answer)
        {
            Debug.Log(_answerInput.text.ToLower() + " " + possibleAnswer.ToLower());
            if (_answerInput.text.ToLower() == possibleAnswer.ToLower())
                return true;
        }

        return false;
    }


    private void ShowAnswer()
    {
        bool isCorrect = false;
        if (_answerInput.text != "")
            isCorrect = CheckAnswer();

        QuestionAnswered?.Invoke(_answerInput.text, isCorrect);
    }

    private void ToggleInteraction(bool interactable)
    {
        _confirmButton.interactable = interactable;
        _passButton.interactable = interactable;
        _answerInput.interactable = false;

        _allowInput = interactable;
    }

    private void UpdateTimer()
    {
        _currentTime -= Time.deltaTime;
        _timerBar.fillAmount = _currentTime / _startTime;

        // Update the timer label in the format of 00:00
        _timerLabel.text = string.Format("{0:00}:{1:00}", ((int)_currentTime / 60), ((int)_currentTime % 60));

        if (_currentTime <= 0)
            TimeUp();
    }

    private void TimeUp()
    {
        OnPassButtonClicked();
    }
}

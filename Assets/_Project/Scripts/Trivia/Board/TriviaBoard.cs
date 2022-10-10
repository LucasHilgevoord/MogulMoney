using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriviaBoard : MonoBehaviour
{
    [SerializeField] private float _appearDuration = 1f;
    [SerializeField] private Transform _line;
    private TriviaQuestion _selectedQuestion;

    [Header("Categories")]
    private List<BoardCategory> _boardCategories;
    [SerializeField] private BoardCategory _categoryPrefab;
    [SerializeField] private GameObject _categoryParent;
    [SerializeField] private float _categoryFadeDelay = 0.2f;
    [SerializeField] private float _categoryFadeDuration = 0.5f;
    [SerializeField] private TextMeshProUGUI _categorySelectedTitle;

    [Header("QuestionCard")]
    [SerializeField] private TriviaQuestion _questionPrefab;
    [SerializeField] private float _questionFadeDelay = 0.05f;
    [SerializeField] private Color _questionSelectedColor;
    [SerializeField] private Color _questionPointsColor;
    [SerializeField] private float _questionRecolorDuration;
    [SerializeField] private float _questionScaleDuration;
    [SerializeField] private Vector2 _questionPanelSize;
    [SerializeField] private Vector2 _questionPanelCenterFactor;
    [SerializeField] private float _questionPointRescale;
    [SerializeField] private Transform _hideMask;

    internal void Initialize(TriviaCategoryData[] categories)
    {
        _boardCategories = new List<BoardCategory>();
        for (int i = 0; i < categories.Length; i++)
        {
            // Create a new category based on the JSON data
            BoardCategory category = Instantiate(_categoryPrefab, _categoryParent.transform);
            category.Title.text = categories[i].title;

            // Hide the title so we can fade it in later
            category.Title.alpha = 0;

            // Create the questions for the category
            category.Questions = new TriviaQuestion[categories[i].questions.Length];
            for (int j = 0; j < categories[i].questions.Length; j++)
            {
                // Create the question and assign the question data
                TriviaQuestion question = Instantiate(_questionPrefab, category.QuestionParent.transform);
                question.Initialize(categories[i].questions[j], category);
                category.Questions[j] = question;
            }

            _boardCategories.Add(category);
        }

        ResetBoard();
    }

    private void ResetBoard()
    {
        _categoryParent.SetActive(true);

        // Make the line small so we can scale it in later
        _line.localScale = new Vector3(0, 1, 1);
        _categorySelectedTitle.alpha = 0;

        // Hide the selected category label
        _categorySelectedTitle.gameObject.SetActive(false);
        _categorySelectedTitle.alpha = 0;

        for (int i = 0; i < _boardCategories.Count; i++)
        {
            // Hide all the titles of the categories
            _boardCategories[i].Title.alpha = 0;

            for (int j = 0; j < _boardCategories[i].Questions.Length; j++)
            {
                // Hide the question so we can show it later
                _boardCategories[i].Questions[j].canvasGroup.alpha = 0;
                _boardCategories[i].Questions[j].canvasGroup.interactable = false;
            }
        }
    }

    internal void OpenBoard()
    {
        ResetBoard();
        ShowCategories();
    }

    private void ShowCategories()
    {
        _line.gameObject.SetActive(true);

        // Fade in the category titles
        for (int i = 0; i < _boardCategories.Count; i++)
        {
            _boardCategories[i].Title.DOFade(1, _appearDuration)
                .SetEase(Ease.OutSine)
                .SetDelay(_categoryFadeDelay * i);
        }

        _line.DOScaleX(1, _appearDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(ShowQuestions);
    }

    /// <summary>
    /// Show the questions for the selected category
    /// </summary>
    private void ShowQuestions()
    {
        // Fade in the questions
        for (int i = 0; i < _boardCategories.Count; i++)
        {
            for (int j = 0; j < _boardCategories[i].Questions.Length; j++)
            {
                _boardCategories[i].Questions[j].canvasGroup.DOFade(1, _appearDuration)
                    .SetEase(Ease.OutSine)
                    .SetDelay((_questionFadeDelay * i) + ((_questionFadeDelay * _boardCategories.Count) * j));
            }
        }

        // TODO: wait until the last question is faded in
        ToggleQuestionInteraction(true);

        TriviaQuestion.QuestionClicked += OnQuestionClicked;
    }

    /// <summary>
    /// Start the sequence of opening the question once it has been clicked
    /// </summary>
    /// <param name="question"></param>
    private void OnQuestionClicked(TriviaQuestion question)
    {
        TriviaQuestion.QuestionClicked -= OnQuestionClicked;
        _selectedQuestion = question;

        ToggleQuestionInteraction(false);
        ShowFocussedCategory();

        // Fade in the color, then open the card
        question.FadeBackgroundColor(_questionSelectedColor, _questionPointsColor, _questionRecolorDuration, () =>
        {
            Vector2 movePos = new Vector2(Screen.width / 2 * _questionPanelCenterFactor.x, Screen.height / 2 * _questionPanelCenterFactor.y);
            question.OpenCard(_questionPanelSize, _questionPointRescale, movePos, _questionScaleDuration);
        });
    }

    /// <summary>
    /// Toggle the interaction of all the questions
    /// </summary>
    /// <param name="enable">TRUE if all questions should be interactable</param>
    private void ToggleQuestionInteraction(bool enable)
    {
        for (int i = 0; i < _boardCategories.Count; i++)
        {
            for (int j = 0; j < _boardCategories[i].Questions.Length; j++)
            {
                _boardCategories[i].Questions[j].canvasGroup.interactable = enable;
            }
        }
    }

    /// <summary>
    /// Fade in the category that was selected
    /// </summary>
    private void ShowFocussedCategory()
    {
        _categorySelectedTitle.gameObject.SetActive(true);
        _categorySelectedTitle.text = _selectedQuestion.parentCategory.Title.text;

        // Hide all the categories
        Sequence hideSequence = DOTween.Sequence();
        for (int i = 0; i < _boardCategories.Count; i++)
        {
            hideSequence.Join(_boardCategories[i].Title.DOFade(0, _categoryFadeDuration).SetEase(Ease.OutSine));
        }

        hideSequence.Play();

        // Once the sequence is complete then show the category we are focussed on
        hideSequence.OnComplete(() => { 
            _categorySelectedTitle.DOFade(1, _categoryFadeDuration); 
        });
    }

    internal void HideBoard(Action OnComplete)
    {
        _categorySelectedTitle.DOFade(0, _categoryFadeDuration);
        _selectedQuestion.HideInfo(() => { 
            OnQuestionHidden(OnComplete); 
        });
    }

    private void OnQuestionHidden(Action OnComplete)
    {
        // Hide the rest of the questions
        _categoryParent.SetActive(false);
        _selectedQuestion.MovePanelUp(_hideMask, () => {
            _line.DOScaleX(0, _categoryFadeDelay).SetEase(Ease.OutSine);
            OnComplete?.Invoke(); 
        });
    }
}

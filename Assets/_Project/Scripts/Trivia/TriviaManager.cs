using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaManager : MonoBehaviour
{
    private bool _hasStarted;
    private string _triviaJsonPath = "Data/Trivia";
    private TriviaData _trivia;

    [SerializeField] private GameObject _triviaPanel;
    [SerializeField] private CanvasGroup _triviaCanvasGroup;
    [SerializeField] private float _fadeInDuration = 1f;

    [SerializeField] private CategoryPreviewer _categoryPreviewer;
    [SerializeField] private TriviaBoard _triviaBoard;
    [SerializeField] private QuestionInputHandler _questionHandler;

    public void Awake()
    {
        _triviaPanel.SetActive(false);
        _triviaCanvasGroup.alpha = 0;
    }

    public void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _trivia = GetJsonData();

        // Initialization of the systems
        _categoryPreviewer.Initialize(_trivia.categories);
        _triviaBoard.Initialize(_trivia.categories);
    }

    private TriviaData GetJsonData()
    {
        TextAsset json = Resources.Load(_triviaJsonPath) as TextAsset;
        return JsonUtility.FromJson<TriviaData>(json.text);
    }

    internal void StartGame()
    {
        if (_hasStarted) { return; }
        _hasStarted = true;

        _triviaPanel.SetActive(true);
        _triviaCanvasGroup.DOFade(1, _fadeInDuration).OnComplete(() =>
        {
            //StartCategoryPreview();
            StartTriviaBoard();
        });
    }

    private void StartCategoryPreview()
    {
        CategoryPreviewer.CategoriesPreviewed += OnCategoriesPreviewed;
        _categoryPreviewer.StartPreview();
    }

    private void OnCategoriesPreviewed()
    {
        CategoryPreviewer.CategoriesPreviewed -= OnCategoriesPreviewed;
        _categoryPreviewer.Disable();

        StartTriviaBoard();
    }

    private void StartTriviaBoard()
    {
        _triviaBoard.OpenBoard();
        TriviaQuestion.QuestionDisplayed += OnQuestionDisplayed;
    }

    private void OnQuestionDisplayed(TriviaQuestion question)
    {
        TriviaQuestion.QuestionDisplayed -= OnQuestionDisplayed;

        // Create a timer before moving to the other view
        IEnumerator WaitUntilHide()
        {
            yield return new WaitForSeconds(3f);
            StageManager.Instance.ChangeView(StagePresets.Contestants);

            // Hide the trivia board
            _triviaBoard.HideBoard(() =>
            {
                _triviaCanvasGroup.DOFade(0, 0.5f);
            });
        }
        StartCoroutine(WaitUntilHide());
    }

    private void OnBuzzerPressed()
    {
        //_questionHandler.StartInput(question);
    }
}

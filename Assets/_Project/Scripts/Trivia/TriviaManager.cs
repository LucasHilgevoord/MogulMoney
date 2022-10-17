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
    private float _hideQuestionPanelDelay = 3f;

    [SerializeField] private CategoryPreviewer _categoryPreviewer;
    [SerializeField] private TriviaBoard _triviaBoard;
    [SerializeField] private BuzzerHandler _buzzerHandler;
    [SerializeField] private QuestionInputHandler _questionHandler;

    private TriviaQuestion _currentQuestion;

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


        _buzzerHandler.SetupStage("What is the capital of France?");
        _buzzerHandler.StartBuzzer(4);
        return;

        StageManager.Instance.ChangeView(StagePresets.Front);

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
        _currentQuestion = question;

        // Setup the buzzer handler
        _buzzerHandler.SetupStage(_currentQuestion.QuestionData.question);
        StageManager.Instance.ChangeView(StagePresets.Contestants);

        // Wait a bit until we hide the panel
        StartCoroutine(HideQuestionPanel());
    }

    private IEnumerator HideQuestionPanel()
    {
        yield return new WaitForSeconds(_hideQuestionPanelDelay);

        // Hide the trivia board
        bool isHidden = false;
        _triviaBoard.HideBoard(() =>
        {
            _triviaCanvasGroup.DOFade(0, 0.5f);
            isHidden = true;

            
        });

        // Wait until we have hidden the board
        while (isHidden) { yield return null; }

        yield return new WaitForSeconds(1f);

        BuzzerHandler.ButtonPressed += OnBuzzerPressed;
        _buzzerHandler.StartBuzzer(10);
    }

    private void OnBuzzerPressed(int candidate)
    {
        StageManager.Instance.ChangeView(StagePresets.SingleContestant, new object[] { candidate });
        //_questionHandler.StartInput(question);
    }
}

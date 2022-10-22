using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriviaManager : MonoBehaviour
{
    private bool _hasStarted;
    private string _triviaJsonPath = "Data/TriviaData";
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
    private Contestant _answeringContestant;

    private float _buzzerTime = 10f;
    private float _answerTime = 30f;

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

        // TESTING
        //_currentQuestion = _triviaBoard._boardCategories[0].Questions[0];
        //OnBuzzerPressed(1);
        //return;
        // TESTING

        StartTriviaUI();
    }

    private void StartTriviaUI()
    {
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
            _triviaCanvasGroup.DOFade(0, 0.5f).OnComplete(() => { _triviaPanel.SetActive(false); });
            isHidden = true;
        });

        // Wait until we have hidden the board
        while (isHidden) { yield return null; }
        yield return new WaitForSeconds(1f);

        StartBuzzer();
    }

    private void StartBuzzer()
    {
        _buzzerHandler.SetupPanel(_currentQuestion.QuestionData.question);

        BuzzerHandler.BuzzerPressed += OnBuzzerPressed;
        _buzzerHandler.StartBuzzer(_buzzerTime); 
    }

    private void OnBuzzerPressed(int candidate)
    {
        BuzzerHandler.BuzzerPressed -= OnBuzzerPressed;
        if (candidate == -1)
        {
            // No one pressed
        } else
        {
            _answeringContestant = PlayerManager.Instance.Contestants[candidate];
            StartCoroutine(ShowQuestionInput());
        }
    }

    private IEnumerator ShowQuestionInput()
    {
        StageManager.Instance.ChangeLight(LightingGroup.SingleContestant, 0.5f);

        // Fade out the other displays a bit
        foreach (Contestant contestant in PlayerManager.Instance.Contestants)
        {
            if (contestant == _answeringContestant) { continue; }
            contestant.Display.TogglePanelActivity(false);
        }

        yield return new WaitForSeconds(2f);
        QuestionInputHandler.QuestionAnswered += OnQuestionAnswered;

        _questionHandler.SetupPanel(_currentQuestion.QuestionData, _answerTime);
        _questionHandler.ShowPanel();
    }

    private void OnQuestionAnswered(string answer, bool correct)
    {
        QuestionInputHandler.QuestionAnswered -= OnQuestionAnswered;
        _questionHandler.HidePanel(0, () => StartCoroutine(SetupAnswerDisplay(answer, correct)));
    }

    private IEnumerator SetupAnswerDisplay(string answer, bool correct)
    {
        yield return new WaitForSeconds(1f);

        _answeringContestant.Display.EnableAnswerPanel(answer);
        _answeringContestant.Display.ShowCorrectColor(correct, 5f);
    }   
}

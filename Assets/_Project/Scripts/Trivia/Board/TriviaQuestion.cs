using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class TriviaQuestion : MonoBehaviour
{
    public static event Action<TriviaQuestion> QuestionClicked;
    public static event Action<TriviaQuestion> QuestionDisplayed;
    public BoardCategory parentCategory { get; private set; }

    [Header("Parameters")]
    public RectTransform panel;
    public Image background;
    public TextMeshProUGUI pointLabel;
    public TextMeshProUGUI questionLabel;
    public CanvasGroup canvasGroup;
    
    [SerializeField] private RectTransform _rectTransform;
    private Tween scaleTween;

    private bool IsInteractable => canvasGroup.interactable;

    [Header("Data")]
    public TriviaQuestionData QuestionData;

    [Header("Values")]
    private float _enterScale = 1.1f;
    private float _hooverScaleDuration = 0.2f;
    private float _showDuration = 0.2f;
    
    internal void Initialize(TriviaQuestionData question, BoardCategory parentCategory)
    {
        this.parentCategory = parentCategory;
        QuestionData = question;

        pointLabel.text = QuestionData.points.ToString();
        questionLabel.text = QuestionData.question;
    }

    public void OnPointerEnter()
    {
        if (!IsInteractable) { return; }

        if (scaleTween != null) DOTween.Kill(scaleTween);
        scaleTween = _rectTransform.DOScale(_enterScale, _hooverScaleDuration).SetEase(Ease.OutSine);
    }

    public void OnPointerExit()
    {
        if (!IsInteractable) { return; }

        if (scaleTween != null) DOTween.Kill(scaleTween);
        scaleTween = _rectTransform.DOScale(1, _hooverScaleDuration).SetEase(Ease.InSine);
    }

    public void OnPointerClick()
    {
        if (!IsInteractable) { return; }
        QuestionClicked?.Invoke(this);
    }

    internal void FadeBackgroundColor(Color backgroundColor, Color pointsColor, float duration, Action OnComplete)
    {
        pointLabel.DOColor(pointsColor, duration);
        background.DOColor(backgroundColor, duration).OnComplete(() => { OnComplete?.Invoke(); });
    }

    internal void OpenCard(Vector2 cardScale, float pointsSize, Vector2 pos, float duration)
    {
        DOTween.Kill(scaleTween);

        // Sort the canvas group so it can be placed ontop of all the other things
        Canvas sortingCanvas = this.AddComponent<Canvas>();
        sortingCanvas.overrideSorting = true;
        sortingCanvas.sortingOrder = 1;


        Sequence sequence = DOTween.Sequence();
        float rectWidth = _rectTransform.rect.width;
        sequence.Append(DOTween.To(() => rectWidth, x => rectWidth = x, cardScale.x, duration)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
        {
            _rectTransform.sizeDelta = new Vector2(rectWidth, _rectTransform.rect.height);
        }));

        float rectHeight = _rectTransform.rect.height;
        sequence.Join(DOTween.To(() => rectHeight, x => rectHeight = x, cardScale.y, duration)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.rect.width, rectHeight);
        }));

        sequence.Join(background.transform.DOMove(new Vector2(pos.x, pos.y), duration).SetEase(Ease.InOutSine));
        sequence.Join(pointLabel.transform.DOMove(new Vector2(pos.x, pos.y), duration).SetEase(Ease.InOutSine));
        sequence.Join(pointLabel.transform.DOScale(pointsSize, duration).SetEase(Ease.InOutSine));

        sequence.OnComplete(ShowInfo);
        sequence.Play();
    }

    private void ShowInfo()
    {
        questionLabel.alpha = 0;
        questionLabel.gameObject.SetActive(true);
        
        Sequence infoSequence = DOTween.Sequence();
        infoSequence.SetDelay(0.5f);
        infoSequence.Append(pointLabel.DOFade(0, _showDuration));
        infoSequence.Append(questionLabel.DOFade(1, _showDuration));

        infoSequence.Play();
        infoSequence.OnComplete(() => { 
            QuestionDisplayed?.Invoke(this); 
        });
    }

    internal void HideInfo(Action OnComplete)
    {
        questionLabel.DOFade(0, _showDuration).OnComplete(() => { 
            OnComplete?.Invoke(); 
        });
    }

    internal void MovePanelUp(Transform newParent, Action OnComplete)
    {
        // We need it to go to a new mask parent so we can move it out
        panel.SetParent(newParent);
        background.transform.DOLocalMoveY(panel.rect.height, 0.5f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => { OnComplete?.Invoke(); }); ;
    }
}

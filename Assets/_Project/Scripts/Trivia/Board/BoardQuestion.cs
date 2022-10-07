using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class BoardQuestion : MonoBehaviour
{
    public static event Action<BoardQuestion> QuestionClicked;
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
    public TriviaQuestion Question;

    [Header("Values")]
    [SerializeField] private float _enterScale = 1.1f;
    [SerializeField] private float _hooverScaleDuration;
    [SerializeField] private float _scaleOffsetFactor;
    
    internal void Initialize(TriviaQuestion question, BoardCategory parentCategory)
    {
        this.parentCategory = parentCategory;
        pointLabel.text = question.points.ToString();
        questionLabel.text = question.question;
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

        Canvas sortingCanvas = this.AddComponent<Canvas>();
        sortingCanvas.overrideSorting = true;
        sortingCanvas.sortingOrder = 1;

        sequence.OnComplete(ShowInfo);
        sequence.Play();
    }

    private void ShowInfo()
    {
        questionLabel.alpha = 0;
        questionLabel.gameObject.SetActive(true);
        
        Sequence infoSequence = DOTween.Sequence();
        infoSequence.SetDelay(0.5f);
        infoSequence.Append(pointLabel.DOFade(0, 0.5f));
        infoSequence.Append(questionLabel.DOFade(1, 0.5f));

        infoSequence.Play();
    }
}

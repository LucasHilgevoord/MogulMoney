using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryPreviewer : MonoBehaviour
{
    public static Action CategoriesPreviewed;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform _categoryParent;
    [SerializeField] private CategoryPreview _categoryPreviewPrefab;

    private int _currentIndex = 0;
    private TriviaCategoryData[] _categories;
    private List<CategoryPreview> _previews;
    
    [SerializeField] private float _scrollDuration = 0.75f;
    [SerializeField] private float _appearDuration = 1f;
    [SerializeField] private float _disappearDelay = 1f;

    internal void Initialize(TriviaCategoryData[] categories)
    {
        _categories = categories;
        CreateLineUp();

        // Set the parent start position so we don't see the first one immediately
        _categoryParent.localPosition =  Vector3.right * Screen.width;
    }

    private void CreateLineUp()
    {
        _previews = new List<CategoryPreview>();
        for (int i = 0; i < _categories.Length; i++)
        {
            CategoryPreview category = Instantiate(_categoryPreviewPrefab, _categoryParent);
            category.CategoryTitle.text = _categories[i].title;

            _previews.Add(category);
        }
    }

    internal void StartPreview()
    {
        _canvasGroup.DOFade(1, _appearDuration).OnComplete(NextCategory);
    }
    
    private void NextCategory()
    {
        _categoryParent.DOLocalMoveX(-Screen.width * _currentIndex, _scrollDuration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            _previews[_currentIndex].CategoryTitle.DOFade(1, _appearDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _previews[_currentIndex].CategoryTitle.DOFade(0, _appearDuration)
                .SetEase(Ease.OutSine)
                .SetDelay(_disappearDelay)
                .OnComplete(() => {
                    if (_currentIndex < _categories.Length - 1)
                    {
                        _currentIndex++;
                        NextCategory();
                    }
                    else
                    {
                        FinishedPreview();
                    }
                });
                
            });
        });
        _previews[_currentIndex].CoinImage.DOFade(0, _scrollDuration * 0.5f)
            .SetDelay(_scrollDuration * 0.75f)
            .SetEase(Ease.OutSine);
    }

    private void FinishedPreview()
    {
        _canvasGroup.DOFade(0, _appearDuration).OnComplete(CategoriesPreviewed.Invoke);
    }

    internal void Disable()
    {
        gameObject.SetActive(false);
    }
}

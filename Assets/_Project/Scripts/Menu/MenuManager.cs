using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _backUI;
    [SerializeField] private Canvas _UICanvas;

    [SerializeField] private RawImage _videoUI;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private AudioSource _audioSource;
    private float _videoFadeOffset = 1f;
 
    private List<GameObject> _enabledUI = new List<GameObject>();

    private void Start()
    {
        _mainMenuUI.SetActive(true);
        _enabledUI.Add(_mainMenuUI);
        
        // Disable the back button at start
        _backUI.SetActive(false);
    }

    public void OpenNewMenu(GameObject newMenu)
    {
        // Disable the current menu
        _enabledUI[_enabledUI.Count - 1].SetActive(false);

        // Add the new menu and open it
        _enabledUI.Add(newMenu);
        newMenu.SetActive(true);

        // Only show the back button if we have more than one menu open.
        if (_enabledUI.Count > 1)
            _backUI.SetActive(true);
    }


    public void PressedBackButton()
    {
        if (_enabledUI.Count > 1)
        {
            _enabledUI[_enabledUI.Count - 1].SetActive(false);
            _enabledUI.RemoveAt(_enabledUI.Count - 1);
            
            _enabledUI[_enabledUI.Count - 1].SetActive(true);
        }

        // Disable the back button if we only have the first menu open
        if (_enabledUI.Count == 1)
            _backUI.SetActive(false);
    }

    public void StartGame()
    {
        _enabledUI[_enabledUI.Count - 1].SetActive(false);
        _videoUI.gameObject.SetActive(true);
        _audioSource.volume = 0;

        _videoUI.DOFade(1, 1).OnComplete(() =>
        {
            _UICanvas.gameObject.SetActive(false);
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        });
        _audioSource.DOFade(1, 1);

        _audioSource.Play();
        _videoPlayer.Play();
        StartCoroutine(StopVideo(_videoPlayer.clip.length / _videoPlayer.playbackSpeed - _videoFadeOffset));
    }

    private IEnumerator StopVideo(double duration)
    {
        Debug.Log(duration);
        yield return new WaitForSeconds((float)duration);

        Debug.Log("NOW");
        Color c = _videoUI.color;
        DOTween.ToAlpha(() => c, x => c = x, 0, _videoFadeOffset)
            .OnUpdate(() =>
            {
                _videoUI.color = c;
            });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionInputHandler : MonoBehaviour
{
    public static event Action<bool> QuestionAnswered;

    [SerializeField] private TextMeshProUGUI _categoryLabel;
    [SerializeField] private TextMeshProUGUI _questionLabel;
    [SerializeField] private TMP_InputField _answerInput;

    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _passButton;

    private TriviaQuestion question;

    private void ResetPanels()
    {
        
    }

    internal void StartInput(TriviaQuestion question)
    {
        this.question = question;

        // Set the catogory label to match the selected question
        _categoryLabel.text = question.parentCategory.Title.text;

        // Set the question label to match the selected question
        _questionLabel.text = question.QuestionData.question;

        // TODO: Create an opening sequence for the input
        this.gameObject.SetActive(true);
    }

    public void ConfirmAnswer()
    {
        ToggleButtonInteraction(false);
        ShowAnswer();
    }

    public void PassQuestion()
    {
        ToggleButtonInteraction(false);
        _answerInput.text = "";
        ShowAnswer();
    }

    private bool CheckAnswer()
    {
        foreach (string possibleAnswer in question.QuestionData.answer)
        {
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

        Debug.Log(isCorrect);

        QuestionAnswered?.Invoke(isCorrect);
    }

    private void ToggleButtonInteraction(bool interactable)
    {
        _confirmButton.interactable = interactable;
        _passButton.interactable = interactable;
    }
}

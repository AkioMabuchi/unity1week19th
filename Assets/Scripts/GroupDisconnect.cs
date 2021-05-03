using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroupDisconnect : SingletonMonoBehaviour<GroupDisconnect>
{
    [SerializeField] private GameObject gameObjectImageForm;
    [SerializeField] private GameObject gameObjectTextMeshProHeader;
    [SerializeField] private GameObject gameObjectTextMeshProMessage;

    private TextMeshProUGUI _textMeshProHeader;
    private TextMeshProUGUI _textMeshProMessage;

    private void OnEnable()
    {
        _textMeshProHeader = gameObjectTextMeshProHeader.GetComponent<TextMeshProUGUI>();
        _textMeshProMessage = gameObjectTextMeshProMessage.GetComponent<TextMeshProUGUI>();
    }

    public void ShowForm(string textHeader, string textMessage)
    {
        _textMeshProHeader.text = textHeader;
        _textMeshProMessage.text = textMessage;
        
        gameObjectImageForm.SetActive(true);
    }

    public void HideForm()
    {
        gameObjectImageForm.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Tooltip : MonoBehaviour
{
    [SerializeField] GameObject tipGameObject;
    [SerializeField] Text tooltipText;
    [SerializeField] Text CloseText;

    public string CloseString { get => closeString; set => closeString = value; }
    [SerializeField] string closeString = "������� ������ Y ����� �������.";

    [SerializeField] InputActionReference controllerTipButton;
    [SerializeField] Image Image;

    public Action EndOfButtonHold;

    DI di;

    private bool canClose;
    [SerializeField] private int time;

    public void ChangeTooltipText(string value)
    {
        if(!tipGameObject.activeSelf)
            tooltipText.text = value;
    }

    public void ChangeTooltipCloseText(string value)
    {
        if (!tipGameObject.activeSelf)
            tooltipText.text = value;
    }

    public void ShowTipWithTimer()
    {
        ResetTime();
        tipGameObject.SetActive(true);
        di.holdThreeSeconds.ReduceTime += di.tooltip.ReduceTime;
        di.holdThreeSeconds.ResetTime += di.tooltip.ResetTime;
        canClose = false;
    }
    public void ShowTip()
    {
        tipGameObject.SetActive(true);
        canClose = false;
    }

    public void CloseTip()
    {
        tipGameObject.SetActive(false);
        canClose = true;
    }
    public void ReduceTime()
    {
        time = time-1;
        UpdateTimerText();
        if (time <= 0)
        {
            EndOfButtonHold?.Invoke();
            tipGameObject.SetActive(false);
        }
            
    }
    public void ResetTime()
    {
        time=3;
        UpdateTimerText();
    }

    public void UpdateTimerText()
    {
        CloseText.text = $"����������� ����� ����������� {time} ������� ��� ������ �� ��������";
    }

    public void CloseImage()
    {
        Image.enabled = false;
    }

    public void ChangeImage(Sprite _sprite)
    {
        if(_sprite == null)
            CloseImage();
        else
        {
            Image.enabled = true;
            Image.sprite = _sprite;
        }
        
    }


    private void OnEnable()
    {
        di = DI.instance;
        time = 3;

        if (tipGameObject == null)
            Debug.Log("������� ��������� ������� " + gameObject.name);

        if (controllerTipButton == null)
            Debug.Log("������� ������ ��� ��������� �� " + gameObject.name);

        if (CloseText == null)
            Debug.Log("������� ����� �������� ������� " + gameObject.name);

        if (Image == null)
            Debug.Log("������� �������� ������� " + gameObject.name);

        CloseText.text = "";
        canClose = true;

        controllerTipButton.action.performed += TipPress;
    }
    private void OnDisable()
    {
        controllerTipButton.action.performed -= TipPress;
        di.holdThreeSeconds.ReduceTime -= di.tooltip.ReduceTime;
        di.holdThreeSeconds.ResetTime -= di.tooltip.ResetTime;
    }
    private void TipPress(InputAction.CallbackContext obj)
    {
        if(canClose)
            tipGameObject.SetActive(!tipGameObject.activeSelf);
    }
}

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
    [SerializeField] private Text CloseText;

    public string closeString
    {
        get => _closeString;
        set
        {
            _closeString = value;
            CloseText.text = _closeString;
        }
    }
    //��� ���������� �� ������ ������ ���� ��������� �������, � ����� �������� �������� ����� �� ����� ����������
    //�� ���� ��������� ����������� � ������ �� �� �� ������ ������ ���� �����
    [SerializeField] string _closeString = "������� ������ Y ����� �������.";

    public string simpleTooltipText { get; set; }
    public string timerTooltipText { get; set; }

    [SerializeField] InputActionReference controllerTipButton;
    [SerializeField] Image Image;

    public Action EndOfButtonHold;

    DI di;

    enum StateTip
    {
        SimpleTip,
        TimerTip,
        Closed
    }

    private StateTip stateTip = StateTip.Closed;

    [SerializeField] private int time;

    public void ChangeTooltipText(string value)
    {
        tooltipText.text = value;
    }

    /*
    public void ChangeTooltipCloseText(string value)
    {
        closeString = value;
    }*/

    public void ShowTipWithTimer()
    {
        ResetTime();
        tipGameObject.SetActive(true);
        ChangeTooltipText(timerTooltipText);
        UpdateTimerText();
        DI.instance.holdThreeSeconds.ReduceTime += ReduceTime;
        DI.instance.holdThreeSeconds.ResetTime += ResetTime;
        stateTip = StateTip.TimerTip;
    }
    public void ShowSimpleTip()
    {
        if(stateTip!=StateTip.TimerTip)
        {
            tipGameObject.SetActive(true);
            ChangeTooltipText(simpleTooltipText);
            if (closeString.Length == 0)
            {
                closeString = "������� ������ Y ����� �������.";
            }
            stateTip = StateTip.SimpleTip;
        }
        
    }

    public void CloseSimpleTip()
    {
        if (stateTip != StateTip.TimerTip)
        {
            tipGameObject.SetActive(false);
            closeString = "";
            stateTip = StateTip.Closed;
        }
        
    }
    public void ReduceTime()
    {
        time = time-1;
        UpdateTimerText();
        if (time <= 0)
        {
            EndOfButtonHold?.Invoke();
            di.holdThreeSeconds.ReduceTime -= ReduceTime;
            di.holdThreeSeconds.ResetTime -= ResetTime;
            tipGameObject.SetActive(false);
            closeString = "";
            stateTip = StateTip.Closed;
        }
            
    }
    public void ResetTime()
    {
        time=3;
        UpdateTimerText();
    }

    public void UpdateTimerText()
    {
        closeString = $"����������� ����� ����������� {time} ������� ��� ������ �� ��������";
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

        tooltipText.text = "���������";
        closeString = "";
        stateTip = StateTip.Closed;

        controllerTipButton.action.performed += TipPress;
    }
    private void OnDisable()
    {
        controllerTipButton.action.performed -= TipPress;
        DI.instance.holdThreeSeconds.ReduceTime -= ReduceTime;
        DI.instance.holdThreeSeconds.ResetTime -= ResetTime;
    }
    private void TipPress(InputAction.CallbackContext obj)
    {
        //���� ��������� � ��������� ������� ��������� ��� �������
        if (stateTip == StateTip.SimpleTip)
            CloseSimpleTip();
        else if (stateTip == StateTip.Closed)
        {
            closeString = "������� ������ Y ����� �������.";
            ShowSimpleTip();
        }
            
    }
}

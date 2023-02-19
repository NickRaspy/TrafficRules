using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// ��������� ������� ���������������� ���� �� ������ ����� �������� �������
/// ������� � ����������� ����� ����� ������� �������������
/// </summary>

public class Quest : MonoBehaviour
{
    [Space]
    public UnityEvent eventBeforeStartSteps = new UnityEvent();

    public int CurrentNumberStep { get; private set; }
    public Step[] steps;


    [Tooltip("������� ������ ��� �������� � ���������� ����")]
    [SerializeField] InputActionReference controllerNextStep;

    
    //______________________system variables
    //���� �������� ������� �� ����
    private AudioSource auDictor;
    IEnumerator waitSpeachDictor;
    Action playstep;
    public int testStep;
    //_______________________
    
    private void Start()
    {
        CurrentNumberStep = 0;
    }

    public void StartFirstStep()
    {
        if (steps.Length > 0)
        {
            steps[CurrentNumberStep].StartStep();
            steps[CurrentNumberStep].StepComplete += NextStep;
        }
        else
            Debug.Log("����� ���");
        
    }

    protected virtual void OnEnable()
    {
        controllerNextStep.action.performed += ClickNextStepButton;
    }

    protected virtual void OnDisable()
    {
        controllerNextStep.action.performed -= ClickNextStepButton;
    }

    void ClickNextStepButton(InputAction.CallbackContext obj)
    {
        if (steps[CurrentNumberStep].clickToNextStep)
            NextStep();
    }

    private void NextStep()
    {
        if (steps.Length > 0)
        {
            //�������� ���������� ��� ���� �� �� ������
            if (CurrentNumberStep > 0)
            {
                steps[CurrentNumberStep].StepComplete -= NextStep;
            }

            //���������� ����
            if(auDictor.isPlaying)
            {
                auDictor.Stop();
            }
            auDictor.clip = null;

            //��������� ��� ������� ��� �� ��������� �� ����������
            if (CurrentNumberStep < steps.Length)
            {
                CurrentNumberStep++;

                steps[CurrentNumberStep].StartStep();

                //���� ���� �� ����
                if(steps[CurrentNumberStep].dictor!=null)
                {
                    //�������� � ���������� ���� �� ����
                    auDictor.clip = steps[CurrentNumberStep].dictor;
                    //�������� ����
                    auDictor.Play();
                }

                //������ ��������� �� ������, � �� �� ���� �.�. ���� ����� �������� ��������� ���
                //� ���� �� ������ ������������� ���� �� �����, � ��������� ��� ����� ������ ������� �.�. ������ �� ������ ���������� � ������

                //���� ��������� � ���������� ���� ����� ������� �� ��������� �������� ������� ��� ��� ����������
                if (steps[CurrentNumberStep].nextAfterDictor)
                {
                    if (waitSpeachDictor != null)
                    {
                        StopCoroutine(waitSpeachDictor);
                        waitSpeachDictor = null;
                        auDictor.Stop();
                        auDictor.clip = null;
                    }
                    waitSpeachDictor = WaitSpeachDictor();
                    StartCoroutine(waitSpeachDictor);
                }

                steps[CurrentNumberStep].StepComplete += NextStep;
            }
            else
            {
                //��� ���� ���������
                DI.instance.win.WinMethod();
            }
        }
        else
            Debug.Log("����� ���");
    }
    
    //��� ���� ������ �����������
    IEnumerator WaitSpeachDictor()
    {
        if (steps[CurrentNumberStep].dictor != null)
        {
            yield return new WaitForSeconds(auDictor.clip.length + 0.5f);
        }
        else
        {
            yield return null;
        }
        waitSpeachDictor = null;
        //�� ���������� ����� ��������� NextStep
        NextStep();
    }

}

/// <summary>
/// ������ �� ����� ����
/// ���� �� ��� ���������� ������� ����������
/// </summary>
[System.Serializable]
public class Step
{
    [SerializeField] private string name;
    
    [Tooltip("������� ��������")]
    public AudioClip dictor;

    [Space]
    [Tooltip("���������� ����� ����� ���� �������?")]
    public bool nextAfterDictor = false;

    //��� ���� ���������� ������� �� ������ ��� ����� ������� � ����� �� � DI ����������, � � ������� ������ �� ������ ��� 3 ����
    //�� � �� ���� ��� ��� �������
    [Space]
    [Tooltip("������� �� ��������� ������������� �� ���� ����?")]
    [SerializeField]private bool forcedShowToolTip = false;
    [Tooltip("��� � ����������")]
    [SerializeField] private bool likePrevious = true;
    [Tooltip("��������� ���������")]
    [SerializeField] private string tipText;

    [Space]
    [Tooltip("�������, ������� ������ ���� ����������� ��� ����������� ��������")]
    [SerializeField] private List<Interactive> shouldBeCompleted;

    [Space]
    [Tooltip("������� � ���������� ���� ����� �� ����� �� ������")]
    public bool clickToNextStep;

    [Space]
    [Tooltip("������� �������� ��������")]
    [SerializeField] private int RemainingActions;

    [Space]
    [SerializeField] private UnityEvent eventAfterStep = new UnityEvent();

    public Action StepComplete;
    
    //�� �������� ��� ������ ���� �����������, �� � �� ���� ����� �� ����������
    public void StartStep()
    {
        RemainingActions = shouldBeCompleted.Count;

        //���� ��������� �� ��� ����������
        if (!likePrevious)
        {
            //�������� ���������
            DI.instance.tooltip.ChangeTooltipText(tipText);
        }

        //���� ��������� �����������
        if (forcedShowToolTip)
        {
            //�������� ���������
            DI.instance.tooltip.ShowTip();
        }

        //����������� ����� OneActionHasCompleted �� ��� ������� CompleteAction �� ������� shouldBeCompleted
        foreach(Interactive item in shouldBeCompleted)
        {
            item.CompleteAction += OneActionHasCompleted;
        }
    }

    public void OneActionHasCompleted()
    {
        RemainingActions--;
        if(RemainingActions<=0)
        {
            //���������� ����� OneActionHasCompleted �� ��� ������� CompleteAction �� ������� shouldBeCompleted
            foreach (Interactive item in shouldBeCompleted)
            {
                item.CompleteAction -= OneActionHasCompleted;
            }
            eventAfterStep?.Invoke();
            StepComplete?.Invoke();
        }
    }
}

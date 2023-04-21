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
        auDictor = gameObject.GetComponent<AudioSource>();
        if (auDictor == null) auDictor = gameObject.AddComponent<AudioSource>();
        auDictor.volume = 0.8f;

        CurrentNumberStep = testStep;
        StartFirstStep();
    }

    public void StartFirstStep()
    {
        if (steps.Length > 0)
        {
            for (int i = 0; i < steps.Length; i++) // ���� �� ���� ��������� ������� Step
            {
                if (steps[i].shouldBeCompleted != null) // ���� ������ �� null
                {
                    List<Interactive> interactiveObjects = steps[i].shouldBeCompleted; // �������� ������ �� ������ �������� shouldBeCompleted �� �������� ��������

                    if (interactiveObjects.Count > 0) // ���� ������ �� ����
                    {
                        foreach (Interactive interactiveObject in interactiveObjects) // ���� �� ���� �������� ������
                        {
                            interactiveObject.enabled = false; // ������������� enabled � false
                        }
                    }
                }
                
            }

            eventBeforeStartSteps?.Invoke();
            DictorSoundOn();
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
        if (CurrentNumberStep < steps.Length)
        {
            if (steps[CurrentNumberStep].clickToNextStep)
            {
                steps[CurrentNumberStep].StopStep();
            }
        } 
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

            CurrentNumberStep++;

            //��������� ��� ������� ��� �� ��������� �� ����������
            if (CurrentNumberStep < steps.Length)
            {
                steps[CurrentNumberStep].StartStep();
                DictorSoundOn();

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
            //��� ���� ���������
        }
        else
            Debug.Log("����� ���");
    }

    private void DictorSoundOn()
    {
        //���� ���� �� ����
        if (steps[CurrentNumberStep].dictor != null)
        {
            //�������� � ���������� ���� �� ����
            auDictor.clip = steps[CurrentNumberStep].dictor;

            //�������� ����
            StartCoroutine(DictorOnStart());
        }
    }
    
    //������ ������� �� ��� ��������� �������� �����
    IEnumerator DictorOnStart()
    {
        if (CurrentNumberStep == 0)
        {
            yield return new WaitForSeconds(7f);
        }
        else
            yield return null;

        auDictor.Play();
    }

    //��� ���� ������ �����������
    IEnumerator WaitSpeachDictor()
    {
        steps[CurrentNumberStep].StartStep();
        if (steps[CurrentNumberStep].dictor != null)
        {
            if (CurrentNumberStep == 0)
                yield return new WaitForSeconds(7f);
            yield return new WaitForSeconds(auDictor.clip.length + 0.5f);
        }
        else
        {
            yield return null;
        }
        waitSpeachDictor = null;

        //�� ���������� ��������� ��� ����� ���������� ������� ����� ����
        steps[CurrentNumberStep].StopStep();
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
    public List<Interactive> shouldBeCompleted;

    [Space]
    [Tooltip("������� � ���������� ���� ����� �� ����� �� ������")]
    public bool clickToNextStep;

    [Space]
    [Tooltip("������� �������� ��������")]
    [SerializeField] private int RemainingActions;

    [Space]
    public UnityEvent eventAfterStep = new UnityEvent();

    public Action StepComplete;
    
    //�� �������� ��� ������ ���� �����������, �� � �� ���� ����� �� ����������
    public void StartStep()
    {
        if (shouldBeCompleted != null) // ���� ������ �� null
        {
            if (shouldBeCompleted.Count > 0) // ���� ������ �� ����
            {
                foreach (Interactive interactiveObject in shouldBeCompleted) // ���� �� ���� �������� ������
                {
                    interactiveObject.enabled = true; // ������������� enabled � true
                }
            }
        }

        //���� ��������� �� ��� ����������
        if (!likePrevious)
        {
            //�������� ���������
            DI.instance.tooltip.simpleTooltipText = tipText;
        }

        //���� ��������� �����������
        if (forcedShowToolTip)
        {
            //�������� ���������
            DI.instance.tooltip.ShowSimpleTip();
        }

        
        if(shouldBeCompleted.Count>0)
        {

            RemainingActions = shouldBeCompleted.Count;

            //����������� ����� OneActionHasCompleted �� ��� ������� CompleteAction �� ������� shouldBeCompleted
            for (int i = 0; i< shouldBeCompleted.Count; i++)
            {
                Interactive item = shouldBeCompleted[i];
                if (item != null)
                {
                    item.CompleteAction += OneActionHasCompleted;
                }
                else
                {
                    Debug.Log("� ���� " + name + $" ���� � �������� {i} �� ���������");
                }
            }
        }
        else if(!clickToNextStep && !nextAfterDictor)
        {
            Debug.Log("������� ��� " + name);
            StopStep();
        }
        else
        {
            //��� ����� ��� �������
            Debug.Log("� ���� " + name + " ��� ������������� ��������");
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
            StopStep();
        }
    }

    public void StopStep()
    {
        eventAfterStep?.Invoke();
        StepComplete?.Invoke();
    }
}

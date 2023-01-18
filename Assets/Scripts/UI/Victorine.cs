using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victorine : MonoBehaviour
{
    private DI di;

    //������ ��������
    public Question[] Questions;

    public int CurrentQuestion = 0;

    [SerializeField]private Color greenColor;
    [SerializeField]private Color redColor;

    private int CountAlreadyReplyAnswers = 0;

    private void Start()
    {
        //��������� ��� �������
        //��������� ��� ������� ��������

        //��� ������ ����� ����� ��������� �� �� ���, � �� ���-�� ���������
        if (Questions.Length > 0)
        {
            foreach (Question q in Questions)
            {
                q.CloseText.text = "";
                q.Canvas.enabled = false;
            }
        }
        else
        {
            Debug.Log("������� ������ Questions");
        }
    }

    public void Activate()
    {
        Questions[CurrentQuestion].Canvas.enabled = true;
    }

    //��������� ��� ������
    private void OnEnable()
    {
        
        foreach (Question q in Questions)
        {
            if (q.Answers!=null)
            {
                foreach (Answer a in q.Answers)
                    a.Button.onClick.AddListener(() => OnButtonClick(a));
            }
            else
            {
                Debug.Log("������� ������ Answers");
            }
        }
        
    }

    //��������� ��� ������
    private void OnDisable()
    {
        //��� ������ ����� ����� ��������� �� �� ���, � �� ���-�� ���������
        if (Questions.Length > 0)
        {
            foreach (Question q in Questions)
            {
                if (q.Answers.Length > 0)
                {
                    foreach (Answer a in q.Answers)
                        a.Button.onClick.RemoveAllListeners();
                }
                else
                {
                    Debug.Log("������� ������ Answers");
                }
            }
        }
        else
        {
            Debug.Log("������� ������ Questions");
        }
    }

    void OnButtonClick(Answer _answer)
    {
        if(CountAlreadyReplyAnswers < Questions[CurrentQuestion].CountRightAnswers)
        {
            //���������� ��������� ������ ������ ������
            if (_answer.IsRight)
                _answer.Button.image.color = greenColor;
            else
                _answer.Button.image.color = redColor;

            CountAlreadyReplyAnswers++;
        }
        
        if(CountAlreadyReplyAnswers >= Questions[CurrentQuestion].CountRightAnswers)
        {
            //�������� ������� ������� ����� �� 3 ���
            Questions[CurrentQuestion].UpdateCloseText();

            //���������� �������
            di.holdThreeSeconds.ReduceTime += Questions[CurrentQuestion].ReduceTime;
            //����� �������
            di.holdThreeSeconds.ResetTime += Questions[CurrentQuestion].ResetTime;

            Questions[CurrentQuestion].NextQuestion += NextQuestion;
        }
        
    }

    private void NextQuestion()
    {
        //���������� �������
        di.holdThreeSeconds.ReduceTime -= Questions[CurrentQuestion].ReduceTime;
        //����� �������
        di.holdThreeSeconds.ResetTime -= Questions[CurrentQuestion].ResetTime;

        //������� ������� ������
        Questions[CurrentQuestion].Canvas.enabled = false;

        //���� ������� ������ ���������
        //�� ���� �� ������

        //���� ������� ������ �� ��������� �� �������� ���������

        CurrentQuestion++;
        if (CurrentQuestion < Questions.Length)
            Questions[CurrentQuestion].Canvas.enabled = true;
    }

    

    
}

[System.Serializable]
public class Question
{
    //����� � ��������
    public Canvas Canvas;

    public Text CloseText;

    //����� ��� �������
    public Text TextQuestion;

    //������ �������(��� ������ ���� ������)
    public Answer[] Answers;

    public Action NextQuestion;

    //������� ������ ���� ������ �������?
    public int CountRightAnswers
    {
        get
        {
            if (countRightAnswers <= 0)
            {
                if (Answers.Length > 0)
                {
                    foreach (Answer a in Answers)
                    {
                        if (a.IsRight)
                            countRightAnswers++;
                    }
                }
                else
                {
                    Debug.Log("�������� ������ � ������");
                }
            }
            return countRightAnswers;
        }
        set
        {
            countRightAnswers = value;
        }
    }
    public void UpdateCloseText()
    {
        CloseText.text = $"����������� ����� ����������� {time} ������� ��� ������ �� ��������";
    }

    public void ReduceTime()
    {
        time = time - 1;
        UpdateCloseText();
        if (time <= 0)
            NextQuestion?.Invoke();
    }

    public void ResetTime()
    {
        time = 3;
        UpdateCloseText();
    }

    private int countRightAnswers = 0;
    private int time;

}

[System.Serializable]
public class Answer
{
    //������
    public Button Button;

    //������?
    public bool IsRight;
}


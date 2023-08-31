using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Victorine : MonoBehaviour
{
    [SerializeField] bool TurnOnInStart;
    //массив вопросов
    public Question[] Questions;

    public int CurrentQuestion = 0;
    public int RightAnsweredQuestions = 0;

    [SerializeField]private UnityEvent CompleteVictorine;

    [SerializeField]private Color greenColor;
    [SerializeField]private Color redColor;

    private DI di;
    [SerializeField]private int CountAlreadyReplyAnswers = 0;
    private bool isTheEnd = false;

    private void Start()
    {
        di = DI.instance;
        //выключить все канвасы
        //выключить все надписи закрытия
        
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
            Debug.Log("Заполни массив Questions");
        }

        if (TurnOnInStart)
        {
            Activate();
        }
    }

    public void Activate()
    {
        CountAlreadyReplyAnswers = 0;

        Questions[CurrentQuestion].Canvas.enabled = true;
        if(CurrentQuestion != 0) Questions[CurrentQuestion-1].Canvas.enabled = false;
        Questions[CurrentQuestion].Time = 3;
        
    }

    //забиндить все кнопки
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
                Debug.Log("Заполни массив Answers");
            }
        }
        
    }

    //отбиндить все кнопки
    private void OnDisable()
    {
        //тут скорее всего нужно проверять не на нул, а на кол-во элементов
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
                    Debug.Log("Заполни массив Answers");
                }
            }
        }
        else
        {
            Debug.Log("Заполни массив Questions");
        }
    }

    void OnButtonClick(Answer _answer)
    {
        //если мы ответили не на все вопросы
        if(CountAlreadyReplyAnswers < Questions[CurrentQuestion].CountRightAnswers)
        {
            //подсветить выбранную кнопку нужным цветом
            if (_answer.IsRight)
            {
                _answer.Button.image.color = greenColor;

                /*
                //тут опять же стоило подумать над архитектурой, но сроки горят
                di.tooltip.ChangeImage(di.Svet1);
                di.tooltip.ChangeTooltipText("Ура! Вы получаете светоотражающий брелок!");
                di.tooltip.UpdateCloseText();
                
                //включить табличку
                di.tooltip.ShowTip();*/

                Questions[CurrentQuestion].AfterRightAnswer?.Invoke();
                RightAnsweredQuestions++;
            }
            else
            {
                _answer.Button.image.color = redColor;
                Questions[CurrentQuestion].AfterWrongAnswer?.Invoke();
            }
                

            CountAlreadyReplyAnswers++;
        }
        
        //если мы ответили на все вопросы
        if(CountAlreadyReplyAnswers >= Questions[CurrentQuestion].CountRightAnswers)
        {

            //включить надпись зажмите курок на 3 сек
            Questions[CurrentQuestion].UpdateCloseText();
            Questions[CurrentQuestion].ResetTime();

            //уменьшение времени
            di.holdThreeSeconds.ReduceTime += Questions[CurrentQuestion].ReduceTime;
            //сброс времени
            di.holdThreeSeconds.ResetTime += Questions[CurrentQuestion].ResetTime;

            Questions[CurrentQuestion].NextQuestion += NextQuestion;
        }
        
    }

    private void NextQuestion()
    {
        //уменьшение времени
        di.holdThreeSeconds.ReduceTime -= Questions[CurrentQuestion].ReduceTime;
        //сброс времени
        di.holdThreeSeconds.ResetTime -= Questions[CurrentQuestion].ResetTime;

        //если текущий канвас последний
        //то ничё не делаем

        Questions[CurrentQuestion].NextQuestion -= NextQuestion;

        //если текущий канвас не последний то включаем следующий
        CurrentQuestion++;
        if (CurrentQuestion < Questions.Length)
            Activate();
        else CompleteVictorine?.Invoke();
    }
    public void EndVictorine(GameObject endScreen)
    {
        Questions[CurrentQuestion - 1].Canvas.enabled = false;
        endScreen.GetComponent<Canvas>().enabled = true;
        endScreen.transform.Find("TextQuestion").GetComponent<Text>().text = $"Тест окончен.\r\nВаш счет: {RightAnsweredQuestions} из 10";
    }
    public void ExitVictorine()
    {
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public class Question
{
    //экран с вопросом
    public Canvas Canvas;

    //массив ответов(это должны быть кнопки)
    public Answer[] Answers;

    public Text CloseText;

    public Action NextQuestion;
    
    public UnityEvent AfterRightAnswer;
    public UnityEvent AfterWrongAnswer;

    public int Time;

    private int countRightAnswers = 0;

    //сколько должно быть верных ответов?
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
                    Debug.Log("Добавьте ответы в массив");
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
        CloseText.text = $"Удерживайте курок контроллера {Time} секунды для перехода далее.";
    }

    public void ReduceTime()
    {
        Time = Time - 1;
        UpdateCloseText();
        if (Time <= 0)
            NextQuestion?.Invoke();
    }

    public void ResetTime()
    {
        Time = 3;
        UpdateCloseText();
    }

}

[System.Serializable]
public class Answer
{
    //кнопка
    public Button Button;

    //верный?
    public bool IsRight;
}


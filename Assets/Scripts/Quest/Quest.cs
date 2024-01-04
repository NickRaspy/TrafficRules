using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Квестовая система позаимствованная мной из одного моего прошлого проекта
/// который я переделывал после целой команды программистов
/// </summary>

public class Quest : MonoBehaviour
{
    [Space]
    public UnityEvent eventBeforeStartSteps = new UnityEvent();

    public int CurrentNumberStep { get; private set; }
    public Step[] steps;


    [Tooltip("Задайте кнопку для перехода к следующему шагу")]
    [SerializeField] InputActionReference controllerNextStep;

    
    //______________________system variables
    //сюда помещаем диктора из шага
    [SerializeField]private AudioSource auDictor;
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
            for (int i = 0; i < steps.Length; i++) // Цикл по всем элементам массива Step
            {
                if (steps[i].shouldBeCompleted != null) // Если список не null
                {
                    List<Interactive> interactiveObjects = steps[i].shouldBeCompleted; // Получаем ссылку на список объектов shouldBeCompleted из текущего элемента

                    if (interactiveObjects.Count > 0) // Если список не пуст
                    {
                        foreach (Interactive interactiveObject in interactiveObjects) // Цикл по всем объектам списка
                        {
                            interactiveObject.enabled = false; // Устанавливаем enabled в false
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
            Debug.Log("Шагов нет");
        
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

    public void NextStep()
    {
        if (steps.Length > 0)
        {
            //отписать предыдущий шаг если он не первый
            if (CurrentNumberStep > 0)
            {
                steps[CurrentNumberStep].StepComplete -= NextStep;
            }

            //остановить звук
            if(auDictor.isPlaying)
            {
                auDictor.Stop();
            }
            auDictor.clip = null;

            CurrentNumberStep++;

            //проверить что текущий шаг не превышает их количество
            if (CurrentNumberStep < steps.Length)
            {
                steps[CurrentNumberStep].StartStep();
                DictorSoundOn();

                //звуком управляем из квеста, а не из шага т.к. звук может досрочно завершить шаг
                //и звук не должен накладываться друг на друга, а завершить его можно только снаружи т.к. ссылка на кнопку завершения у квеста

                //если переходим к следующему шагу после диктора то запускаем корутину которая ждёт его завершения
                if (steps[CurrentNumberStep].nextAfterDictor)
                {
                    if (waitSpeachDictor != null)
                    {
                        StopCoroutine(waitSpeachDictor);
                        waitSpeachDictor = null;
                        auDictor.Stop();
                        //auDictor.clip = null;
                    }
                    waitSpeachDictor = WaitSpeachDictor();
                    StartCoroutine(waitSpeachDictor);
                }
                steps[CurrentNumberStep].StepComplete += NextStep;
            }
            //все шаги кончились
        }
        else
            Debug.Log("Шагов нет");
    }

    private void DictorSoundOn()
    {
        //если звук не нулл
        if (steps[CurrentNumberStep].dictor != null)
        {
            //вставить в аудиосоурс звук из шага
            auDictor.clip = steps[CurrentNumberStep].dictor;

            //включить звук
            StartCoroutine(DictorOnStart());
        }
    }
    
    //просто костыль хз как отследить загрузку сцены
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

    //ждём пока диктор выговорится
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

        //по завершении запускаем шаг чтобы отработали события после него
        steps[CurrentNumberStep].StopStep();
    }

}

/// <summary>
/// Данные об одном шаге
/// надо бы все переменные сделать приватными
/// </summary>
[System.Serializable]
public class Step
{
    [SerializeField] private string name;
    
    [Tooltip("Озвучка диктором")]
    public AudioClip dictor;

    [Space]
    [Tooltip("Продолжить сразу после речи диктора?")]
    public bool nextAfterDictor = false;

    //это надо наследника сделать от квеста для этого проекта и пусть он к DI обращается, а у старшей версии не делать эти 3 фичи
    //но я не знаю как это сделать
    [Space]
    [Tooltip("Вывести ли подсказку принудительно на этом шаге?")]
    [SerializeField]private bool forcedShowToolTip = false;
    [Tooltip("Как в предыдущем")]
    [SerializeField] private bool likePrevious = true;
    [Tooltip("Текстовая подсказка")]
    [SerializeField] private string tipText;
    [Space]
    [Tooltip("Если вместо диктора текстовое окно с заданиями")]
    [SerializeField] private bool taskWindowsInteadOfDictor;
    [Tooltip("Текст в окне с заданиями")]
    [SerializeField] private string taskText;
    [Space]
    [Tooltip("Объекты, которые должны быть выполнеными для продолжения сценария")]
    public List<Interactive> shouldBeCompleted;

    [Space]
    [Tooltip("Переход к следующему шагу будет по клику на кнопку")]
    public bool clickToNextStep;

    [Space]
    [Tooltip("Сколько действий осталось")]
    [SerializeField] private int RemainingActions;

    [Space]
    public UnityEvent eventAfterStep = new UnityEvent();

    public Action StepComplete;
    
    //по хорошему тут должен быть конструктор, но я не знаю когда он вызывается
    public void StartStep()
    {
        if (shouldBeCompleted != null) // если список не null
        {
            if (shouldBeCompleted.Count > 0) // если список не пуст
            {
                foreach (Interactive interactiveObject in shouldBeCompleted) // цикл по всем объектам списка
                {
                    interactiveObject.enabled = true; // устанавливаем enabled в true
                }
            }
        }

        //если подсказка не как предыдущая
/*        if (!likePrevious)
        {
            //заменить подсказку
            DI.instance.tooltip.simpleTooltipText = tipText;
        }*/

        //если подсказка обязательна
        if (forcedShowToolTip)
        {
            //показать подсказку
            DI.instance.tooltip.ShowSimpleTip();
        }
        if (taskWindowsInteadOfDictor)
        {
            DI.instance.tasksWindow.SetText(taskText);
            if(clickToNextStep) DI.instance.tasksWindow.closeText.enabled = true;
            else DI.instance.tasksWindow.closeText.enabled = false;
        }
        
        if(shouldBeCompleted.Count>0)
        {

            RemainingActions = shouldBeCompleted.Count;

            //подписываем метод OneActionHasCompleted на все события CompleteAction из массива shouldBeCompleted
            for (int i = 0; i< shouldBeCompleted.Count; i++)
            {
                Interactive item = shouldBeCompleted[i];
                if (item != null)
                {
                    item.CompleteAction += OneActionHasCompleted;
                }
                else
                {
                    Debug.Log("В шаге " + name + $" вещь с индексом {i} не назначена");
                }
            }
        }
        else if(!clickToNextStep && !nextAfterDictor)
        {
            Debug.Log("Скипаем шаг " + name);
            StopStep();
        }
        else
        {
            //ждём клика или диктора
            Debug.Log("В шаге " + name + " нет интерактивных объектов");
        }
        
    }

    public void OneActionHasCompleted()
    {
        RemainingActions--;
        if(RemainingActions<=0)
        {
            //отписываем метод OneActionHasCompleted на все события CompleteAction из массива shouldBeCompleted
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

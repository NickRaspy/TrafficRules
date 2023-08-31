using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowMessageInTrigger : MonoBehaviour
{
    protected DI di;

    public string Text { get => text; set => text = value; }
    [SerializeField] protected string text = "Конец";

    public Sprite Image { get => image; set => image = value; }
    [SerializeField] protected Sprite image = null;

    protected void Start()
    {
        di = DI.instance;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MainMethod();
        }
    }
    protected virtual void MainMethod()
    {
        DI.instance.tooltip.timerTooltipText = Text;
        DI.instance.tooltip.ChangeImage(Image);
        //включить табличку
        DI.instance.tooltip.ShowTipWithTimer();

        DI.instance.tooltip.EndOfButtonHold += LoadMainMenu;

        //отключить телепорт
        DI.instance.rightTeleportController.SetActive(false);
        DI.instance.leftTeleportController.SetActive(false);
    }

    protected void LoadMainMenu()
    {
        DI.instance.tooltip.EndOfButtonHold -= LoadMainMenu;
        SceneManager.LoadScene(0);
    }
}

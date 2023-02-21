using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    private DI di;

    private string winText = "���������";
    public GameObject arrow;

    public string WinText { get => winText; set => winText = value; }

    //bool tablNotSubscribe;
    void Start()
    {
        di = DI.instance;
        //tablNotSubscribe = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WinMethod();
        }
    }

    public void WinMethod()
    {
        if (arrow != null)
            arrow.SetActive(false);
        //������� ����� �� ��������
        di.tooltip.ChangeTooltipText(WinText);
        di.tooltip.UpdateCloseText();
        //di.tooltip.ChangeImage(di.Svet2);
        //�������� ��������
        di.tooltip.ShowTip();

        di.tooltip.EndOfButtonHold += LoadMainMenu;
        /*
        //����������� �������� � �������
        if (tablNotSubscribe)
        {
            di.holdThreeSeconds.ReduceTime += di.tooltip.ReduceTime;
            di.holdThreeSeconds.ResetTime += di.tooltip.ResetTime;
            tablNotSubscribe = false;
        }*/

        //��������� ��������
        di.rightTeleportController.SetActive(false);
        di.leftTeleportController.SetActive(false);
    }

    private void LoadMainMenu()
    {
        di.tooltip.EndOfButtonHold -= LoadMainMenu;
        SceneManager.LoadScene("MainMenu");
    }
}

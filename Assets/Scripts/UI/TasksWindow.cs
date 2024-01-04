using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TasksWindow : MonoBehaviour
{
    public Text mainText;
    public Text closeText;
    private void Awake()
    {
        closeText.enabled = false;
    }
    public void SetText(string text)
    {
        mainText.text = text;
    }
}

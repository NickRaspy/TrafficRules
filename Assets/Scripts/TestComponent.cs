using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestComponent : MonoBehaviour
{
    private void Awake()
    {       
        DontDestroyOnLoad(gameObject);
    }
    public void ClickButton(Button button)
    {
        button.onClick.Invoke();
    }
}

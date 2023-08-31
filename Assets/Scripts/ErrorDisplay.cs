using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorDisplay : MonoBehaviour
{
    private Text errorText;
    void Start()
    {
        errorText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {

        }
        catch(Exception ex)
        {
            errorText.text = ex.Message;
        }
    }
}

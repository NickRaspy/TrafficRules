using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabInteractive : Interactive
{
    public void Grabbed()
    {
        Debug.Log("grabbed");
        IsCompleted = true;
    }
}

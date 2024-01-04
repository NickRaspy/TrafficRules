using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Outline>() != null)
        {
            other.GetComponent<Outline>().enabled = true;
        }
        other.GetComponent<ObjectInteractive>()?.HaveBeenSeen();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Outline>() != null)
        {
            other.GetComponent<Outline>().enabled = false;
        }
    }
}

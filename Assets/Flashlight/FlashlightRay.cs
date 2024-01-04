using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightRay : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PeopleInteractive>()?.HaveBeenSeen();
    }
}

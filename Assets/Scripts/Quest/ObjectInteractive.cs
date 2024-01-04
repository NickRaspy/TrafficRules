using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractive : Interactive
{
    public void HaveBeenSeen()
    {
        IsCompleted = true;
    }
}

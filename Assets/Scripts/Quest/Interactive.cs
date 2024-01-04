using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public Action CompleteAction;

    public bool IsCompleted
    {
        get
        {
            return isCompleted;
        }
        set
        {
            isCompleted = value;
            if (isCompleted) CompleteAction?.Invoke();
        }
    }
    
    [SerializeField, ReadOnly]
    protected bool isCompleted;
    
}

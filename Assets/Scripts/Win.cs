using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : ShowMessageInTrigger
{
    public GameObject arrow;

    protected override void MainMethod()
    {
        base.MainMethod();

        arrow?.SetActive(false);
    }
    public void DoWin()
    {
        MainMethod();
    }
}

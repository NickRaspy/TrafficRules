using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : ShowMessageInTrigger
{
    protected override void MainMethod()
    {
        base.MainMethod();

        //"�������"
        di.blackColor.SetActive(true);
        di.flashBlindness.SetActive(true);

        //�������� ����
        di.dieSound?.Play();
    }
}

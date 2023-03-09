using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchCross : Watcher
{
    [SerializeField] PeopleCounter simpleCrosswalk;
    protected override void Awake()
    {
        //
        if (simpleCrosswalk == null)
            Debug.Log("������� crosswalk ����� watcher'�");
        simpleCrosswalk.ChangeStatus += ResetWatcher;
        return;
    }

    protected override void Start()
    {
        base.Start();
        di = DI.instance;
    }

    protected override bool OtherPeopleCanGo()
    {
        //��� �������� ����� Player
        if (simpleCrosswalk.CompareTag("Player"))
            return true;
        else
            return false;
    }

    protected override void SemaphoreWarningOn()
    {
        dieZone.SetActive(true);
    }
}


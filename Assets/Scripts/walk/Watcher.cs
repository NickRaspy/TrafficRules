using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : MonoBehaviour
{
    // ���� �����������(�� ����� 2)
    // � ������ ����������� 2 ������� ����� ���������� ������ � �������
    public Direction[] directions;

    protected DI di;

    public SemaphorePeople semaphore;
    [SerializeField] protected GameObject ColorBoxCrosswalk;
    [SerializeField] protected GameObject dieZone;

    protected virtual void Awake()
    {
        semaphore.ChangeLightColor += ResetWatcher;

        CheckAllRight();
    }

    protected virtual void Start()
    {
        di = DI.instance;
        foreach (Direction dir in directions)
        {
            foreach (ZoneColor zc in dir.playerLooked)
            {
                zc.OnPlayerLookedToThis += CheckWatcher;
            }
        }
    }

    //���������� ����� ����� ��������� �� ����
    //����� ��������� ��������
    public virtual void CheckWatcher()
    {
        if(ManLookedAround())
        {
            ColorBoxCrosswalk.GetComponent<MeshRenderer>().material = di._correctMat;
            dieZone.SetActive(false);
        }
    }

    private void CheckAllRight()
    {
        if (ColorBoxCrosswalk == null)
            Debug.Log("������� ���� ������ �� ColorBoxCrosswalk");

        if (directions.Length <= 1)
            Debug.Log("������� ������ directions � Watcher, ������� ���� ���� � ������� ������ ���������� �����.");

        if (semaphore == null)
            Debug.Log("������� ���� ������ �� semaphore");
    }

    protected virtual bool ManLookedAround()
    {
        foreach (Direction dir in directions)
        {
            if (dir.mainZone.playerStayInZone)
            {
                //���� ����� ����� � ����
                if (OtherPeopleCanGo() == false)
                    return false;

                foreach (ZoneColor zc in dir.playerLooked)
                {
                    if (zc.PlayerLookedToThis == false)
                        return false;
                }
                return true;
            }
        }
        return false;
    }

    protected virtual bool OtherPeopleCanGo()
    {
        return semaphore.PEOPLE_CAN;
    }

    public void ResetWatcher()
    {
        if (OtherPeopleCanGo())
        {
            //di.tooltip.ChangeTooltipText("����� ��������� ����� ������� ����������� ����� ���������� �� ��������.");
        }
        if (OtherPeopleCanGo() == false)
        {
            ResetZones();
            SemaphoreWarningOn();
        }

    }

    protected virtual void SemaphoreWarningOn()
    {
        //di.tooltip.ChangeTooltipText("�� ������� �� ������� ������ ���������!");
        StartCoroutine(OnDieZone());
    }

    private void ResetZones()
    {
        //��� ���� �� ���� �������� ����������� ������� �������
        foreach (Direction dir in directions)
        {
            foreach (ZoneColor zc in dir.playerLooked)
            {
                zc.PlayerLookedToThis = false;
            }
        }

        ColorBoxCrosswalk.GetComponent<MeshRenderer>().material = di._inCorrectMat;
    }

    IEnumerator OnDieZone()
    {
        yield return new WaitForSeconds(3f);
        dieZone.SetActive(true);
    }
}

[System.Serializable]
public class Direction
{
    public ZoneColor[] playerLooked;
    public MainZone mainZone;
}


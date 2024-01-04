using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DI : MonoBehaviour
{
    #region DI
    public static DI instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }

        Destroy(this.gameObject);
    }
    #endregion
    
    public GameObject player;
    public Win win;
    public Tooltip tooltip;
    public TasksWindow tasksWindow;
    public HoldThreeSeconds holdThreeSeconds;
    public AudioSource dieSound;
    public GameObject rightTeleportController;
    public GameObject leftTeleportController;
    public GameObject blackColor;
    public GameObject flashBlindness;
    public Material _correctMat;
    public Material _inCorrectMat;
    public GameObject RightRay;
    public GameObject LeftRay;
    public Sprite Svet1;
    public Sprite Svet2;

    private void Start()
    {
        if (player == null)
            player = GameObject.Find("PlayerXR");
        if (tooltip == null)
            tooltip = player.GetComponent<Tooltip>();
        if (holdThreeSeconds == null)
            holdThreeSeconds = player.GetComponent<HoldThreeSeconds>();
        if (dieSound == null)
            dieSound = GameObject.Find("DieSound").GetComponent<AudioSource>();
        if (rightTeleportController == null)
            rightTeleportController = player.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        if (leftTeleportController == null)
            leftTeleportController = player.transform.GetChild(0).GetChild(2).GetChild(1).gameObject;
        if(_correctMat == null)
            _correctMat = Resources.Load("Materials/Correct", typeof(Material)) as Material;
        if (_inCorrectMat == null)
            _inCorrectMat = Resources.Load("Materials/Incorrect", typeof(Material)) as Material;
        if (blackColor == null)
            blackColor = player.transform.GetChild(5).gameObject;
        if (flashBlindness == null)
            flashBlindness = player.transform.GetChild(4).gameObject;
        if (RightRay == null)
            RightRay = GameObject.Find("Right Ray");
        if (LeftRay == null)
            LeftRay = GameObject.Find("Left Ray");
        if (win == null)
            win = GameObject.Find("WinPosition").GetComponent<Win>();
    }
}

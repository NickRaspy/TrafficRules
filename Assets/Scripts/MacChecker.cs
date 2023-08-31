using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Проверяет комп на соответствие мак адресу
/// </summary>
public static class MacChecker
{
    static string sceneName = "Error";
    static string sceneMenu = "MainMenu";
    private static string[] Macs =//Писать через черточки, с любым регистром
    {
            "04-D9-F5-7C-5D-3D",
            "80-F3-EF-E6-59-8B",
            "80-F3-EF-9C-A6-2E",
            "B4-17-A8-46-7C-E8"
    };

    public static string CurrentMacAdd()
    {
        string currentmac = CurrentMac(NetworkInterface.GetAllNetworkInterfaces()[0]);

        string[] newMacs = new string[Macs.Length + 1];
        for (int i = 0; i < Macs.Length; i++)
        {
            newMacs[i] = Macs[i];
        }
        newMacs[newMacs.Length - 1] = currentmac;

        return currentmac;
    }
    public static void RestartScene()
    {
        SceneManager.LoadScene(sceneMenu);
    }
    public static bool Drm()
    {
        bool canStart = false; //можно запускаться?
        IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

        string mac;
        foreach (NetworkInterface adapter in nics)
        {
            mac = CurrentMac(adapter);
            canStart = StrCHecker(mac) || canStart;
        }

        if (!canStart)
            SceneManager.LoadScene(sceneName);

        return canStart;
    }
    private static string CurrentMac(NetworkInterface adapter)
    {
        string mac = "";
        PhysicalAddress address = adapter.GetPhysicalAddress();
        byte[] bytes = address.GetAddressBytes();
        for (int i = 0; i < bytes.Length; i++)
        {
            mac = string.Concat(mac + (string.Format("{0}", bytes[i].ToString("X2"))));
            if (i != bytes.Length - 1)
            {
                mac = string.Concat(mac + "-");
            }
        }
        return mac;
    }
    private static bool StrCHecker(string info)
    {
        for (int i = 0; i < Macs.Length; i++)
        {
            if (Macs[i].ToUpper() == info) return true;
        }

        return false;
    }
}


#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.Assertions;
using Unity.VisualScripting;

public class Main : MonoBehaviour
{
    [SerializeField] Button? asHostButton;
    [SerializeField] Button? asClientButton;
    [SerializeField] TMP_InputField? ipaddress;

    [SerializeField] Button? exitButton;
    [SerializeField] GameObject? joystick;

    string? addr = null;

    void initAddress()
    {
        if (PlayerPrefs.HasKey("address"))
        {
            this.addr = PlayerPrefs.GetString("address");
        } else
        {
            this.addr = "127.0.0.1";
            saveAddress();
        }
    }

    void saveAddress()
    {
        if (addr != null)
        {
            PlayerPrefs.SetString("address", this.addr);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        initAddress();
        if (ipaddress != null)
        {
            ipaddress.text = this.addr ?? "";
        }
    }

    void Start()
    {
        if(asClientButton==null || asHostButton == null || ipaddress==null || exitButton==null || joystick==null)
        {
            Debug.LogError("Invalid Main Setup");
            return;
        }

#if !UNITY_ANDROID
        joystick.SetActive(false);
#endif

        asHostButton.OnClickAsObservable().Subscribe(_ => {
            NetworkManager.Singleton.StartHost();
            asClientButton.interactable = false;
        }).AddTo(this);

        ipaddress.ObserveEveryValueChanged(i => i.text).Subscribe(text => {
            this.addr = text;
            this.asClientButton.interactable = !string.IsNullOrEmpty(this.addr);
        }).AddTo(this);

        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (transport == null || this.addr==null)
        {
            asClientButton.interactable = false;
        }
        else {
            asClientButton.OnClickAsObservable().Subscribe(_ =>
            {
                asHostButton.interactable = false;
                transport.SetConnectionData(this.addr, 7777);
                NetworkManager.Singleton.StartClient();
                saveAddress();
            }).AddTo(this);            
        }

        exitButton.OnClickAsObservable().Subscribe(_ => {
            NetworkManager.Singleton.Shutdown();
            asClientButton.interactable = true;
            asHostButton.interactable = true;
        }).AddTo(this);
    }
}

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

public class Main : NetworkBehaviour
{
    [SerializeField] UIView? UI;
    [SerializeField] GameObject? joystick;
    [SerializeField] NetworkObject? sphereObject;

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
        if (UI != null)
        {
            UI.Initialize(this.addr ?? "");
            UI.Address.Subscribe(addr => {
                this.addr = addr;
            }).AddTo(this);
        }
    }

    void Start()
    {
        if (joystick == null || UI == null)
        {
            Debug.LogError("Invalid Main Setup");
            return;
        }

#if !UNITY_ANDROID
        joystick.SetActive(false);
#endif

        UI.OnStartAsHost.Subscribe(_ => {
            NetworkManager.Singleton.StartHost();
        }).AddTo(this);

        UI.OnStartAsClient.Subscribe(_ =>
        {
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            if (transport != null)
            {
                transport.SetConnectionData(this.addr, 7777);
                NetworkManager.Singleton.StartClient();
                saveAddress();
            }
        }).AddTo(this);

        UI.OnExit.Subscribe(_ => {
            NetworkManager.Singleton.Shutdown();
        }).AddTo(this);

        UI.OnSphereCreated.Subscribe(_ => {
            CreateSphereServerRpc();
        }).AddTo(this);
    }

    [ServerRpc(RequireOwnership = false)]
    void CreateSphereServerRpc()
    {
        Instantiate(sphereObject)?.Spawn();
    }
}

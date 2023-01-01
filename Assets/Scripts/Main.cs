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

public class Main : MonoBehaviour
{
    [SerializeField] Button? asHostButton;
    [SerializeField] Button? asClientButton;
    [SerializeField] TMP_InputField? ipaddress;

    void Start()
    {
        asHostButton?.OnClickAsObservable().Subscribe(_ => {
            NetworkManager.Singleton.StartHost();
        }).AddTo(this);

        asClientButton?.OnClickAsObservable().Subscribe(_ => {

            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            if(transport!=null && ipaddress!=null) {
                transport.SetConnectionData(ipaddress.text, 7777);
            }
            NetworkManager.Singleton.StartClient();
        }).AddTo(this);
    }
}

#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;

public class Main : MonoBehaviour
{
    [SerializeField] Button? asHostButton;
    [SerializeField] Button? asClientButton;

    void Start()
    {
        asHostButton?.OnClickAsObservable().Subscribe(_ => {
            NetworkManager.Singleton.StartHost();
        }).AddTo(this);

        asClientButton?.OnClickAsObservable().Subscribe(_ => {
            NetworkManager.Singleton.StartClient();
        }).AddTo(this);
    }
}

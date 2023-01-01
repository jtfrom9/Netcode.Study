#nullable enable

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class UIView : MonoBehaviour
{
    [SerializeField] Button? asHostButton;
    [SerializeField] Button? asClientButton;
    [SerializeField] TMP_InputField? ipaddressField;

    [SerializeField] Button? exitButton;
    [SerializeField] Button? sphereButton;
    
    ReactiveProperty<string> addressProperty = new ReactiveProperty<string>();
    public IReadOnlyReactiveProperty<string> Address => addressProperty;
    public ISubject<Unit> OnSphereCreated = new Subject<Unit>();
    public ISubject<Unit> OnStartAsHost = new Subject<Unit>();
    public ISubject<Unit> OnStartAsClient = new Subject<Unit>();
    public ISubject<Unit> OnExit = new Subject<Unit>();

    public void Initialize(string initialAddress)
    {
        //this.addressProperty.Value = initialAddress;    
        if (this.ipaddressField != null)
        {
            this.ipaddressField.text = initialAddress;
        }
    }

    void Start()
    {
        if (asClientButton == null || asHostButton == null || ipaddressField == null || exitButton == null
            || sphereButton == null)
        {
            Debug.LogError("UIView: Invalid Main Setup");
            return;
        }

        exitButton.interactable = false;

        asHostButton.OnClickAsObservable().Subscribe(_ => {
            OnStartAsHost.OnNext(Unit.Default);
            asClientButton.interactable = false;
            exitButton.interactable = true;
            ipaddressField.interactable = false;
        }).AddTo(this);

        ipaddressField.ObserveEveryValueChanged(i => i.text).Subscribe(text => {
            this.asClientButton.interactable = !string.IsNullOrEmpty(text);
            this.addressProperty.Value = text;
        }).AddTo(this);

        asClientButton.OnClickAsObservable().Subscribe(_ =>
        {
            OnStartAsClient.OnNext(Unit.Default);
            asHostButton.interactable = false;
            exitButton.interactable = true;
        }).AddTo(this);

        exitButton.OnClickAsObservable().Subscribe(_ =>
        {
            OnExit.OnNext(Unit.Default);
            asClientButton.interactable = true;
            asHostButton.interactable = true;
            ipaddressField.interactable = true;
        }).AddTo(this);

        sphereButton.OnClickAsObservable().Subscribe(_ => {
            OnSphereCreated.OnNext(Unit.Default);
        }).AddTo(this);
    }
}

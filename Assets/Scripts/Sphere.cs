using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Sphere : NetworkBehaviour
{
    NetworkObject networkObject;

    //private void Awake()
    //{
    //    var x = UnityEngine.Random.Range(-3, 3);
    //    var y = UnityEngine.Random.Range(2, 8);
    //    var z = UnityEngine.Random.Range(-3, 3);
    //    var scale = UnityEngine.Random.Range(0.1f, 1);
    //    transform.position = new Vector3(x, y, z);
    //    transform.localScale = new Vector3(scale, scale, scale);

    //    var mr = GetComponent<MeshRenderer>();
    //    mr.material.color = UnityEngine.Random.ColorHSV();

    //    var timeToDestroy = UnityEngine.Random.Range(3, 10);
    //    Invoke("Destroy", timeToDestroy);

    //    TryGetComponent<NetworkObject>(out networkObject);
    //}


    private NetworkVariable<Color> color = new NetworkVariable<Color>();

    public override void OnNetworkSpawn()
    {
        //base.OnNetworkSpawn();
        Debug.Log("Network Spawn");
        if (IsServer)
        {
            var x = UnityEngine.Random.Range(-3, 3);
            var y = UnityEngine.Random.Range(2, 8);
            var z = UnityEngine.Random.Range(-3, 3);
            var scale = UnityEngine.Random.Range(0.1f, 1);
            transform.position = new Vector3(x, y, z);
            transform.localScale = new Vector3(scale, scale, scale);

            //var mr = GetComponent<MeshRenderer>();
            //mr.material.color = UnityEngine.Random.ColorHSV();
            color.Value = UnityEngine.Random.ColorHSV();

            var timeToDestroy = UnityEngine.Random.Range(3, 10);
            Invoke("Destroy", timeToDestroy);

            TryGetComponent<NetworkObject>(out networkObject);
        }
    }

    private void Start()
    {
        {
            var mr = GetComponent<MeshRenderer>();
            mr.material.color = color.Value;
        }
    }

    private void Destroy()
    {
        networkObject.Despawn();
        Destroy(gameObject);
    }
}

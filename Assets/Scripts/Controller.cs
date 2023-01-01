using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Controller : NetworkBehaviour
{
    Rigidbody rigitbody;
    Vector3 moving, latestPos;
    float speed;

    Joystick joystick;

    private void Awake()
    {
#if UNITY_ANDROID
        joystick = GameObject.FindObjectOfType<Joystick>();
#endif
        Debug.Log(joystick);
    }

    void Start()
    {
        rigitbody = GetComponent<Rigidbody>();
        speed = 5;
    }

    void Update()
    {
        if (IsOwner)
        {
            this.moving = MovementControll();
            SetMovingServerRpc(this.moving);
        }
        if(IsServer)
        {
            Movement();
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            RotateToMovingDirection();
        }
    }

    [ServerRpc]
    void SetMovingServerRpc(Vector3 v) {
        this.moving = v;
    }

    Vector3 MovementControll()
    {
        if (joystick != null)
        {
            var moving = new Vector3(-joystick.Horizontal, 0, -joystick.Vertical);
            moving.Normalize();
            moving = moving * speed;
            return moving;
        }
        else
        {
            //斜め移動と縦横の移動を同じ速度にするためにVector3をNormalize()する。
            var moving = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            moving.Normalize();
            moving = moving * speed;
            return moving;
        }
    }

    public void RotateToMovingDirection()
    {
        Vector3 differenceDis = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(latestPos.x, 0, latestPos.z);
        latestPos = transform.position;
        //移動してなくても回転してしまうので、一定の距離以上移動したら回転させる
        if (Mathf.Abs(differenceDis.x) > 0.001f || Mathf.Abs(differenceDis.z) > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(differenceDis);
            rot = Quaternion.Slerp(rigitbody.transform.rotation, rot, 0.1f);
            this.transform.rotation = rot;
            //アニメーションを追加する場合
            //animator.SetBool("run", true);
        }
        else
        {
            //animator.SetBool("run", false);
        }
    }

    void Movement()
    {
        rigitbody.velocity = moving;
    }

    public override void OnNetworkSpawn()
    {
        //base.OnNetworkSpawn();
        Debug.Log("Network Spawn");
        if(IsServer)
        {
            var x = UnityEngine.Random.Range(-2, 2);
            var y = UnityEngine.Random.Range(1, 3);
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}

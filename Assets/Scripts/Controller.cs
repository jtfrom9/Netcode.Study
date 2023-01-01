using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Controller : NetworkBehaviour
{
    Rigidbody rigitbody;
    Vector3 moving, latestPos;
    float speed;

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
        //斜め移動と縦横の移動を同じ速度にするためにVector3をNormalize()する。
        var moving = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moving.Normalize();
        moving = moving * speed;
        return moving;
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
}

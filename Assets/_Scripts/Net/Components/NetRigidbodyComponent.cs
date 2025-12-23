using System;
using System.Collections;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Network.Interface;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
public class NetRigidbodyComponent : BaseNetworkComponent
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
        interactable.selectEntered.AddListener((data =>
        {
            isKinematic = true;
        } ));
        interactable.selectExited.AddListener((data =>
        {
            isKinematic = false;
        } ));
    }

    public override void HandleMessage(IMessage message)
    {
        base.HandleMessage(message);
        var msg = message as PushNetRigidbodySynchronizationMessage;
        if (msg == null) return;

        // 赋值
        _rigidbody.isKinematic = msg.isKinematic;
        _rigidbody.mass = msg.mass;
        _rigidbody.angularDrag = msg.angularDrag;
        _rigidbody.useGravity = msg.useGravity;
        _rigidbody.drag = msg.drag;
        // // 速度
        // _rigidbody.velocity = new Vector3(msg.velocity.x, msg.velocity.y, msg.velocity.z);
        // _rigidbody.angularVelocity = new Vector3(msg.angular_velocity.x, msg.angular_velocity.y, msg.angular_velocity.z);

        // 冻结属性
        _rigidbody.constraints = RigidbodyConstraints.None;
        if (msg.freeze_position != null)
        {
            if (msg.freeze_position.x != 0) _rigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
            if (msg.freeze_position.y != 0) _rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
            if (msg.freeze_position.z != 0) _rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        if (msg.freeze_rotation != null)
        {
            if (msg.freeze_rotation.x != 0) _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
            if (msg.freeze_rotation.y != 0) _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationY;
            if (msg.freeze_rotation.z != 0) _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }

    }

    protected virtual void SendUpdateMessage()
    {
        if (FantasyNetMgr.Instance == null || !FantasyNetMgr.isConnected)
            return;

        var message = new SubmitNetRigidbodySynchronizationMessage();
        message.uuid = this.uuid;
        message.isKinematic = _rigidbody.isKinematic;
        message.mass = _rigidbody.mass;
        message.angularDrag = _rigidbody.angularDrag;
        message.useGravity = _rigidbody.useGravity;
        message.drag = _rigidbody.drag;

        // message.velocity = new Fantasy.Vector3() { x = _rigidbody.velocity.x, y = _rigidbody.velocity.y, z = _rigidbody.velocity.z };
        // message.angular_velocity = new Fantasy.Vector3() { x = _rigidbody.angularVelocity.x, y = _rigidbody.angularVelocity.y, z = _rigidbody.angularVelocity.z };

        // 冻结属性
        message.freeze_position = new Fantasy.Vector3()
        {
            x = (_rigidbody.constraints & RigidbodyConstraints.FreezePositionX) != 0 ? 1 : 0,
            y = (_rigidbody.constraints & RigidbodyConstraints.FreezePositionY) != 0 ? 1 : 0,
            z = (_rigidbody.constraints & RigidbodyConstraints.FreezePositionZ) != 0 ? 1 : 0
        };
        message.freeze_rotation = new Fantasy.Vector3()
        {
            x = (_rigidbody.constraints & RigidbodyConstraints.FreezeRotationX) != 0 ? 1 : 0,
            y = (_rigidbody.constraints & RigidbodyConstraints.FreezeRotationY) != 0 ? 1 : 0,
            z = (_rigidbody.constraints & RigidbodyConstraints.FreezeRotationZ) != 0 ? 1 : 0
        };

        SendProtoMessage(message);
    }

    // 速度
    public Vector3 velocity
    {
        get => _rigidbody.velocity;
        set
        {
            _rigidbody.velocity = value;
            SendUpdateMessage();
        }
    }

    // 角速度
    public Vector3 angular_velocity
    {
        get => _rigidbody.angularVelocity;
        set
        {
            _rigidbody.angularVelocity = value;
            SendUpdateMessage();
        }
    }

    // 是否运动学
    public bool isKinematic
    {
        get => _rigidbody.isKinematic;
        set
        {
            _rigidbody.isKinematic = value;
            SendUpdateMessage();
        }
    }

    // 质量
    public float mass
    {
        get => _rigidbody.mass;
        set
        {
            _rigidbody.mass = value;
            SendUpdateMessage();
        }
    }

    // 角阻力
    public float angularDrag
    {
        get => _rigidbody.angularDrag;
        set
        {
            _rigidbody.angularDrag = value;
            SendUpdateMessage();
        }
    }

    // 阻力
    public float drag
    {
        get => _rigidbody.drag;
        set
        {
            _rigidbody.drag = value;
            SendUpdateMessage();
        }
    }

    // 是否使用重力
    public bool useGravity
    {
        get => _rigidbody.useGravity;
        set
        {
            _rigidbody.useGravity = value;
            SendUpdateMessage();
        }
    }

    // 冻结位置
    public Vector3 freeze_position
    {
        get
        {
            return new Vector3(
                (_rigidbody.constraints & RigidbodyConstraints.FreezePositionX) != 0 ? 1 : 0,
                (_rigidbody.constraints & RigidbodyConstraints.FreezePositionY) != 0 ? 1 : 0,
                (_rigidbody.constraints & RigidbodyConstraints.FreezePositionZ) != 0 ? 1 : 0
            );
        }
        set
        {
            var constraints = _rigidbody.constraints;
            constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
            if (value.x != 0) constraints |= RigidbodyConstraints.FreezePositionX;
            if (value.y != 0) constraints |= RigidbodyConstraints.FreezePositionY;
            if (value.z != 0) constraints |= RigidbodyConstraints.FreezePositionZ;
            _rigidbody.constraints = constraints;
            SendUpdateMessage();
        }
    }

    // 冻结旋转
    public Vector3 freeze_rotation
    {
        get
        {
            return new Vector3(
                (_rigidbody.constraints & RigidbodyConstraints.FreezeRotationX) != 0 ? 1 : 0,
                (_rigidbody.constraints & RigidbodyConstraints.FreezeRotationY) != 0 ? 1 : 0,
                (_rigidbody.constraints & RigidbodyConstraints.FreezeRotationZ) != 0 ? 1 : 0
            );
        }
        set
        {
            var constraints = _rigidbody.constraints;
            constraints &= ~(RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ);
            if (value.x != 0) constraints |= RigidbodyConstraints.FreezeRotationX;
            if (value.y != 0) constraints |= RigidbodyConstraints.FreezeRotationY;
            if (value.z != 0) constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.constraints = constraints;
            SendUpdateMessage();
        }
    }
}

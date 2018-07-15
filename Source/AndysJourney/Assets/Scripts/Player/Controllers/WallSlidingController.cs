using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallSlidingController : PlayerController, IControlLocker
{
    public float frictionY;

    bool _lockThis;

    public override void Start()
    {
        base.Start();
        ControlLock.Register(this, "WallSlidingController");
    }

    public override void Update()
    {
        base.Update();
        if (_lockThis)
            return;
        if ((_player.isFrontCollision && _player.GetInputX() != 0) && !_anim.GetBool("isOnGround"))
        {
            ControlLock.Lock("Move");
            if (_rb.velocity.y < 0)
            {
                _rb.velocity += Vector2.up * frictionY;
            }
        }
        else
        {
            ControlLock.ReleaseLock("Move");
        }
    }

    public void Lock(string name)
    {
        if (name == "WallSlidingController")
        {
            _lockThis = true;
        }
    }

    public void ReleaseLock(string name)
    {
        if (name == "WallSlidingController")
        {
            _lockThis = false;
        }
    }
}
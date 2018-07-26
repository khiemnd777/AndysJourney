using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickDownController : PlayerController, IControlLocker
{
    [SerializeField]
    Camera _theCamera;
    [SerializeField]
    float _shakingDuration;
    [SerializeField]
    float _shakingAmount;

    bool _isInCooldown;
    bool _isDown;
    bool _lock;

    public override void Start()
    {
        base.Start();
        ControlLock.Register(this, "KickDown");
    }

    public override void Update()
    {
        base.Update();
        if (_lock)
            return;
        if (Input.GetKeyDown(KeyCode.S))
        {
            _isDown = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _isDown = false;
        }
        if (_isDown && Input.GetKeyDown(KeyCode.J) && !_isInCooldown)
        {
            StartCoroutine(StartKickDown());
        }
    }

    public void Lock(string name)
    {
        if (name == "KickDown")
        {
            _lock = true;
        }
    }

    public void ReleaseLock(string name)
    {
        if (name == "KickDown")
        {
            _lock = false;
        }
    }

    IEnumerator StartKickDown()
    {
        _isInCooldown = true;
        ControlLock.Lock("Move");
        _anim.SetBool("isKickDown", true);
        var length = Utility.GetAnimationLength(_anim, "Kick Down");
        _rb.velocity = Vector2.up * -2.5f;
        var shadowSample = Utility.CreateSpriteRendererBySample(_sprite.sprite, transform.position, transform.localScale, .25f);
        Destroy(shadowSample.gameObject, .075f);
        while (!_anim.GetBool("isOnGround") && !_player.isJump)
        {
            yield return null;
        }
        if (_anim.GetBool("isOnGround"))
        {
            StartCoroutine(Utility.Shaking(_shakingDuration, _shakingAmount, _theCamera.transform
            , () => ControlLock.Lock("Camera")
            , () => ControlLock.ReleaseLock("Camera")));
        }
        ControlLock.ReleaseLock("Move");
        _anim.SetBool("isKickDown", false);
        _isInCooldown = false;
    }
}

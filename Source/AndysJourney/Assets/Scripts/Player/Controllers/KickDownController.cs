using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickDownController : PlayerController
{
    bool _isInCooldown;
    bool _isDown;

    public override void Update()
    {
        base.Update();
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
        ControlLock.ReleaseLock("Move");
        _anim.SetBool("isKickDown", false);
        _isInCooldown = false;
    }
}

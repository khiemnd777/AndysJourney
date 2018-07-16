using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : PlayerController
{
    public float dashingForce = 1.8f;

    bool _isDashing;
    bool _isInCooldown;

    public override void Update()
    {
        base.Update();
        if(_anim.GetBool("isOnGround") && !_isDashing){
            _isInCooldown = false;
        }
        if (Input.GetKeyDown(KeyCode.L) && !_isDashing && !_isInCooldown)
        {
            StartCoroutine(StartDashing());
        }
    }

    IEnumerator StartDashing()
    {
        _isInCooldown = true;
        ControlLock.Lock("Move", "FlipX", "NormalPunch", "Jump");
        _rb.gravityScale = .0f;
        _isDashing = true;
        _anim.SetBool("isDashing", true);
        var length = Utility.GetAnimationLength(_anim, "Dashing Right");
        _rb.velocity = Vector2.right * _faceX * dashingForce;
        yield return new WaitForSeconds(length);
        _isDashing = false;
        _anim.SetBool("isDashing", false);
        _rb.gravityScale = _player.gravity;
        ControlLock.ReleaseLock("Move", "FlipX", "NormalPunch", "Jump");
        // _isInCooldown = false;
    }
}
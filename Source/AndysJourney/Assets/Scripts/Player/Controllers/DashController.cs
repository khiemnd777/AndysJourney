using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : PlayerController
{
    public float dashingForce = 1.8f;

    bool _isDashing;
    bool _isInCooldown;
    float _timeDirection = .02f;

    public override void Update()
    {
        base.Update();
        if ((_anim.GetBool("isWallSliding") || _anim.GetBool("isOnGround")) && !_isDashing)
        {
            _isInCooldown = false;
        }
        if (!_isDashing && !_isInCooldown)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(StartDashing());
            }
        }
    }

    IEnumerator StartDashing()
    {
        var percent = 0f;
        while(percent <= 1f){
            percent += Time.deltaTime / _timeDirection;
            yield return null;
        }
        _isInCooldown = true;
        ControlLock.Lock("Move", "FlipX", "NormalPunch", "Jump");
        _rb.gravityScale = .0f;
        _isDashing = true;
        _anim.SetBool("isDashing", true);
        var length = Utility.GetAnimationLength(_anim, "Dashing Right");
        _rb.velocity = Vector2.right * transform.localScale.x * dashingForce;
        yield return new WaitForSeconds(length);
        _isDashing = false;
        _anim.SetBool("isDashing", false);
        _rb.gravityScale = _player.gravity;
        ControlLock.ReleaseLock("Move", "FlipX", "NormalPunch", "Jump");
    }
}
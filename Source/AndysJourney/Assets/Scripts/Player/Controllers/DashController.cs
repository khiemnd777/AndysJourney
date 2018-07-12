using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : PlayerController
{
    public float dashingForce = 1.8f;
    public float dashingExtraDistance = .35f;
    public float dashingSeconds = .3f;
    public float dashingCooldown = 1f;

    bool _isDashing;
    bool _isInCooldown;

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.L) && !_isDashing && !_isInCooldown)
        {
            StartCoroutine(StartCooldown());
            StartCoroutine(StartDashing());
        }
    }

    IEnumerator StartDashing()
    {
        ControlLock.Lock("Move");
        var gravityScale = _rb.gravityScale;
        _rb.gravityScale = .0f;
        _isDashing = true;
        _anim.SetBool("isDashing", true);
        var time = Utility.TimeByFrame(22, 60);
        _rb.velocity = Vector2.right * _faceX * dashingForce;
        yield return new WaitForSeconds(time);
        _isDashing = false;
        _anim.SetBool("isDashing", false);
        _rb.gravityScale = gravityScale;
        ControlLock.ReleaseLock("Move");
    }

    IEnumerator StartCooldown(){
        _isInCooldown = true;
        yield return new WaitForSeconds(dashingCooldown);
        _isInCooldown = false;
    }
}
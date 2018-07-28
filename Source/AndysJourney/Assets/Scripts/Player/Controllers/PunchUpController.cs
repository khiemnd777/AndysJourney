using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchUpController : PlayerController
{
	bool _lock;
	bool _isPressedUp;
	bool _isInCooldown;

	public override void Update()
    {
        base.Update();
        if (_lock)
            return;
        if (_player.GetInputVertical() > 0)
        {
            _isPressedUp = true;
        }
        if (_player.GetInputVertical() == 0)
        {
            _isPressedUp = false;
        }
        if (_isPressedUp && Input.GetKeyDown(KeyCode.J) && !_isInCooldown)
        {
            StartCoroutine(StartPunchUp());
        }
    }

	IEnumerator StartPunchUp()
	{
		_isInCooldown = true;
        ControlLock.Lock("Move");
        _anim.SetBool("isPunchUp", true);
        var length = Utility.GetAnimationLength(_anim, "Punch Up");
        yield return new WaitForSeconds(length);
        _isInCooldown = false;
        ControlLock.ReleaseLock("Move");
        _anim.SetBool("isPunchUp", false);
	}
}

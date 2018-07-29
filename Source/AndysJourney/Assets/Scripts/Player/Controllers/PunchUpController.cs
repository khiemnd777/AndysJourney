using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchUpController : PlayerController
{
    [SerializeField]
    HitDetector _effect;

    bool _lock;
    bool _isPressedUp;
    bool _isInCooldown;

    public override void Start()
    {
		base.Start();
    }

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
			StateHandling.Handle("KickDown", "Off");
            StartCoroutine(StartPunchUp());
        }
    }

    IEnumerator StartPunchUp()
    {
        _isInCooldown = true;
        ControlLock.Lock("Move");
        _anim.SetBool("isPunchUp", true);
        var length = Utility.GetAnimationLength(_anim, "Punch Up");
        var fx = InstantiateEffect();
		var fxAnim = fx.GetComponent<Animator>();
		var fxLength = fxAnim.GetCurrentAnimatorStateInfo(0).length;
		Destroy(fx.gameObject, fxLength);
        yield return new WaitForSeconds(length);
        _isInCooldown = false;
        ControlLock.ReleaseLock("Move");
        _anim.SetBool("isPunchUp", false);
    }

    HitDetector InstantiateEffect()
    {
        var fx = Instantiate<HitDetector>(_effect, transform.position, Quaternion.identity, transform);
        fx.onDetectedHit = OnDetectedHit;
        fx.gameObject.SetActive(true);
        fx.transform.localPosition = new Vector3(-.02f, -.04f, 0f);
        return fx;
    }

    void OnDetectedHit(HitDetector detector, Collider2D other)
    {

    }
}

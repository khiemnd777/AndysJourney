using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBunchController : PlayerController, IControlLocker
{
    [SerializeField]
    HitDetector _effectOnePrefab;
    [SerializeField]
    HitDetector _effectTwoPrefab;
    Transform _effectPosition;
    bool _isInCooldown;
    bool _isOne = true;
    bool _lockPunch;

    public override void Start()
    {
        base.Start();
        ControlLock.Register("NormalPunch", this);
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.J) && !_isInCooldown && !_lockPunch)
        {
            StartCoroutine(StartPunching());
        }
    }

    void OnDetectedHit(HitDetector detector, Collider2D other)
    {
        // StartCoroutine(DoReflectiveForce());
        ControlLock.Lock("Move");
        _player.extraForceX -= .5f * _faceX;
    }

    IEnumerator DoReflectiveForce()
    {
        ControlLock.Lock("Move");
        var vel = _rb.velocity;
        vel.x -= .5f * _faceX;
        _rb.velocity = vel;
        yield return new WaitForSeconds(.1f);
        ControlLock.ReleaseLock("Move");
    }
    IEnumerator StartPunching()
    {
        _isInCooldown = true;
        ControlLock.Lock("MoveOnGround");
        _anim.SetBool("isBunch", true);
        _anim.SetBool("isBunchOne", _isOne);
        var bunchName = string.Empty;
        var bunchLength = 0f;
        if (_anim.GetBool("isWallSliding"))
        {
            bunchName = "Wall Punch";
            bunchLength = Utility.GetAnimationLength(_anim, bunchName);
        }
        else
        {
            bunchName = string.Format("Normal Bunch {0} {1}", _faceX < 0 ? "Left" : "Right", _isOne ? 1 : 2);
            bunchLength = Utility.GetAnimationLength(_anim, bunchName);
        }
        InstantiateEffect();
        if (!_anim.GetBool("isWallSliding"))
        {
            _isOne = !_isOne;
        }
        yield return new WaitForSeconds(bunchLength);
        _anim.SetBool("isBunch", false);
        _isInCooldown = false;
        ControlLock.ReleaseLock("MoveOnGround", "Move");
    }

    void InstantiateEffect()
    {
        var prefab = _isOne || _anim.GetBool("isWallSliding") ? _effectOnePrefab : _effectTwoPrefab;
        var fx = Instantiate<HitDetector>(prefab, transform.position, Quaternion.identity, transform);
        fx.onDetectedHit = OnDetectedHit;
        fx.gameObject.SetActive(true);
        var dirX = _anim.GetBool("isWallSliding") ? -1 : 1;
        fx.transform.localPosition = new Vector3(.095f * dirX, .095f, 0);
        var scale = fx.transform.localScale;
        scale.x *= dirX;
        fx.transform.localScale = scale;
        var fxAnim = fx.GetComponent<Animator>();
        Destroy(fx.gameObject, fxAnim.GetCurrentAnimatorStateInfo(0).length);
    }

    public void Lock(string name)
    {
        if (name == "NormalPunch")
        {
            _lockPunch = true;
        }
    }

    public void ReleaseLock(string name)
    {
        if (name == "NormalPunch")
        {
            _lockPunch = false;
        }
    }
}

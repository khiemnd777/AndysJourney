using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickDownController : PlayerController, IControlLocker, IStateHandler
{
    [SerializeField]
    Camera _theCamera;
    [SerializeField]
    float _shakingDuration;
    [SerializeField]
    float _shakingAmount;
    [SerializeField]
    HitDetector _effect;

    bool _isInCooldown;
    bool _isPressedDown;
    bool _lock;
    HitDetector _curFx;

    public override void Start()
    {
        base.Start();
        ControlLock.Register(this, "KickDown");
        StateHandling.Register(this, "KickDown");
    }

    public override void Update()
    {
        base.Update();
        if (_lock)
            return;
        if(_anim.GetBool("isOnGround"))
            return;
        if (_player.GetInputVertical() < 0)
        {
            _isPressedDown = true;
        }
        if (_player.GetInputVertical() == 0)
        {
            _isPressedDown = false;
        }
        if (_isPressedDown && Input.GetKeyDown(KeyCode.J) && !_isInCooldown)
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

    public void HandleState(string name, string state)
    {
        if (name == "KickDown")
        {
            if(state == "Off"){
                if(_curFx != null || _curFx is Object && !_curFx.Equals(null)){
                    ControlLock.ReleaseLock("Move");
                    _anim.SetBool("isKickDown", false);
                    _isInCooldown = false;
                    _rb.velocity = Vector2.zero;
                    Destroy(_curFx.gameObject);
                }
            }
        }
    }

    IEnumerator StartKickDown()
    {
        _isInCooldown = true;
        ControlLock.Lock("Move");
        _anim.SetBool("isKickDown", true);
        var length = Utility.GetAnimationLength(_anim, "Kick Down");
        _rb.velocity = Vector2.up * -2.5f;
        // Create the shadow sample
        var shadowSample = Utility.CreateSpriteRendererBySample(_sprite.sprite, transform.position, transform.localScale, .25f);
        Destroy(shadowSample.gameObject, .075f);
        // Create the effect
        _curFx = InstantiateEffect();
        yield break;
    }

    HitDetector InstantiateEffect()
    {
        var fx = Instantiate<HitDetector>(_effect, transform.position, Quaternion.identity, transform);
        fx.onDetectedHit = OnDetectedHit;
        fx.gameObject.SetActive(true);
        fx.transform.localPosition = new Vector3(0f, -.05f, 0);
        return fx;
    }

    void OnDetectedHit(HitDetector detector, Collider2D other)
    {
        if ("Ground".Equals(LayerMask.LayerToName(other.gameObject.layer)))
        {
            _anim.SetBool("isOnGround", true);
            // Shake the screen when character kicks on ground.
            StartCoroutine(Utility.Shaking(_shakingDuration, _shakingAmount, _theCamera.transform
            , () => ControlLock.Lock("Camera1")
            , () => ControlLock.ReleaseLock("Camera1")));
        }
        HandleState("KickDown", "Off");
    }
}

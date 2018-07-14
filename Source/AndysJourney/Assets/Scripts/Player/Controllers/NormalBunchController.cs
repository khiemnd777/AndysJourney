using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBunchController : PlayerController
{
    [SerializeField]
    SpriteRenderer _effectOnePrefab;
    [SerializeField]
    SpriteRenderer _effectTwoPrefab;
    Transform _effectPosition;
    bool _isInCooldown;
    bool _isOne = true;

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.J) && !_isInCooldown)
        {
            StartCoroutine(StartBunching());
        }
    }

    IEnumerator StartBunching()
    {
        _isInCooldown = true;
        ControlLock.Lock("MoveOnGround");
        _anim.SetBool("isBunch", true);
        _anim.SetBool("isBunchOne", _isOne);
        var bunchName = string.Format("Normal Bunch {0} {1}", _faceX < 0 ? "Left" : "Right", _isOne ? 1 : 2);
        var bunchLength = Utility.GetAnimationLength(_anim, bunchName);
        InstantiateEffect();
        yield return new WaitForSeconds(bunchLength);
        _isOne = !_isOne;
        _anim.SetBool("isBunch", false);
        _isInCooldown = false;
        ControlLock.ReleaseLock("MoveOnGround");
    }

    void InstantiateEffect(){
        var prefab = _isOne ? _effectOnePrefab : _effectTwoPrefab;
        var fx = Instantiate<SpriteRenderer>(prefab, transform.position, Quaternion.identity, transform);
        fx.gameObject.SetActive(true);
        fx.transform.localPosition = new Vector3(.095f, .095f, 0);
        var fxAnim = fx.GetComponent<Animator>();
        Destroy(fx.gameObject, fxAnim.GetCurrentAnimatorStateInfo(0).length);
    }
}

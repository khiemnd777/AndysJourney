using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightGetBack : Skill
{
    [SerializeField]
    Transform _target;
    [SerializeField]
    float _backVelocity;
    [SerializeField]
    float _backGravity;
    [SerializeField]
    AnimationClip _getBack;
    [SerializeField]
    AnimationClip _jumpForward;
    [SerializeField]
    float _jumpMaxHeight;
    [SerializeField]
    AnimationClip _idle;
    [SerializeField]
    BoxCollider2D _boundary;
    [SerializeField]
    Animator _dustFxPrefab;
    [SerializeField]
    float _deltaDisplacement;

    Animator _anim;
    Rigidbody2D _rb;

    TheSlashingKi _theSlashingKi;
    TheBlackKnightSlash _theSlash;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        // next skills
        _theSlashingKi = GetComponent<TheSlashingKi>();
        _theSlash = GetComponent<TheBlackKnightSlash>();
    }

    void FlipX()
    {
        // Flip X by own transform.
        var scale = transform.localScale;
        scale.x = _target.position.x > transform.position.x ? 1 : -1;
        transform.localScale = scale;
    }

    Vector2Int DetectTargetDirection()
    {
        var dir = (_target.position - transform.position).normalized;
        return Vector2Int.RoundToInt(dir);
    }

    void InstantiateTheDust(float size = 1f)
    {
        var ins = Instantiate<Animator>(_dustFxPrefab, transform.position, Quaternion.identity);
        // Flip X by own transform.
        var scale = ins.transform.localScale;
        scale.x = _target.position.x < transform.position.x ? 1 : -1;
        ins.transform.localScale = scale * size;
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    bool ShouldGetBack()
    {
        var bounds = _boundary.bounds;
        var isTargetLeft = _target.position.x < transform.position.x;
        var displacementTarget = Mathf.Abs(_target.position.x - transform.position.x);
        var displacementBoundMin = Mathf.Abs(transform.position.x - bounds.min.x);
        var displacementBoundMax = Mathf.Abs(transform.position.x - bounds.max.x);
        if (isTargetLeft && displacementTarget < _deltaDisplacement && displacementBoundMax > .15f)
        {
            return true;
        }
        if (!isTargetLeft && displacementTarget < _deltaDisplacement && displacementBoundMin > .15f)
        {
            return true;
        }
        return false;
    }

    bool ShouldJump()
    {
        var bounds = _boundary.bounds;
        var isTargetLeft = _target.position.x < transform.position.x;
        var displacementTarget = Mathf.Abs(_target.position.x - transform.position.x);
        var displacementBoundMin = Mathf.Abs(transform.position.x - bounds.min.x);
        var displacementBoundMax = Mathf.Abs(transform.position.x - bounds.max.x);
        if(isTargetLeft && displacementTarget < _deltaDisplacement && displacementBoundMax < .15f){
            return true;
        }
        if(!isTargetLeft && displacementTarget < _deltaDisplacement && displacementBoundMin < .15f){
            return true;
        }
        return false;
    }

    IEnumerator Jump()
    {
        var isTargetLeft = _target.position.x < transform.position.x;
        var jumpX = transform.position.x + 1.325f * (isTargetLeft ? -1 : 1);
        var targetPos = new Vector3(jumpX, transform.position.y, transform.position.z);
        var gravity = JumpVelocityCalculator.GetGravity2D(_rb);
        var jumpVel = JumpVelocityCalculator.Calculate(transform.position, targetPos, gravity, _jumpMaxHeight, true);
        _rb.velocity = jumpVel.velocity;
        yield return new WaitForSeconds(jumpVel.simulatedTime);
    }

    public override IEnumerator Play()
    {
        FlipX();
        _anim.Play(_idle.name);
        if(ShouldJump()){
            _anim.Play(_jumpForward.name);
            yield return StartCoroutine(Jump());
            _anim.Play(_idle.name);
            yield return new WaitForSeconds(.75f);
            yield break;
        }
        if (!ShouldGetBack()){
            yield return new WaitForSeconds(.75f);
            yield break;
        }
        InstantiateTheDust(.75f);
        var dirX = _target.position.x < transform.position.x ? 1 : -1;
        var currentGravity = _rb.gravityScale;
        _rb.gravityScale = _backGravity;
        _rb.velocity = new Vector2(_backVelocity * dirX, _rb.velocity.y);
        _anim.Play(_getBack.name);
        yield return new WaitForSeconds(_getBack.length);
        _anim.Play(_idle.name);
        _rb.gravityScale = currentGravity;
    }

    public override IEnumerator Next()
    {
        var nextList = new Skill [] {_theSlash, _theSlashingKi};
        var rand = Random.Range(0, nextList.Length);
        yield return StartCoroutine(nextList[rand].Play());
        yield return StartCoroutine(nextList[rand].Next());
    }
}

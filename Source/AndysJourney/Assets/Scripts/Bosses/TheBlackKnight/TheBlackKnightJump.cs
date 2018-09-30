using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightJump : Skill
{
    [SerializeField]
    Transform _target;
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
    DirectedDust _directedDust;

    Animator _anim;
    Rigidbody2D _rb;

    TheSlashingKi _theSlashingKi;
    TheBlackKnightSlash _theSlash;
    TheBlackKnightGetBack _theGetBack;
    TheBlackKnightDampingDown _dampingDown;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        // next skills
        _theSlashingKi = GetComponent<TheSlashingKi>();
        _theSlash = GetComponent<TheBlackKnightSlash>();
        _theGetBack = GetComponent<TheBlackKnightGetBack>();
        _dampingDown = GetComponent<TheBlackKnightDampingDown>();
    }

    void FlipX()
    {
        // Flip X by own transform.
        var scale = transform.localScale;
        scale.x = _target.position.x > transform.position.x ? 1 : -1;
        transform.localScale = scale;
    }

    void InstantiateTheDust(float size = 1f)
    {
        var ins = Instantiate<Animator>(_dustFxPrefab, transform.position, Quaternion.identity);
        // Flip X by own transform.
        var scale = ins.transform.localScale;
        scale.x = _target.position.x < transform.position.x ? -1 : 1;
        ins.transform.localScale = scale * size;
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    void InstantiateTheDirectedDust()
    {
        var ins = Instantiate<DirectedDust>(_directedDust, transform.position, Quaternion.identity);
        StartCoroutine(ins.Play(transform));
    }

    IEnumerator Jump()
    {
        var isTargetLeft = _target.position.x < transform.position.x;
        var jumpX = transform.position.x + 1.325f * (isTargetLeft ? -1 : 1);
        var targetPos = new Vector3(jumpX, transform.position.y, transform.position.z);
        var gravity = JumpVelocityCalculator.GetGravity2D(_rb);
        var jumpVel = JumpVelocityCalculator.Calculate(transform.position, targetPos, gravity, _jumpMaxHeight, true);
        _rb.velocity = jumpVel.velocity;
        InstantiateTheDirectedDust();
        InstantiateTheDust(.75f);
        yield return new WaitForSeconds(jumpVel.simulatedTime);
    }

    public override IEnumerator Next()
    {
        yield return StartCoroutine(_theGetBack.Play());
        yield return StartCoroutine(Next(_theSlash, _theSlashingKi, _dampingDown));
    }

    public override IEnumerator Play()
    {
        FlipX();
        _anim.Play(_jumpForward.name);
        yield return StartCoroutine(Jump());
        _anim.Play(_idle.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightDampingDown : Skill
{
    [SerializeField]
    Transform _target;
    [SerializeField]
    Camera _theCamera;
    [SerializeField]
    AnimationClip _jump;
    [SerializeField]
    AnimationClip _dash;
    [SerializeField]
    AnimationClip _getBack;
    [SerializeField]
    float _jumpMaxHeight;
    [SerializeField]
    float _deltaJumpLimit;
    [SerializeField]
    float _timeDashing;
    [SerializeField]
    AnimationClip _dampingDown;
    [SerializeField]
    BoxCollider2D _boundary;
    [SerializeField]
    Animator _dustFxPrefab;
    [SerializeField]
    DirectedDust _directedDust;

    Animator _anim;
    Rigidbody2D _rb;

    TheBlackKnightGetBack _theGetBack;
    TheBlackKnightSlash _slash;
    TheSlashingKi _slashingKi;
    TheBlackKnightDampingDown _theDampingDown;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        // next skills
        _theGetBack = GetComponent<TheBlackKnightGetBack>();
        _slash = GetComponent<TheBlackKnightSlash>();
        _slashingKi = GetComponent<TheSlashingKi>();
        _theDampingDown = GetComponent<TheBlackKnightDampingDown>();
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

    bool ShouldJump()
    {
        var bounds = _boundary.bounds;
        var isTargetLeft = _target.position.x < transform.position.x;
        var displacementTarget = Mathf.Abs(_target.position.x - transform.position.x);
        var halfX = bounds.min.x + (bounds.max.x - bounds.min.x) / 2f;
        var displacementHalfX = Mathf.Abs(transform.position.x - halfX);
        return displacementHalfX >= _deltaJumpLimit;
    }

    IEnumerator Dash()
    {
        FlipX();
        var bounds = _boundary.bounds;
        var halfX = bounds.min.x + (bounds.max.x - bounds.min.x) / 2f;
        var startPos = transform.position;
        var targetPos = new Vector3(halfX, transform.position.y, transform.position.z);
        var isGetBack = _target.position.x < transform.position.x && transform.position.x < halfX;
        var dashingName = isGetBack ? _getBack.name : _dash.name;
        _anim.Play(dashingName);
        InstantiateTheDust(.625f);
        var percent = 0f;
        while (percent <= 1f)
        {
            percent += Time.deltaTime / _timeDashing;
            transform.position = Vector2.Lerp(startPos, targetPos, percent);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Jump()
    {
        FlipX();
        var isTargetLeft = _target.position.x < transform.position.x;
        var bounds = _boundary.bounds;
        var halfX = bounds.min.x + (bounds.max.x - bounds.min.x) / 2f;
        var targetPos = new Vector3(halfX, transform.position.y, transform.position.z);
        var gravity = JumpVelocityCalculator.GetGravity2D(_rb);
        var jumpVel = JumpVelocityCalculator.Calculate(transform.position, targetPos, gravity, _jumpMaxHeight, true);
        _rb.velocity = jumpVel.velocity;
        _anim.Play(_jump.name);
        InstantiateTheDirectedDust();
        InstantiateTheDust(.75f);
        yield return new WaitForSeconds(jumpVel.simulatedTime);
    }

    // Invoke from Animation Event
    void EarthQuake()
    {
        StartCoroutine(Utility.Shaking(.175f, .02f, _theCamera.transform, null, null));
    }

    public override IEnumerator Next()
    {
        yield return StartCoroutine(_theGetBack.Play());
        yield return StartCoroutine(Next(_slash, _slashingKi));
        // yield return StartCoroutine(Next(_theDampingDown));
    }

    public override IEnumerator Play()
    {
        if (!ShouldJump())
        {
            yield return StartCoroutine(Dash());
        }
        else
        {
            yield return StartCoroutine(Jump());
        }
        yield return null;
        _anim.Play(_dampingDown.name);
        yield return new WaitForSeconds(_dampingDown.length);
    }
}

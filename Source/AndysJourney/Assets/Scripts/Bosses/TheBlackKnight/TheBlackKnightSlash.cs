﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightSlash : Skill
{
    [SerializeField]
    Animator _dustFxPrefab;
    [SerializeField]
    Animator _smashThunderFxPrefab;
    [SerializeField]
    Animator _dashingSmashingFireFxPrefab;
    [SerializeField]
    Transform _smashThunderFxPoint;
    [SerializeField]
    AnimationClip _prepare;
    [SerializeField]
    AnimationClip _dashing;
    [SerializeField]
    AnimationClip[] _sequence;
    [SerializeField]
    float[] _velocities;
    [SerializeField]
    Transform _target;
    [SerializeField]
    Camera _theCamera;
    [SerializeField]
    Transform _slashCornerCheck;
    [SerializeField]
    LayerMask _cornerChecks;
    [SerializeField]
    BoxCollider2D _slashCollision;

    Vector2 _lastPos;
    Animator _anim;
    Rigidbody2D _rigid;

    TheSlashingKi _theSlashingKi;
    TheBlackKnightGetBack _theGetBack;
    TheDashingDownWithPower _theSlamDownWithPower;
    TheBlackKnightJump _theJump;
    TheBlackKnightDampingDown _dampingDown;

    int currentExecutedIndex = 0;
    bool _isCornerCollided;
    Collider2D currentCollidedCorner;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        // next skills
        _theSlashingKi = GetComponent<TheSlashingKi>();
        _theGetBack = GetComponent<TheBlackKnightGetBack>();
        _theSlamDownWithPower = GetComponent<TheDashingDownWithPower>();
        _theJump = GetComponent<TheBlackKnightJump>();
        _dampingDown = GetComponent<TheBlackKnightDampingDown>();
    }

    int numberExecuteTheSmashThunderFx;

    void FixedUpdate()
    {
        // corner check
        currentCollidedCorner = Physics2D.OverlapBox(_slashCornerCheck.position, _slashCollision.bounds.extents, 180, _cornerChecks);
        if (_isCornerCollided = currentCollidedCorner == true)
        {
            if (currentExecutedIndex == _sequence.Length - 2)
            {
                if(numberExecuteTheSmashThunderFx == 0){
                    ++numberExecuteTheSmashThunderFx;
                    InstantiateTheSmashThunderFx();
                    EarthQuake();
                }   
            }            
        }

        if(currentExecutedIndex != _sequence.Length - 2){
            numberExecuteTheSmashThunderFx = 0;
        }
    }

    void FlipX()
    {
        var directionOfTarget = DetectTargetDirection();
        // Flip X by direction
        var scale = transform.localScale;
        if (scale.x < 0 && directionOfTarget.x > 0 || scale.x > 0 && directionOfTarget.x < 0)
        {
            scale.x = directionOfTarget.x < 0 ? -1 : 1;
            transform.localScale = scale;
        }
    }

    public override IEnumerator Next()
    {
        yield return StartCoroutine(_theGetBack.Play());
        yield return StartCoroutine(Next(_theJump, _theSlamDownWithPower, _theSlashingKi, _dampingDown));
    }

    public override IEnumerator Play()
    {
        var inx = 0;
        FlipX();
        _anim.Play(_prepare.name);
        yield return new WaitForSeconds(_prepare.length);
        yield return StartCoroutine(Dash());
        inx = 0;
        foreach (var sq in _sequence)
        {
            currentExecutedIndex = inx;
            var isTargetLeft = _target.position.x < transform.position.x;
            var dirX = isTargetLeft ? -1 : 1;
            FlipX();
            // Instantiate the dust.
            if (inx != _sequence.Length - 1)
            {
                InstantiateTheDust(inx == _sequence.Length - 2 ? 1.45f : 1);
            }
            else
            {
                transform.position = _lastPos;
            }
            // Instante the Dashing Smashing Fire
            // if (inx == _sequence.Length - 2)
            // {
            //     StartCoroutine(InstantiateDashingSmashingFire());
            // }
            _anim.Play(sq.name);
            // Calculate velocity by direction
            var vel = Vector2.right * Time.fixedDeltaTime * _velocities[inx] * dirX;
            _rigid.velocity = vel;
            // Wait for the next sequence.
            yield return new WaitForSeconds(sq.length + .0625f);
            // Stop the current velocity
            _rigid.velocity = Vector2.zero;
            // Store the last position for the next sequence.
            _lastPos = transform.position;
            inx++;
        }
        yield return new WaitForSeconds(.125f);
    }

    IEnumerator Dash()
    {
        var dis = Vector2.Distance(transform.position, _target.position);
        if (dis <= .5f * 2)
        {
            yield break;
        }
        InstantiateTheDust(1.45f);
        _anim.Play(_dashing.name);
        var pc = 0f;
        // Flip X by direction
        var isTargetLeft = _target.position.x < transform.position.x;
        var scale = transform.localScale;
        scale.x = isTargetLeft ? -1 : 1;
        transform.localScale = scale;
        var targetPosX = _target.position.x - .5f * scale.x;
        var targetPos = new Vector2(targetPosX, _target.position.y);
        var currentPos = _rigid.position;
        while (pc <= 1f)
        {
            pc += Time.fixedDeltaTime * 4.5f;
            var newX = Mathf.Lerp(currentPos.x, targetPosX, pc);
            _rigid.position = new Vector2(newX, _rigid.position.y);
            yield return null;
        }
    }

    IEnumerator InstantiateDashingSmashingFire()
    {
        var fxLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        var endTime = Time.time + fxLength;
        while (Time.time <= endTime)
        {
            var ins = Instantiate<Animator>(_dashingSmashingFireFxPrefab, transform.position, Quaternion.identity);
            // Flip X by own transform.
            var scale = ins.transform.localScale;
            scale.x = transform.localScale.x;
            ins.transform.localScale = scale * .625f;
            Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
            yield return new WaitForSeconds(fxLength / 5f);
        }
    }

    // Invoke from Animation Event
    void InstantiateTheSmashThunderFx()
    {
        var insPos = new Vector3(currentCollidedCorner.transform.position.x, _slashCornerCheck.position.y, 0f);
        var ins = Instantiate<Animator>(_smashThunderFxPrefab, insPos, Quaternion.identity);
        // Flip X by own transform.
        var scale = ins.transform.localScale;
        scale.x = transform.localScale.x;
        ins.transform.localScale = scale * .6f;
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    // Invoke from Animation Event
    void EarthQuake()
    {
        StartCoroutine(Utility.Shaking2D(.175f, .02f, _theCamera.transform, null, null));
    }

    void InstantiateTheDust(float size = 1f)
    {
        var ins = Instantiate<Animator>(_dustFxPrefab, transform.position, Quaternion.identity);
        var dir = DetectTargetDirection();
        // Flip X by own transform.
        var scale = ins.transform.localScale;
        scale.x = dir.x < 0 ? -1 : dir.x == 0 ? transform.localScale.x : 1;
        ins.transform.localScale = scale * size;
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    Vector2Int DetectTargetDirection()
    {
        var dir = (_target.position - transform.position).normalized;
        return Vector2Int.RoundToInt(dir);
    }
}

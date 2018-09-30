using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSlashingKi : Skill
{
    [SerializeField]
    Transform _target;
    [SerializeField]
    Animator _slashingKiPrefab;
    [SerializeField]
    AnimationClip _appear;
    [SerializeField]
    AnimationClip _disappear;
    [SerializeField]
    AnimationClip _prepare;
    [SerializeField]
    AnimationClip _kiUp;
    [SerializeField]
    AnimationClip _kiDown;
    [SerializeField]
    Transform _theKiSpawnPoint;
    [SerializeField]
    float _kiMaxSpeed;
    [SerializeField]
    float _delay;
    [SerializeField]
    SpawnPoint _spawnPoint1;
    [SerializeField]
    SpawnPoint _spawnPoint2;

    [SerializeField]
    float _backVelocity;
    [SerializeField]
    float _backGravity;
    [SerializeField]
    AnimationClip _getBack;
    [SerializeField]
    BoxCollider2D _boundary;
    [SerializeField]
    Animator _dustFxPrefab;

    AnimationCurve _kiLinear = AnimationCurve.Linear(.0f, .0f, 1f, 1f);
    Animator _anim;
    Transform _cachedTransform;
    Rigidbody2D _rb;
    int[] _kiKinds;

    TheBlackKnightGetBack _theGetBack;
    TheDashingDownWithPower _theSlamDownWithPower;
    TheBlackKnightSlash _theSlash;
    TheBlackKnightJump _theJump;
    TheBlackKnightDampingDown _dampingDown;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _cachedTransform = transform;
        _rb = GetComponent<Rigidbody2D>();
        // Generate by probability
        _kiKinds = Probability.Initialize(new[] { 0, 1 }, new[] { 75f, 25f });

        _theGetBack = GetComponent<TheBlackKnightGetBack>();
        _theSlamDownWithPower = GetComponent<TheDashingDownWithPower>();
        _theSlash = GetComponent<TheBlackKnightSlash>();
        _theJump = GetComponent<TheBlackKnightJump>();
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
        if (isTargetLeft && displacementBoundMax > .625f)
        {
            return true;
        }
        if (!isTargetLeft && displacementBoundMin > .625f)
        {
            return true;
        }
        return false;
    }

    IEnumerator GetBack()
    {
        FlipX();
        if (!ShouldGetBack()){
            yield break;
        }
        InstantiateTheDust(.75f);
        var dirX = _target.position.x < transform.position.x ? 1 : -1;
        var currentGravity = _rb.gravityScale;
        _rb.gravityScale = _backGravity;
        _rb.velocity = new Vector2(_backVelocity * dirX, _rb.velocity.y);
        _anim.Play(_getBack.name);
        yield return new WaitForSeconds(_getBack.length);
        _rb.gravityScale = currentGravity;
    }

    public override IEnumerator Next()
    {
        yield return StartCoroutine(_theGetBack.Play());
        yield return StartCoroutine(Next(_theJump, _theSlamDownWithPower, _theSlash, _dampingDown));
    }

    public override IEnumerator Play()
    {
        yield return StartCoroutine(GetBack());
        var actTimes = Random.Range(1, 2);
        while (actTimes-- > 0)
        {
            FlipX();
            // // Disappear
            // _anim.Play(_disappear.name);
            // yield return new WaitForSeconds(_disappear.length);
            // var spawn = GetRandomSpawnPoint();
            // // Flip X by Spawn point
            // var scale = _cachedTransform.localScale;
            // scale.x = spawn.flipX ? -1 : 1;
            // _cachedTransform.localScale = scale;
            // // Assign into spawn point
            // _cachedTransform.position = spawn.spawn.position;
            // // Appear
            // _anim.Play(_appear.name);
            // yield return new WaitForSeconds(_appear.length);
            // Prepare
            _anim.Play(_prepare.name);
            yield return new WaitForSeconds(_prepare.length);
            yield return StartCoroutine(SlashingKi());
        }
    }

    IEnumerator SlashingKi()
    {
        var slashes = new[] { _kiUp, _kiDown };
        var slashTimes = Random.Range(1, 4);
        while (slashTimes-- > 0)
        {
            foreach (var slash in slashes)
            {
                FlipX();
                _anim.Play(slash.name);
                if (Probability.GetValueInProbability(_kiKinds) == 0)
                {
                    StartCoroutine(GenerateTheSingleKi());
                }
                else
                {
                    StartCoroutine(GenerateTheDoubleKies());
                }
                yield return new WaitForSeconds(slash.length + _delay);
            }
        }
    }

    IEnumerator GenerateTheSingleKi()
    {
        var insPos = _theKiSpawnPoint.position;
        var targetPos = _target.position;
        yield return StartCoroutine(GenerateTheKi(Utility.RotateToTarget(insPos, targetPos, Vector3.forward)));
    }

    IEnumerator GenerateTheDoubleKies()
    {
        var dir = _target.position.x - _theKiSpawnPoint.position.x;
        StartCoroutine(GenerateTheKi(Quaternion.Euler(0f, 0f, dir >= 0 ? 20f : 180f - 20f)));
        StartCoroutine(GenerateTheKi(Quaternion.Euler(0f, 0f, dir >= 0 ? .0f : 180f)));
        yield break;
    }

    IEnumerator GenerateTheKi(Quaternion rot)
    {
        yield return new WaitForSeconds(.02f);
        var ins = Instantiate<Animator>(_slashingKiPrefab, _theKiSpawnPoint.position, Quaternion.identity);
        // Visible the Ki in seconds
        StartCoroutine(VisibleTheKiInSeconds(ins.GetComponent<SpriteRenderer>(), .165f));
        var lengh = ins.GetCurrentAnimatorStateInfo(0).length;
        // Rotate
        ins.transform.rotation = rot;
        // Move to target
        var normalizedDir = rot * Vector3.right;
        var t = 0f;
        while (t <= lengh)
        {
            t += Time.deltaTime;
            var deltaSpeed = t / lengh;
            ins.transform.position += normalizedDir * _kiLinear.Evaluate(deltaSpeed) * _kiMaxSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(ins.gameObject);
    }

    IEnumerator VisibleTheKiInSeconds(SpriteRenderer sprite, float t)
    {
        var insColor = sprite.color;
        insColor.a = 0f;
        sprite.color = insColor;
        yield return new WaitForSeconds(t);
        insColor.a = 255f;
        sprite.color = insColor;
    }

    SpawnPoint GetRandomSpawnPoint()
    {
        var points = new[] { _spawnPoint1, _spawnPoint2 };
        var rand = Random.Range(0, 2);
        return points[rand];
    }
}

[System.Serializable]
public struct SpawnPoint
{
    public Transform spawn;
    public bool flipX;
}

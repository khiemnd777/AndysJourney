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

    AnimationCurve _kiLinear = AnimationCurve.Linear(.0f, .0f, 1f, 1f);
    Animator _anim;
    Transform _cachedTransform;
    int[] _kiKinds;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _cachedTransform = transform;
        // Generate by probability
        _kiKinds = Probability.Initialize(new[] { 0, 1 }, new[] { 75f, 25f });
    }

    public override IEnumerator Play()
    {
        var actTimes = Random.Range(1, 2);
        while (actTimes-- > 0)
        {
            // Disappear
            _anim.Play(_disappear.name);
            yield return new WaitForSeconds(_disappear.length);
            var spawn = GetRandomSpawnPoint();
            // Flip X by Spawn point
            var scale = _cachedTransform.localScale;
            scale.x = spawn.flipX ? -1 : 1;
            _cachedTransform.localScale = scale;
            // Assign into spawn point
            _cachedTransform.position = spawn.spawn.position;
            // Appear
            _anim.Play(_appear.name);
            yield return new WaitForSeconds(_appear.length);
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

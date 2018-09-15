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
    SpawnPoint _spawnPoint1;
    [SerializeField]
    SpawnPoint _spawnPoint2;

    AnimationCurve _kiLinear = AnimationCurve.Linear(.0f, .0f, 1f, 1f);
    Animator _anim;
    Transform _cachedTransform;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _cachedTransform = transform;
        // StartCoroutine(Play());
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
        var delay = .1f;
        while (slashTimes-- > 0)
        {
            foreach (var slash in slashes)
            {
                _anim.Play(slash.name);
                StartCoroutine(GenerateTheKi());
                yield return new WaitForSeconds(slash.length + delay);
            }
        }
    }

    IEnumerator GenerateTheKi()
    {
        yield return new WaitForSeconds(.02f);
        var ins = Instantiate<Animator>(_slashingKiPrefab, _theKiSpawnPoint.position, Quaternion.identity);
        var lengh = ins.GetCurrentAnimatorStateInfo(0).length;
        // Rotate to target
        ins.transform.rotation = Utility.RotateToTarget(ins.transform, _target, Time.deltaTime * 10000f);
        // Move to target
        var normalizedDir = (_target.position - ins.transform.position).normalized;
        var t = 0f;
        while (t <= lengh)
        {
            t += Time.deltaTime;
            var deltaSpeed = t / lengh;
            // deltaSpeed = deltaSpeed > 1 ? 1 : deltaSpeed;
            ins.transform.position += normalizedDir * _kiLinear.Evaluate(deltaSpeed) * _kiMaxSpeed * Time.deltaTime;
            // ins.transform.position += normalizedDir * _kiMaxSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(ins.gameObject);
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

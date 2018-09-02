using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightSlash : MonoBehaviour
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
    AnimationClip[] _sequence;
    [SerializeField]
    float[] _velocities;
    [SerializeField]
    Transform _target;

    Vector2 _lastPos;
    Animator _anim;
    Rigidbody2D _rigid;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();

        StartCoroutine(Play());
    }

    public IEnumerator Play()
    {
        var inx = 0;
        while (true)
        {
            inx = 0;
            foreach (var sq in _sequence)
            {
                var directionOfTarget = DetectTargetDirection();
                // Flip X by direction
                var scale = transform.localScale;
                if (scale.x < 0 && directionOfTarget.x > 0 || scale.x > 0 && directionOfTarget.x < 0)
                {
                    scale.x = directionOfTarget.x < 0 ? -1 : 1;
                    transform.localScale = scale;
                }
                // Instantiate the dust.
                if (inx != _sequence.Length - 1)
                {
                    InstantiateTheDust(inx);
                }
                else
                {
                    transform.position = _lastPos;
                }
                // Instante the Dashing Smashing Fire
                if (inx == _sequence.Length - 2)
                {
                    StartCoroutine(InstantiateDashingSmashingFire());
                }
                _anim.Play(sq.name);
                // Calculate velocity by direction
                var vel = Vector2.right * Time.fixedDeltaTime * _velocities[inx];
                vel.x *= directionOfTarget.x;
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
    }

    IEnumerator Dash()
    {
        _rigid.velocity = Vector2.right * Time.deltaTime * 25f;
        yield return new WaitForFixedUpdate();
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
            ins.transform.localScale = scale;
            Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
            yield return new WaitForSeconds(fxLength / 5f);
        }
    }

    void InstantiateTheSmashThunderFx()
    {
        var ins = Instantiate<Animator>(_smashThunderFxPrefab, _smashThunderFxPoint.position, Quaternion.identity);
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    void InstantiateTheDust(int index)
    {
        var ins = Instantiate<Animator>(_dustFxPrefab, transform.position, Quaternion.identity);
        // Flip X by own transform.
        var scale = ins.transform.localScale;
        scale.x = transform.localScale.x;
        ins.transform.localScale = scale;
        if (index == _sequence.Length - 2)
        {
            ins.transform.localScale = scale * 1.45f;
        }
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    Vector2Int DetectTargetDirection()
    {
        var dir = (_target.position - transform.position).normalized;
        return Vector2Int.RoundToInt(dir);
    }
}

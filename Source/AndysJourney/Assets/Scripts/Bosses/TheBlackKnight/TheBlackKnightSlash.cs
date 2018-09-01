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

    Vector2 _lastPos;
    Vector2 _startPos;
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
        foreach (var sq in _sequence)
        {
            _startPos = transform.position;
            if (inx != _sequence.Length - 1)
            {
                InstantiateTheDust(inx);
            }
            else
            {
                transform.position = _lastPos;
            }
			if(inx == _sequence.Length - 2){
				StartCoroutine(InstantiateDashingSmashingFire());
			}
            _anim.Play(sq.name);
            _rigid.velocity = Vector2.right * Time.fixedDeltaTime * _velocities[inx];
            yield return new WaitForSeconds(sq.length);
            _rigid.velocity = Vector2.zero;
            _lastPos = transform.position;
            inx++;
        }
    }

    IEnumerator Dash()
    {
        _rigid.velocity = Vector2.right * Time.deltaTime * 25f;
        yield return new WaitForFixedUpdate();
    }

    IEnumerator InstantiateDashingSmashingFire()
    {
        var endTime = Time.time + _anim.GetCurrentAnimatorStateInfo(0).length;
        while (Time.time <= endTime)
        {
			var ins = Instantiate<Animator>(_dashingSmashingFireFxPrefab, transform.position, Quaternion.identity);
			Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
			yield return new WaitForSeconds(endTime/20f);
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
        if (index == _sequence.Length - 1)
        {
            ins.transform.localScale = Vector3.one * 1.45f;
        }
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }
}

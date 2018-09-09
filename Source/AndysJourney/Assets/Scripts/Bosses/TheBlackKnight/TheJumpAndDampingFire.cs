using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheJumpAndDampingFire : Skill
{
    [SerializeField]
    Transform _target;
    [SerializeField]
    AnimationClip _prepare;
    [SerializeField]
    AnimationClip _jump;
    [SerializeField]
    AnimationClip _damping;
    [SerializeField]
    float _height;
    [SerializeField]
    bool _debug;

    Animator _anim;
    Rigidbody2D _rb;
    Transform _cachedTransform;

    JumpVelocityCalculator _jumpCalc;

    void Start()
    {
        _cachedTransform = transform;
        _jumpCalc = new JumpVelocityCalculator();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (_debug)
        {
			var gravity = _jumpCalc.GetGravity2D(_rb);
            _jumpCalc.DrawPath(transform, _target.transform, gravity, _height, true);
        }
    }

    void Jump()
    {
		var gravity = _jumpCalc.GetGravity2D(_rb);
        _rb.velocity = _jumpCalc.Calculate(transform, _target.transform, gravity, _height, true).velocity;
    }

    public override IEnumerator Play()
    {
        // yield return StartCoroutine(Utility.JumpToDestination(_cachedTransform, _cachedTransform.position, _target.position));
        // _rb.velocity = Utility.CalculateJumpVelocity2(_cachedTransform.position, _target.position, .2f, Physics2D.gravity.magnitude);
        yield break;
    }
}

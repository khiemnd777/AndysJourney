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
	AnimationClip _idle;
	[SerializeField]
	BoxCollider2D _boundary;
	[SerializeField]
    Animator _dustFxPrefab;

	Animator _anim;
	Rigidbody2D _rb;

	void Awake()
	{
		_anim = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody2D>();
	}

	void FlipX()
    {
        var dir = DetectTargetDirection();
        // Flip X by own transform.
        var scale = transform.localScale;
        scale.x = dir.x < 0 ? -1 : dir.x == 0 ? transform.localScale.x : 1;
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
        var dir = DetectTargetDirection();
        // Flip X by own transform.
        var scale = ins.transform.localScale;
        scale.x = dir.x < 0 ? 1 : -1;
        ins.transform.localScale = scale * size;
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    public override IEnumerator Play()
    {
		FlipX();
		InstantiateTheDust(.75f);
		var dir = DetectTargetDirection();
		var dirX = dir.x < 0 ? 1 : -1;
        var currentGravity = _rb.gravityScale;
        _rb.gravityScale = _backGravity;
		_rb.velocity = new Vector2(_backVelocity * dirX, _rb.velocity.y);
		_anim.Play(_getBack.name);
		yield return new WaitForSeconds(_getBack.length);
		_anim.Play(_idle.name);
        _rb.gravityScale = currentGravity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSlashingKiWhenGetBack : Skill
{
	[SerializeField]
	Transform _target;
	[SerializeField]
	AnimationClip _getBack;
	[SerializeField]
	AnimationClip _idle;
	[SerializeField]
	BoxCollider2D _boundary;
	[SerializeField]
	Animator _slashingKiPrefab;
	[SerializeField]
	Transform _theKiSpawnPoint;
	[SerializeField]
    float _kiMaxSpeed;
	[SerializeField]
    Animator _dustFxPrefab;

	AnimationCurve _kiLinear = AnimationCurve.Linear(.0f, .0f, 1f, 1f);

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

    IEnumerator GenerateTheSingleKi()
    {
        var insPos = _theKiSpawnPoint.position;
        var targetPos = _target.position;
        yield return StartCoroutine(GenerateTheKi(Utility.RotateToTarget(insPos, targetPos, Vector3.forward)));
    }

	IEnumerator GenerateTheKi(Quaternion rot)
    {
        yield return new WaitForSeconds(.02f);
        var ins = Instantiate<Animator>(_slashingKiPrefab, _theKiSpawnPoint.position, Quaternion.identity);
		ins.transform.localScale = Vector3.one * .9f;
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
		InstantiateTheDust(.625f);
		StartCoroutine(GenerateTheSingleKi());
		var dir = DetectTargetDirection();
		var dirX = dir.x < 0 ? 1 : -1;
		_rb.velocity = new Vector2(1.75f * dirX, _rb.velocity.y);
		_anim.Play(_getBack.name);
		yield return new WaitForSeconds(_getBack.length);
		_anim.Play(_idle.name);
        yield break;
    }

    public override IEnumerator Next()
    {
        yield break;
    }
}

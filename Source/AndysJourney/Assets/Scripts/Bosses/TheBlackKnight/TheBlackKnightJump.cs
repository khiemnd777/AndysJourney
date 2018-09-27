// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TheJumpAndDampingFire : Skill
// {
//     [SerializeField]
//     Transform _target;
//     [SerializeField]
//     AnimationClip _prepare;
//     [SerializeField]
//     AnimationClip _jump;
//     [SerializeField]
//     AnimationClip _smashDown;
// 	[SerializeField]
//     AnimationClip _smashDownOnGround;
//     [SerializeField]
//     float _jumpMaxHeight;
//     [SerializeField]
// 	Transform _executedPoint;
// 	[SerializeField]
// 	DirectedDust _directedDust;
// 	[SerializeField]
//     Animator _dustFxPrefab;
// 	[SerializeField]
// 	Transform _groundCheck;
// 	[SerializeField]
// 	float _smashDownVelocity;
// 	[SerializeField]
// 	LayerMask _groundLayer;
// 	[SerializeField]
// 	Camera _theCamera;
// 	[SerializeField]
// 	float _earthQuakeDuration;
// 	[SerializeField]
// 	float _earthQuakeAmount;

// 	BoxCollider2D _boxCollider;
//     Animator _anim;
//     Rigidbody2D _rb;
//     Transform _cachedTransform;
// 	bool _isOnGround;

//     void Awake()
//     {
//         _cachedTransform = transform;
//         _anim = GetComponent<Animator>();
//         _rb = GetComponent<Rigidbody2D>();
// 		_boxCollider = GetComponent<BoxCollider2D>();
//     }

// 	void Update(){
// 		// var targetPos = DetectExecutedJump();
// 		// var gravity = JumpVelocityCalculator.GetGravity2D(_rb);
// 		// JumpVelocityCalculator.DrawPath(_cachedTransform.position, targetPos, gravity, _jumpMaxHeight, true);
// 	}

// 	void FixedUpdate()
// 	{
// 		// Ground check
// 		var state = Physics2D.OverlapBox(_groundCheck.position, _boxCollider.bounds.extents, 90, _groundLayer);	
// 		_isOnGround = state == true;
// 	}

// 	Vector2 DetectExecutedJump(){
// 		var executedPos = _target.position;
// 		executedPos.y = _executedPoint.position.y;
// 		return executedPos;
// 	}

// 	IEnumerator Jump()
//     {
// 		var targetPos = DetectExecutedJump();
// 		var gravity = JumpVelocityCalculator.GetGravity2D(_rb);
// 		var jumpVel = JumpVelocityCalculator.Calculate(_cachedTransform.position, targetPos, gravity, _jumpMaxHeight, true);
//         _rb.velocity = jumpVel.velocity;
// 		yield return new WaitForSeconds(jumpVel.simulatedTime);
//     }

// 	void InstantiateTheDust(float size = 1f)
//     {
//         var ins = Instantiate<Animator>(_dustFxPrefab, _cachedTransform.position, Quaternion.identity);
//         var dir = DetectTargetDirection();
//         // Flip X by own transform.
//         var scale = ins.transform.localScale;
//         scale.x = dir.x < 0 ? -1 : dir.x == 0 ? _cachedTransform.localScale.x : 1;
//         ins.transform.localScale = scale * size;
//         Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
//     }

// 	Vector2Int DetectTargetDirection()
//     {
//         var dir = (_target.position - _cachedTransform.position).normalized;
//         return Vector2Int.RoundToInt(dir);
//     }

// 	void FlipX(){
// 		var dir = DetectTargetDirection();
//         // Flip X by own transform.
//         var scale = _cachedTransform.localScale;
//         scale.x = dir.x < 0 ? -1 : dir.x == 0 ? _cachedTransform.localScale.x : 1;
//         _cachedTransform.localScale = scale;
// 	}

//     public override IEnumerator Play()
//     {
// 		if(!_isOnGround)
// 			yield break;
// 		FlipX();
// 		// Prepare
// 		_anim.Play(_prepare.name);
// 		yield return new WaitForSeconds(_prepare.length);
// 		FlipX();
// 		// Jump
// 		InstantiateTheDust(1.325f);
// 		InstantiateTheDirectedDust();
// 		_anim.Play(_jump.name);
// 		yield return StartCoroutine(Jump());
// 		// Smash down
// 		_anim.Play(_smashDown.name);
// 		_rb.velocity = Vector2.up * _smashDownVelocity;
// 		yield return new WaitUntil(() => _isOnGround);
// 		_rb.velocity = Vector2.zero;
// 		// Smash down on ground
// 		EarthQuake();
// 		_anim.Play(_smashDownOnGround.name);
// 		yield return new WaitForSeconds(_smashDownOnGround.length);
//     }

// 	void EarthQuake(){
//         StartCoroutine(Utility.Shaking(_earthQuakeDuration, _earthQuakeAmount, _theCamera.transform, null, null));
//     }

// 	void InstantiateTheDirectedDust(){
// 		var ins = Instantiate<DirectedDust>(_directedDust, _cachedTransform.position, Quaternion.identity);
// 		StartCoroutine(ins.Play(_cachedTransform));
// 	}
// }

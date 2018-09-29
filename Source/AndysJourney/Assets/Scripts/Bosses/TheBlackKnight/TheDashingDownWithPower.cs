using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheDashingDownWithPower : Skill
{
    [SerializeField]
    Transform _target;
    [SerializeField]
    AnimationClip _slide;
    [SerializeField]
    Transform _executedPoint;
    [SerializeField]
    float _jumpMaxHeight;
    [SerializeField]
    float _smashDownVelocity;
    [SerializeField]
    float _preparedDashingDownVelocity;
    [SerializeField]
    float maxDisplacementToSlide;
    [SerializeField]
    Transform _groundCheck;
    [SerializeField]
    LayerMask _groundLayer;
    [SerializeField]
    DirectedDust _directedDust;
    [SerializeField]
    Camera _theCamera;
    [SerializeField]
    AnimationClip _prepare;
    [SerializeField]
    AnimationClip _prepareDashingDown;
    [SerializeField]
    AnimationClip _jump;
    [SerializeField]
    AnimationClip _smashDown;
    [SerializeField]
    AnimationClip _smashDownOnGround;
    [SerializeField]
    Transform _blackFirePrefab;
    [SerializeField]
    Animator _dustFxPrefab;
    [SerializeField]
    float _earthQuakeDuration;
    [SerializeField]
    float _earthQuakeAmount;
    [SerializeField]
    BoxCollider2D _boundary;

    Animator _anim;
    BoxCollider2D _boxCollider;
    Rigidbody2D _rb;
    Transform _cachedTransform;
    bool _isOnGround;

    TheBlackKnightGetBack _theGetBack;
    TheBlackKnightSlash _theSlash;
    TheSlashingKi _theSlashingKi;

    // Use this for initialization
    void Awake()
    {
        _cachedTransform = transform;
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        // next skills
        _theGetBack = GetComponent<TheBlackKnightGetBack>();
        _theSlash = GetComponent<TheBlackKnightSlash>();
        _theSlashingKi = GetComponent<TheSlashingKi>();
    }

    void Update(){
        var targetPos = DetectExecutedJump();
        var storedGravityScale = _rb.gravityScale;
        var gravity = JumpVelocityCalculator.GetGravity2D(_rb);
        JumpVelocityCalculator.DrawPath(_cachedTransform.position, targetPos, gravity, _jumpMaxHeight, false);
    }

    void FixedUpdate()
    {
        // Ground check
        var state = Physics2D.OverlapBox(_groundCheck.position, _boxCollider.bounds.extents, 90, _groundLayer);
        _isOnGround = state == true;
    }

    bool DetectTargetDisplacementToSlide(){
        var displacement = _target.position - _cachedTransform.position;
        return Mathf.Abs(displacement.x) - maxDisplacementToSlide >= 0;
    }

    Vector2Int DetectTargetDirection()
    {
        var dir = (_target.position - _cachedTransform.position).normalized;
        return Vector2Int.RoundToInt(dir);
    }

    void FlipX()
    {
        var dir = DetectTargetDirection();
        // Flip X by own transform.
        var scale = _cachedTransform.localScale;
        scale.x = dir.x < 0 ? -1 : dir.x == 0 ? _cachedTransform.localScale.x : 1;
        _cachedTransform.localScale = scale;
    }

    Vector2 DetectExecutedJump(){
		var executedPos = _target.position;
		executedPos.y = _executedPoint.position.y;
		return executedPos;
	}

    IEnumerator Jump()
    {
        // var targetPos = new Vector3(_boundary.transform.position.x, _executedPoint.position.y, 0);
        var targetPos = DetectExecutedJump();
        var storedGravityScale = _rb.gravityScale;
        _rb.gravityScale *= 1.125f;
        var gravity = JumpVelocityCalculator.GetGravity2D(_rb);
        var jumpVel = JumpVelocityCalculator.Calculate(_cachedTransform.position, targetPos, gravity, _jumpMaxHeight, true);
        _rb.velocity = jumpVel.velocity;
        yield return new WaitForSeconds(jumpVel.simulatedTime);
        _rb.gravityScale = storedGravityScale;
    }

    IEnumerator Slide(){
        FlipX();
        InstantiateTheDust();
        var faceDir = _cachedTransform.localScale.x > 0 ? 1 : -1;
        var currentGravityScale = _rb.gravityScale;
        // _rb.gravityScale = 1f;
        _rb.velocity = new Vector3(2.75f *  faceDir, 0f);
        _anim.Play(_slide.name);
        // yield return new WaitUntil(() => Mathf.Approximately(_rb.velocity.x, 0));
        yield return new WaitForSeconds(_slide.length);
        _rb.velocity = Vector2.zero;
        _rb.gravityScale = currentGravityScale;
    }

    void EarthQuake()
    {
        StartCoroutine(Utility.Shaking(_earthQuakeDuration, _earthQuakeAmount, _theCamera.transform, null, null));
    }

    void InstantiateTheDirectedDust()
    {
        var ins = Instantiate<DirectedDust>(_directedDust, _cachedTransform.position, Quaternion.identity);
        StartCoroutine(ins.Play(_cachedTransform));
    }

    void InstantiateTheDust(float size = 1f)
    {
        var ins = Instantiate<Animator>(_dustFxPrefab, _cachedTransform.position, Quaternion.identity);
        var dir = DetectTargetDirection();
        // Flip X by own transform.
        var scale = ins.transform.localScale;
        scale.x = dir.x < 0 ? -1 : dir.x == 0 ? _cachedTransform.localScale.x : 1;
        ins.transform.localScale = scale * size;
        Destroy(ins.gameObject, ins.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator PrepareDashingDown()
    {
        var gs = _rb.gravityScale;
        _anim.Play(_prepareDashingDown.name);
        _rb.velocity = Vector2.up * _preparedDashingDownVelocity;
        _rb.gravityScale = gs / 5f;
        yield return new WaitForSeconds(_prepareDashingDown.length / 2.25f);
        _rb.gravityScale = gs;
    }

	float _totalGenerateBlackFireTime;

    IEnumerator GenerateBlackFire(int side)
    {
        var generatedDeltaTime = Time.fixedDeltaTime * 4f;
        var min = _boundary.bounds.min;
        var fireDeltaSpace = .4f;
        var offsetX = _boundary.transform.position.x - fireDeltaSpace - min.x;
        var numberOfFires = Mathf.FloorToInt(offsetX / fireDeltaSpace);
        var count = 0;
        var y = min.y;
		_totalGenerateBlackFireTime = numberOfFires * generatedDeltaTime;
        while (count < numberOfFires)
        {
            var firePos = new Vector2(fireDeltaSpace * (count + 1) * side, y);
            var blackFire = Instantiate<Transform>(_blackFirePrefab, firePos, Quaternion.identity);
            var blackFireAnimator = blackFire.GetComponent<Animator>();
            Destroy(blackFire.gameObject, blackFireAnimator.GetCurrentAnimatorStateInfo(0).length);
            yield return new WaitForSeconds(generatedDeltaTime);
            ++count;
        }
    }

    public override IEnumerator Play()
    {
        if (!_isOnGround)
            yield break;
        FlipX();
        // Prepare
        _anim.Play(_prepare.name);
        yield return new WaitForSeconds(_prepare.length);
        if(DetectTargetDisplacementToSlide()){
            yield return StartCoroutine(Slide());
        }
        FlipX();
        // Jump
        InstantiateTheDust(1.325f);
        InstantiateTheDirectedDust();
        _anim.Play(_jump.name);
        yield return StartCoroutine(Jump());
        // Prepare to Dashing down
        yield return StartCoroutine(PrepareDashingDown());
        // Slam down
        _anim.Play(_smashDown.name);
        _rb.velocity = Vector2.up * _smashDownVelocity;
        yield return new WaitUntil(() => _isOnGround);
        _rb.velocity = Vector2.zero;
        // Smash down on ground
        EarthQuake();
        _anim.Play(_smashDownOnGround.name);
        // Generate black fires
        // StartCoroutine(GenerateBlackFire(1));
        // StartCoroutine(GenerateBlackFire(-1));
		
        yield return new WaitForSeconds(_smashDownOnGround.length + _totalGenerateBlackFireTime);
    }

    public override IEnumerator Next()
    {
        yield return StartCoroutine(_theGetBack.Play());
        yield return StartCoroutine(Next(_theSlash, _theSlashingKi));
    }
}

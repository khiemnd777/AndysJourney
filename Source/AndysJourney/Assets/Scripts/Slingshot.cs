using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public Transform projectileUp;
    public Transform projectileDown;
    public Transform projectileLeft;
    public Transform projectileRight;
    public Transform projectileUpRight;
    public Transform projectileUpLeft;
    public Transform projectileDownRight;
    public Transform projectileDownLeft;
    [Space]
    public Projectile stonedBulletPrefab;
    public float force = 2f;
    [Space]
    public Projectile bigShotStoneBulletPrefab;
    public float bigShotTakingTime = 1.5f;

    SpriteRenderer _renderer;
    Animator _animator;
    DirectionGetter2D _directionGetter;
    PlayerMovement _movement;
    bool _isTakingBigShot;
    float _holdBigShotTime;

    const string _slingshotHoldTriggerStr = "slingshot hold trigger";
    const string _slingshotTriggerReleaseStr = "slingshot trigger release";

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _directionGetter = GetComponent<DirectionGetter2D>();
        _movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HoldTrigger();
        ReleaseTrigger();
    }

    void HoldTrigger()
    {
        if (Input.GetKey(KeyCode.J))
        {
            _movement.stopX = _movement.stopY = true;
            _animator.SetBool(_slingshotHoldTriggerStr, true);
            _animator.SetBool(_slingshotTriggerReleaseStr, false);
            // attempt to get a big shot
            if (_isTakingBigShot)
                return;
            _isTakingBigShot = true;
            _holdBigShotTime = Time.time;
            StartCoroutine(PowerHoldingOn());
        }
    }

    void ReleaseTrigger()
    {
        if (Input.GetKeyUp(KeyCode.J))
        {
            _movement.stopX = _movement.stopY = false;
            _animator.SetBool(_slingshotHoldTriggerStr, false);
            _animator.SetBool(_slingshotTriggerReleaseStr, true);
            // release the trigger after holding
            _isTakingBigShot = false;
            var holdInTime = Time.time - _holdBigShotTime;
            if (holdInTime >= bigShotTakingTime)
            {
                Debug.Log("Big shot");
                InstantiateBigShotBullet();
            }
            else
            {
                Debug.Log("Ordinary shot");
                InstantiateBullet();
            }
        }
    }

    void InstantiateBigShotBullet()
    {
        var projectileDir = GetProjectileDirection();
        var bulletAngle = GetBulletAngle();
        var bulletDirRot = Quaternion.AngleAxis(bulletAngle, Vector3.forward);
        // var angle = Mathf.Atan2(_directionGetter.direction.y, _directionGetter.direction.x) * Mathf.Rad2Deg;
        var bullet = Instantiate<Projectile>(bigShotStoneBulletPrefab, projectileDir.position, bulletDirRot);
        bullet.gameObject.SetActive(true);
        bullet.direction = _directionGetter.direction;
        bullet.projectileAngle = bulletAngle;
        var bulletRigid = bullet.GetComponent<Rigidbody2D>();
        bulletRigid.velocity = _directionGetter.direction * force;
    }

    void InstantiateBullet()
    {
        var projectileDir = GetProjectileDirection();
        var bulletAngle = GetBulletAngle();
        var bulletDirRot = Quaternion.AngleAxis(bulletAngle, Vector3.forward);
        // var angle = Mathf.Atan2(_directionGetter.direction.y, _directionGetter.direction.x) * Mathf.Rad2Deg;
        var bullet = Instantiate<Projectile>(stonedBulletPrefab, projectileDir.position, bulletDirRot);
        bullet.gameObject.SetActive(true);
        bullet.direction = _directionGetter.direction;
        bullet.projectileAngle = bulletAngle;
        var bulletRigid = bullet.GetComponent<Rigidbody2D>();
        bulletRigid.velocity = _directionGetter.direction * force;
    }

    IEnumerator PowerHoldingOn()
    {
        // yield return new WaitForSeconds(bigShotTakingTime);
        var t = Time.time;
        while(true){
            if(!_isTakingBigShot)
                yield break;
            if (bigShotTakingTime < Time.time - t){
                break;
            }
            yield return null;
        }
        Debug.Log("Power Holding On");
        var count = 5;
        while(count-- >= 0){
            var spriteObj = new GameObject("Power Holding On", typeof(SpriteRenderer));
            var spriteTransform = spriteObj.transform;
            spriteTransform.position = transform.position;
            spriteTransform.localScale = Vector3.one * 1.625f;
            // spriteTransform.SetParent(transform);
            var spriteObjRenderer = spriteObj.GetComponent<SpriteRenderer>();
            spriteObjRenderer.sprite = _renderer.sprite;
            spriteObjRenderer.sortingOrder = 98;
            spriteObjRenderer.color = new Color(255f, 255f, 255f, .175f);
            StartCoroutine(ScaleShadowToOriginal(spriteTransform));
            yield return new WaitForSeconds(Time.deltaTime * 2f);
        }
    }

    IEnumerator ScaleShadowToOriginal(Transform shadow)
    {
        var percent = 0f;
        while(percent <= 1f)
        {
            percent += Time.deltaTime * 2.75f;
            shadow.localScale = Vector3.Lerp(Vector3.one * 1.625f, Vector3.one, percent);
            yield return null;
        }
        Destroy(shadow.gameObject);
    }

    Transform GetProjectileDirection()
    {
        return _directionGetter.directionEnum == Direction.Down ? projectileDown
            : _directionGetter.directionEnum == Direction.Up ? projectileUp
                : _directionGetter.directionEnum == Direction.Left ? projectileLeft
                    : _directionGetter.directionEnum == Direction.Right ? projectileRight
                        : _directionGetter.directionEnum == Direction.DownLeft ? projectileDownLeft
                            : _directionGetter.directionEnum == Direction.DownRight ? projectileDownRight
                                : _directionGetter.directionEnum == Direction.UpLeft ? projectileUpLeft
                                    : projectileUpRight;
    }

    float GetBulletAngle()
    {
        return _directionGetter.directionEnum == Direction.Down ? 0f
            : _directionGetter.directionEnum == Direction.Up ? 180f
                : _directionGetter.directionEnum == Direction.Right ? 90f
                    : _directionGetter.directionEnum == Direction.Left ? 270f
                        : _directionGetter.directionEnum == Direction.DownRight ? 45f
                            : _directionGetter.directionEnum == Direction.UpRight ? 135f
                                : _directionGetter.directionEnum == Direction.UpLeft ? 225f
                                    : 315f;
    }
}

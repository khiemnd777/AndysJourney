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

    Animator _animator;
    DirectionGetter2D _directionGetter;
    PlayerMovement _movement;

    const string _slingshotHoldTriggerStr = "slingshot hold trigger";
    const string _slingshotTriggerReleaseStr = "slingshot trigger release";

    void Start()
    {
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
        }
    }

    void ReleaseTrigger()
    {
        if (Input.GetKeyUp(KeyCode.J))
        {
            _movement.stopX = _movement.stopY = false;
            _animator.SetBool(_slingshotHoldTriggerStr, false);
            _animator.SetBool(_slingshotTriggerReleaseStr, true);
            // do action
            InstantiateBullet();
        }
    }

    void InstantiateBullet()
    {
        var projectileDir = _directionGetter.directionEnum == Direction.Down ? projectileDown
            : _directionGetter.directionEnum == Direction.Up ? projectileUp
                : _directionGetter.directionEnum == Direction.Left ? projectileLeft
                    : _directionGetter.directionEnum == Direction.Right ? projectileRight
                        : _directionGetter.directionEnum == Direction.DownLeft ? projectileDownLeft
                            : _directionGetter.directionEnum == Direction.DownRight ? projectileDownRight
                                : _directionGetter.directionEnum == Direction.UpLeft ? projectileUpLeft
                                    : projectileUpRight;
        var bulletAngle = _directionGetter.directionEnum == Direction.Down ? 0f
            : _directionGetter.directionEnum == Direction.Up ? 180f
                : _directionGetter.directionEnum == Direction.Right ? 90f
                    : _directionGetter.directionEnum == Direction.Left ? 270f
                        : _directionGetter.directionEnum == Direction.DownRight ? 45f
                            : _directionGetter.directionEnum == Direction.UpRight ? 135f
                                : _directionGetter.directionEnum == Direction.UpLeft ? 225f
                                    : 315f;
        var bulletDirRot = Quaternion.AngleAxis(bulletAngle, Vector3.forward);
        // var angle = Mathf.Atan2(_directionGetter.direction.y, _directionGetter.direction.x) * Mathf.Rad2Deg;
        var bullet = Instantiate<Projectile>(stonedBulletPrefab, projectileDir.position, bulletDirRot);
        bullet.gameObject.SetActive(true);
        bullet.direction = _directionGetter.direction;
        bullet.projectileAngle = bulletAngle;
        var bulletRigid = bullet.GetComponent<Rigidbody2D>();
        bulletRigid.velocity = _directionGetter.direction * force;
    }
}

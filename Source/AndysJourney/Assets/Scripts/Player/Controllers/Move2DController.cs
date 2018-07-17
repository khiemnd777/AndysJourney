using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2DController : PlayerController
{
    public float deltaDistance = .008f;
    public float jumpForce;
    public int extraJumpCount;
    [Header("Ground check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    [Header("Collision check")]
    public Transform beindCollisionCheck;
    public float collisionCheckRadius;
    public LayerMask collisionLayer;
    [Header("Wall sliding/jumping")]
    public float frictionY;
    public bool allowWallSliding;

    bool _isOnGround = true;
    bool _isMoving;
    bool _isJump;
    bool _isBehindCollision;
    bool _jumpByWall;
    bool _lockMove;
    bool _lockMoveOnGround;
    bool _lockJump;
    int _extraJump;

    public override void Start()
    {
        base.Start();
        _extraJump = extraJumpCount;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        var dirX = _player.GetInputHorizontal() * (deltaDistance / Time.fixedDeltaTime);
        _player.movement.AddHorizontal(dirX);
    }

    public bool Active()
    {
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : MonoBehaviour
{
    public Transform carrier;
    public float throwingForce = 5f;
    SpriteRenderer _spriteRenderer;
    Animator _animator;
    PlayerStateHandler _stateHandler;
    CarryableObject _objectCanCarried;
    CarryableObject _carriedObject;
    Collider2D _objectCollider;
    Transform _cachedTransform;
    DirectionGetter2D _directionGetter;
    PlayerMovement _player;
    float _deltaWeight = .075f;
    bool _canCarry;
    bool _carrying;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _stateHandler = GetComponent<PlayerStateHandler>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _directionGetter = GetComponent<DirectionGetter2D>();
        _player = GetComponent<PlayerMovement>();
        _cachedTransform = transform;
    }

    void Update()
    {
        CarryInput();
        SortOrderWhenGoesUp();
    }

    void SortOrderWhenGoesUp()
    {
        
    }

    void CarryInput()
    {
        if (!Input.GetKeyDown(KeyCode.K))
            return;
        if (_canCarry)
        {
            // Carry
            _canCarry = false;
            _carrying = true;
            _stateHandler.state = PlayerState.Carrying;
            _objectCanCarried.Carry(carrier, _spriteRenderer);
            _objectCollider.isTrigger = true;
            _carriedObject = _objectCanCarried;
            _player.speed -= _deltaWeight;
            _objectCanCarried = null;
        }
        else if (_carrying)
        {
            // Throw
            _carrying = false;
            _carriedObject.transform.SetParent(null);
            var carriedObjRigid = _carriedObject.GetComponent<Rigidbody2D>();
            _carriedObject.startDirection = _directionGetter.direction;
            _carriedObject.Throw(_directionGetter.direction, throwingForce);
            // _objectCollider.isTrigger = false;
            _stateHandler.state = PlayerState.Walking;
            _player.speed += _deltaWeight;
            _carriedObject = null;
            _objectCollider = null;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        var obstacleObj = other.gameObject.GetComponent<CarryableObject>();
        if (obstacleObj != null && obstacleObj is Object && !obstacleObj.Equals(null))
        {
            // be able carrying
            if(_carriedObject != null && _carriedObject is Object && !_carriedObject.Equals(null))
                return;
            _canCarry = obstacleObj.canCarry;
            _objectCanCarried = obstacleObj;
            _objectCollider = other.collider;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        _canCarry = false;
        _objectCollider = null;
        _objectCanCarried = null;
    }
}
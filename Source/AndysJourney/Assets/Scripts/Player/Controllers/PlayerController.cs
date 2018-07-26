using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Animator _anim;
    protected BoxCollider2D _col;
    protected SpriteRenderer _sprite;
    protected float _faceX;
    protected PlayerSettings _player;

    public virtual void Awake()
    {
        _player = GetComponent<PlayerSettings>();
    }

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _faceX = 1;
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        _faceX = _player.faceX;
    }

    public virtual void LateUpdate()
    {

    }
}
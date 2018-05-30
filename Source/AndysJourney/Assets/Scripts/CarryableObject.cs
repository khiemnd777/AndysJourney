using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryableObject : InteractableObject
{
    [Header("Carryable")]
    public bool canCarry;
    public float weight = 1f;
    [System.NonSerialized]
    protected bool _throwing;
    protected Vector2 _velocity;
    protected Rigidbody2D _rigidbody;
    protected SpriteRenderer _spriteRenderer;
    protected Vector3 _originalScaleRatio;
    protected float maxRange = 1f;

    public override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Throwing();
    }

    void Throwing()
    {
        if (!_throwing)
            return;
        _rigidbody.velocity = _velocity;
    }

    public virtual void Throw(Vector2 direction, float force)
    {
        if (_throwing)
            return;
        transform.localScale = _originalScaleRatio;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var deltaUp = angle < 0 ? -1f : 1f;
        angle = angle >= 90 ? angle - 180 : angle;
        angle = deltaUp == -1f ? angle * deltaUp - 90 : angle;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _velocity = direction * force / weight;
        _throwing = true;
    }

    public virtual void Carry(Transform carrier, SpriteRenderer parentSpriteRenderer)
    {
        _originalScaleRatio = transform.localScale;
        transform.SetParent(carrier);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * 1.25f;
        // sort order
        if (parentSpriteRenderer == null)
            return;
        if (_spriteRenderer != null && _spriteRenderer is Object && !_spriteRenderer.Equals(null))
        {
            var carrierSpriteRenderer = parentSpriteRenderer.GetComponent<SpriteRenderer>();
            if (carrierSpriteRenderer != null && carrierSpriteRenderer is Object && !carrierSpriteRenderer.Equals(null))
            {
                _spriteRenderer.sortingOrder = carrierSpriteRenderer.sortingOrder + 1;
            }
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        var obstacleObj = other.GetComponent<InteractableObject>();
        if (obstacleObj != null && obstacleObj is Object && !obstacleObj.Equals(null))
        {
            if (_throwing)
            {
                obstacleObj.Hit(transform);
                other.isTrigger = false;
                _throwing = false;
                _velocity = Vector3.zero;
                // _rigidbody.bodyType = RigidbodyType2D.Static;
            }
        }
    }
}

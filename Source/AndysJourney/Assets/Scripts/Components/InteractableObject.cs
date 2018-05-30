using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [System.NonSerialized]
    public Vector2 startDirection;
    [Header("Destructable")]
    public bool destructable;
    public float destructableHealth = 1;
    public System.Action onDestruct;

    Vector3 _originalLocalPosition;

    public virtual void Start()
    {
        destructableHealth = destructableHealth <= 0 ? 1 : destructableHealth;
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Hit(Transform hitFrom)
    {
        StartCoroutine(Shaking(0.2f, .02f));
        TakeDamage();
    }

    public virtual void TakeDamage()
    {
        if (!destructable)
            return;
        --destructableHealth;
        if (destructableHealth == 0){
            if(onDestruct != null)
                onDestruct();
            Destroy(gameObject);
        }
    }

    public virtual void DestroyByInteraction()
    {
        
    }

    IEnumerator Shaking(float duration, float amount)
    {
        _originalLocalPosition = transform.localPosition;
        var endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            transform.localPosition = _originalLocalPosition + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _originalLocalPosition;
    }
}

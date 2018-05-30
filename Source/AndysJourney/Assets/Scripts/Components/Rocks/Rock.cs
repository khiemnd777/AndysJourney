using UnityEngine;

public class Rock : CarryableObject
{
    [Header("Others")]
    public Transform destructablePrefab;
    public ParticleSystem dustPrefab;

    public override void Throw(Vector2 direction, float force)
    {
        var dust = Instantiate<ParticleSystem>(dustPrefab, Vector3.zero, Quaternion.identity, transform);
        dust.transform.localPosition = Vector3.zero;
        base.Throw(direction, force);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        var obstacleObj = other.GetComponent<InteractableObject>();
        if (obstacleObj != null && obstacleObj is Object && !obstacleObj.Equals(null))
        {
            if (_throwing)
            {
                var destructable = Instantiate<Transform>(destructablePrefab, Vector3.zero, Quaternion.identity);
                destructable.transform.position = transform.position;
                destructable.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 180), Vector3.forward);
                var length = 0f;
                var particleSystem = destructable.GetComponent<ParticleSystem>();
                if(particleSystem != null && particleSystem is Object && !particleSystem.Equals(null)){
                    length = particleSystem.main.duration;
                }
                var destructableAnimator = destructable.GetComponent<Animator>();
                if(destructableAnimator != null && destructableAnimator is Object && !destructableAnimator.Equals(null)){
                    length = destructableAnimator.GetCurrentAnimatorStateInfo(0).length;
                }
                Destroy(gameObject);
                Destroy(destructable.gameObject, length);
            }
        }
        base.OnTriggerEnter2D(other);
    }
}
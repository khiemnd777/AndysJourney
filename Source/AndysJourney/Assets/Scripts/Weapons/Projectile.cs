using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public Transform projectileDestructPrefab;
	[System.NonSerialized]
	public Vector2 direction;
	[System.NonSerialized]
	public float projectileAngle;

	public virtual void OnTriggerEnter2D(Collider2D other)
    {
        var obstacleObj = other.GetComponent<InteractableObject>();
        if (obstacleObj != null && obstacleObj is Object && !obstacleObj.Equals(null))
        {
			var angle = Quaternion.AngleAxis(projectileAngle, Vector3.forward);
            var destruct = Instantiate<Transform>(projectileDestructPrefab, transform.position, angle);
			var anim = destruct.GetComponent<Animator>();
			var length = anim.GetCurrentAnimatorStateInfo(0).length;
			Destroy(gameObject);
			Destroy(destruct.gameObject, length);
        }
    }
}

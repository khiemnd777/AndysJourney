using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : InteractableObject
{
    public ParticleSystem destructablePrefab;

    public override void Hit(Transform hitFrom)
    {

    }

    void InteractByEventEvent(InteractableObject obstacleObj)
    {
        var destructable = Instantiate<ParticleSystem>(destructablePrefab, Vector3.zero, Quaternion.identity);
        destructable.transform.position = obstacleObj.transform.position;
        Destroy(destructable.gameObject, destructable.main.duration);
        Destroy(obstacleObj.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var obstacleObj = other.GetComponent<InteractableObject>();
        if (obstacleObj != null && obstacleObj is Object && !obstacleObj.Equals(null))
        {
            InteractByEventEvent(obstacleObj);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        var obstacleObj = other.gameObject.GetComponent<InteractableObject>();
        if (obstacleObj != null && obstacleObj is Object && !obstacleObj.Equals(null))
        {
            InteractByEventEvent(obstacleObj);
        }
    }
}

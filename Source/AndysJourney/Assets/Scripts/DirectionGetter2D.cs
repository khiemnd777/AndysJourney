using UnityEngine;

public class DirectionGetter2D : MonoBehaviour
{
    public Vector2 direction;
    
    Vector2 _lastPosition;

    void Update()
    {
        var heading = new Vector2(transform.position.x, transform.position.y) - _lastPosition;
        var distance = heading.magnitude;
        if(distance == 0f)
            return;
        direction = heading / distance;
        _lastPosition = transform.position;
    }
}
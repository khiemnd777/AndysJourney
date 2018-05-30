using UnityEngine;

public enum Direction
{
    Up, Right, Down, Left,
    UpRight, DownRight, DownLeft, UpLeft
}

public class DirectionGetter2D : MonoBehaviour
{
    public Vector2 direction;
    public Direction directionEnum;

    Vector2 _lastPosition;

    void Update()
    {
        var heading = new Vector2(transform.position.x, transform.position.y) - _lastPosition;
        var distance = heading.magnitude;
        if (distance == 0f)
            return;
        direction = heading / distance;
        _lastPosition = transform.position;
        // Set value into Direction enum
        if(direction.x > 0 && direction.y == 0)
            directionEnum = Direction.Right;
        else if(direction.x < 0 && direction.y == 0)
            directionEnum = Direction.Left;
        else if(direction.x == 0 && direction.y > 0)
            directionEnum = Direction.Up;
        else if(direction.x == 0 && direction.y < 0)
            directionEnum = Direction.Down;
        else if(direction.x < 0 && direction.y < 0)
            directionEnum = Direction.DownLeft;
        else if(direction.x < 0 && direction.y > 0)
            directionEnum = Direction.UpLeft;
        else if(direction.x > 0 && direction.y > 0)
            directionEnum = Direction.UpRight;
        else    
            directionEnum = Direction.DownRight;
    }
}
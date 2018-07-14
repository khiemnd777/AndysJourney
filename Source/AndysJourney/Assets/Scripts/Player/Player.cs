using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IControlLocker
{
	[System.NonSerialized]
    public float faceX;

    bool _lockFlipX;

    void Start()
    {
        faceX = 1;
        ControlLock.Register("FlipX", this);
    }

    void Update()
    {
        faceX = Input.GetAxisRaw("Horizontal") == 0 ? faceX : Input.GetAxisRaw("Horizontal");
        FlipX();
    }

    void FlipX()
    {
        if (_lockFlipX)
            return;
        var localScale = transform.localScale;
        var scaleVal = new Vector3(Mathf.Abs(localScale.x) * faceX, localScale.y, localScale.z);
        transform.localScale = scaleVal;
    }

    public void Lock(string name)
    {
        if (name == "FlipX")
        {
            _lockFlipX = true;
        }
    }

    public void ReleaseLock(string name)
    {
        if (name == "FlipX")
        {
            _lockFlipX = false;
        }
    }
}

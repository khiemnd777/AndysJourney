using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public Sprite[] spriteDown;
    public Sprite[] spriteUp;
    public Sprite[] spriteLeft;
    public Sprite[] spriteRight;

    Animator _animator;
    DirectionGetter2D _directionGetter;
	PlayerMovement _movement;

	const string _slingshotHoldTriggerStr = "slingshot hold trigger";
	const string _slingshotTriggerReleaseStr = "slingshot trigger release";

    void Start()
    {
        _animator = GetComponent<Animator>();
        _directionGetter = GetComponent<DirectionGetter2D>();
		_movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        Prepare();
    }

    void Prepare()
    {
        if (Input.GetKey(KeyCode.J))
        {
			_movement.stopX = _movement.stopY = true;
			_animator.SetBool(_slingshotHoldTriggerStr, true);
			_animator.SetBool(_slingshotTriggerReleaseStr, false);
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
			_movement.stopX = _movement.stopY = false;
			_animator.SetBool(_slingshotHoldTriggerStr, false);
			_animator.SetBool(_slingshotTriggerReleaseStr, true);
        }
    }
}

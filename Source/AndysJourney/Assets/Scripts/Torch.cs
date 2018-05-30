using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer torchFire;
    public WeaponFireflyCommand baitCommand;

    public bool hasFire
    {
        get
        {
            return torchFire.gameObject.activeSelf;
        }
    }

	public int fireflyOnBaitCount
	{
		get
		{
			var fireflies = baitCommand.GetComponentsInChildren<WeaponFireflyObject>();
			return fireflies.Length;
		}
	}

    void Start()
    {
        baitCommand.radius = .75f;
    }

    void Update()
    {
        baitCommand.useCommand = !torchFire.gameObject.activeSelf;
    }
}

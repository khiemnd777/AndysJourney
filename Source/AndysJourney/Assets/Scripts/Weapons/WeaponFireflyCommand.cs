using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFireflyCommand : MonoBehaviour 
{
	[System.NonSerialized]
	public int baitCount = 5;
	[System.NonSerialized]
	public float baitRadius = .035f;
	[System.NonSerialized]
	public float radius = .625f;
	[System.NonSerialized]
	public bool useCommand;
}

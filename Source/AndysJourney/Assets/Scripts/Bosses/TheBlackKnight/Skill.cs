using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {

	public abstract IEnumerator Play();
	public abstract IEnumerator Next();
}

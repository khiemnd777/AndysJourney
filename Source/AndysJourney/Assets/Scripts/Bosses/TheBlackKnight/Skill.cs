using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {

	public abstract IEnumerator Play();
	public abstract IEnumerator Next();

	public virtual IEnumerator Next(params Skill[] skills)
    {
        var rand = Random.Range(0, skills.Length);
        yield return StartCoroutine(skills[rand].Play());
        StartCoroutine(skills[rand].Next());
    }
}

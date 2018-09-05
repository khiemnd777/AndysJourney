using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightSkillControl : MonoBehaviour
{

    TheBlackKnightSlash _slash;
    TheSlashingKi _slashingKi;
    Skill[] _skills;

    // Use this for initialization
    void Start()
    {
        _slash = GetComponent<TheBlackKnightSlash>();
        _slashingKi = GetComponent<TheSlashingKi>();
        _skills = new Skill[] { _slash, _slashingKi };
		StartCoroutine(Play());
    }

	IEnumerator Play(){
		while(true){
			var rand = Random.Range(0, _skills.Length);
			yield return StartCoroutine(_skills[rand].Play());
		}
	}

    // Update is called once per frame
    void Update()
    {

    }
}

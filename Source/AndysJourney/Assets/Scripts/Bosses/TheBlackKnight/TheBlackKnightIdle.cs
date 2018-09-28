using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBlackKnightIdle : Skill
{
	[SerializeField]
	AnimationClip _idle;

    public override IEnumerator Next()
    {
        yield break;
    }

    public override IEnumerator Play()
    {
        yield break;
    }
}

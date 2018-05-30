using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretCaveDoorOpen : MonoBehaviour
{
    public Torch torch1;
    public Torch torch2;
    public Torch torch3;
    public Torch torch4;

    public SpriteRenderer[] destructs;

	bool eligible;

    void Update()
    {
		OpenCaveDoor();
    }

    void OpenCaveDoor()
    {
		if(eligible)
			return;
        if (!torch1.hasFire && !torch2.hasFire && !torch3.hasFire && !torch4.hasFire)
        {
            if (torch1.fireflyOnBaitCount == 0 && torch2.fireflyOnBaitCount >= 20 && torch3.fireflyOnBaitCount >= 20 && torch4.fireflyOnBaitCount == 0)
            {
				eligible = true;
				StartCoroutine(DestructingCaveDoor());
            }
        }
    }

	IEnumerator DestructingCaveDoor()
	{
		foreach(var destruct in destructs)
		{
			var percent = 0f;
			while(percent <= 1f)
			{
				percent += Time.deltaTime * 5f;
				var dTransform = destruct.transform;
				dTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, percent);
				yield return null;
			}
			destruct.gameObject.SetActive(false);
		}
	}
}

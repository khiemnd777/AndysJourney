using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TorchLightController : MonoBehaviour
{
    bool on = true;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            var torchs = GameObject.FindGameObjectsWithTag("Torch");
            foreach (var torch in torchs)
            {
                var torchFire = torch.transform.Find("Torch Fire");
                torchFire.gameObject.SetActive(!on);
            }
            on = !on;
        }
    }
}

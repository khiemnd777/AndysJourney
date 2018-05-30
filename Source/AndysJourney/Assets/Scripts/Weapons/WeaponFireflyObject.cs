using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponFireflyObject : MonoBehaviour
{
    public Transform target;
    public float velocity;
    public bool command = false;

    bool closestTarget;

    void Update()
    {
        Fly();
    }

    void Fly()
    {
        if (!command)
        {
            transform.RotateAround(target.position, transform.up, velocity * Time.deltaTime);
        }
        else
        {
            if (target != null && target is Object && !target.Equals(null))
            {
                if (!closestTarget)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * .625f);
                    var dist = Vector3.Distance(transform.position, target.position);
                    if (dist <= .02f)
                    {
                        closestTarget = true;
                        transform.SetParent(target);
                        transform.localPosition = Vector3.zero;
                        var command = target.GetComponent<WeaponFireflyCommand>();
                        var pos = transform.position;
                        pos.x -= command.baitRadius;
                        transform.position = pos;
                        WeaponFirefly.instance.InstantiateLightAndAssign(target);
                    }
                }
                else
                {
                    transform.RotateAround(target.position, transform.up, velocity * Time.deltaTime);
                }
            }
        }
    }

    public WeaponFireflyCommand FindClosestBait()
    {
        var go = GameObject.FindGameObjectsWithTag("FireflyBait");
        var commands = go
            .Select(x => x.GetComponent<WeaponFireflyCommand>())
            .Where(x => x.useCommand)
            .ToArray();
        if (!commands.Any())
            return null;
        WeaponFireflyCommand closest = null;
        var min = Mathf.Infinity;
        var currentPos = target.position;
        foreach (var command in commands)
        {
            var dist = Vector3.Distance(command.transform.position, currentPos);
            if (dist > command.radius / 2f)
                continue;
            if (dist < min)
            {
                closest = command;
                min = dist;
            }
        }
        return closest;
    }
}
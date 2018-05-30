using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponFirefly : MonoBehaviour
{
    static WeaponFirefly _instance;
    public static WeaponFirefly instance
    {
        get
        {
            if (_instance)
                return _instance;
            _instance = FindObjectOfType<WeaponFirefly>();
            if (!_instance)
                Debug.LogError("Setup WeaponFirefly");
            return _instance;
        }
    }

    public int amount = 10;
    public Transform owner;
    public WeaponFireflyObject prefab;
    public Light lightPrefab;
    public float twinkleVelocity = .25f;
    [System.NonSerialized]
    public List<WeaponFireflyObject> twinkles;

    Light _light;

    void Start()
    {
        twinkles = new List<WeaponFireflyObject>();
    }

    void Update()
    {
        UpdateTwinklePosition();
        Command();
        Call();
        HandleInsensityLight();
    }

    void UpdateTwinklePosition()
    {
        foreach (var twinkle in twinkles)
        {
            twinkle.velocity = twinkleVelocity;
        }
    }

    void Call()
    {
        if (!Input.GetKeyUp(KeyCode.L))
            return;
        Create();
    }

    public void Create()
    {
        var length = amount - twinkles.Count;
        for (var i = 0; i < length; i++)
        {
            InstantiateFirefly();
        }
        // Instantiate light and adjusting intensity of light, then assign it into target
        if (_light != null && _light is Object && !_light.Equals(null))
            DestroyImmediate(_light.gameObject);
        _light = Instantiate<Light>(lightPrefab, Vector3.zero, Quaternion.identity, owner);
        _light.transform.localPosition = new Vector3(0f, 0f, -.5f);
        _light.intensity = twinkles.Count;
        _light.range = 4f;
        _light.gameObject.SetActive(true);
    }

    void InstantiateLightAndAssignTarget()
    {
        if (_light != null && _light is Object && !_light.Equals(null))
            DestroyImmediate(_light.gameObject);
        _light = Instantiate<Light>(lightPrefab, Vector3.zero, Quaternion.identity, owner);
        _light.transform.localPosition = new Vector3(0f, 0f, -.5f);
        _light.intensity = twinkles.Count;
        _light.range = 4f;
        _light.gameObject.SetActive(true);
    }

    void HandleInsensityLight()
    {
        if (_light == null || _light is Object && _light.Equals(null))
            return;
        if (twinkles.Count == 0)
        {
            DestroyImmediate(_light.gameObject);
            return;
        }
        _light.intensity = twinkles.Count;
    }

    public void InstantiateLightAndAssign(Transform target)
    {
        var firefliesAccordingTarget = target.GetComponentsInChildren<WeaponFireflyObject>();
        Debug.Log(firefliesAccordingTarget.Length);
        if (firefliesAccordingTarget.Length <= 1)
        {
            var light = Instantiate<Light>(lightPrefab, Vector3.zero, Quaternion.identity, target);
            light.transform.localPosition = new Vector3(0f, 0f, -.5f);
            light.intensity = firefliesAccordingTarget.Length;
            light.range = 4f;
            light.gameObject.SetActive(true);
        }
        else
        {
            var light = target.GetComponentInChildren<Light>();
            light.intensity = firefliesAccordingTarget.Length;
        }
    }

    void InstantiateFirefly()
    {
        // Instantiate firefly object
        var position = (Random.insideUnitSphere * (owner.localScale.magnitude / 10f) + owner.position);
        var twinkle = Instantiate<WeaponFireflyObject>(prefab, position, Random.rotation, owner);
        twinkle.target = owner;
        twinkle.velocity = twinkleVelocity;
        twinkles.Add(twinkle);
    }

    public void Command()
    {
        if (!Input.GetKey(KeyCode.I))
            return;
        var fireflyWithoutCommand = twinkles.Where(x => !x.command).FirstOrDefault();
        if (fireflyWithoutCommand == null)
            return;
        var closest = fireflyWithoutCommand.FindClosestBait();
        if (closest == null || closest is Object && closest.Equals(null))
            return;
        fireflyWithoutCommand.command = true;
        fireflyWithoutCommand.transform.SetParent(null);
        fireflyWithoutCommand.target = closest.transform;
        // remove firefly if it is undercommand
        twinkles.RemoveAll(x => x.transform.GetInstanceID() == fireflyWithoutCommand.transform.GetInstanceID());
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utility
{
    public static float TimeByFrame(float frameNumber, float frameRate)
    {
        return (1f / frameRate) * frameNumber;
    }

    public static float FrameByTime(float time, float frameRate)
    {
        return time * frameRate;
    }


    public static void RotateAround(Rigidbody rigid, Transform own, Transform target, Vector3 axis, float targetMagnitude, float velocity)
    {
        own.RotateAround(target.position, axis, velocity * Time.deltaTime);
    }

    public static Vector3 CalculateVelocity(Quaternion rotation, float ambientSpeed)
    {
        var forward = Vector3.forward;
        forward = rotation * forward;
        var velocity = forward * ambientSpeed;
        return velocity;
    }

    public static void ActiveLayer(Animator animator, string layerName)
    {
        for (var i = 0; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
        animator.SetLayerWeight(animator.GetLayerIndex(layerName), 1);
    }

    public static float GetAnimationLength(Animator animator, string animationName)
    {
        var controller = animator.runtimeAnimatorController;
        var animations = controller.animationClips;
        for (var i = 0; i < animations.Length; i++)
        {
            var anim = animations[i];
            if (!animationName.Equals(anim.name))
                continue;
            return anim.length;
        }
        return 0;
    }

    public static Quaternion RotateToTarget(Transform own, Transform target, float maxDegreesDelta)
    {
        var offset = target.position - own.position;
		var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
		var angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
		return Quaternion.RotateTowards(own.rotation, angleAxis, maxDegreesDelta);
    }

    public static IEnumerator JumpToDestination(Transform owner, Vector3 source, Vector3 destination, System.Action toBeAtDestination = null)
    {
        var percent = .0f;
        while (percent <= 1f)
        {
            percent += Time.deltaTime * 2;
            var currentPos = Vector2.Lerp(source, destination, percent);
            var trajectoryHeight = Random.Range(.09f, .175f);
            currentPos.y += trajectoryHeight * Mathf.Sin(Mathf.Clamp01(percent) * Mathf.PI);
            owner.position = currentPos;
            yield return null;
        }
        if (toBeAtDestination != null)
            toBeAtDestination();
    }

    public static void JumpToDestination(Transform owner, Vector3 source, Vector3 destination, float trajectoryHeight, float percent, System.Action toBeAtDestination = null)
    {
        var currentPos = Vector2.Lerp(source, destination, percent);
        currentPos.y += trajectoryHeight * Mathf.Sin(Mathf.Clamp01(percent) * Mathf.PI);
        owner.position = currentPos;
        if (toBeAtDestination != null)
            toBeAtDestination();
    }

    public static Vector2 CalculateJumpVelocity2(Vector2 own, Vector2 target, float height, float gravity)
    {
        var offsetY = target.y - own.y;
        var offsetX = target.x - own.x;
        var time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (offsetY - height) / gravity);
        var velY = Vector2.up * Mathf.Sqrt(-2 * gravity * height);
        var velX = new Vector2(offsetX, 0) / time;
        var vel = velX + velY * -Mathf.Sign(gravity);
        return vel;
    }

    public static Vector3 CameraInBound(Camera camera, BoxCollider2D bound, Vector3 position)
    {
        var min = bound.bounds.min;
        var max = bound.bounds.max;
        var halfHeight = camera.orthographicSize;
        var halfWidth = halfHeight * Screen.width / Screen.height;
        var x = Mathf.Clamp(position.x, min.x + halfWidth, max.x - halfWidth);
        var y = Mathf.Clamp(position.y, min.y + halfHeight, max.y - halfHeight);
        return new Vector3(x, y, position.z);
    }

    public static Vector2 CameraBoundEdge(Camera camera, BoxCollider2D bound, Vector3 position)
    {
        var min = bound.bounds.min;
        var max = bound.bounds.max;
        var halfHeight = camera.orthographicSize;
        var halfWidth = halfHeight * Screen.width / Screen.height;
        var realMinX = min.x + halfWidth;
        var realMaxX = max.x - halfWidth;
        var realMinY = min.y + halfHeight;
        var realMaxY = max.y - halfHeight;
        var x = Mathf.Clamp(position.x, realMinX, realMaxX);
        var y = Mathf.Clamp(position.y, realMinY, realMaxY);
        var rx = Mathf.Approximately(x, realMinX) ? 0 : Mathf.Approximately(x, realMaxX) ? 1 : -1;
        var ry = Mathf.Approximately(y, realMinY) ? 0 : Mathf.Approximately(y, realMaxY) ? 1 : -1;
        return new Vector2(rx, ry);
    }

    public static bool LayerInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    public static bool IsNull(object checkedObject)
    {
        return checkedObject == null || checkedObject is Object && checkedObject.Equals(null);
    }

    public static SpriteRenderer CreateSpriteRendererBySample(Sprite sample, Vector3 position, Vector3 scale, float opacity)
    {
        var spriteObj = new GameObject("Surfing Shadow", typeof(SpriteRenderer));
        spriteObj.transform.position = position;
        spriteObj.transform.localScale = scale;
        var spriteObjRenderer = spriteObj.GetComponent<SpriteRenderer>();
        spriteObjRenderer.sprite = sample;
        spriteObjRenderer.color = new Color(255f, 255f, 255f, opacity);
        return spriteObjRenderer;
    }

    public static IEnumerator Shaking(float duration, float amount, Transform target, System.Action before, System.Action after)
    {
        if (before != null)
        {
            before();
        }
        var originalPos = target.transform.localPosition;
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            target.localPosition = originalPos + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime;
            yield return null;
        }
        target.localPosition = originalPos;
        if (after != null)
        {
            after();
        }
    }

    public static IEnumerator VectorLerp(Transform owner, Vector3 source, Vector3 destination, float seconds, System.Func<YieldInstruction> returnBy, System.Action toBeAtDestination = null)
    {
        var percent = .0f;
        while (percent <= 1f)
        {
            percent += Time.deltaTime / seconds;
            var currentPos = Vector3.Lerp(source, destination, percent);
            owner.position = currentPos;
            yield return returnBy();
        }
        if (toBeAtDestination != null)
            toBeAtDestination();
    }
}
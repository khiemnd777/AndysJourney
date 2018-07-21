using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitDetector : MonoBehaviour
{
	public LayerMask theDetectedLayers;
	public System.Action<HitDetector, Collider2D> onDetectedHit;
	
	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		if(!Utility.LayerInLayerMask(other.gameObject.layer, theDetectedLayers))
			return;
		if(onDetectedHit != null){
			onDetectedHit(this, other);
		}
	}
}

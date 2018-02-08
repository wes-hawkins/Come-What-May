using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCSEmitterComponent : MonoBehaviour {

	public bool up, down, left, right, forward, back = false;

	private void OnDrawGizmosSelected () {
		if(up)
			Gizmos.DrawRay(transform.position, Vector3.up);
		if(down)
			Gizmos.DrawRay(transform.position, -Vector3.up);
		if(left)
			Gizmos.DrawRay(transform.position, -Vector3.right);
		if(right)
			Gizmos.DrawRay(transform.position, Vector3.right);
		if(forward)
			Gizmos.DrawRay(transform.position, Vector3.forward);
		if(back)
			Gizmos.DrawRay(transform.position, -Vector3.forward);
	} // End of OnDrawGizmosSelected().

} // End of RCSEmitter.

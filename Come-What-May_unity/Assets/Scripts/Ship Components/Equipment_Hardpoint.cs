using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Hardpoint : MonoBehaviour {
	
	private Weapon myWeapon = null; public Weapon MyWeapon { get { return myWeapon; } }
	protected Entity target = null; public Entity Target { get { return target; } }
	private Vector2 pitchYaw = Vector2.zero;
	[SerializeField] private float rotateSpeed = 5f;

	private void Awake () {
		myWeapon = GetComponentInChildren<Weapon>();
	} // End of Awake().
	
	public void SetTrigger(bool trigger) {
		if(myWeapon)
			myWeapon.Local_SetTrigger(trigger);
	} // End of SetTrigger().

	public void SetTarget(Entity newTarget) {
		target = newTarget;
		if(myWeapon)
			myWeapon.SetTarget(newTarget);
	} // End of SetTarget().

	private void Update() {
		Vector2 targetPitchYaw = Vector2.zero;
		if(target) {
			Vector3 targetLocalPosition = transform.InverseTransformPoint(target.transform.position);
			targetPitchYaw.x = Mathf.Atan2(-targetLocalPosition.y, targetLocalPosition.z) * Mathf.Rad2Deg;
			targetPitchYaw.y = Mathf.Atan2(targetLocalPosition.x, targetLocalPosition.z) * Mathf.Rad2Deg;
		}

		pitchYaw = Vector2.MoveTowards(pitchYaw, targetPitchYaw, rotateSpeed * Time.deltaTime);
		myWeapon.transform.localEulerAngles = pitchYaw;
	} // End of Update().

} // End of Equipment_Hardpoint.

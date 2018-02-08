using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script should be attached to any ship the local player is controlling. It's the 'keys' to the ship.
// Stuff like the camera controller will look for this script to focus the camera.
public class LocalPlayerController : MonoBehaviour {

	public static LocalPlayerController Inst = null;
	private Ship myShip = null; public Ship MyShip { get { return myShip; } }
	public static Ship LocalShip { get { return Inst? Inst.myShip : null; } }

	Vector2 smoothedMouse = Vector2.zero;
	Vector2 smoothedMouseVel = Vector2.zero;

	private Entity selectedTarget = null;
	private float distanceToTarget = 0f; public float DistanceToTarget { get { return distanceToTarget; } }
	private float distanceToTargetVel = 0f;

	private Vector3 linearThrottle = Vector3.zero; public Vector3 LinearThrottle { get { return linearThrottle; } }
	private Vector3 rotationalThrottle = Vector3.zero; public Vector3 RotationalThrottle { get { return rotationalThrottle; } }


	private void Awake() {
		Inst = this;
		myShip = GetComponent<Ship>();
	} // End of Awake().

	private void Update () {
		linearThrottle = Vector3.zero;
		if(Input.GetKey(KeyCode.W))
			linearThrottle.z += 1f;
		if(Input.GetKey(KeyCode.S))
			linearThrottle.z -= 1f;
		if(Input.GetKey(KeyCode.A))
			linearThrottle.x -= 1f;
		if(Input.GetKey(KeyCode.D))
			linearThrottle.x += 1f;
		if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.R))
			linearThrottle.y += 1f;
		if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.F))
			linearThrottle.y -= 1f;

		rotationalThrottle = Vector3.zero;
		if(Input.GetKey(KeyCode.Q))
			rotationalThrottle.z += 1f;
		if(Input.GetKey(KeyCode.E))
			rotationalThrottle.z -= 1f;

		Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 0.01f * PreferencesManager.Inst.MouseSensitivity / Time.deltaTime;
		smoothedMouse = Vector2.SmoothDamp(smoothedMouse, mouseInput, ref smoothedMouseVel, 0.1f, Mathf.Infinity, Time.deltaTime);
		smoothedMouse = Vector2.ClampMagnitude(smoothedMouse, 1f);
		float rotationThrottleMag = Mathf.Clamp01(smoothedMouse.magnitude);

		float mouseRadians = Mathf.Atan2(smoothedMouse.y, smoothedMouse.x);
		rotationalThrottle.x = -Mathf.Sin(mouseRadians) * rotationThrottleMag;
		rotationalThrottle.y = Mathf.Cos(mouseRadians) * rotationThrottleMag;

		//ChatManager.Inst.ConsoleMessage(rotationalThrottle.ToString());
			
		myShip.SetThrottles(linearThrottle, rotationalThrottle);
		myShip.SetTrigger(0, Input.GetMouseButton(0));
		myShip.SetTrigger(1, Input.GetMouseButton(1));

		// Gunnery
		Entity gunsTarget = null;
		Ray shipRay = new Ray(transform.position, transform.forward);
		float lowestDeflection = 20f;
		for(int i = 0; i < Entity.allEntities.Count; ++i) {
			Vector3 vectorToEntity = Entity.allEntities[i].transform.position - transform.position;

			float deflectionToEntity = Vector3.Angle(shipRay.direction, vectorToEntity);
			if(deflectionToEntity < lowestDeflection) {
				lowestDeflection = deflectionToEntity;
				gunsTarget = Entity.allEntities[i];
			}
		}
		
		// Set up targetting stuff
		float targetDistanceToTarget;
		if(gunsTarget)
			targetDistanceToTarget = Vector3.Distance(transform.position, gunsTarget.transform.position);
		else
			targetDistanceToTarget = 500f;
		distanceToTarget = Mathf.SmoothDamp(distanceToTarget, targetDistanceToTarget, ref distanceToTargetVel, 0.1f);

		myShip.SetGunsTarget(gunsTarget);

		if(Input.GetKeyDown(KeyCode.Alpha1))
			MyShip.LaunchDeployable(0);
		if(Input.GetKeyDown(KeyCode.Alpha2))
			MyShip.LaunchDeployable(1);
		if(Input.GetKeyDown(KeyCode.Alpha3))
			MyShip.LaunchDeployable(2);
		if(Input.GetKeyDown(KeyCode.Alpha4))
			MyShip.LaunchDeployable(3);

		if(Input.GetMouseButtonDown(1)) {
			for(int i = 0; i < MyShip.Deployables.Length; i++) {
				if(MyShip.Deployables[i] != null) {
					MyShip.LaunchDeployable(i);
					break;
				}
			}
		}

		if(Input.GetKeyDown(KeyCode.End))
			GetComponent<TakeDamage>().Local_Explode();

	} // End of Update().

	private void OnDestroy() {
		Inst = null;
	} // End of OnDestroy().

} // End of LocalPlayerController.

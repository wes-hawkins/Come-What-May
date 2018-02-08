using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public static CameraController Inst;
	
	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	Vector3 targetPosition = Vector3.zero;
	Vector3 positionVel = Vector3.zero;


	void Awake () {
		Inst = this;
	} // End of Awake(); 
	

	void FixedUpdate () {
		if(LocalPlayerController.LocalShip)
			targetPosition = LocalPlayerController.LocalShip.transform.position + (LocalPlayerController.LocalShip.transform.forward * LocalPlayerController.LocalShip.CameraOffset.x) + (LocalPlayerController.LocalShip.transform.up * LocalPlayerController.LocalShip.CameraOffset.y);

		position = Vector3.SmoothDamp(position, targetPosition, ref positionVel, 0.05f);
		if(LocalPlayerController.LocalShip)
			rotation = LocalPlayerController.LocalShip.transform.rotation;

		transform.position = position;
		transform.rotation = rotation;
	} // End of Update().

} // End of CameraController.
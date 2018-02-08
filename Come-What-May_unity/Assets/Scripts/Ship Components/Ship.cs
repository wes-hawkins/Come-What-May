using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// A Ship is any object that can be driven around under its own power.
public class Ship : Entity {

	private Equipment_Engine[] engines = null;
	private Equipment_Gyro[] gyros = null;
	private Equipment_RCS[] rcsSystems = null;
	private Equipment_Hardpoint[] hardpoints = null;
	private IDeployable[] deployables = null; public IDeployable[] Deployables { get { return deployables; } }

	private Vector3 linearThrottle = Vector3.zero;
	private Vector3 rotationalThrottle = Vector3.zero;
	private bool[] triggers;

	[HideInInspector] public bool inhibitStartAuthority = false;

	[SerializeField] private Vector2 cameraOffset = Vector2.zero; public Vector2 CameraOffset { get { return cameraOffset; } }
	
	private Entity gunsTarget = null; public Entity GunsTarget { get { return gunsTarget; } }


	public override void OnStartAuthority() {
		base.OnStartAuthority();
		if(inhibitStartAuthority)
			return;

		//PlayerController.Local.Local_AssignShip(this);
		gameObject.AddComponent<LocalPlayerController>();
		NetworkPilot.Local.Local_AssignShip(this);
		NetworkPilot.Local.Cmd_AssumeAuthority(gameObject);

	} // End of OnStartAuthority().

	protected override void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
		
		ResetModules();

		triggers = new bool[2];
	} // End of Awake().


	public void SetThrottles(Vector3 linearThrottle, Vector3 rotationalThrottle) {
		this.linearThrottle = linearThrottle;
		this.rotationalThrottle = rotationalThrottle;
	} // End of SetThrottles().

	public void SetTrigger(int index, bool value) {
		triggers[index] = value;
	} // End of SetTrigger().

	public void SetGunsTarget(Entity newTarget) {
		gunsTarget = newTarget;
	} // End of SetGunsTarget().

	protected override void LateUpdate() {
		base.LateUpdate();

		for(int i = 0; i < engines.Length; i++)
			engines[i].SetThrottle(linearThrottle.z);
		for(int i = 0; i < gyros.Length; i++)
			gyros[i].SetThrottle(rotationalThrottle);
		for(int i = 0; i < rcsSystems.Length; i++)
			rcsSystems[i].SetThrottle(linearThrottle);
		for(int i = 0; i < hardpoints.Length; i++) {
			hardpoints[i].SetTrigger(triggers[0]);
			hardpoints[i].SetTarget(gunsTarget);
		}

		uiElement.SetPosition(transform.position);
	} // End of Update().

	public void LaunchDeployable(int index = 0) {
		if((deployables.Length > index) && (deployables[index] != null)) {
			deployables[index].Local_Launch(gunsTarget);
			deployables[index] = null;
		}
	} // End of FireDeployable().

	public void ResetModules() {
		engines = GetComponentsInChildren<Equipment_Engine>();
		gyros = GetComponentsInChildren<Equipment_Gyro>();
		rcsSystems = GetComponentsInChildren<Equipment_RCS>();
		hardpoints = GetComponentsInChildren<Equipment_Hardpoint>();
		deployables = GetComponentsInChildren<IDeployable>();
	} // End of ResetModules().

} // End of Ship.

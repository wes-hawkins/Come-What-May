using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Gyro : Equipment {

	// The 'target torque' for the gyro. Each vec3 component -1f to 1f.
	private Vector3 throttle = Vector3.zero;
	private Vector3 output = Vector3.zero;
	[Tooltip("Gyro torque at 100% power.")]
	[SerializeField] private float maximumTorque = 20f;
	[SerializeField] private float maximumRotSpeed = 90f;
	[SerializeField] private SpatialAudioClip gyroClip = null;
	private SpatialAudioSource gyroSource = null;

	Vector3 lastThrottle = Vector3.zero;
	float motorSoundSpeed = 0f;
	float motorSoundSpeedVel = 0f;

	
	protected override void Awake() {
		base.Awake();
		
		if(gyroClip) {
			gyroSource = SpatialAudioManager.AttachClipToTransform(gyroClip, transform);
			gyroSource.loop = true;
		}
	} // End of Awake().

	public void SetThrottle(Vector3 _throttle) {
		throttle = Vector3.ClampMagnitude(new Vector3(Mathf.Clamp(_throttle.x, -1f, 1f), Mathf.Clamp(_throttle.y, -1f, 1f), Mathf.Clamp(_throttle.z, -1f, 1f)), 1f);
	} // End of SetThrottle().

	void FixedUpdate () {

		float maxChange = Time.deltaTime * 4f;
		Vector3 flux = throttle - output;
		float smoothedChange = maxChange * Mathf.Clamp01(flux.magnitude);

		output = Vector3.MoveTowards(output, throttle, smoothedChange);
		myRigidbody.angularVelocity = myRigidbody.rotation * output * maximumRotSpeed * Mathf.Deg2Rad;


		motorSoundSpeed = Mathf.SmoothDamp(motorSoundSpeed, Mathf.Clamp01(flux.magnitude), ref motorSoundSpeedVel, 0.1f);
		gyroSource.volume = motorSoundSpeed;
		gyroSource.pitch = motorSoundSpeed;
		gyroSource.PlayControl((gyroSource.volume > 0f) && (gyroSource.pitch > 0f));

	} // End of FixedUpdate().

} // End of Equipment_Gyro.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Engine : Equipment {
	
	// The 'target output' for the engine, 0f to 1f.
	private float throttle = 0f;
	// The actual current output of the engine, 0f to 1f.
	private float output = 0f;
	[SerializeField] private Color trailColor = Color.white;
	[Tooltip("Engine output at 100% power.")]
	[SerializeField] private float maximumForce = 20f;
	[Tooltip("How long the engine takes to go from 0% to 100% output.")]
	[SerializeField] private float responseTime = 0.5f;
	[Tooltip("Can the engine provide reverse thrust? (Can be 0!)")]
	[SerializeField] private float reverseFraction = 0.2f;
	// These will glow while the engine is producing thrust.
	private TrailRenderer[] trailRenderers = new TrailRenderer[0];

	[SerializeField] SpatialAudioClip engineClip = null;
	private SpatialAudioSource engineSource = null;
	[SerializeField] private float idleVolume = 0f;
	[SerializeField] private float idlePitch = 0f;


	protected override void Awake () {
		base.Awake();

		trailRenderers = GetComponentsInChildren<TrailRenderer>();
		if(engineClip) {
			engineSource = SpatialAudioManager.AttachClipToTransform(engineClip, transform);
			engineSource.loop = true;
		}
	} // End of Awake().
	
	public void SetThrottle(float _throttle) {
		if(_throttle > 0f)
			throttle = Mathf.Clamp01(_throttle);
		else if(reverseFraction > 0f)
			throttle = -Mathf.Clamp01(-_throttle * reverseFraction);
	} // End of SetThrottle().

	void Update () {
		output = Mathf.MoveTowards(output, throttle, Time.deltaTime / responseTime);

		// Trail renderers
		for(int i = 0; i < trailRenderers.Length; i++)
			trailRenderers[i].material.SetColor("_TintColor", Color.Lerp(Color.black, trailColor, output));

		if(engineSource) {
			engineSource.volume = Mathf.Lerp(idleVolume, 1f, Mathf.Abs(output));
			engineSource.pitch = Mathf.Lerp(idlePitch, 1f, Mathf.Abs(output));
			engineSource.PlayControl((engineSource.volume > 0f) && (engineSource.pitch > 0f));
		}
	} // End of Update().

	void FixedUpdate() {
		// Apply propulsion force
		if(myRigidbody && (throttle > 0f))
			myRigidbody.AddForce(transform.forward * output * maximumForce, ForceMode.Force);
	} // End of FixedUpdate().


} // End of Equipment_Engine.

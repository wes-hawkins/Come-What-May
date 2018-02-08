using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_RCS : Equipment {

	// The 'target power' for the rcs. Each vec3 component -1f to 1f.
	private Vector3 throttle = Vector3.zero;
	[Tooltip("RCS force at 100% power.")]
	[SerializeField] private float maximumForce = 5f;
	[Tooltip("This will copy and flip all emitters from right to left.")]
	[SerializeField] private bool mirrorEmitters = true;
	private RCSEmitter[] emitters;
	private ParticleSystem particleSys = null;

	[SerializeField] private SpatialAudioClip rcsClip = null;
	private SpatialAudioSource rcsSource = null;
	private float rcsSoundRamp = 0f;

	private class RCSEmitter {
		private bool up, down, left, right, forward, back;
		public bool Up { get { return Up; } }
		public bool Down { get { return Down; } }
		public bool Left { get { return left; } }
		public bool Right { get { return right; } }
		public bool Forward { get { return forward; } }
		public bool Back { get { return back; } }
		private Vector3 localPos; public Vector3 LocalPos { get { return localPos; } }

		public RCSEmitter(Vector3 _localPos, bool _up, bool _down, bool _left, bool _right, bool _forward, bool _back) {
			localPos = _localPos;
			up = _up;
			down = _down;
			left = _left;
			right = _right;
			forward = _forward;
			back = _back;
		} // End of constructor.

	} // End of RCSEmitter().

	protected override void Start() {
		base.Start();

		// Set up sound effects.
		if(rcsClip) {
			rcsSource = SpatialAudioManager.AttachClipToTransform(rcsClip, transform);
			rcsSource.loop = true;
		}

		// Set up emitters
		RCSEmitterComponent[] emitterComponents = gameObject.GetComponentsInChildren<RCSEmitterComponent>();
		emitters = new RCSEmitter[emitterComponents.Length * (mirrorEmitters? 2 : 1)];
		for(int i = 0; i < emitterComponents.Length; ++i) {

			// Main emitters
			emitters[i] = new RCSEmitter(
				emitterComponents[i].transform.localPosition, 
				emitterComponents[i].up,
				emitterComponents[i].down,
				emitterComponents[i].left,
				emitterComponents[i].right,
				emitterComponents[i].forward,
				emitterComponents[i].back
			);

			// Mirrored emitters
			if(mirrorEmitters) {
				emitters[emitterComponents.Length + i] = new RCSEmitter(
					Vector3.Scale(new Vector3(-1f, 1f, 1f), emitterComponents[i].transform.localPosition), 
					emitterComponents[i].up,
					emitterComponents[i].down,
					emitterComponents[i].left,
					emitterComponents[i].right,
					emitterComponents[i].forward,
					emitterComponents[i].back
				);
			}

			Destroy(emitterComponents[i].gameObject);
		}

		particleSys = GetComponentInChildren<ParticleSystem>();
	} // End of Start().
	
	public void SetThrottle(Vector3 _throttle) {
		throttle = Vector3.ClampMagnitude(_throttle, 1f);
	} // End of SetThrottle().

	public void Update() {
		if(throttle != Vector3.zero) {
			for(int i = 0; i < emitters.Length; i++) {
				if(Random.Range(0f, 1f) < 0.1f) {
					ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
					emitParams.position = emitters[i].LocalPos + -throttle;
					emitParams.velocity = -throttle * Random.Range(2f, 5f);
					particleSys.Emit(emitParams, 1);
				}
			}
		}

		rcsSoundRamp = Mathf.MoveTowards(rcsSoundRamp, throttle.magnitude, Time.deltaTime * 5f); 
		rcsSource.volume = rcsSoundRamp;
		rcsSource.pitch = Mathf.Lerp(0.5f, 1f, rcsSoundRamp);
		rcsSource.PlayControl(rcsSource.volume > 0f);
	} // End of Update().

	void FixedUpdate () {
		// Apply torque
		if(myRigidbody && (throttle.sqrMagnitude > 0f))
			myRigidbody.AddForce(myRigidbody.rotation * throttle * maximumForce, ForceMode.Force);
	} // End of FixedUpdate().

} // End of Equipment_RCS.

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Missile : NetworkBehaviour, IDeployable {

	public static List<Missile> allActiveMissiles = new List<Missile>();

	public SpatialAudioClip deployClip = null;
	public SpatialAudioClip ignitionClip = null;
	public SpatialAudioClip thrusterClip = null;
	public SpatialAudioClip explodeClip = null;
	public Transform explosionPrefab = null;
	public Transform ignitionPrefab = null;
	private SpatialAudioSource thrusterSource = null;

	[SerializeField] private float fuel = 10f; // Seconds at maximum burn.
	[SerializeField] private float deployVel = 2f;

	[SerializeField] private float damage = 20f;
	[SerializeField] private float explosionRadius = 20f;
	[SerializeField] private float explosionForce = 10f;

	// Guidance
	private bool deployed = false; // Launched from host
	private bool ignition = false; // Engines have been activated.
	private bool derelict = false; // Out of fuel
	[SerializeField] private float maxTurnRate = 180.0f; // turn rate in degrees per second
	[SerializeField] private float maxThrust = 10.0f; // maximum acceleration
	[SerializeField] private float thrustRamp = 0.5f; // how quickly full thrust becomes available
	[SerializeField] private float turnRamp = 0.5f;	// how quickly full turning becomes available
	[SerializeField] private float gain = 3.0f;	// 'navigational constant' gain, usually between 3 and 5
	[SerializeField] private float ignitionDelay = 0.2f; // delay between launch and activating thruster

	private float startTime;
	private float thrust = 0.0f;
	private float turnRate = 0.0f;
	private Vector3 los;  // line of sight
	private Vector3 acceleration;

	private TrailRenderer trail = null;
	private Collider myCollider = null;
	private Rigidbody myRigidbody = null;
	private List<Collider> launcherColliders = null;
	private Entity target = null;


	void Start(){
		myCollider = GetComponent<Collider>();
		if(myCollider)
			myCollider.enabled = false;

		myRigidbody = GetComponent<Rigidbody>();
		myRigidbody.isKinematic = true;

		trail = GetComponentInChildren<TrailRenderer>();
		trail.enabled = false;

		launcherColliders = new List<Collider>(transform.root.GetComponentsInChildren<Collider>());
	} // End of Start().


	private void Launch(Entity _target){
		deployed = true;
		startTime = Time.time;
		target = _target;

		if(deployClip)
			SpatialAudioManager.PlayClipAtPoint(deployClip, transform.position);

		// don't collide with our own launcher
		if(myCollider) {
			myCollider.enabled = true;
			foreach (Collider someLauncherCollider in launcherColliders){
				if(someLauncherCollider && (someLauncherCollider != myCollider)){
					if(someLauncherCollider.enabled && !someLauncherCollider.isTrigger && myCollider.enabled){
						Physics.IgnoreCollision(myCollider, someLauncherCollider, true);
					}
				}
			}
		}

		transform.parent = null;
		myRigidbody.isKinematic = false;
		myRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		myRigidbody.velocity = transform.root.GetComponent<Rigidbody>().velocity;
		myRigidbody.velocity += -transform.up * deployVel;
	} // End of Launch().
	public void Local_Launch(Entity _target) {
		Launch(_target);
		//Cmd_Launch(NetworkPilot.LocalNetIdent, _target.gameObject);
	} // End of Local_Launcher().
	[Command] private void Cmd_Launch(NetworkIdentity sender, GameObject _target) {
		Rpc_Launch(sender, _target);
	} // End of Cmd_Launch().
	[ClientRpc] private void Rpc_Launch(NetworkIdentity sender, GameObject _target) {
		if(!sender.isLocalPlayer)
			Launch(_target.GetComponent<Entity>());
	} // End of Rpc_Launch().

	
	private void Update(){
		// End of lifetime
		if(ignition && !derelict) {
			fuel = Mathf.MoveTowards(fuel, 0f, (thrust / maxThrust) * Time.deltaTime);
			if(fuel == 0f) {
				derelict = true;

				if(trail){
					trail.autodestruct = true;
					trail.transform.parent = null;
				}

				target = null;
				allActiveMissiles.Remove(this);
				if(thrusterSource)
					thrusterSource.Destroy();
			}
		}
	} // End of Update().


	private void FixedUpdate(){
		
		// Ignition!
		if(deployed && !ignition && (Time.time - startTime > ignitionDelay)){
			ignition = true;
			if(ignitionClip)
				SpatialAudioManager.PlayClipAtPoint(ignitionClip, transform.position);
			if(thrusterClip)
				thrusterSource = SpatialAudioManager.PlayClipOnTransform(thrusterClip, transform);
			if(ignitionPrefab)
				Instantiate(ignitionPrefab, transform.position + (-transform.forward * transform.lossyScale.z * 0.5f), transform.rotation);
			if(trail)
				trail.enabled = true;

			allActiveMissiles.Add(this);
		} else if(ignition && !derelict) {

			if(thrusterSource)
				thrusterSource.volume = thrust / maxThrust;

			// build up to maximum thrust and turn rate
			if(thrust < maxThrust){
				// don't go over in case thrustRamp is very small
				float increase = Time.fixedDeltaTime * maxThrust / thrustRamp;
				thrust = Mathf.Min(thrust + increase, maxThrust);
			}

			if(turnRate < maxTurnRate){
				float increase = Time.fixedDeltaTime * maxTurnRate/turnRamp;
				turnRate = Mathf.Min(turnRate + increase, maxTurnRate);
			}

			if(target){
				// Proportional Navigation evaluates the rate of change of the 
				// Line Of Sight (los) to our target. If the rate of change is zero,
				// the missile is on a collision course. If it is not, we apply an
				// acceleration to correct course.
				Vector3 prevLos = los;
				los = target.transform.position - transform.position;
				Vector3 dLos = los - prevLos;

				// we only want the component perpendicular to the line of sight
				dLos = dLos - Vector3.Project(dLos, los);
			
				// plain PN would be:
				// acceleration = Time.fixedDeltaTime*los + dLos * nc;

				// augmented PN in addition takes acceleration into account
				acceleration = Time.fixedDeltaTime*los + dLos * gain + Time.fixedDeltaTime * acceleration * gain / 2f;
			
				// limit acceleration to our maximum thrust
				acceleration = Vector3.ClampMagnitude(acceleration * thrust, thrust);
			
				// draw some debug lines to visualize the output of the algorithm {
				// velocity, LOS rotation rate and resulting acceleration
				//Debug.DrawLine(transform.position, transform.position + rigidbody.velocity, Color.green);
				//Debug.DrawLine(transform.position, transform.position + dLos/Time.fixedDeltaTime, Color.red);
				//Debug.DrawLine(transform.position, transform.position + acceleration, Color.blue);
			
				// to immediately accelerate towards the target {
				// rigidbody.AddForce(acceleration, ForceMode.Acceleration);

				// instead, turn our rigidbody towards it and apply forward thrust
				// this makes the missiles less perfect and more realistic
				Quaternion targetRotation = Quaternion.LookRotation(acceleration, transform.up);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * turnRate);
			} else {
				acceleration = transform.forward * thrust;
			}

			myRigidbody.AddForce(transform.forward * acceleration.magnitude, ForceMode.Acceleration);
		}

		// Ray is cast from missile's estimated previous position towards it's current position.
		if(myRigidbody.velocity.sqrMagnitude > 0.1f) {
			Ray flightRay = new Ray(transform.position, myRigidbody.velocity);
			Debug.DrawRay(flightRay.origin, flightRay.direction * myRigidbody.velocity.magnitude * Time.deltaTime);
			RaycastHit impact = new RaycastHit();
			if(Physics.Raycast(flightRay, out impact, myRigidbody.velocity.magnitude * Time.fixedDeltaTime)){
				if((impact.collider != myCollider) && !launcherColliders.Contains(impact.collider))
					Detonate(impact.point - (myRigidbody.velocity.normalized * 1f));
			}
		}
	} // End of FixedUpdate().


	void OnCollisionEnter(){
		if(target)
			Detonate(transform.position - transform.forward);
	} // End of OnCollisionEnter().

	
	void Detonate(Vector3 expPos){
		Explosion.AtPoint(expPos, damage, explosionRadius, explosionForce);

		if(explosionPrefab)
			Instantiate(explosionPrefab, expPos, Quaternion.identity);
		
		if(explodeClip)
			SpatialAudioManager.PlayClipAtPoint(explodeClip, transform.position);

		if(trail){
			trail.autodestruct = true;
			trail.transform.parent = null;
		}

		if(thrusterSource)
			thrusterSource.Destroy();
		
		allActiveMissiles.Remove(this);
		Destroy(gameObject);
		
		Destroy(gameObject);
	} // End of Detonate().


#if UNITY_EDITOR
	protected void OnDrawGizmosSelected(){
		float deployDist = deployVel * ignitionDelay;

		// Deployment line
		Vector3 ignitionPoint = transform.position + (-transform.up * deployDist);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, -transform.up * deployDist);
		Gizmos.DrawSphere(ignitionPoint, 0.1f);

		// Launch line
		Gizmos.color = Color.cyan;
		UnityEditor.Handles.color = Color.cyan;
		float pointerDist = 5f;
		Gizmos.DrawRay(ignitionPoint, transform.forward * 5f);
		UnityEditor.Handles.ConeCap(0, ignitionPoint + transform.forward * pointerDist, transform.rotation, 0.3f);
		
	} // End of OnDrawGizmos().
#endif

} // End of Missile.

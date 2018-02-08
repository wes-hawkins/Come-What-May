using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.FastLineRenderer;

public class Weapon_Instant : Weapon {
	
	// The method of calculating the physics/hit/damage.
	[SerializeField] private HitscanType myHitscanType = HitscanType.surface; public HitscanType MyHitscanType { get { return myHitscanType; } }
	public enum HitscanType {
		checkless, // Will not check for hitscan, at all.
		occasional, // Will check every X number of shots.
		surface, // Impacts a surface, then stops.
		penetrating // Impacts a surface and 'digs in' looking for more targets, shedding damage as it goes.
	}
	
	[Tooltip("How many rounds/second this weapon fires. Set to 0 for a constant beam.")]
	[SerializeField] private float rateOfFire = 10f;
	public bool IsBeam { get { return rateOfFire == 0f; } }
	[Tooltip("How much damage this weapon does per shot, or how much damage per second of sustained fire for beam weapons.")]
	[SerializeField] private float damage = 10f;
	[Tooltip("How 'wide' 95% of the weapon's shots can go off from the muzzle axis (in degrees), based on a gaussian distribution.")]
	[SerializeField] private float standardInaccuracy = 0f;
	[Tooltip("How damage energy this weapon uses per shot, or how much damage per second of sustained fire for beam weapons.")]
	[SerializeField] private float energy = 10f;
	[Tooltip("The maximum range of this weapon.")]
	[SerializeField] private float maxRange = 1000f;

	private float cooldown = 0f; // Timer between shots.
	private float lineDecay = 0f; // Fades the lineRenderer.
	[Tooltip("How long the line will remain visible, fading out over this time. 0 means 1-frame flash.")]
	[SerializeField] private float lineDecayTime = 0f;
	
	[Tooltip("Created when the weapon fires (or starts firing, for constant weapons.")]
	[SerializeField] private GameObject muzzleFlashPrefab = null;
	private ParticleSystemGroup muzzleParticleSys = null; // This is looked for in the muzzle object.
	[Tooltip("The 'impact.' Impulse weapons will instantiate this prefab. Beam weapons will look for a particle system in this object to control.")]
	[SerializeField] private GameObject impactEffect = null;
	private ParticleSystemGroup impactParticleSys = null;

	[Tooltip("This sound will play when the weapon fires.")]
	[SerializeField] private SpatialAudioClip fireClip = null;
	[SerializeField] private bool loopFireClip = false;
	private SpatialAudioSource fireLoopingSource = null;
	[Tooltip("This sound will play weapon the weapon stops firing.")]
	[SerializeField] private SpatialAudioClip fireCooldownClip = null;


	private void Awake () {
		if(IsBeam) {
			impactParticleSys = impactEffect.GetComponent<ParticleSystemGroup>();
		}

		muzzleParticleSys = muzzle.GetComponent<ParticleSystemGroup>();
		

		if(loopFireClip)
			fireLoopingSource = SpatialAudioManager.AttachClipToTransform(fireClip, muzzle);

	} // End of Awake().

	
	protected override void Update () {
		base.Update();

		cooldown = Mathf.MoveTowards(cooldown, 0f, Time.deltaTime);
		
		// Fire!
		if(trigger && (cooldown == 0f)) {
			if(IsBeam) {

			} else {
				if(fireLoopingSource) {
					// Randomize looping source in some way?
				} else if(fireClip)
					SpatialAudioManager.PlayClipOnTransform(fireClip, muzzle);

				if(muzzleFlashPrefab)
					Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation, null);

				cooldown = 1f / rateOfFire;

				if(myHitscanType == HitscanType.surface) {
					Ray muzzleRay = new Ray(muzzle.position, muzzle.forward);
					RaycastHit rayHit = new RaycastHit();
					Vector3 lineEnd;
					if(Physics.Raycast(muzzleRay, out rayHit, maxRange))
						lineEnd = rayHit.point;
					else
						lineEnd = muzzleRay.origin + (muzzleRay.direction * maxRange);
				}

				if(target && (myHitscanType == HitscanType.checkless)) {
					Vector3 vectorToTarget = target.transform.position - muzzle.position;
					float distanceToTarget = vectorToTarget.magnitude;
					float deflectionFromTarget = Vector3.Angle(muzzle.forward, vectorToTarget);
					
					// Radius of the target's sillouette, in degrees.
					float targetAngularSillouette = Mathf.Atan2(target.RoughSize, distanceToTarget) * Mathf.Rad2Deg;
					
					float innacuracyDeflection = PeterAcklamInverseCDF.NormInv(Random.Range(0f, 1f), deflectionFromTarget, standardInaccuracy);
					bool hit = Mathf.Abs(innacuracyDeflection) < targetAngularSillouette;
					if(hit) {
						hitIndicate = 0.1f;

						TakeDamage hitTakeDamage = target.GetComponent<TakeDamage>();
						if(hitTakeDamage)
							hitTakeDamage.ChangeHitpoints(-damage);
					}
				}
			}
		}

		if(!trigger && fireLoopingSource.IsPlaying && fireCooldownClip)
			SpatialAudioManager.PlayClipOnTransform(fireCooldownClip, muzzle);
		if(fireLoopingSource)
			fireLoopingSource.PlayControl(trigger);

		if(reticleUI != null)
			reticleUI.SetPosition(muzzle.position + transform.forward * LocalPlayerController.Inst.DistanceToTarget);

		if(muzzleParticleSys)
			muzzleParticleSys.SetEmission(trigger);
		
	} // End of Update().

} // End of Weapon_DirectFire.


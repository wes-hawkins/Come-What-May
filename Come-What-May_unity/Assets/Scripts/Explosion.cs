using UnityEngine;
using System.Collections;

public class Explosion {


	public static void AtPoint(Vector3 point, float damage, float radius, float force = 0f){

		//MonoBehaviour.print("BOOM!");

		Collider[] hitColliders = Physics.OverlapSphere(point, radius);
		for(int i = 0; i < hitColliders.Length; i++){
			Collider curCollider = hitColliders[i];

			/*
			Shield colliderShield = curCollider.gameObject.GetComponent<Shield>();
			if(colliderShield){
				Vector3 closestShieldPoint = curCollider.ClosestPointOnBounds(pos);
				float distFromShieldBounds = Vector3.Distance(pos, closestShieldPoint);
				colliderShield.DealDamage(damage * (1f - (distFromShieldBounds / radius)));
				continue;
			}
			*/
			Entity colliderEntity = curCollider.gameObject.GetFirstComponentUpHierarchy<Entity>();
			if(!colliderEntity)
				continue;

			//MonoBehaviour.print("Testing " + colliderEntity.gameObject.name);

			// This allows for the explosion to 'wash' around smaller obstructions.
			float washDist = radius * 0.4f;

			Vector3 closestPoint = curCollider.ClosestPointOnBounds(point);
			float distFromBounds = Vector3.Distance(point, closestPoint);

			float angularSize = Mathf.Abs(Mathf.Atan2(colliderEntity.RoughSize, distFromBounds)) * Mathf.Rad2Deg;

			Ray obstructRay = new Ray(point, closestPoint - point);
			RaycastHit[] obstructionHits = Physics.RaycastAll(obstructRay, distFromBounds);
			bool obstructed = false;
			for(int j = 0; j < obstructionHits.Length; j++){
				RaycastHit curObstHit = obstructionHits[j];
				Entity curObstEntity = curObstHit.collider.gameObject.GetFirstComponentUpHierarchy<Entity>();
				if(!curObstEntity)
					continue;

				/*
				if(curObstHit.collider.transform.GetComponent<Shield>()){
					obstructed = true;
					continue;
				}
				*/

				float obstAngSize = Mathf.Abs(Mathf.Atan2(curObstEntity.RoughSize, curObstHit.distance + washDist)) * Mathf.Rad2Deg;

				// Account for leniency based on the size of the explosion.
				//MonoBehaviour.print("  " + WesExt.ColorText(Color.white, curObstPart.gameObject.name) + "  size: " + curObstPart.RoughSize + "  dist: " + obstDist + "  angSize: " + obstAngSize);
				if(obstAngSize > (angularSize)){
					obstructed = true;
					//MonoBehaviour.print("Obstructed!");
					//Debug.DrawRay(obstructRay.origin, obstructRay.direction * distFromBounds, Color.cyan);
					break;
				}

			}

			if(!obstructed){
				//MonoBehaviour.print(WesExt.ColorText(Color.red, colliderPart.gameObject.name + " damaged!"));
				TakeDamage colliderTakeDamage = colliderEntity.GetComponent<TakeDamage>();
				colliderTakeDamage.ChangeHitpoints(-damage * (1f - Mathf.Clamp01(distFromBounds / radius)));
				//Debug.DrawRay(obstructRay.origin, obstructRay.direction * distFromBounds, Color.red);

				if(colliderEntity.MyRigidbody) {
					Vector3 vectorFromExplosion = colliderEntity.MyRigidbody.worldCenterOfMass - point;
					colliderEntity.MyRigidbody.AddForce(vectorFromExplosion.normalized * force * Mathf.Clamp01(1f - (vectorFromExplosion.magnitude / radius)), ForceMode.Impulse);
					MonoBehaviour.print(vectorFromExplosion.normalized * force * Mathf.Clamp01(1f - (vectorFromExplosion.magnitude / radius)));
				}
			}
		}

		/*
		for(int i = 0; i < Part.all.Count; i++){
			Part curPart = Part.all[i];
			Vector3 vecToPart = curPart.transform.position - pos;
			float distToPart = vecToPart.magnitude;
			if(distToPart < radius){
				curPart.DealDamage(damage * (1f - (distToPart / radius)));
			}
		}
		*/

	} // End of AtPoint().

} // End of Explosion.

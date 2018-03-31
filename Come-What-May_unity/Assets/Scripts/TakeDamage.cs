using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TakeDamage : NetworkBehaviour {

	[SerializeField] private float maxHitpoints = 100f; public float MaxHitpoints { get { return maxHitpoints; } }
	private float hitpoints; public float Hitpoints { get { return hitpoints; } }
	[SerializeField] private bool destoyAt0HP = true;
	[SerializeField] private GameObject destroyExplosion = null;
	[SerializeField] private SpatialAudioClip destroyClip = null;


	private void Awake () {
		hitpoints = maxHitpoints;
	} // End of Awake().
	
	public void ChangeHitpoints(float hitpointsChange) {
		hitpoints += hitpointsChange;
		
		if(hitpoints <= 0f)
			Local_Explode();
	} // End of Update().


	// EXPLODE ------------------------------------------------------------------------------ //
	private void Explode() {
		if(destroyExplosion)
			Instantiate(destroyExplosion, transform.position, Quaternion.identity);
		if(destroyClip)
			SpatialAudioManager.PlayClipAtPoint(destroyClip, transform.position);
		Destroy(gameObject);
	} // End of Explode().
	public void Local_Explode() {
		Explode();
		//Cmd_Explode(NetworkPilot.LocalNetIdent);
	} // End of Local_SetName().
	[Command] private void Cmd_Explode(NetworkIdentity sender) {
		Rpc_Explode(sender);
	} // End of Cmd_SetName().
	[ClientRpc] private void Rpc_Explode(NetworkIdentity sender) {
		if(!sender.isLocalPlayer)
			Explode();
	} // End of Rpc_SetName().

} // End of TakeDamage.

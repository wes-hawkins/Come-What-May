using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour {

	[SerializeField] private string weaponName = "New Weapon";
	[SerializeField] protected Transform muzzle = null;
	[SerializeField] protected Sprite reticle = null;
	[SerializeField] protected float reticleSize = 25f;
	protected DiageticUIElement reticleUI = null;
	protected float hitIndicate = 0f;

	protected bool trigger = false;


	protected virtual void Start() {
		if(reticle)
			reticleUI = GameGUIController.Inst.NewDiageticUIElement(reticle, reticleSize, false);
	} // End of Start().

	protected virtual void Update() {
		// Dealing damage audio indication
		hitIndicate = Mathf.MoveTowards(hitIndicate, 0f, Time.deltaTime);

		if(hitIndicate > 0f){
			GameGUIController.Inst.HitImpactSound();
			if((Time.time % 0.1) < 0.05f)
				reticleUI.SetColor(Color.red);
			else
				reticleUI.SetColor(Color.black);
		/*}else if(hitShieldIndicate > 0f){
			if((Time.time % 0.1) < 0.05f)
				reticleUI.SetColor(Color.cyan);
			else
				reticleUI.SetColor(Color.black);*/
		}else if(target){
			reticleUI.SetColor(Color.white);
		}else
			reticleUI.SetColor(new Color(1f, 1f, 1f, 0.05f));
	} // End of Update().

	protected Entity target = null;
	public void SetTarget(Entity newTarget) {
		target = newTarget;
	} // End of SetTarget().
	
	protected void OnDestroy() {
		if(reticleUI)
			Destroy(reticleUI.gameObject);
	} // End of OnDestroy().


	// SET TRIGGER ------------------------------------------------------------------------------ //
	private void SetTrigger(bool newTrigger) {
		trigger = newTrigger;
	} // End of SetTrigger().
	public void Local_SetTrigger(bool newTrigger) {
		if(trigger != newTrigger) {
			SetTrigger(newTrigger);
			Cmd_SetTrigger(NetworkPilot.LocalNetIdent, newTrigger);
		}
	} // End of Local_SetTrigger().
	[Command] private void Cmd_SetTrigger(NetworkIdentity sender, bool newTrigger) {
		Rpc_SetTrigger(sender, newTrigger);
	} // End of Cmd_SetTrigger().
	[ClientRpc] private void Rpc_SetTrigger(NetworkIdentity sender, bool newTrigger) {
		if(!sender.isLocalPlayer)
			SetTrigger(newTrigger);
	} // End of Rpc_SetTrigger().

} // End of Weapon.

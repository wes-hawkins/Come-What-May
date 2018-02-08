using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Deploy : Weapon {

	[SerializeField] protected IDeployable[] deployables = null;

	protected override void Update(){
		base.Update();

		if(reticleUI != null) {
			if(target)
				reticleUI.SetPosition(target.transform.position);
			else
				reticleUI.SetPosition(muzzle.position + transform.forward * LocalPlayerController.Inst.DistanceToTarget);
		}
	} // End of Update().

} // End of Weapon_Deployer.

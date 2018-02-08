using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemGroup : MonoBehaviour {

	private ParticleSystem[] systems = new ParticleSystem[0];

	private void Awake () {
		systems = GetComponentsInChildren<ParticleSystem>();
	} // End of Awake().
	
	public void SetEmission(bool emit) {
		for(int i = 0; i < systems.Length; i++) {
			ParticleSystem.EmissionModule emissionModule = systems[i].emission;
			emissionModule.enabled = emit;
		}
	} // End of SetEmission().

} // End of ParticleSystemGroup.

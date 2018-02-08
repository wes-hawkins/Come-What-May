using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.FastLineRenderer;

public class EffectsManager : MonoBehaviour {

	public static EffectsManager Inst = null;

	private FastLineRenderer mainLineRenderer; public FastLineRenderer FastLineRenderer { get { return mainLineRenderer; } }


	private void Awake () {
		Inst = this;
		mainLineRenderer = GetComponent<FastLineRenderer>();
	} // End of Awake().
	
	private void LateUpdate () {
		EffectsManager.Inst.FastLineRenderer.Apply();
	} // End of Update().

} // End of EffectsManager.

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DAGNet;

public class PreferencesManager : MonoBehaviour {

	public static PreferencesManager Inst = null;
	private float mouseSensitivity = 1f; public float MouseSensitivity { get { return mouseSensitivity; } }


	private void Awake () {
		Inst = this;

		if(PlayerPrefs.HasKey("mouseSensitivity"))
			mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
	} // End of Awake().


	private void Update() {
		if(Input.GetKeyDown(KeyCode.PageUp)) {
			mouseSensitivity += 0.1f;
			mouseSensitivity = (float)Math.Round(mouseSensitivity, 1);
			PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);
			ChatManager.Inst.ConsoleMessage("Mouse sensitivity is now " + mouseSensitivity.ToString("F1"), Color.gray);
		}
		if(Input.GetKeyDown(KeyCode.PageDown)) {
			mouseSensitivity -= 0.1f;
			mouseSensitivity = (float)Math.Round(mouseSensitivity, 1);
			mouseSensitivity = Mathf.Max(mouseSensitivity, 0.1f);
			PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);
			ChatManager.Inst.ConsoleMessage("Mouse sensitivity is now " + mouseSensitivity.ToString("F1"), Color.gray);
		}
	} // End of Update().

} // End of PreferencesManager.

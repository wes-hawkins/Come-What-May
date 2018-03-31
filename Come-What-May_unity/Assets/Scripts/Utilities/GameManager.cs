using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using DAGNet;

public class GameManager : MonoBehaviour {

	public static GameManager Inst = null;
	private NetworkManagerHUD networkHud;
	private bool showDebug = false; public bool ShowDebug { get { return showDebug;}}
	private bool mouseLock = true;
	

	void Awake () {
		Inst = this;
		networkHud = GetComponent<NetworkManagerHUD>();
	} // End of Awake().
	
	void Start() {
		ChatManager.Inst.ConsoleMessage("Welcome to <b>Come What May</b> by Double Action Games.\nPlease send bug reports/suggestions/verbal abuse to Wes.");

		if(Application.isEditor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	} // End of Start().

	void Update () {
		if(Input.GetKeyDown(KeyCode.BackQuote) && !ChatManager.Inst.ChatFocused)
			showDebug = !showDebug;

		networkHud.enabled = ((!NetworkServer.active && !NetworkClient.active) || Input.GetKey(KeyCode.F1));

		if(Input.GetKeyDown(KeyCode.F2))
			CursorLock(!mouseLock);
		
	} // End of Update().

	public void CursorLock(bool lockMouse) {
		mouseLock = lockMouse;
		Cursor.lockState = mouseLock? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !mouseLock;
	} // End of CursorLock().

} // End of GameManager.

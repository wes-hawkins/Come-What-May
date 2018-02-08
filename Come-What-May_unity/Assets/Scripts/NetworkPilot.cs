using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// PlayerController serves as the nexus through which all relavent information about a player's actions travel to the rest of the game.
public class NetworkPilot : NetworkBehaviour {

	public static NetworkPilot Local = null;
	public static NetworkIdentity LocalNetIdent { get { return Local? Local.MyNetIdent : null; } }

	private string playerName = "New Player"; public string PlayerName { get { return playerName; } }
	private NetworkIdentity myNetIdent; public NetworkIdentity MyNetIdent { get { return myNetIdent; } }
	
	[SerializeField] private GameObject defaultShip = null;
	private Ship myShip = null; public Ship MyShip { get { return myShip; } }

	
	private void Awake() {
		myNetIdent = GetComponent<NetworkIdentity>();
	} // End of Awake().

	public override void OnStartLocalPlayer() { 
		base.OnStartLocalPlayer();
		Local = this;
	} // End of OnStartLocalPlayer().

	private void Update() {
		if(!LocalPlayerController.LocalShip && isLocalPlayer && Input.GetKeyDown(KeyCode.Insert))
			Local_Spawn();
	} // End of Update().


	// Spawn us a new avatar. (Inherently networked.)
	public void Local_Spawn() {
		// Gotta check if we're the server... if so, we let the new HumanController roll OnStartAuthority.
		//   If not, we'll ask the server to make our HumanController... but the server will set an inhibitor on it's OnStartAuthority
		//   because otherwise the HumanController will assign itself to the Server's NetworkPlayer.
		if(isServer) {
			GameObject newShip = Instantiate(defaultShip, GameObject.Find("_spawn").transform.position, Quaternion.identity);
			NetworkServer.SpawnWithClientAuthority(newShip, connectionToClient);
		} else
			Cmd_Spawn();
	} // End of SpawnLocal().
	[Command] private void Cmd_Spawn() {
		GameObject newShip = Instantiate(defaultShip, GameObject.Find("_spawn").transform.position, Quaternion.identity);
		newShip.GetComponent<Ship>().inhibitStartAuthority = true;
		NetworkServer.SpawnWithClientAuthority(newShip, connectionToClient);
	} // End of Cmd_Spawn().


	// ASSIGN SHIP TO THIS PLAYER ------------------------------------------------ //
	private void AssignShip(Ship newShip) {
		ChatManager.Inst.DebugMessage("New ship assigned for " + playerName);
		myShip = newShip;
		myShip.UIElement.SetLabel(playerName);
	} // End of AssignAvatar().
	public void Local_AssignShip(Ship newShip) {
		ChatManager.Inst.DebugMessage(playerName + " calling local AssignAvatar...");
		AssignShip(newShip);
		newShip.UIElement.gameObject.SetActive(false);
	} // End of Local_AssignAvatar().
	[Command] private void Cmd_AssignShip(NetworkIdentity sender, GameObject newAvatar) {
		Rpc_AssignShip(sender, newAvatar);
	} // End of AssignAvatar().
	[ClientRpc] private void Rpc_AssignShip(NetworkIdentity sender, GameObject newAvatar) {
		ChatManager.Inst.DebugMessage("Executing Rpc_AssignAvatar() for " + playerName);
		if(!sender.isLocalPlayer) {
			ChatManager.Inst.DebugMessage("Confirmed !sender.isLocalPlayer in Rpc_AssignAvatar() for " + playerName);
			AssignShip(newAvatar.GetComponent<Ship>());
		}
	} // End of AssignAvatar().


	// AUTHORITY --------------------------------------------------------------- //
	[Command] public void Cmd_AssumeAuthority(GameObject netObject) {
		netObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
		// Assume authority for any and all sub-objects.
		NetworkIdentity[] childNetIdents = netObject.GetComponentsInChildren<NetworkIdentity>();
		for(int i = 0; i < childNetIdents.Length; i++)
			if(childNetIdents[i].clientAuthorityOwner != connectionToClient)
				childNetIdents[i].AssignClientAuthority(connectionToClient);
	} // End of Cmd_AssignAuthority().

	public void Local_SurrenderAuthority(GameObject netObject) {
		if(!isServer)
			Cmd_SurrenderAuthority(netObject);
	} // End of Local_SurrenderAuthority().
	[Command] private void Cmd_SurrenderAuthority(GameObject netObject) {
		netObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
		// Remove authority for any and all sub-objects.
		NetworkIdentity[] childNetIdents = netObject.GetComponentsInChildren<NetworkIdentity>();
		for(int i = 0; i < childNetIdents.Length; i++)
			if(childNetIdents[i].clientAuthorityOwner == connectionToClient)
				childNetIdents[i].RemoveClientAuthority(connectionToClient);
	} // End of Cmd_AssignAuthority().


	// SET PLAYER NAME ----------------------------------------------------------- //
	private void SetPlayerName(string newName) {
		playerName = newName;
		name = "[Player] " + newName;
	} // End of SetName().
	public void Local_SetName(string newName) {
		ChatManager.Inst.ConsoleMessage("You are now known as " + newName + ".");
		SetPlayerName(newName);
		Cmd_SetPlayerName(LocalNetIdent, newName);
	} // End of Local_SetName().
	[Command] private void Cmd_SetPlayerName(NetworkIdentity sender, string newName) {
		Rpc_SetPlayerName(sender, newName);
	} // End of Cmd_SetName().
	[ClientRpc] private void Rpc_SetPlayerName(NetworkIdentity sender, string newName) {
		if(!sender.isLocalPlayer) {
			ChatManager.Inst.ConsoleMessage(playerName + " is now known as " + newName + ".");
			SetPlayerName(newName);
		}
	} // End of Rpc_SetName().


	// CONSOLE MESSAGE ------------------------------------------------------------ //
	[Command] public void Cmd_ConsoleMessage(string message) {
		Rpc_ConsoleMessage(message);
	} // End of Cmd_ConsoleMessage().
	[ClientRpc] private void Rpc_ConsoleMessage(string message) {
		ChatManager.Inst.ConsoleMessage(message);
	} // End of Cmd_ConsoleMessage().

} // End of PlayerController().

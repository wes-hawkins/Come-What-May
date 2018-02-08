using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

// Strongly-secured chat manager. Directly handles input.
public class ChatManager : MonoBehaviour {

	public static ChatManager Inst = null;

	[SerializeField] private TextMeshProUGUI chatText = null;
	private List<ChatMessage> chatLog = new List<ChatMessage>();
	private bool chatFocused = false; public bool ChatFocused { get { return chatFocused; } }
	private bool showBacklog = false;
	private string inputString = "";
	private float inputCaratRunner = 0f;


	private class ChatMessage {
		public string message;
		public float lifeTime;

		public ChatMessage(string message, float lifeTime) {
			this.message = message;
			this.lifeTime = lifeTime;
		}
	} // End of ChatMessage.


	void Awake() {
		Inst = this;
	} // End of Awake().

	
	void Update () {

		if(Input.GetKeyDown(KeyCode.Tab))
			showBacklog = !showBacklog;

		// Open chat with Enter
		if(NetworkPilot.Local && !chatFocused && Input.GetKeyDown(KeyCode.Return)) {
			chatFocused = true;
			inputCaratRunner = 0f;
		// Chat already open
        } else if(chatFocused) {
			inputCaratRunner += Time.deltaTime;
			foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
				if(Input.GetKeyDown(vKey)){
					string inputKey = vKey.ToString();
					bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

					if(inputKey.Equals("Return")) {
						Submit(inputString);
						inputString = "";
						chatFocused = false;
					} else if(inputKey.Equals("Backspace")) {
						if(inputString.Length > 0)
							inputString = inputString.Substring(0, inputString.Length - 1);
					} else if(inputKey.Equals("Space")) {
						inputString += " ";
					} else {
						if(inputKey.Equals("Minus"))
							inputString += shift? "_" : "-";
						else if(inputKey.Equals("Slash"))
							inputString += shift? "?" : "/";
						else if(inputKey.Equals("Quote"))
							inputString += shift? "\"" : "'";
						else if(inputKey.Equals("Equals"))
							inputString += shift? "=" : "+";
						else if(inputKey.Equals("Comma"))
							inputString += ",";
						else if(inputKey.Equals("Period"))
							inputString += ".";
						else if(inputKey.Equals("Semicolon"))
							inputString += shift? ":" : ";";
						else if(inputKey.Equals("BackQuote"))
							inputString += shift? "~" : "`";

						else if(inputKey.Equals("Alpha1"))
							inputString += shift? "!" : "1";
						else if(inputKey.Equals("Alpha2"))
							inputString += shift? "@" : "2";
						else if(inputKey.Equals("Alpha3"))
							inputString += shift? "#" : "3";
						else if(inputKey.Equals("Alpha4"))
							inputString += shift? "$" : "4";
						else if(inputKey.Equals("Alpha5"))
							inputString += shift? "%" : "5";
						else if(inputKey.Equals("Alpha6"))
							inputString += shift? "^" : "6";
						else if(inputKey.Equals("Alpha7"))
							inputString += shift? "&" : "7";
						else if(inputKey.Equals("Alpha8"))
							inputString += shift? "*" : "8";
						else if(inputKey.Equals("Alpha9"))
							inputString += shift? "(" : "9";
						else if(inputKey.Equals("Alpha0"))
							inputString += shift? ")" : "0";

						else if(inputKey.Length == 1)
							inputString += shift? inputKey.ToUpper() : inputKey.ToLower();
							//inputString += vKey.ToString();
					}
				}
			}
		}

		string chatLogString = "";
		
		bool foundMessageToExpire = false; // We only expire the 'oldest' message.
		for(int i = 0; i < chatLog.Count; i++) {
			if((chatLog[i].lifeTime > 0f) && !foundMessageToExpire) {
				chatLog[i].lifeTime = Mathf.MoveTowards(chatLog[i].lifeTime, 0f, Time.deltaTime);
				foundMessageToExpire = true;
			}

			if(showBacklog || (chatLog[i].lifeTime > 0f))
				chatLogString += chatLog[i].message + "\n";
		}

		if(chatFocused) {
			chatLogString += "\n" + inputString;
			chatLogString += (chatFocused && ((inputCaratRunner % 1f) < 0.5f))? "_" : "";
		} else if(NetworkPilot.Local)
			chatLogString += "\n<i><color=#FFFFFF4A>Press ENTER to chat.</color></i>";
		else
			chatLogString += "\n";
		
		chatText.text = chatLogString;
	} // End of Update().


	void Submit(string input) {
		if(!string.IsNullOrEmpty(input)){
			// Console command
			if((input.Length > 0) && (input[0] == '/'))
				ConsoleInterpreter.Inst.ProcCommand(input.Substring(1));
			// Chat message
			else if(NetworkPilot.Local)
				NetworkPilot.Local.Cmd_ConsoleMessage("<b>[" + NetworkPilot.Local.PlayerName + "]</b> " + input);
		}
	} // End of Submit().

	
	public void DebugMessage(string message) {
		if(GameManager.Inst.ShowDebug)
			ConsoleMessage(message, Color.green);
	} // End of DebugMessage


	public void ConsoleMessage(string message, Color? color = null) {
		chatLog.Add(new ChatMessage(message, 3f + (message.Length * 0.1f)));
	} // End of ConsoleMessage().

} // End of ChatManager.

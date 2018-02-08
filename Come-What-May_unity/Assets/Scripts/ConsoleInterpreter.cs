using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleInterpreter : MonoBehaviour {

	public static ConsoleInterpreter Inst = null;

	private class ConsoleCommand {
		private string command; public string Command { get { return command; } }
		private int maxArgs; public int MaxArgs { get { return maxArgs; } }
		public static implicit operator bool(ConsoleCommand exists){ return exists != null; }

		public ConsoleCommand(string command, int maxArgs) {
			this.command = command;
			this.maxArgs = maxArgs;
		}
	} // End of ConsoleCommand.

	// Last argument will be a string with spaces if user inputs more words than arguments accepted by the command.
	private ConsoleCommand[] commands = {
		new ConsoleCommand("name", 1)
	};


	void Awake() {
		Inst = this;
	} // End of Awake().


	public void ProcCommand(string message) {
		string[] elems = message.Split(' ');
		string command = elems[0];
		
		// Make sure it's a valid command.
		ConsoleCommand validCommand = null;
		for(int i = 0; i < commands.Length; i++) {
			if(commands[i].Command == command) {
				validCommand = commands[i];
				break;
			}
		}

		// Evaluate command.
		if(validCommand) {
			string[] args = new string[Mathf.Min(validCommand.MaxArgs, elems.Length - 1)];
			for(int i = 1; i < elems.Length; i++) {
				if(i < (args.Length + 1))
					args[i - 1] = elems[i];
				else
					args[args.Length - 1] += " " + elems[i];
				
			}

			switch(command) {
				case "name":
					if(args.Length == 1) {
						string newName = args[0].Substring(0, Mathf.Min(args[0].Length, 10));
						if(NetworkPilot.Local.PlayerName != newName) {
							NetworkPilot.Local.Local_SetName(newName);
							PlayerPrefs.SetString("playerName", newName);
						}
					}
					break;
			}
		}
	} // End of Proc().


} // End of ConsoleInterpreter.

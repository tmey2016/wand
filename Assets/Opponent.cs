using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Prime31;
using UnityEngine.UI;

public class Opponent : MonoBehaviour {
	public Transform[] opponentButtons;


	protected string[] opponentNames = new string[1];
	protected Queue<byte[]> incomingMessages = new Queue<byte[]>();

	//constructor: advertise
	public Opponent() {
#if UNITY_IPHONE
		//GameKit advertise
		MultiPeer.advertiseCurrentDeviceWithNearbyServiceAdvertiser(true, "wand");

		//Message Receiver
		MultiPeerManager.receivedRawDataEvent += multiPeerRawMessageReceiver;

#endif
	}

	
#region Message receivers

	//receive a message
	void multiPeerRawMessageReceiver( string peerId, byte[] bytes )
	{
		incomingMessages.Enqueue(bytes);
	}
	
#endregion

	//get names of opponents in range
	public List<string> AvailableOpponents() {
#if UNITY_EDITOR
		List<string> testPeers = new List<string>();
		testPeers.Add ("Adam");
		testPeers.Add ("Bill");
		testPeers.Add ("Calvin");
		return testPeers;
#endif


#if UNITY_IPHONE
		return MultiPeer.getConnectedPeers();
#endif

	}

	//get my name
	public string MyName() {
#if UNITY_EDITOR
		return "My name here";
#endif
#if UNITY_IPHONE
		return MultiPeer.getLocalPeerId();
#endif
	}

	//choose an opponent by name, must be in AvailableOpponents
	public void ChooseOpponent(string name) {
		opponentNames[0] = name;
	}

	//send a message to your opponent
	public bool Send(string message) {
#if UNITY_IPHONE

		byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(message);

		return MultiPeer.sendRawMessageToPeers(opponentNames, bytes, true);
#endif
	}

	//get and remove incoming message from the queue
	public string Receive() {
		if (incomingMessages.Count > 0) {
			byte[] bytes = incomingMessages.Dequeue();
			return System.Text.UTF8Encoding.UTF8.GetString(bytes);
		}
		return "";
	}

	//set text on opponent buttons
	public void SetOpponentButtonText() {
		List<string> availableOpponents = AvailableOpponents();
		for (int n = 0; n < opponentButtons.Length; ++n) {
			if (n < availableOpponents.Count) {
				SetButtonText(opponentButtons[n], availableOpponents[n]);
			} else {
				SetButtonText(opponentButtons[n], "");
			}
		}
	}

	//set text on a gui button
	public static void SetButtonText(Transform button, string text) {
		button.GetComponentsInChildren<Text>()[0].text = text;
	}
}

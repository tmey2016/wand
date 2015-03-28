using UnityEngine;
using System.Collections.Generic;
using Vuforia;
using Prime31;

public class Game : MonoBehaviour {

	protected const int STATE_WAITING_FOR_PEERS = 1;
	protected const int STATE_CONNECTED_TO_PEER = 2;
	protected int state = STATE_WAITING_FOR_PEERS;

	public float startTime;
	public Transform wand;
	public bool flashState = false;
	public bool continuousFocusState = false;
	public byte[] lastMessageSent;
	public byte[] lastMessageReceived;
	private GUIStyle gs = null;
	private GUIStyle gsText = null;

	// Use this for initialization
	void Start () {
		//no auto-focus
		CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);

#if UNITY_IPHONE
		//GameKit advertise
		MultiPeer.advertiseCurrentDevice(true, "wand" );

		MultiPeerManager.receivedRawDataEvent += multiPeerRawMessageReceiver;
#endif

		//start time
		startTime = Time.time;

	}
	
	// Update is called once per frame
	void Update () {
		//time
		if (Time.time - startTime >= 3) {
			Wand wandComponent = wand.GetComponent<Wand>();
			int quadrant = wandComponent.Quadrant();
#if UNITY_IPHONE

			lastMessageSent = System.Text.UTF8Encoding.UTF8.GetBytes(quadrant.ToString());
			MultiPeer.sendRawMessageToAllPeers(lastMessageSent, true);
#endif
			startTime = Time.time;
		}
	}

#if UNITY_IPHONE

	#region Message receivers
	
	void multiPeerRawMessageReceiver( string peerId, byte[] bytes )
	{
		lastMessageReceived = bytes;
	}
	
	#endregion
#endif

	void OnGUI() {
		if (gs == null) { 
			gs = new GUIStyle(GUI.skin.button);
			gs.fontSize = 30;

			gsText = new GUIStyle(GUI.skin.textArea);
			gsText.fontSize = 30;
		}
		

		Wand wandComponent = wand.GetComponent<Wand>();
		int quadrant = wandComponent.Quadrant();
		//lastMessageReceived = System.BitConverter.GetBytes(4);

		int buttonWidth = 140;
		int buttonHeight = 50;

		GUI.TextArea(new Rect(0, 0, buttonWidth, buttonHeight), "Me: " + quadrant.ToString (), gsText);
		if (lastMessageReceived.GetLength (0) > 0) {
			GUI.TextArea (new Rect(buttonWidth, 0, buttonWidth, buttonHeight), "You: " + System.Text.UTF8Encoding.UTF8.GetString( lastMessageReceived ), gsText); 
		}

		GUI.TextArea (new Rect(buttonWidth * 2, 0, buttonWidth, buttonHeight), "Peers: " + MultiPeer.getConnectedPeers().Count, gsText); 
		GUI.TextArea (new Rect(buttonWidth * 3, 0, buttonWidth, buttonHeight), "Sent: " + System.Text.UTF8Encoding.UTF8.GetString( lastMessageSent ), gsText); 

		if (GUI.Button (new Rect(0, buttonHeight, buttonWidth, buttonHeight), "Focus", gs)) {
			//auto-focus once
			CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
		}
		if (GUI.Button (new Rect(buttonWidth, buttonHeight, buttonWidth, buttonHeight), "Flashlight", gs)) {
			flashState = !flashState;
			CameraDevice.Instance.SetFlashTorchMode(flashState);
		}
		if (GUI.Button (new Rect(buttonWidth*2, buttonHeight, buttonWidth, buttonHeight), "Invite", gs)) {
			//GameKit get peer
			MultiPeer.showPeerPicker();
		}
		if (GUI.Button (new Rect(buttonWidth*3, buttonHeight, buttonWidth, buttonHeight), "Auto-Focus", gs)) {
			continuousFocusState = !continuousFocusState;
			if (continuousFocusState) {
				CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
			} else {
				CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
			}
		}
	}
}

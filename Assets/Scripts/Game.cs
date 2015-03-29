using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using Prime31;

public class Game : MonoBehaviour {

	public float startTime;
	public Transform wand;

    private Dictionary<string, int> PowerHash = new Dictionary<string,int>();
    private Dictionary<string, int> VulnerabilityHash = new Dictionary<string,int>();

	protected const string ATTACK = "A";
	protected const string SHIELD = "B";
	protected const string EXPEL  = "C";

	protected const int WIN = 1;
	protected const int LOSE = -1;
	protected const int DRAW = 0;

    private int player1WinCount = 3;
    private int player2WinCount = 3;

	public int roundTime;
	public int timeBetweenRounds;

	public string debugText = "";

	public GameObject countDownTimers;

    protected Opponent opponent;

	protected bool gameStarted = false;
	
	public GameObject GetReady;
	public GameObject Win;

	public GameObject player1Bar;
	public GameObject player2Bar;

	public GameObject P1Win;
	public GameObject P2Win;
	public GameObject triangle;
	public GameObject life1;
	public GameObject life2;
	public GameObject BackToMain;


	// Use this for initialization
	void Start () {
		//no auto-focus
		CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

		//start time
		startTime = Time.time;

        opponent = gameObject.GetComponent<Opponent>();

     

		//flashlight on
		CameraDevice.Instance.SetFlashTorchMode(true);

		//load spells
		UpdateSpellBook();

#if UNITY_EDITOR
		StartCoroutine(StartRound(timeBetweenRounds));
#endif

	}
	
    void UpdateSpellBook()
    {
        PowerHash["ABABA"] = 6;
        VulnerabilityHash["ABABA"] = 6;

        PowerHash["BCBCB"] = 4;
        VulnerabilityHash["BCBCB"] = 2;

        PowerHash["CACAC"] = 2;
        VulnerabilityHash["CACAC"] = 1;
    }


    IEnumerator CalculateWinCondition(float Time)
    {
		UpdatePlayerBars ();
		GetReady.SetActive (false);
        Log("GameManager : Calculating Win Conditions in " + Time);
        //yield return new WaitForSeconds(Time);



		while (Time > 0) 
		{
			SetCountDown((int)Time);
			Log ("GameTime Remaining : " + Time);
			yield return new WaitForSeconds(1);
			Time--;
		}

        Wand wandComponent = wand.GetComponent<Wand>();

        string player1String = wandComponent.GetSpell();


        //Get PlayerString from the bluetooth data we received.
		bool sent = opponent.Send (player1String);
		if (sent) {
			Log("Send successful");
		} else {
			Log ("Send failed");
		}
		Log("Sent message: " + player1String);
		SetCountDown (0);
        Log("GameManager: Waiting to receive Data from player");

        yield return new WaitForSeconds(1.0f);

        Log("GameManager: Received Data from player");
        
        string player2String = opponent.Receive();

       // string player2String = "B";
        wandComponent.bStartDuel = false;


		//special attack from me
		if (PowerHash.ContainsKey(player1String)) {
			//special power from another player
			Log ("TODO: Special Attack");
		}

		//special attack from opponent
		if (PowerHash.ContainsKey(player2String)) {
			//special power from another player
			Log ("TODO: Special Attack");
		}

		string player1SpellType = SpellType(player1String);
		string player2SpellType = SpellType(player2String);

		Log ("Player 1 String: " + player1String);
		Log ("Player 1 Type: " + player1SpellType);

		int loseDrawWin = LoseDrawWin(player1SpellType, player2SpellType);
		if (loseDrawWin == WIN) {
            // Insert Win Screen here
            Log("You Won the Duel");
			player2WinCount--;
		} else if (loseDrawWin == LOSE) {
            // Insert Lose Screen here
            Log("You Lost the Duel");
			player1WinCount--;

		} else {
            //Insert Draw Screen Here
            Log("Draw");
		};

		UpdatePlayerBars ();
		ShowResult (loseDrawWin);

		SpawnProj spawnProj = wand.GetComponent<SpawnProj>();
		if (loseDrawWin == WIN) 
		{
			spawnProj.FireProjectile ();
		} 
		else if (loseDrawWin == LOSE) 
		{
			spawnProj.FireLoseProjectile ();
		}

		yield return new WaitForSeconds (3);

		ShowResult (-2);

		spawnProj.UnlockWand ();

		wandComponent.Reset();

        if (player1WinCount == 0) 
		{
			P2Win.SetActive(true);
			triangle.SetActive (false);
			life1.SetActive (false);
			life2.SetActive (false);
			BackToMain.SetActive(true);

		} 
		else if (player2WinCount == 0) 
		{
			P1Win.SetActive(true);
			triangle.SetActive (false);
			life1.SetActive (false);
			life2.SetActive (false);
			BackToMain.SetActive(true);
		}
        else
        {
            //Get Ready for the next round;
            StartCoroutine(StartRound(timeBetweenRounds));
        }
        //StartCoroutine(StartRound(3));
    }

    IEnumerator StartRound(int startTime)
    {
		GetReady.SetActive (true);
        while(startTime > 0)
        {
          //  Log("GameManager: Starting Round in " + startTime + "seconds!!");
			SetCountDown(startTime);
			if (startTime == 1) {
				CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
			}
            yield return new WaitForSeconds(1);
            startTime--;
        }

       // Log("GameManager: Duel Start");
        //Set Start Duel Timer here
        Wand wand = FindObjectOfType<Wand>();
        wand.bStartDuel = true;

        StartCoroutine(CalculateWinCondition(roundTime));
    }

	//which of the three spell types is this?
	protected string SpellType(string spellString) {
		string spellType;
		if (PowerHash.ContainsKey(spellString)) {
			//special power from another player
			spellType = ATTACK; //special spells are always attacks

			Log ("TODO: Special Attack");
		} else {
			spellType = spellString.Substring(spellString.Length - 1);
		}
		return spellType;
	}

	//LOSE DRAW WIN
	protected int LoseDrawWin(string player1Type, string player2Type) {
		Log("Player 1: " + player1Type);
		Log ("Player 2: " + player2Type);
		if (player1Type == player2Type) {
			return DRAW;
		}

		if (player1Type == ATTACK) {
			if (player2Type == SHIELD) {
				return LOSE;
			}
		} else if (player1Type == SHIELD) {
			if (player2Type == EXPEL) {
				return LOSE;
			}
		} else if (player1Type == EXPEL) {
			if (player2Type == ATTACK) {
				return LOSE;
			}
		}
		return WIN;
	}


	//debugging
	void OnGUI() {
		GUI.TextField (new Rect(0, 0, 600, 200), debugText);
	}

	public void Log(string text) {
		Debug.Log (text);
		debugText += "\n" + text;
	}

	void Update() {
#if UNITY_IPHONE
		if (!gameStarted) {
			if (MultiPeer.getConnectedPeers().Count > 0) {
				Log ("First avail opponent: " + opponent.AvailableOpponents()[0]);
				opponent.ChooseOpponent(opponent.AvailableOpponents()[0]);
				StartCoroutine(StartRound(timeBetweenRounds));
				gameStarted = true;
			}
		}
#endif
	}

	void SetCountDown(int number)
	{
		//Log ("SetCountDown : " + number);
		for (int n = 10; n >=1; n--) {
			//Log ("SetCountDownTimer Child : " + countDownTimers.transform.GetChild(n - 1).name);
			countDownTimers.transform.GetChild(n-1).gameObject.SetActive(n == number);
		}
	}

	void  ShowResult( int result)
	{
		
		for (int n = -1; n <=1; n++) 
		{
			Log ("Show Result  : " + Win.transform.GetChild(n + 1).name);
			Win.transform.GetChild(n+1).gameObject.SetActive(n == result);
		}
	}

	void UpdatePlayerBars()
	{
		for (int n = 0; n <  3; n++) 
		{
			if(n < player1WinCount)
				player1Bar.transform.GetChild(n).gameObject.SetActive(true);
			else
				player1Bar.transform.GetChild(n).gameObject.SetActive(false);
		}

		for (int n = 0; n <  3; n++) {
			if( n < player2WinCount)
				player2Bar.transform.GetChild(n).gameObject.SetActive(true);
			else
				player2Bar.transform.GetChild(n).gameObject.SetActive(false);
		}
	}
}


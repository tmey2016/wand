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

	// Use this for initialization
	void Start () {
		//no auto-focus
		CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
		
		//start time
		startTime = Time.time;

		StartCoroutine(CalculateWinCondition(10.0f));
        
		//flashlight on
		CameraDevice.Instance.SetFlashTorchMode(true);

		//load spells
		UpdateSpellBook();
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

	// Update is called once per frame
	void Update () {
	}


    IEnumerator CalculateWinCondition(float Time)
    {
        Wand wandComponent = wand.GetComponent<Wand>(); 

        string player1String = wandComponent.GetSpell();

		Opponent opponent = gameObject.GetComponent<Opponent>();
		opponent.Send(player1String);

        //TODO: Get PlayerString from the bluetooth data we received.
        string player2String = "B";

        yield return new WaitForSeconds(Time);


		string player1SpellType = SpellType(player1String);
		string player2SpellType = SpellType(player2String);

		int loseDrawWin = LoseDrawWin(player1SpellType, player2SpellType);
		if (loseDrawWin == WIN) {
            // Insert Win Screen here
            Debug.Log("You Won the Duel");
		} else if (loseDrawWin == LOSE) {
            // Insert Lose Screen here
            Debug.Log("You Lost the Duel");

		} else {
            //Insert Draw Screen Here
            Debug.Log("Draw");
		}
    }

	//which of the three spell types is this?
	protected string SpellType(string spellString) {
		string spellType;
		if (PowerHash.ContainsKey(spellString)) {
			//special power from another player
			spellType = "A"; //special spells are always attacks
		} else {
			spellType = spellString.Substring(spellString.Length - 1);
		}
		return spellType;
	}

	//LOSE DRAW WIN
	protected int LoseDrawWin(string player1Type, string player2Type) {
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
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wand : MonoBehaviour {

    enum WandSpellState
    {
        StateInvalid,
        StateA,
        StateB,
        StateC,
        StateMax
    }
    
    public Vector3 SpellA;
    public Vector3 SpellB;
    public Vector3 SpellC;

    private WandSpellState currentSpell;

    List<string> spellList = new List<string>();

    public bool bStartDuel = false;

	public GameObject spellASelected;
	public GameObject spellBSelected;
	public GameObject spellCSelected;

	public GameObject GetReady;

    void Start()
    {
        currentSpell = WandSpellState.StateInvalid;

        SpellA = new Vector3(0.5f, 0.866f, 0.0f);
        SpellB = new Vector3(1.0f, 0.0f, 0.0f);
        SpellC = new Vector3(0.0f, 0.0f, 0.0f);

        spellList.Clear();

        //InvokeRepeating("WaitForPrint", 0.0f, 10.0f);
        //StartRoundTimer(3);
        

    }

	/**
	 * Get number for the quadrant the wand is currently in
	 * relative to screen position
	 * 1 2 
	 * 3 4
	 */
	WandSpellState GetCurrentSpell() 
    {
		Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        float minDistance = 10000000.0f;
        viewportPosition.z = 0;

        var distanceFromA = Vector3.Distance(viewportPosition, SpellA);
        var distanceFromB = Vector3.Distance(viewportPosition, SpellC);
        var distanceFromC = Vector3.Distance(viewportPosition, SpellB);

        if(distanceFromA < minDistance)
        {
            minDistance = distanceFromA;
        }
        
        if(distanceFromB < minDistance)
        {
            minDistance = distanceFromB;
        }
        
        if( distanceFromC < minDistance)
        {
            minDistance = distanceFromC;
        }

        if(minDistance == distanceFromA)
        {
            return WandSpellState.StateA;
        }
        else if(minDistance == distanceFromB)
        {
            return WandSpellState.StateB;
        }
        else if(minDistance == distanceFromC)
        {
            return WandSpellState.StateC;
        }

		return WandSpellState.StateInvalid;
	}

	// Update is called once per frame
	void Update () 
    {
        if (!bStartDuel) 
				return;

        WandSpellState newSpell = GetCurrentSpell();

        if(newSpell != currentSpell)
        {
            switch(newSpell)
            {
                case WandSpellState.StateA:
                    spellList.Add("A");
                    break;
                
                case WandSpellState.StateB:
                    spellList.Add("B");
                    break;
                
                case WandSpellState.StateC:
                    spellList.Add("C");
                    break;

                default:
                    Debug.Log("Invalid Spell!!!");
                    break;
            }

            currentSpell = newSpell;
            
            if(spellList.Count > 5)
            {
                spellList.RemoveAt(0);
            }

            WaitForPrint();
			ShowSelectedSpell();
        }
	}

    void WaitForPrint()
    {
        Debug.Log(string.Join(string.Empty, spellList.ToArray()));
    }

    public string GetSpell()
    {
        return string.Join(string.Empty, spellList.ToArray());
    }

    IEnumerator StartRoundTimer(float startTime)
    {
        while( startTime > 0)
        {
            Debug.Log("Starting Game in " + startTime);
            yield return new WaitForSeconds(1);
            startTime--;
        }

        bStartDuel = true;
        
    }

	public void Reset() 
	{
		spellList = new List<string>();
		currentSpell = WandSpellState.StateInvalid;
	}

	public void ShowSelectedSpell()
	{
		spellASelected.SetActive (false);
		spellBSelected.SetActive (false);
		spellCSelected.SetActive (false);
		switch(currentSpell)
		{
		case WandSpellState.StateA:
			spellASelected.SetActive (true);
			break;
			
		case WandSpellState.StateB:
			spellBSelected.SetActive (true);
			break;
			
		case WandSpellState.StateC:
			spellCSelected.SetActive (true);
			break;
			
		default:
			Debug.Log("Invalid Spell!!!");
			break;
		}
	}	
}

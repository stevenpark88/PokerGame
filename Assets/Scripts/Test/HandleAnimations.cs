using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HandleAnimations : MonoBehaviour
{
	public CardAnimations cardAnim;

	// Use this for initialization
//	void Start () {
//		cardAnim= GameObject.FindGameObjectWithTag("GamePlayPanel").GetComponent<CardAnimations> ();
//	}
	void Awake ()
	{
		cardAnim = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CardAnimations> ();
	}


	public CardAnimations GetInstanceCardAnimation(){

		return this.cardAnim;
	}


	public void AnimDisplayDefaultCard (int roundName)
	{

		switch (roundName) {
		case GameConstant.TEXASS_ROUND_FLOP:
			StartCoroutine (FlopCardsAnim ());
			break;
		case GameConstant.TEXASS_ROUND_TURN:
			StartCoroutine (TurnCardAnim ());
			break;
		case GameConstant.TEXASS_ROUND_RIVER:
			StartCoroutine (RiverCardAnim ());
			break;
	
		}

	}

	IEnumerator FlopCardsAnim ()
	{
		yield return new WaitForSeconds (0.0f);
		cardAnim.DefaultCardAnimation ("Flop1_Card");
			
		yield return new WaitForSeconds (1f);
		cardAnim.DefaultCardAnimation ("Flop2_Card");
			
		yield return new WaitForSeconds (1f);
		cardAnim.DefaultCardAnimation ("Flop3_Card");

	}

	IEnumerator TurnCardAnim ()
	{
		
		yield return new WaitForSeconds (0.0f);
		cardAnim.DefaultCardAnimation ("Turn_Card");

	}

	IEnumerator RiverCardAnim ()
	{
		
		yield return new WaitForSeconds (0.0f);
		cardAnim.DefaultCardAnimation ("River_Card");
		
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}

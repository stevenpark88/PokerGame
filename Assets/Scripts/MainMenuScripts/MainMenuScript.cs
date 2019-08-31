using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuScript : MonoBehaviour {

	public GameObject mainMenuPanel;
	public GameObject roomListPanel;
	public GameObject loginPanel;
	public GameObject pokerGamesBtnsPanel;
	public GameObject tournamentPanel;
	public GameObject tournamentGamesBtnsPanel;
	public GameObject pokerGamesDataPanel;
	public GameObject tournamentGamesDataPanel;
	public Text headerText;
	public Dropdown dpBuyIn;
	public Dropdown dpStake;
	private List<string> stakeList = new List<string> ();

	string gameName = GameConstant.GAME_TEXAS_HOLD + " " + GameConstant.GAME_LIMIT;
	string gameTime = GameConstant.GAME_SPEED_REGULAR;
	int buyIn = 0;
	int maxPlr = 3;
	string strStake = "";
	// Use this for initialization
	void Start () 
	{
		if (appwarp.isTexassGame) {
			headerText.text = "Texas Hold'em Poker";
		} else {
			headerText.text = "WhoopAss Poker";
		}	
		for (int i = 1; i < 10; i++) {
			dpBuyIn.options.Add (new Dropdown.OptionData(""+ (100*i)));
		}
		dpBuyIn.value = 0;
		dpBuyIn.RefreshShownValue();
	}
	
	public void OnPokerGamesClick()
	{
		if (tournamentPanel.activeSelf) 
		{
			tournamentPanel.SetActive(false);
		}
		if(tournamentGamesDataPanel.activeSelf)
		{
			tournamentGamesDataPanel.SetActive(false);
		}
		if(tournamentGamesBtnsPanel.activeSelf)
		{
			tournamentGamesBtnsPanel.SetActive(false);
		}
		pokerGamesBtnsPanel.SetActive (true);
	}

	public void OnTournamentClick()
	{
		if (pokerGamesBtnsPanel.activeSelf) 
		{
			pokerGamesBtnsPanel.SetActive(false);
		}
		if(pokerGamesDataPanel.activeSelf)
		{
			pokerGamesDataPanel.SetActive(false);
		}
		if(tournamentGamesDataPanel.activeSelf)
		{
			tournamentGamesDataPanel.SetActive(false);
		}
		if(tournamentGamesBtnsPanel.activeSelf)
		{
			tournamentGamesBtnsPanel.SetActive(false);
		}
		tournamentPanel.SetActive (true);
	}

	public void OnPlayBtnClick()
	{
		mainMenuPanel.SetActive (false);
		roomListPanel.SetActive (true);
		DEBUG.Log("Creating game...");
	}

	public void TournamentGames1()
	{
		tournamentPanel.SetActive (false);
		tournamentGamesBtnsPanel.SetActive (true);
	}

	public void PokerGames()
	{
		pokerGamesDataPanel.SetActive (true);
	}

	public void TournamentGames()
	{
		tournamentGamesDataPanel.SetActive (true);
	}

	public void OnBackBtnClick()
	{
		mainMenuPanel.SetActive (false);
		loginPanel.SetActive (true);
	}

	public void onGameTypeSelected(Dropdown dropdown){
		switch (dropdown.value) {
		case 0:
			gameName = GameConstant.GAME_TEXAS_HOLD + " " + GameConstant.GAME_LIMIT;
			break;
		case 1:
			gameName = GameConstant.GAME_TEXAS_HOLD + " " + GameConstant.GAME_NO_LIMIT;
			break;
		case 2:
			gameName = GameConstant.GAME_WHOOPASS + " " + GameConstant.GAME_LIMIT;
			break;
		case 3:
			gameName = GameConstant.GAME_WHOOPASS + " " + GameConstant.GAME_NO_LIMIT;
			break;
		}
		DEBUG.Log ("Game Type : "+ gameName);
	}

	public void onGameSpeedSelected(Dropdown dropdown){
		switch (dropdown.value) {
			case 0:
				gameTime = GameConstant.GAME_SPEED_REGULAR;
				break;
			case 1:
				gameTime = GameConstant.GAME_SPEED_TURBO;
				break;
		}
		DEBUG.Log ("Game Time : "+ gameTime);
	}

	public void onGameBuyInSelected(){
		buyIn = 100 * (dpBuyIn.value + 1);
		DEBUG.Log ("Game Buy : "+ buyIn);
	}

	public void onGameStakeSelected(){
		strStake = stakeList[dpStake.value];
		DEBUG.Log ("Stake : "+ strStake);
	}
	public void onGameMaximumPlayerSelected(Dropdown dropdown){
		switch (dropdown.value) {
		case 0:
			maxPlr = 3;
			break;
		case 1:
			maxPlr = 6;
			break;
		case 2:
			maxPlr = 9;
			break;
		}
		DEBUG.Log ("Max Plr : "+ maxPlr);
	}
}

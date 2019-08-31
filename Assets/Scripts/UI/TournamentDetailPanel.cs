using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TournamentDetailPanel : MonoBehaviour
{
	public static TournamentDetailPanel Instance;

	#region PUBLIC_VARIABLES

	public Image imgBackground;
	public Transform parentObject;

	public Loader loader;
	public Text txtMessage;

	[Space (10)]
	[Header ("Basic Info")]
	public Text txtTitle;
	public Text txtTournamentType;
	public Text txtGameType;
	public Text txtPrizePool;
	public Text txtPlacesPaid;

	[Space (10)]
	[Header ("Detail")]
	public Text txtTournamentDetail;
	public Text txtTournamentMoneyTypeAndLevel;

	[Space (10)]
	[Header ("Players")]
	public Text txtPlayers;
	public Text txtPerTable;
	public Text txtEnrolled;

	[Space (10)]
	[Header ("Awards Paid Info")]
	public Text txtAwardsPaid;
	public Text txtWinnersPlacesPaid;

	public GameData gameData;

	[Space (10)]
	[Header ("Buttons")]
	public Button btnPlay;
	public Button btnRegister;

	[Space (10)]
	public TournamentWinnersDetailPanel tournamentWinnersDetailPanel;
	public TournamentRegisteredPlayerPanel tournamentRegisteredPlayerPanel;

	[Space (10)]
	public TourRegConfirmationPanel tourRegConfirmationPanel;

	#endregion

	#region PRIVATE_VARIABLES

	private const float animationDuration = .1f;

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Start ()
	{
		Instance = this;
	}

	void OnEnable ()
	{
		imgBackground.CrossFadeAlpha (0f, 0f, true);

		imgBackground.CrossFadeAlpha (1f, animationDuration, true);
		Hashtable htScale = new Hashtable ();
		htScale.Add ("scale", Vector3.one);
		htScale.Add ("time", animationDuration);
		htScale.Add ("easetype", iTween.EaseType.easeOutCirc);
		iTween.ScaleTo (parentObject.gameObject, htScale);

//		loader.gameObject.SetActive (true);
//		LobbyAPIManager.GetInstance ().GetSingleGameDetail (gameData.id);

		LobbyAPIManager.singleGameInfoReceived += HandleSingleGameInfoReceived;
	}

	void OnDisable ()
	{
		imgBackground.CrossFadeAlpha (0f, 0f, true);
		parentObject.localScale = Vector3.zero;

		tournamentWinnersDetailPanel.gameObject.SetActive (false);
		tourRegConfirmationPanel.gameObject.SetActive (false);

		tournamentWinnersDetailPanel.gameObject.SetActive (false);
		tournamentRegisteredPlayerPanel.gameObject.SetActive (false);

		LobbyAPIManager.singleGameInfoReceived -= HandleSingleGameInfoReceived;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape) && !UIManager.Instance.loader.gameObject.activeSelf) {
			if (UIManager.Instance.logoutConfirmationPanel.gameObject.activeSelf)
				UIManager.Instance.logoutConfirmationPanel.gameObject.SetActive (false);
			else if (tourRegConfirmationPanel.gameObject.activeSelf)
				tourRegConfirmationPanel.gameObject.SetActive (false);
			else if (tournamentWinnersDetailPanel.gameObject.activeSelf)
				tournamentWinnersDetailPanel.gameObject.SetActive (false);
			else if (tournamentRegisteredPlayerPanel.gameObject.activeSelf)
				tournamentRegisteredPlayerPanel.gameObject.SetActive (false);
			else
				gameObject.SetActive (false);

			SoundManager.Instance.PlayButtonTapSound ();
		}
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleSingleGameInfoReceived (API_SingleGameInfo singleGameInfo)
	{
		if (gameData.id.Equals (singleGameInfo.id)) {
			loader.gameObject.SetActive (false);

			if (singleGameInfo.users.Count > 0) {
			} else {
				txtMessage.text = APIConstants.MESSAGE_NO_PLAYERS_IN_GAME;
			}
		}
	}

	#endregion

	#region PUBLIC_METHODS

	public void DisplayGameDetail (GameData data)
	{
		gameData = data;

		bool isRealMoney = gameData.money_type.Equals ("real money");
		bool isSnG = data.game_type.Equals (APIConstants.SNG_TOUR_GAME_TYPE);

		txtTitle.text = data.game_name;
		if (data.game_type.Equals (APIConstants.REGULAR_TOUR_GAME_TYPE))
			txtTournamentType.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Regular Tournament</color>";
		else if (data.game_type.Equals (APIConstants.SNG_TOUR_GAME_TYPE))
			txtTournamentType.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">SnG Tournament</color>";
		else
			txtTournamentType.text = "";

		if (data.poker_type.Equals (APIConstants.KEY_WHOOPASS))
			txtGameType.text = "WhoopAss";
		else if (data.poker_type.Equals (APIConstants.KEY_TEXASS))
			txtGameType.text = "Texass Holde'm";
		else if (data.poker_type.Equals (APIConstants.KEY_TABLE))
			txtGameType.text = "WhoopAss Table";

		string prizePoolAmount = Utility.GetCommaSeperatedAmount (data.prize_pool);
		if (isRealMoney)
			prizePoolAmount = Utility.GetRealMoneyAmount (data.prize_pool);

		txtPrizePool.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Prize Pool : </color>" + prizePoolAmount;
		txtPlacesPaid.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">3 Places Paid</color>";

		string fee = Utility.GetCommaSeperatedAmount (data.fee + data.entry_fee);
		if (isRealMoney)
			fee = Utility.GetRealMoneyAmount (data.fee + data.entry_fee);
		string buyIn = Utility.GetCommaSeperatedAmount (data.buy_in);
		if (isRealMoney)
			buyIn = Utility.GetRealMoneyAmount (data.buy_in);
		string entryFee = Utility.GetCommaSeperatedAmount (data.fee - data.buy_in);
		if (isRealMoney)
			entryFee = Utility.GetRealMoneyAmount (data.fee - data.buy_in);
		string blindAmount = Utility.GetCommaSeperatedAmount (data.small_blind) + "/" + Utility.GetCommaSeperatedAmount (data.small_blind * 2);
		if (isRealMoney)
			blindAmount = Utility.GetRealMoneyAmount (data.small_blind) + "/" + Utility.GetRealMoneyAmount (data.small_blind * 2);

		txtTournamentDetail.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Fee</color> : " + fee + " (" + buyIn + " Buy-in + " + entryFee + " Entry Fee)";
		txtTournamentDetail.text += "\n<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Start at</color> : " + (isSnG ? "-" : data.start_time);
		txtTournamentDetail.text += "\n<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Remaining Players</color> : " + (data.maximum_players - data.users) + " of " + data.maximum_players;

		txtTournamentMoneyTypeAndLevel.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Money type</color> : " + data.money_type.ToCamelCase ();
		txtTournamentMoneyTypeAndLevel.text += "\n\n<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Current Level</color> : Blinds " + blindAmount;

		txtPlayers.text = "Players";
		txtPerTable.text = data.poker_type.Equals (APIConstants.KEY_WHOOPASS) ? "6 Per Table" : "9 Per Table";
		txtEnrolled.text = data.users + " Enrolled";

		txtAwardsPaid.text = "Winners / Award Paid";
		txtWinnersPlacesPaid.text = "3 Places Paid";

		if (data.status.Equals (APIConstants.TOURNAMENT_STATUS_FINISHED)) {
			btnPlay.gameObject.SetActive (false);
			btnRegister.gameObject.SetActive (false);
		} else {
			if (data.game_type.Equals (APIConstants.SNG_TOUR_GAME_TYPE) && data.joined) {
				btnPlay.gameObject.SetActive (true);
				btnRegister.gameObject.SetActive (false);

				btnPlay.interactable = data.maximum_players == data.users;
			} else {
				if (data.joined) {
					btnPlay.gameObject.SetActive (true);
					btnRegister.gameObject.SetActive (false);

					btnPlay.interactable = true;
				} else {
					btnPlay.gameObject.SetActive (false);

					btnRegister.gameObject.SetActive (data.maximum_players != data.users);
					btnRegister.interactable = data.maximum_players != data.users;
				}
			}
		}

		gameObject.SetActive (true);
	}

	public void UpdateTournamentDetailText (string gameID)
	{
		if (!gameData.id.Equals (gameID) || !gameObject.activeSelf)
			return;

		bool isRealMoney = gameData.money_type.Equals ("real money");
		bool isSnG = gameData.game_type.Equals (APIConstants.SNG_TOUR_GAME_TYPE);

		string fee = Utility.GetCommaSeperatedAmount (gameData.fee + gameData.entry_fee);
		if (isRealMoney)
			fee = Utility.GetRealMoneyAmount (gameData.fee + gameData.entry_fee);
		string buyIn = Utility.GetCommaSeperatedAmount (gameData.buy_in);
		if (isRealMoney)
			buyIn = Utility.GetRealMoneyAmount (gameData.buy_in);
		string entryFee = Utility.GetCommaSeperatedAmount (gameData.fee - gameData.buy_in);
		if (isRealMoney)
			entryFee = Utility.GetRealMoneyAmount (gameData.fee - gameData.buy_in);

		txtTournamentDetail.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Fee</color> : " + fee + " (" + buyIn + " Buy-in + " + entryFee + " Entry Fee)";
		txtTournamentDetail.text += "\n<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Start at</color> : " + (isSnG ? "-" : gameData.start_time);
		txtTournamentDetail.text += "\n<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Remaining Players</color> : " + (gameData.maximum_players - gameData.users) + " of " + gameData.maximum_players;
		txtEnrolled.text = gameData.users + " Enrolled";

		if (gameData.game_type.Equals (APIConstants.SNG_TOUR_GAME_TYPE) && gameData.joined) {
			btnPlay.gameObject.SetActive (true);
			btnRegister.gameObject.SetActive (false);

			btnPlay.interactable = gameData.maximum_players == gameData.users;
		}
	}

	public void UpdatePrizePool (string gameID)
	{
		if (!gameData.id.Equals (gameID) || !gameObject.activeSelf)
			return;

		bool isRealMoney = gameData.money_type.Equals ("real money");

		string prizePoolAmount = Utility.GetCommaSeperatedAmount (gameData.prize_pool);
		if (isRealMoney)
			prizePoolAmount = Utility.GetRealMoneyAmount (gameData.prize_pool);

		txtPrizePool.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Prize Pool : </color>" + prizePoolAmount;

	}

	public void OnCloseButtonTap ()
	{
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnWinnerDetailButtonTap ()
	{
		tournamentWinnersDetailPanel.gameID = gameData.id;
		tournamentWinnersDetailPanel.gameObject.SetActive (true);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnPlayerInfoButtonTap ()
	{
		tournamentRegisteredPlayerPanel.gameID = gameData.id;
		tournamentRegisteredPlayerPanel.isRealMoney = gameData.money_type.Equals ("real money");
		tournamentRegisteredPlayerPanel.gameObject.SetActive (true);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnPlayButtonTap ()
	{
		WebGLLogin loginDetails = new WebGLLogin ();
		loginDetails.buyin = gameData.poker_type.Equals ("table") ? LoginScript.loggedInPlayer.balance_chips.ToString () : "1000";
		loginDetails.GameRoomID = gameData.id;

		loginDetails.GameType = gameData.poker_type.ToUpper () + "_" + (gameData.game_type.Equals (APIConstants.SNG_TOUR_GAME_TYPE) ? "SNG" : "REGULAR");
		loginDetails.game_id = gameData.id;
		loginDetails.game_type = gameData.money_type;
		loginDetails.isLimit = gameData.limit.Equals ("yes") ? "1" : "0";
		loginDetails.speed = gameData.game_speed;
		loginDetails.stake = gameData.small_blind + "/" + gameData.small_blind * 2;
		loginDetails.user_name = LoginScript.loggedInPlayer.name;
		loginDetails.max_player = gameData.maximum_players.ToString ();
		loginDetails.real_money = LoginScript.loggedInPlayer.balance_cash.ToString ();
		loginDetails.play_money = LoginScript.loggedInPlayer.balance_chips.ToString ();

		UIManager.Instance.loginPanel.StartMobileGame (JsonUtility.ToJson (loginDetails));

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnRegisterButtonTap ()
	{
		tourRegConfirmationPanel.gameData = gameData;
		tourRegConfirmationPanel.gameObject.SetActive (true);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion

	#region COROUTINES

	#endregion
}
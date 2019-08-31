using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CashGameDetailPanel : MonoBehaviour
{
	public static CashGameDetailPanel Instance;

	#region PUBLIC_VARIABLES

	public Image imgBackground;
	public Transform parentObject;

	public Text txtTitle;
	public Text txtGameType;
	public Text txtPokerType;
	public Text txtGameInfo;

	public Button btnPlay;

	public GameData gameData;

	public GameObject playerPrefab;
	public RectTransform content;

	public Loader loader;
	public Text txtMessage;

	public BuyInSelectPanel buyInSelectPanel;

	public RectTransform scrollViewContent;

	#endregion

	#region PRIVATE_VARIABLES

	private const float animationDuration = .1f;

	private List<GameObject> playersList;

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Start ()
	{
		Instance = this;
	}

	void OnEnable ()
	{
		txtMessage.text = "";

		imgBackground.CrossFadeAlpha (0f, 0f, true);

		imgBackground.CrossFadeAlpha (1f, animationDuration, true);
		Hashtable htScale = new Hashtable ();
		htScale.Add ("scale", Vector3.one);
		htScale.Add ("time", animationDuration);
		htScale.Add ("easetype", iTween.EaseType.easeOutCirc);
		iTween.ScaleTo (parentObject.gameObject, htScale);

		loader.gameObject.SetActive (true);
		LobbyAPIManager.GetInstance ().GetSingleGameInfo (gameData.id);

		LobbyAPIManager.singleGameInfoReceived += HandleSingleGameInfoReceived;
	}

	void OnDisable ()
	{
		imgBackground.CrossFadeAlpha (0f, 0f, true);
		parentObject.localScale = Vector3.zero;

		buyInSelectPanel.gameObject.SetActive (false);

		LobbyAPIManager.singleGameInfoReceived -= HandleSingleGameInfoReceived;

		DestroyAllPlayers ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape) && !UIManager.Instance.loader.gameObject.activeSelf) {
			if (!UIManager.Instance.messagePanel.gameObject.activeSelf) {
				if (UIManager.Instance.logoutConfirmationPanel.gameObject.activeSelf)
					UIManager.Instance.logoutConfirmationPanel.gameObject.SetActive (false);
				else if (buyInSelectPanel.gameObject.activeSelf)
					buyInSelectPanel.gameObject.SetActive (false);
				else
					gameObject.SetActive (false);

				SoundManager.Instance.PlayButtonTapSound ();
			}
		}
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleSingleGameInfoReceived (API_SingleGameInfo singleGameInfo)
	{
		if (gameData.id.Equals (singleGameInfo.id)) {
			loader.gameObject.SetActive (false);

			playersList = new List<GameObject> ();
			if (singleGameInfo.users.Count > 0) {
				int i = 0;
				foreach (SingleGameInfoPlayer pi in singleGameInfo.users) {
					GameObject obj = Instantiate (playerPrefab) as GameObject;
					obj.transform.SetParent (content);
				
					CashGamePlayerObject p = obj.GetComponent<CashGamePlayerObject> ();
					p.txtName.text = pi.name;

					if (gameData.money_type.Equals (APIConstants.KEY_REAL_MONEY)) {
						p.txtTotalChips.text = Utility.GetCommaSeperatedAmount (pi.balance_cash, true);
						p.txtBuyinAmount.text = Utility.GetCommaSeperatedAmount (pi.buy_in, true);
					} else {
						p.txtTotalChips.text = Utility.GetCommaSeperatedAmount (pi.balance_chips);
						p.txtBuyinAmount.text = Utility.GetCommaSeperatedAmount (pi.buy_in);
					}

					obj.transform.localScale = Vector3.one;

					Color bgColor = new Color ();
					//				ColorUtility.TryParseHtmlString (i % 2 == 0 ? APIConstants.HEX_COLOR_GAME_LIST_ODD_ROW : APIConstants.HEX_COLOR_GAME_LIST_EVEN_ROW, out bgColor);
					bgColor.a = i % 2 == 0 ? 0f : .25f;
					p.imgBackground.color = bgColor;

					playersList.Add (obj);
					i++;
				}

				SetContentHeight (singleGameInfo.users.Count);
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

		txtTitle.text = data.game_name;
		txtGameType.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Cash Game</color>";

		string pokerType = "";
		if (data.poker_type.Equals (APIConstants.KEY_WHOOPASS))
			pokerType = APIConstants.STRING_WHOOPASS_GAME;
		else if (data.poker_type.Equals (APIConstants.KEY_TEXASS))
			pokerType = APIConstants.STRING_TEXASS_GAME;
		else if (data.poker_type.Equals (APIConstants.KEY_TABLE))
			pokerType = APIConstants.STRING_TABLE_GAME;
		txtPokerType.text = pokerType;

		if (data.poker_type.Equals (APIConstants.KEY_TABLE)) {
			txtGameInfo.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Buy-in</color> : " + Utility.GetCommaSeperatedAmount (data.buy_in, data.money_type.Equals (APIConstants.KEY_REAL_MONEY)) +
			"          <color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Stake</color> : --" +
			"          <color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Speed</color> : " + data.game_speed.ToCamelCase () +
			"          <color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Money</color> : " + data.money_type.ToCamelCase ();
		} else {
			txtGameInfo.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Buy-in</color> : " + Utility.GetCommaSeperatedAmount (data.buy_in, data.money_type.Equals (APIConstants.KEY_REAL_MONEY)) +
			"          <color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Stake</color> : " + Utility.GetCommaSeperatedAmount (data.small_blind, data.money_type.Equals (APIConstants.KEY_REAL_MONEY)) + "/" + Utility.GetCommaSeperatedAmount (data.small_blind * 2, data.money_type.Equals (APIConstants.KEY_REAL_MONEY)) +
			"          <color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Speed</color> : " + data.game_speed.ToCamelCase () +
			"          <color=" + APIConstants.HEX_COLOR_RED_HEADER + ">Money</color> : " + data.money_type.ToCamelCase ();
		}

		btnPlay.interactable = true;

		gameObject.SetActive (true);
	}

	public void OnCloseButtonTap ()
	{
		UIManager.Instance.isRealMoney = false;
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnPlayButtonTap ()
	{
		if (gameData.money_type.Equals ("real money") && LoginScript.loggedInPlayer.balance_cash < gameData.buy_in) {
			UIManager.Instance.DisplayMessagePanel ("Insufficient Real Money", "Not enough Real Money in your account to play this game.", null);
			return;
		} else if (!gameData.money_type.Equals ("real money") && LoginScript.loggedInPlayer.balance_chips < gameData.buy_in) {
			UIManager.Instance.DisplayMessagePanel ("Insufficient Chips", "You do not have enough chips to play in this game.", null);
			return;
		}

		buyInSelectPanel.gameData = gameData;

		UIManager.Instance.isRealMoney = gameData.money_type.Equals ("real money");

		double buyIn = gameData.buy_in < 0 ? 0 : gameData.buy_in;

//		if (gameData.poker_type.Equals (APIConstants.KEY_TABLE)) {
//			if (gameData.money_type.Equals ("real money"))
//				buyIn = Constants.TABLE_GAME_REAL_MIN_MONEY;
//			else
//				buyIn = Constants.TABLE_GAME_PLAY_MIN_CHIPS;
//		}

		double maxVal = 0;
		if (gameData.money_type.Equals ("real money"))
			maxVal = LoginScript.loggedInPlayer.balance_cash < 0 ? 0 : LoginScript.loggedInPlayer.balance_cash;
		else
			maxVal = LoginScript.loggedInPlayer.balance_chips < 0 ? 0 : LoginScript.loggedInPlayer.balance_chips;

		if (maxVal < buyIn)
			maxVal = buyIn;

		buyInSelectPanel.sliderBuyin.minValue = (float)buyIn;
		buyInSelectPanel.sliderBuyin.maxValue = (float)maxVal;

		buyInSelectPanel.sliderBuyin.value = (float)gameData.small_blind;

		buyInSelectPanel.gameObject.SetActive (true);

		btnPlay.interactable = false;

		SoundManager.Instance.PlayButtonTapSound ();
	}

	#endregion

	#region PRIVATE_METHODS

	private void DestroyAllPlayers ()
	{
		if (playersList != null)
			foreach (GameObject go in playersList)
				Destroy (go);

		playersList = new List<GameObject> ();
	}

	private void SetContentHeight (int totalObjects)
	{
		scrollViewContent.sizeDelta = new Vector2 (scrollViewContent.sizeDelta.x, totalObjects * 60f);
	}

	#endregion

	#region COROUTINES

	#endregion
}
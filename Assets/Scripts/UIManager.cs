using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
	public bool isBreakTime = false;
	public bool isWebGLBuild = false;
	public bool isRealMoney = false;
	public bool isAffiliate = false;
	public bool isTestingExe = false;

	[Header ("Static Hand")]
	public bool isStaticHand = false;
	public WINNING_RANK staticHand;

	[HideInInspector]
	public int breakTimeTillSeconds = 300;
	[HideInInspector]
	public bool noTablesAtBreaktime = false;

	[Header ("Tournament types")]
	public bool isLimitGame = false;
	public bool isRegularTournament = false;
	public bool isSitNGoTournament = false;

	[Header ("Panels")]
	public RegisterScript registerPanel;
	public LoginScript loginPanel;
	public RoomListScript roomsPanel;
	public CreateRoomScript createRoomPanel;
	public GameManager gamePlayPanel;
	public MainMenuScript mainMenuPanel;
	public SettingsPanel settingsPanel;
	public AccountInfoPanel accountInfoPanel;
	public NewsPanel newsPanel;
	public BottomBarPanel bottomBarPanel;
	public Loading loaderPanel;
	public WinReportPanel winReportPanel;
	public ReconnectingPanel reconnectingPanel;
	public ConnectionFailPanel connectionFailPanel;
	public TexassGame texassGamePanel;
	public BreakTimePanel breakTimePanel;
	public TournamentWinnerPanel tournamentWinnerPanel;
	public WhoopAssGame whoopAssGamePanel;
	public BreakingTablePanel breakingTablePanel;
	public PlayerEliminatedPanel playerEliminatedPanel;

	public AnteAndBlindBetPanel anteAndBlindBetPanel;
	public StraightOrBetterBetPanel straightOrBetterBetPanel;
	public Bet1RoundSelectionPanel bet1RoundSelectionPanel;
	public Bet2RoundSelectionPanel bet2RoundSelectionPanel;
	public Bet3RoundSelectionPanel bet3RoundSelectionPanel;
	public WhoopAssCardRoundPanel whoopAssCardRoundPanel;
	public Bet4RoundSelectionPanel bet4RoundSelectionPanel;

	public NotRegisteredWithTourna notRegisteredWithTournamentPanel;
	public WinnerAnimationPanel winnerAnimationPanel;
	public BackConfirmationPanel backConfirmationPanel;
	public NoEnoughChipsPanel noEnoughChipsPanel;

	public ChatTemplates chatTemplatesPanel;

	public LobbyPanel lobbyPanel;
	public PlayerDetailPanel playerDetailPanel;
	public NoConnectionPanel noConnectionPanel;
	public Loader loader;
	public LogoutConfirmationPanel logoutConfirmationPanel;
	public MaxSitoutPanel maxSitoutPanel;
	public InsufficientChipsEliminationPanel insufficientChipsEliminationPanel;
	public SomethingWrongPanel somethingWrongPanel;
	public RebuyDetailPanel rebuyDetailPanel;
	public UIAccount.DashboardPanel dashboardPanel;

	public RedirectToTournamentPanel tournamentStartingPanel;
	public UtilityMessagePanel messagePanel;
	public HourlymessagePanel PanelHourlymessage;
	[Header ("Game type")]
	public POKER_GAME_TYPE gameType;
	public SERVER server;

	public static UIManager Instance;

	// Use this for initialization
	void Awake ()
	{
		Instance = this;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		Application.ExternalCall ("getAffiliateId");

		if (!UIManager.Instance.isAffiliate) {
			SetAffiliateData ("0");
		}

		Debug.unityLogger.logEnabled = true;
		PanelHourlymessage.gameObject.SetActive (false);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			//  Handle back button here
		} else if (Input.GetKey (KeyCode.LeftControl) && Input.GetKeyDown (KeyCode.F)) {
			GoFullscreen ();
		}
	}

	string ToTitleCase (string stringToConvert)
	{
		string firstChar = stringToConvert [0].ToString ();
		return (stringToConvert.Length > 0 ? firstChar.ToUpper () + stringToConvert.Substring (1) : stringToConvert);
	}

	public void DisplayLoader (string message = "")
	{
		loaderPanel.message.text = message;

		loaderPanel.gameObject.SetActive (true);
	}

	public void HideLoader ()
	{
		loaderPanel.gameObject.SetActive (false);
	}

	public void DisplayReconnectingPanel ()
	{
		reconnectingPanel.gameObject.SetActive (true);
	}

	public void HideReconnectingPanel ()
	{
		reconnectingPanel.gameObject.SetActive (false);
	}
	public void OnGamblingAddictionButtonTap ()
	{
		Application.OpenURL (Constants.gamblingAddictionUrl);
	}
	public void BackToLobbyOnWebsite ()
	{
		NetworkManager.Instance.Disconnect ();
		string tournamentType = "";
		if (!isRegularTournament && !isSitNGoTournament)
			tournamentType = "cashgame";
		else if (isRegularTournament)
			tournamentType = "regulartournament";
		else if (isSitNGoTournament)
			tournamentType = "sngtournament";

		Application.ExternalCall ("BackToLobby", tournamentType);
	}

	public void DisplayNotEnoughChipsPanel (bool canAddChips)
	{
		if (canAddChips) {
			noEnoughChipsPanel.allowAddChips.SetActive (true);
			noEnoughChipsPanel.notAllowAddChips.SetActive (false);
		} else {
			noEnoughChipsPanel.allowAddChips.SetActive (false);
			noEnoughChipsPanel.notAllowAddChips.SetActive (true);

			NetworkManager.Instance.Disconnect ();
		}

		noEnoughChipsPanel.gameObject.SetActive (true);
	}

	public void GoFullscreen ()
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			Screen.fullScreen = !Screen.fullScreen;
//			Application.ExternalCall ("SetFullscreen", 1);
		}
	}

	public void DisplayMessagePanel (string title, string message, UnityAction playerAction = null)
	{
		messagePanel.txtTitle.text = title;
		messagePanel.txtMessage.text = message;
		messagePanel.btnOk.GetComponentInChildren<Text> ().text = "OK";

		messagePanel.btnOk.onClick.RemoveAllListeners ();

		if (playerAction != null)
			messagePanel.btnOk.onClick.AddListener (playerAction);

		messagePanel.gameObject.SetActive (true);
	}

	public void DisplayMessagePanel (string title, string message, string buttonTitle, UnityAction playerAction = null)
	{
		messagePanel.txtTitle.text = title;
		messagePanel.txtMessage.text = message;
		messagePanel.btnOk.GetComponentInChildren<Text> ().text = buttonTitle;

		messagePanel.btnOk.onClick.RemoveAllListeners ();

		if (playerAction != null)
			messagePanel.btnOk.onClick.AddListener (playerAction);

		messagePanel.gameObject.SetActive (true);
	}

	public void CaptureKeyboardInputOn ()
	{
		#if !UNITY_EDITOR && UNITY_WEBGL
		WebGLInput.captureAllKeyboardInput = true;
		#endif
	}

	public void CaptureKeyboardInputOff ()
	{
		#if !UNITY_EDITOR && UNITY_WEBGL
		WebGLInput.captureAllKeyboardInput = false;
		#endif
	}

	public void SetAffiliateData (string affiliateId)
	{
		APIManager.GetInstance ().affiliateId = affiliateId;

		if (UIManager.Instance.isAffiliate)
			StartCoroutine (GetAffiliateDataAtInterval ());

		APIManager.GetInstance ().GetAvailableGame (APIManager.GetInstance ().affiliateId, (www) => {
			Debug.Log ("Affiliate id  : " + APIManager.GetInstance ().affiliateId + "\nAvailable games  : " + www.text);
			if (string.IsNullOrEmpty (www.error)) {
				API_AvailableGames availableGames = JsonUtility.FromJson <API_AvailableGames> (www.text);

				UIManager.Instance.lobbyPanel.RefreshForAffiliateGameUpdate (availableGames);
			}
		});
	}

	private IEnumerator GetAffiliateDataAtInterval ()
	{
		while (true) {
			if (UIManager.Instance.lobbyPanel.gameObject.activeSelf || UIManager.Instance.dashboardPanel.buyChipsPanel.gameObject.activeSelf) {
				APIManager.GetInstance ().GetAvailableGame (APIManager.GetInstance ().affiliateId, (www) => {
					Debug.Log ("Affiliate id  : " + APIManager.GetInstance ().affiliateId + "\nAvailable games  : " + www.text);
					API_AvailableGames availableGames = JsonUtility.FromJson <API_AvailableGames> (www.text);

					UIManager.Instance.lobbyPanel.RefreshForAffiliateGameUpdate (availableGames);
				});
			}

			yield return new WaitForSeconds (10f);
		}
	}

	private API_AvailableGames GetRandomGames ()
	{
		API_AvailableGames ag = new API_AvailableGames ();

		ag.gameType = new System.Collections.Generic.List<string> ();
		ag.moneyType = new System.Collections.Generic.List<string> ();

		if (Random.Range (0, 2) == 0)
			ag.moneyType.Add ("All");
		if (Random.Range (0, 2) == 0)
			ag.moneyType.Add ("Play Money");
		if (Random.Range (0, 2) == 0)
			ag.moneyType.Add ("Real Money");

		if (Random.Range (0, 2) == 0)
			ag.gameType.Add ("All");
		if (Random.Range (0, 2) == 0)
			ag.gameType.Add ("WhoopAss");
		if (Random.Range (0, 2) == 0)
			ag.gameType.Add ("Texas");
		if (Random.Range (0, 2) == 0)
			ag.gameType.Add ("Table");

		return ag;
	}
}

public enum SERVER
{
	LIVE,
	LOCAL_C,
	LOCAL_R
}
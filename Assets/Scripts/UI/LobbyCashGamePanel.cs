using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using BestHTTP.SocketIO;

public class LobbyCashGamePanel : MonoBehaviour
{
	public static LobbyCashGamePanel Instance;

	#region PUBLIC_VARIABLES

	public GameObject objPrefab;

	public RectTransform scrollViewContent;

	public CashGameDetailPanel cashGameDetailPanel;
	public Text txtMessage;

	public Loader loaderPanel;

	public API_GameInfo gameInfo;

	#endregion

	#region PRIVATE_VARIABLES

	private List<GameObject> cashGameObjectList;

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

		PokerSocketManager.onJoinCashGame += HandleOnJoinCashGame;
		PokerSocketManager.onLeaveCashGame += HandleOnLeaveCashGame;
		PokerSocketManager.onCashGameStackUpdated += HandleOnCashGameStackUpdated;
	}

	void OnDisable ()
	{
		cashGameDetailPanel.gameObject.SetActive (false);

		DestroyAllObjects ();
		
		PokerSocketManager.onJoinCashGame -= HandleOnJoinCashGame;
		PokerSocketManager.onLeaveCashGame -= HandleOnLeaveCashGame;
		PokerSocketManager.onCashGameStackUpdated -= HandleOnCashGameStackUpdated;
	}

	#endregion

	#region PUBLIC_METHODS

	public void SetCashGameObjects (API_GameInfo games)
	{
		DestroyAllObjects ();
		gameInfo = games;

		cashGameObjectList = new List<GameObject> ();

		if (games.gameList.Count > 0) {
			txtMessage.text = "";

			for (int i = 0; i < games.gameList.Count; i++) {
				GameObject o = Instantiate (objPrefab) as GameObject;
				o.transform.SetParent (scrollViewContent);
				o.transform.localScale = Vector3.one;

				cashGameObjectList.Add (o);

				o.transform.position = new Vector3 (o.transform.position.x, o.transform.position.y, scrollViewContent.position.z);

				CashGameObject cgo = o.GetComponent<CashGameObject> ();
				cgo.txtName.text = games.gameList [i].name;
				cgo.txtBuyin.text = Utility.GetCommaSeperatedAmount (games.gameList [i].buy_in, games.gameList [i].money_type.Equals (APIConstants.KEY_REAL_MONEY));
				if (games.gameList [i].poker_type.Equals (APIConstants.KEY_TABLE)) {
//					double buyIn = games.gameList [i].money_type.Equals (APIConstants.KEY_PLAY_MONEY) ? Constants.TABLE_GAME_PLAY_MIN_CHIPS : Constants.TABLE_GAME_REAL_MIN_MONEY;
//					cgo.txtBuyin.text = "" + buyIn;
					cgo.txtStake.text = "--/--";
				} else {
					cgo.txtStake.text = Utility.GetCommaSeperatedAmount (games.gameList [i].small_blind, games.gameList [i].money_type.Equals (APIConstants.KEY_REAL_MONEY)) + "/" + Utility.GetCommaSeperatedAmount (games.gameList [i].small_blind * 2, games.gameList [i].money_type.Equals (APIConstants.KEY_REAL_MONEY));
				}
				cgo.txtPlayers.text = games.gameList [i].users + "/" + games.gameList [i].maximum_players;

				Color bgColor = new Color ();
				bgColor.a = i % 2 == 0 ? 0f : .25f;
				cgo.imgBackground.color = bgColor;

				cgo.gameID = games.gameList [i].id;

				cgo.index = i;
			}
		

		} else {
			txtMessage.text = APIConstants.MESSAGE_NO_GAMES_FOUND;
		}

		SetContentHeight (games.gameList.Count);

		gameObject.SetActive (true);
	}

	#endregion

	#region PRIVATE_METHODS

	private void SetContentHeight (int totalObjects)
	{
		scrollViewContent.sizeDelta = new Vector2 (scrollViewContent.sizeDelta.x, totalObjects * 50f);
	}

	private void DestroyAllObjects ()
	{
		if (cashGameObjectList == null)
			return;
		
		foreach (GameObject o in cashGameObjectList)
			Destroy (o);
	}

	private void UpdatePlayersCount (string gameID, int totalPlayers)
	{
		totalPlayers = totalPlayers < 0 ? 0 : totalPlayers;

		foreach (GameObject go in cashGameObjectList) {
			CashGameObject cgo = go.GetComponent<CashGameObject> ();
//			DebugLog.Log (".... "+gameID +" == "+cgo.gameID);
			if (cgo.gameID.Equals (gameID)) {
				GameData gd = gameInfo.gameList [cgo.index];
				gd.users = totalPlayers;
				cgo.txtPlayers.text = gd.users + "/" + gd.maximum_players; 
//				DebugLog.Log (".... "+gameID +" == "+cgo.txtPlayers.text.ToString());
				break;
			}
		}
	}

	private void UpdateStack (string gameID, string stack)
	{
		foreach (GameObject go in cashGameObjectList) {
			CashGameObject cgo = go.GetComponent<CashGameObject> ();

			if (cgo.gameID.Equals (gameID)) {
				GameData gd = gameInfo.gameList [cgo.index];

				long smallBlind = long.Parse (stack.Split ('/') [0]);
				gd.small_blind = smallBlind;

				cgo.txtStake.text = Utility.GetCommaSeperatedAmount (smallBlind) + "/" + Utility.GetCommaSeperatedAmount (smallBlind * 2);
			}
		}
	}

	#endregion

	private void HandleOnJoinCashGame (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");

			DebugLog.Log (data);

			JSON_Object obj = new JSON_Object (data);
			string gameID = obj.getString ("game_id");
			int totalPlayers = obj.getInt ("user_count");

			UpdatePlayersCount (gameID, --totalPlayers);
		} catch (Exception e) {
			Debug.Log ("Exception  : " + e);
		}
	}

	private void HandleOnLeaveCashGame (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");

			DebugLog.Log (data);

			JSON_Object obj = new JSON_Object (data);
			string gameID = obj.getString ("game_id");
			int totalPlayers = obj.getInt ("user_count");

			UpdatePlayersCount (gameID, --totalPlayers);
		} catch (Exception e) {
			Debug.Log ("Exception  : " + e);
		}
	}

	private void HandleOnCashGameStackUpdated (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");

			DebugLog.Log (data);

			JSON_Object obj = new JSON_Object (data);
			string gameID = obj.getString ("game_id");
			string stack = obj.getString ("stack");
			UpdateStack (gameID, stack);
		} catch (Exception e) {
			Debug.Log ("Exception  : " + e);
		}
	}

	#region COROUTINES

	#endregion
}
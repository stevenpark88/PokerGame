using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using BestHTTP.SocketIO;

public class LobbySngTournamentPanel : MonoBehaviour
{
	public static LobbySngTournamentPanel Instance;

	#region PUBLIC_VARIABLES

	public GameObject objPrefab;

	public RectTransform scrollViewContent;

	public TournamentDetailPanel tournamentDetailPanel;
	public Text txtMessage;

	public Loader loaderPanel;

	public API_GameInfo gameInfo;

	#endregion

	#region PRIVATE_VARIABLES

	private List<GameObject> sngTournamentObjectList;

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

		PokerSocketManager.onSnGTournamentStarted += HandleOnSnGTournamentStarted;
		PokerSocketManager.onSnGStackUpdated += HandleOnSnGGameStackUpdated;
		PokerSocketManager.onPlayerEliminate += HandleOnSnGPlayerEliminated;
		PokerSocketManager.onPlayerJoinedSnGTournament += HandleOnPlayerJoinedSnGTournament;
		PokerSocketManager.onSnGLevelUpdated += HandleOnSnGLevelUpdated;
	}

	void OnDisable ()
	{
		tournamentDetailPanel.gameObject.SetActive (false);

		DestroyAllObjects ();

		PokerSocketManager.onSnGTournamentStarted -= HandleOnSnGTournamentStarted;
		PokerSocketManager.onSnGStackUpdated -= HandleOnSnGGameStackUpdated;
		PokerSocketManager.onPlayerEliminate -= HandleOnSnGPlayerEliminated;
		PokerSocketManager.onPlayerJoinedSnGTournament -= HandleOnPlayerJoinedSnGTournament;
		PokerSocketManager.onSnGLevelUpdated -= HandleOnSnGLevelUpdated;
	}

	#endregion

	#region PUBLIC_METHODS

	public void SetSngTournamentObjects (API_GameInfo games)
	{
		DestroyAllObjects ();
		gameInfo = games;

		sngTournamentObjectList = new List<GameObject> ();

		if (games.gameList.Count > 0) {
			txtMessage.text = "";
			for (int i = 0; i < games.gameList.Count; i++) {
				GameObject o = Instantiate (objPrefab) as GameObject;
				o.transform.SetParent (scrollViewContent);
				o.transform.localScale = Vector3.one;

				sngTournamentObjectList.Add (o);

				o.transform.position = new Vector3 (o.transform.position.x, o.transform.position.y, scrollViewContent.position.z);

				SnGTournamentObject sto = o.GetComponent<SnGTournamentObject> ();
				sto.txtName.text = games.gameList [i].game_name;
				sto.txtBuyin.text = Utility.GetCommaSeperatedAmount (games.gameList [i].buy_in);
				sto.txtPrizePool.text = Utility.GetCommaSeperatedAmount (games.gameList [i].prize_pool);
				sto.txtStatus.text = games.gameList [i].status.ToCamelCase ();
				sto.txtEnrolled.text = games.gameList [i].users + "/" + games.gameList [i].maximum_players; 
				sto.gameID = games.gameList [i].id;

				sto.imgBackground.color = new Color (0, 0, 0, i % 2 == 0 ? .25f : 0f);

				if (games.gameList [i].status.Equals (APIConstants.TOURNAMENT_STATUS_REGISTERING))
					sto.imgTournamentStatus.color = Color.green;
				else if (games.gameList [i].status.Equals (APIConstants.TOURNAMENT_STATUS_PENDING) ||
				         games.gameList [i].status.Equals (APIConstants.TOURNAMENT_STATUS_RUNNING))
					sto.imgTournamentStatus.color = Color.yellow;
				else
					sto.imgTournamentStatus.color = Color.red;

				sto.index = i;
			}
		} else {
			txtMessage.text = APIConstants.MESSAGE_NO_GAMES_FOUND;
		}

//		SetContentHeight (games.gameList.Count);

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
		if (sngTournamentObjectList == null)
			return;

		foreach (GameObject o in sngTournamentObjectList)
			Destroy (o);
	}

	private void UpdateTournamentStatusToRunning (string gameID)
	{
		foreach (GameObject go in sngTournamentObjectList) {
			SnGTournamentObject sto = go.GetComponent<SnGTournamentObject> ();

			if (sto.gameID.Equals (gameID)) {
				GameData gd = gameInfo.gameList [sto.index];
				gd.status = APIConstants.TOURNAMENT_STATUS_RUNNING;

				sto.txtStatus.text = gd.status.ToCamelCase ();
				sto.imgTournamentStatus.color = Color.green;
			}
		}
	}

	private void UpdateTournamentStack (string gameID, string stack)
	{
		foreach (GameObject go in sngTournamentObjectList) {
			SnGTournamentObject sto = go.GetComponent<SnGTournamentObject> ();

			if (sto.gameID.Equals (gameID)) {
//				sto.imgTournamentStatus.color = Color.green;
			}
		}
	}

	private void UpdatePlayersCount (string gameID, int totalPlayers)
	{
		totalPlayers = totalPlayers < 0 ? 0 : totalPlayers;

		foreach (GameObject go in sngTournamentObjectList) {
			SnGTournamentObject sto = go.GetComponent<SnGTournamentObject> ();

			if (sto.gameID.Equals (gameID)) {
				GameData gd = gameInfo.gameList [sto.index];
				gd.users = totalPlayers;

				sto.txtEnrolled.text = gd.users + "/" + gd.maximum_players; 
			}
		}

		tournamentDetailPanel.UpdateTournamentDetailText (gameID);
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleOnSnGTournamentStarted (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());

			JSON_Object obj = arr.getJSONObject (1).getJSONObject ("data");
			string gameID = obj.getString ("game_id");

			UpdateTournamentStatusToRunning (gameID);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnSnGGameStackUpdated (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());

			JSON_Object obj = arr.getJSONObject (1).getJSONObject ("data");
			string gameID = obj.getString ("game_id");
			string stack = obj.getString ("stack");

			UpdateTournamentStack (gameID, stack);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnSnGPlayerEliminated (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());

			JSON_Object obj = arr.getJSONObject (1).getJSONObject ("data");
			string gameID = obj.getString ("game_id");
			int totalPlayers = obj.getInt ("user_count");

			UpdatePlayersCount (gameID, totalPlayers);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnPlayerJoinedSnGTournament (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());

			JSON_Object obj = arr.getJSONObject (1).getJSONObject ("data");
			JSON_Object gameObj = new JSON_Object (obj.getString ("game"));
			JSONArray playersArr = new JSONArray (gameObj.getString ("users"));
			int totalPlayers = playersArr.Count ();

			UpdatePlayersCount (gameObj.getString ("id"), totalPlayers);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnSnGLevelUpdated (Packet packet)
	{
		
	}

	#endregion

	#region COROUTINES

	#endregion
}
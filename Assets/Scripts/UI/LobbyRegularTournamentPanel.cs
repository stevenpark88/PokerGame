using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using BestHTTP.SocketIO;

public class LobbyRegularTournamentPanel : MonoBehaviour
{
	public static LobbyRegularTournamentPanel Instance;

	#region PUBLIC_VARIABLES

	public GameObject objPrefab;

	public RectTransform scrollViewContent;

	public TournamentDetailPanel tournamentDetailPanel;
	public Text txtMessage;

	public Loader loaderPanel;

	public API_GameInfo gameInfo;

	#endregion

	#region PRIVATE_VARIABLES

	private List<GameObject> regularTournamentObjectList;

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

		PokerSocketManager.onRegularTournamentStarted += HandleOnRegularTournamentStarted;
		PokerSocketManager.onPlayerEliminate += HandleOnPlayerEliminated;
		PokerSocketManager.onPrizePoolUpdated += HandleOnPrizePoolUpdated;
		PokerSocketManager.onPlayerJoinedRegularTournament += HandleOnPlayerJoinedRegularTournament;
		PokerSocketManager.onRegularTournamentFinished += HandleOnRegularTournamentFinished;
	}

	void OnDisable ()
	{
		tournamentDetailPanel.gameObject.SetActive (false);

		DestroyAllObjects ();

		PokerSocketManager.onRegularTournamentStarted -= HandleOnRegularTournamentStarted;
		PokerSocketManager.onPlayerEliminate -= HandleOnPlayerEliminated;
		PokerSocketManager.onPrizePoolUpdated -= HandleOnPrizePoolUpdated;
		PokerSocketManager.onPlayerJoinedRegularTournament -= HandleOnPlayerJoinedRegularTournament;
		PokerSocketManager.onRegularTournamentFinished -= HandleOnRegularTournamentFinished;
	}

	#endregion

	#region PUBLIC_METHODS

	public void SetRegularTournamentObjects (API_GameInfo games)
	{
		DestroyAllObjects ();
		gameInfo = games;

		regularTournamentObjectList = new List<GameObject> ();

		if (games.gameList.Count > 0) {
			txtMessage.text = "";

			for (int i = 0; i < games.gameList.Count; i++) {
				GameObject o = Instantiate (objPrefab) as GameObject;
				o.transform.SetParent (scrollViewContent);
				o.transform.localScale = Vector3.one;

				regularTournamentObjectList.Add (o);

				o.transform.position = new Vector3 (o.transform.position.x, o.transform.position.y, scrollViewContent.position.z);

				RegularTournamentObject rto = o.GetComponent<RegularTournamentObject> ();
				rto.txtStartDate.text = games.gameList [i].start_time;
				rto.txtName.text = games.gameList [i].game_name;
				rto.txtBuyin.text = Utility.GetCommaSeperatedAmount (games.gameList [i].buy_in);
				rto.txtPrizePool.text = Utility.GetCommaSeperatedAmount (games.gameList [i].prize_pool);
				rto.txtStatus.text = games.gameList [i].status.ToCamelCase ();
				rto.txtEnrolled.text = games.gameList [i].users.ToString ();
				rto.gameID = games.gameList [i].id;

				rto.imgBackground.color = new Color (0, 0, 0, i % 2 == 0 ? 0f : .25f);

				if (games.gameList [i].status.Equals (APIConstants.TOURNAMENT_STATUS_REGISTERING))
					rto.imgTournamentStatus.color = Color.green;
				else if (games.gameList [i].status.Equals (APIConstants.TOURNAMENT_STATUS_PENDING) ||
				         games.gameList [i].status.Equals (APIConstants.TOURNAMENT_STATUS_RUNNING))
					rto.imgTournamentStatus.color = Color.yellow;
				else
					rto.imgTournamentStatus.color = Color.red;

				rto.index = i;
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
		if (regularTournamentObjectList == null)
			return;

		foreach (GameObject o in regularTournamentObjectList)
			Destroy (o);
	}

	private void UpdateTournamentStatusToRunning (string gameID)
	{
		foreach (GameObject go in regularTournamentObjectList) {
			RegularTournamentObject rto = go.GetComponent<RegularTournamentObject> ();
			
			if (rto.gameID.Equals (gameID)) {
				rto.txtStatus.text = APIConstants.TOURNAMENT_STATUS_RUNNING;
				rto.imgTournamentStatus.color = Color.green;
			}
		}
	}

	private void UpdateTournamentStatusToFinished (string gameID)
	{
		foreach (GameObject go in regularTournamentObjectList) {
			RegularTournamentObject rto = go.GetComponent<RegularTournamentObject> ();

			if (rto.gameID.Equals (gameID)) {
				rto.txtStatus.text = APIConstants.TOURNAMENT_STATUS_FINISHED;
				rto.imgTournamentStatus.color = Color.red;
			}
		}
	}

	private void UpdatePlayersCount (string gameID, int totalPlayers)
	{
		totalPlayers = totalPlayers < 0 ? 0 : totalPlayers;

		foreach (GameObject go in regularTournamentObjectList) {
			RegularTournamentObject sto = go.GetComponent<RegularTournamentObject> ();

			if (sto.gameID.Equals (gameID)) {
				GameData gd = gameInfo.gameList [sto.index];
				gd.users = totalPlayers;

				sto.txtEnrolled.text = gd.users + "/" + gd.maximum_players; 
			}
		}

		tournamentDetailPanel.UpdateTournamentDetailText (gameID);
	}

	private void UpdatePrizePool (string gameID, long prizePool)
	{
		foreach (GameObject go in regularTournamentObjectList) {
			RegularTournamentObject sto = go.GetComponent<RegularTournamentObject> ();

			if (sto.gameID.Equals (gameID)) {
				GameData gd = gameInfo.gameList [sto.index];
				gd.prize_pool = prizePool;
			}
		}

		tournamentDetailPanel.UpdatePrizePool (gameID);
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleOnRegularTournamentStarted (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");
//			data = data.Remove (0, 1);
//			data = data.Remove (data.Length - 1, 1);

			Debug.Log (data);

			JSON_Object obj = new JSON_Object (data);
			string gameID = obj.getString ("game_id");

			UpdateTournamentStatusToRunning (gameID);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnPlayerEliminated (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");

			Debug.Log (data);

			JSON_Object obj = new JSON_Object (data);
			string gameID = obj.getString ("game_id");
			int totalPlayers = obj.getInt ("user_count");

			UpdatePlayersCount (gameID, totalPlayers);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnPrizePoolUpdated (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");

			Debug.Log (data);

			JSON_Object obj = new JSON_Object (data);
			string gameID = obj.getString ("game_id");
			long prizePool = obj.getLong ("prize_pool");

			UpdatePrizePool (gameID, prizePool);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnPlayerJoinedRegularTournament (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");

			Debug.Log (data);

			JSON_Object obj = new JSON_Object (data);
			JSON_Object gameObj = new JSON_Object (obj.getString ("game"));
			JSONArray playersArr = new JSONArray (gameObj.getString ("users"));
			int totalPlayers = playersArr.Count ();

			UpdatePlayersCount (gameObj.getString ("id"), totalPlayers);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	private void HandleOnRegularTournamentFinished (Packet packet)
	{
		try {
			JSONArray arr = new JSONArray (packet.ToString ());
			JSON_Object dataObj = arr.getJSONObject (1);

			string data = dataObj.getString ("data");

			Debug.Log (data);

			JSON_Object obj = new JSON_Object (data);
			string gameID = obj.getString ("game_id");

			UpdateTournamentStatusToFinished (gameID);
		} catch (Exception e) {
			Debug.LogError ("> Exception  : " + e);
		}
	}

	#endregion

	#region COROUTINES

	#endregion
}
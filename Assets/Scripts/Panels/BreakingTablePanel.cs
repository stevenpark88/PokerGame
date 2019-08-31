using UnityEngine;
using System.Collections;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client;
using UnityEngine.UI;

public class BreakingTablePanel : MonoBehaviour
{
	public string breakingTableNumber = "";
	public Text txtMessage;
	public bool willBreakTable = false;

	void OnEnable()
	{
		UIManager.Instance.breakTimePanel.gameObject.SetActive (false);

		txtMessage.text = Constants.MESSAGE_BREAKING_TABLE;
		StartCoroutine (GetRoomAfterSometime());

		NetworkManager.onRoomListFetched += HandleOnRoomListFetched;
		NetworkManager.onRoomCreationDone += HandleOnRoomCreationDone;
		NetworkManager.onPlayerSubscribedRoom += HandleOnPlayerSubscribedRoom;

		StartCoroutine(ChangeColor());

		willBreakTable = false;
	}

	void OnDisable()
	{
		NetworkManager.onRoomListFetched -= HandleOnRoomListFetched;
		NetworkManager.onRoomCreationDone -= HandleOnRoomCreationDone;
		NetworkManager.onPlayerSubscribedRoom -= HandleOnPlayerSubscribedRoom;
	}

	private void HandleOnRoomListFetched(MatchedRoomsEvent matchedRoom)
	{
		if (matchedRoom.getRoomsData() != null) {
			if (matchedRoom.getRoomsData().Length == 0)
			{
				Dictionary<string, object> dict = new Dictionary<string, object>();
				dict.Add("ROOM_NAME", Constants.GameID + "_" + breakingTableNumber);
				dict.Add("room_name", "room_name");
				if (LoginScript.loginDetails != null) {
					dict.Add (Constants.ROOM_PROP_ROOM_NAME, LoginScript.loginDetails.game_id);
					dict.Add (Constants.ROOM_PROP_GAME_ROOM_ID, LoginScript.loginDetails.game_id);
				} else {
					dict.Add (Constants.ROOM_PROP_ROOM_NAME, Constants.GameID);
					dict.Add (Constants.ROOM_PROP_GAME_ROOM_ID, Constants.GameID);
				}
				dict.Add (Constants.ROOM_PROP_TABLE_NUMBER, breakingTableNumber);

				if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
					NetworkManager.Instance.CreateRoom(Constants.GameID, Constants.TABLE_GAME_MAX_PLAYERS, dict);
				else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS)
					NetworkManager.Instance.CreateRoom(Constants.GameID, Constants.TEXASS_GAME_MAX_PLAYERS, dict);
				else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
					NetworkManager.Instance.CreateRoom(Constants.GameID, Constants.WHOOPASS_GAME_MAX_PLAYERS, dict);
			}
			else
			{
				NetworkManager.Instance.SubscribeRoom(matchedRoom.getRoomsData()[0].getId());
			}
		}
	}

	private void HandleOnRoomCreationDone(RoomEvent roomEvent)
	{
		if (roomEvent.getResult() == WarpResponseResultCode.SUCCESS) {
			NetworkManager.Instance.SubscribeRoom(roomEvent.getData().getId());
		}
	}

	private void HandleOnPlayerSubscribedRoom(RoomEvent roomEvent)
	{
		if (roomEvent.getResult() == WarpResponseResultCode.SUCCESS)
		{
			NetworkManager.Instance.SetPlayerCustomData();
		}
	}

	private IEnumerator GetRoomAfterSometime()
	{
		if (NetworkManager.Instance.playerID.Equals ("Player 1"))
			yield return 0;
		else
			yield return new WaitForSeconds (5f);


		Dictionary<string, object> dict = new Dictionary<string, object> ();
		dict.Add ("room_name", "room_name");
		dict.Add (Constants.ROOM_PROP_TABLE_NUMBER, breakingTableNumber);
		if (LoginScript.loginDetails != null)
			dict.Add (Constants.ROOM_PROP_GAME_ROOM_ID, LoginScript.loginDetails.game_id);
		else
			dict.Add (Constants.ROOM_PROP_GAME_ROOM_ID, Constants.GameID);

		Debug.Log(Constants.REMOVE_BEFORE_LIVE);
		dict.Add(Constants.ROOM_PROP_GAME_TYPE, UIManager.Instance.gameType);

		if (UIManager.Instance.isRegularTournament)
			dict.Add(Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR_TOURNAMENT);
		else if (UIManager.Instance.isSitNGoTournament)
			dict.Add(Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.SIT_N_GO_TOURNAMENT);
		else
			dict.Add(Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR);

		WarpClient.GetInstance ().GetRoomWithProperties (dict);
	}

	private IEnumerator ChangeColor()
	{
		while (true)
		{
			float r = Random.Range(0f, 1f);
			float g = Random.Range(0f, 1f);
			float b = Random.Range(0f, 1f);
			txtMessage.color = new Color(r, g, b, 1);

			yield return new WaitForSeconds(.2f) ;
		}
	}
}
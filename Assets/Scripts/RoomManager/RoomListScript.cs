using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using UnityEngine.SceneManagement;

public class RoomListScript : MonoBehaviour
{
	public Text txtTitle;

	public Button btnCreateRoom;
	public GameObject roomListPanel;
	public GameObject roomPanel;
	public GameObject waRoomPanel;
	public GameObject createRoomPanel;
	public GameObject mainMenuPanel;

	public GameObject gameTypeParentObj;

	public GameObject btnLimit;
	public GameObject btnNoLimit;

	public bool roomFound = false;

	public List<TABLE> tablesList;

	public ScrollList scrollList;

	public Button joinRoomButton;

	public Text txtMessage;

	public string selectedRoomID;

	// Use this for initialization
	void Start ()
	{
		tablesList = new List<TABLE> ();

		if (appwarp.isLimitGame) {
			btnLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX_SELECTED);
			btnNoLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX);
		} else {
			btnLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX);
			btnNoLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX_SELECTED);
		}
	}

	void OnEnable ()
	{
		OnRefreshButtonTap ();

		joinRoomButton.interactable = false;

		NetworkManager.onRoomListFetched += HandleOnRoomListReceived;
		NetworkManager.onPlayerSubscribedRoom += HandleOnPlayerSubscribedRoom;
		NetworkManager.onPlayerJoinedRoom += HandleOnPlayerJoinedRoom;

		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
			gameTypeParentObj.SetActive (false);
		else
			gameTypeParentObj.SetActive (true);

		SetTitle ();
	}

	void OnDisable ()
	{
		NetworkManager.onRoomListFetched -= HandleOnRoomListReceived;
		NetworkManager.onPlayerSubscribedRoom -= HandleOnPlayerSubscribedRoom;
		NetworkManager.onPlayerJoinedRoom -= HandleOnPlayerJoinedRoom;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.F5))
			OnRefreshButtonTap ();
	}

	public void onClickLimitGame ()
	{
		btnLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX_SELECTED);
		btnNoLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX);
		appwarp.isLimitGame = true;
		UIManager.Instance.isLimitGame = true;
	}

	public void onClickNoLimitGame ()
	{
		btnLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX);
		btnNoLimit.transform.GetComponent<Image> ().sprite = Resources.Load<Sprite> (GameConstant.RES_PATH_CHECK_BOX_SELECTED);
		appwarp.isLimitGame = false;
		UIManager.Instance.isLimitGame = false;
	}

	public void loadAvailableRoomsOnZone (LiveRoomInfoEvent liveRoomInfoEvent)
	{
		appwarp.currentRoomId = liveRoomInfoEvent.getData ().getId ();
		TABLE table = new TABLE ();

		Dictionary<string, object> rumData = liveRoomInfoEvent.getProperties ();
		if (rumData.ContainsKey ("ROOM_NAME")) {
			table.roomName = rumData ["ROOM_NAME"].ToString ();
		}

		if (rumData.ContainsKey ("MINCHIPS")) {
			table.minChips = rumData ["MINCHIPS"].ToString ();
		}

		if (liveRoomInfoEvent.getJoinedUsers () != null)
			table.totalUsers = liveRoomInfoEvent.getJoinedUsers ().Length;
		table.maxUsers = 9;

		Debug.Log (">>> " + liveRoomInfoEvent.getJoinedUsers () + "... " + liveRoomInfoEvent.getData ().getId ());

		table.roomId = liveRoomInfoEvent.getData ().getId ();
		//tablesList.Add (table);

		GameObject.FindGameObjectWithTag ("RoomListPanel").GetComponentInChildren<ScrollList> ().Init (table);

//		Dictionary<string, object> data = eventObj.getProperties();
//		tabl = new TABLE();
//		if(data.ContainsKey("MINCHIPS"))
//		{
//			tabl.minChips = data["MINCHIPS"].ToString();
//			
//		}
//		if(data.ContainsKey("ROOMNAME"))
//		{
//			tabl.roomName = data["ROOMNAME"].ToString();
//		}
//		tabl.roomId = eventObj.getData().getId();
//		rumManager.tables.Add(tabl);



//		if(txtAllRooms.text.Contains("Loading")){
//			txtAllRooms.text = "";
//		}
//		appwarp.currentRoomId = liveRoomInfoEvent.getData ().getId ();
//		txtSelectedRoom.text = "Selected Room : "+liveRoomInfoEvent.getData ().getName ();
//
//		txtAllRooms.text= txtAllRooms.text + "\n"+liveRoomInfoEvent.getData().getName();
	}

	public void onCreateRoomRequest ()
	{
		createRoomPanel.SetActive (true);
		roomListPanel.SetActive (false);
	}

	public void onJoinRoomRequest ()
	{
		NetworkManager.Instance.SubscribeRoom (selectedRoomID);

		joinRoomButton.interactable = false;
	}

	public void onJoinRoomSuccessfully (RoomEvent roomEvent)
	{
		gameObject.SetActive (false);
	}

	void OnPointerClick ()
	{
		Debug.Log ("click");
	}

	public void OnBackBtnClick ()
	{
		roomListPanel.SetActive (false);
		mainMenuPanel.SetActive (true);
	}

    public void OnHomeButtonTap()
    {
		LoginScript.loginDetails = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	public void OnToggleValueChanged (Toggle toggleRoomSelector)
	{
		Color c = toggleRoomSelector.GetComponent<Image> ().color;

		if (toggleRoomSelector.isOn) {
			selectedRoomID = toggleRoomSelector.GetComponent<JoinRoomRow> ().roomid;
			Constants.GameID = toggleRoomSelector.GetComponent<JoinRoomRow> ().roomNameText.text;
			joinRoomButton.interactable = true;
			c.a = 1f;
		} else
			c.a = .25f;

		toggleRoomSelector.GetComponent<Image> ().color = c;
	}


	private void HandleOnRoomListReceived (MatchedRoomsEvent matchedRoom)
	{
		roomFound = false;
		if (matchedRoom.getRoomsData () == null) {
			txtMessage.color = Color.red;
			txtMessage.text = Constants.MESSAGE_NO_TABLE;
            CancelInvoke("OnRefreshButtonTap");
			Invoke ("OnRefreshButtonTap", 5f);
			return;
		} else if (matchedRoom.getRoomsData ().Length < 1) {
			txtMessage.color = Color.red;
			txtMessage.text = Constants.MESSAGE_NO_TABLE;
			CancelInvoke("OnRefreshButtonTap");
			Invoke ("OnRefreshButtonTap", 5f);
			return;
		}

		roomFound = true;
		txtMessage.color = Color.green;
		txtMessage.text = Constants.MESSAGE_SELECT_TABLE;
		scrollList.InstantiateRoom (matchedRoom);
	}

	private void HandleOnPlayerSubscribedRoom (RoomEvent roomEvent)
	{
		if (roomEvent.getResult () == WarpResponseResultCode.SUCCESS) {
			NetworkManager.Instance.SetPlayerCustomData ();
		}
	}

	private void HandleOnPlayerJoinedRoom (RoomEvent roomEvent, string playerName)
	{
		if (roomEvent.getResult () == WarpResponseResultCode.SUCCESS)
			onJoinRoomSuccessfully (roomEvent);
	}

	public void OnRefreshButtonTap ()
	{
		Dictionary<string, object> dict = new Dictionary<string, object> ();
		dict.Add ("room_name", "room_name");

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

	public void DisableJoinButtonOnRefresh()
	{
		joinRoomButton.interactable = false;
	}

	private void SetTitle()
	{
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
			txtTitle.text = "WhoopAss Poker - Table Game";
		else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS)
			txtTitle.text = "Texas Hold'em Poker";
		else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
			txtTitle.text = "WhoopAss Poker";
	}
}
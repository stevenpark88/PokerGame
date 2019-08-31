using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using com.shephertz.app42.gaming.multiplayer.client;
using UnityEngine.SceneManagement;

public class CreateRoomScript : MonoBehaviour
{
	public Text txtTitle;

	MenuHandler menuHandler;
	public Slider goChipsSlider;
	public Text minChipsTxt;
	public InputField roomNameTxt;
	public GameObject createRoomPanel;
	public GameObject roomListPanel;
    public Text txtErrorMsg;

	// Use this for initialization
	void Start ()
	{
		menuHandler = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<MenuHandler> ();
	}

	void OnEnable ()
	{
		roomNameTxt.text = "";
		goChipsSlider.value = goChipsSlider.minValue;
        txtErrorMsg.gameObject.SetActive(false);

		SetTitle ();
	}


	public void OnValueChange (float value)
	{
		minChipsTxt.text = goChipsSlider.value.ToString ();
	}


	public void onCreateBtnClick ()
	{
        if (!string.IsNullOrEmpty(roomNameTxt.text) && roomNameTxt.text.Trim().Length != 0)
        {
			string roomName = roomNameTxt.text.Trim ();
            CreateRoomWithName(roomName);
            txtErrorMsg.gameObject.SetActive(false);
        }
        else
        {
            txtErrorMsg.gameObject.SetActive(true);
        }
    }

	public void OnBackBtnClick ()
	{
		createRoomPanel.SetActive (false);
		roomListPanel.SetActive (true);
	}

    private void CreateRoomWithName(string roomName)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("ROOM_NAME", roomName);
        dict.Add("MINCHIPS", minChipsTxt.text);
		dict.Add("room_name", "room_name");
        if (LoginScript.loginDetails != null)
            dict.Add(Constants.ROOM_PROP_ROOM_NAME, LoginScript.loginDetails.game_id);

        //		WarpClient.GetInstance ().CreateTurnRoom (roomNameTxt.text, "Owner_" + roomNameTxt.text, 
        //			9, dict, GameConstant.TURN_TIME);

        if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
            NetworkManager.Instance.CreateRoom(roomName, Constants.TABLE_GAME_MAX_PLAYERS, dict);
        else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS)
            NetworkManager.Instance.CreateRoom(roomName, Constants.TEXASS_GAME_MAX_PLAYERS, dict);
        else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
            NetworkManager.Instance.CreateRoom(roomName, Constants.WHOOPASS_GAME_MAX_PLAYERS, dict);

        menuHandler.createRoomPanel.SetActive(false);

        menuHandler.roomListPanel.SetActive(true);
    }

    public void OnHomeButtonTap()
    {
		LoginScript.loginDetails = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client;

public class UserManager : MonoBehaviour
{
	public static GameObject obj;
//	MenuHandler menuHandler;
	bool isConnecting = false;
	bool isConnected = false;
	public List<string> roomUsers;
	public string minChips = "";
	public string roomName = "";

	public static UserManager instance
	{
		get
		{
			if(obj != null) return obj.GetComponent<UserManager>();
			else return null;
		}
	}

	void Awake()
	{
		obj = gameObject;
//		menuHandler = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<MenuHandler>();
		roomUsers = new List<string> ();
	}


	public static void Init(string uname)
	{
		obj = new GameObject ("UserMaanger");
		UserManager user = obj.AddComponent<UserManager>();
		user.StartCoroutine (user.LoginProgress(uname));
	}


	IEnumerator LoginProgress(string user_name)
	{
		isConnecting = true;
		WarpClient.GetInstance ().Connect (user_name, "");

		while(isConnecting) yield return new WaitForEndOfFrame ();

		if (isConnected) 
		{
			Debug.Log("connected");
//			menuHandler.LoginPanel.SetActive(false);
//			menuHandler.RoomeMenuPanel.SetActive(true);
//			menuHandler.loader.SetActive(false);
			GetRoomsData();
		}
	}

	void GetRoomsData()
	{
		WarpClient.GetInstance ().GetAllRooms ();
	}

	public void SetRoomsData(string[] room_array)
	{

		foreach(string str in room_array)
		{
			Debug.Log(str);
			WarpClient.GetInstance().GetLiveRoomInfo(str);
		}
	}

	public void OnLoginDone(bool result)
	{
		Debug.Log ("connect "+result);
		isConnected = result;
		isConnecting = false;
	}

	public void OnCreateRoomCallback(bool result, string room_id)
	{
		if (result) 
		{
			TABLE tbl = new TABLE();
			Debug.Log("create rooom done");

			tbl.roomId = room_id;
			tbl.roomName = UserManager.instance.roomName;
			tbl.minChips = UserManager.instance.minChips;

//			menuHandler.CreateRoomPanel.SetActive(false);
//			menuHandler.RoomeMenuPanel.SetActive(true);
//			menuHandler.RoomeMenuPanel.GetComponentInChildren<ScrollList>().Init(tbl);
		}
	}

	public void SubscribeRoom(string room_id)
	{
		WarpClient.GetInstance ().SubscribeRoom (room_id);
	}

	public void OnSubscribeRoomDone(bool result, string room_id)
	{
		if(result)
		{
			WarpClient.GetInstance().JoinRoom(room_id);
		}
	}

	public void OnJoinRoomDone(bool result, string room_id)
	{
		if (result)
		{

		}
	}
}

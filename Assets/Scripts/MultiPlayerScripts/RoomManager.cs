using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {

	public static GameObject obj;
//	MenuHandler menuHandler;

	public List<TABLE> tables;

	public int TableCount{get{return tables.Count;}}

	void Start()
	{
//		menuHandler = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<MenuHandler>();
		tables = new List<TABLE> ();
	}

	public void OnGetLiveRoomInfo(bool result, TABLE tbl)
	{
		Debug.Log ("info "+tbl.roomId+".. "+tbl.minChips+".. "+tbl.roomName+".. "+tables.Count);
		if (result) 
		{
//			menuHandler.LoginPanel.SetActive(false);
//			menuHandler.RoomeMenuPanel.SetActive(true);
//		
//			menuHandler.RoomeMenuPanel.GetComponentInChildren<ScrollList>().Init(tbl);
		}

	}

}



//public struct TABLE
//{
//	public string roomId;
//	public string roomName;
//	public string minChips;
//	public string userId;
//}
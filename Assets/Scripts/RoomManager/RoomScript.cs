using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class RoomScript : MonoBehaviour {

	public Text txtAllMembers;
	public Text txtSelectedRoom;
	public InputField ifMessage;
//	string strSelectedRoomId = "";
	// Use this for initialization
	void Start () {
//		RoomListScript roomListScript =  GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<RoomListScript> ();
//		strSelectedRoomId = roomListScript.strSelectedRoomId;
		Debug.Log("Selected Room Id : "+appwarp.currentRoomId);
		WarpClient.GetInstance ().GetLiveRoomInfo (appwarp.currentRoomId);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onRoomDetails(LiveRoomInfoEvent liveRoomInfoEvent){
		txtSelectedRoom.text = liveRoomInfoEvent.getData ().getName ();
//		foreach(string username in liveRoomInfoEvent.getJoinedUsers () ){
//			txtAllMembers.text =txtAllMembers.text +"\n"+username ;
//		}
	}
	public void onNewUserJoined(string username){
		txtAllMembers.text =txtAllMembers.text +"\n"+username ;
	}
	public void btnSendMessage(){
		string strMessage = ifMessage.text.Trim ();
		if (!strMessage.Equals ("")) {
			ifMessage.text = "";
//			WarpClient.GetInstance ().
		}
	}

}

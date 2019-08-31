using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client;

public class JoinRoomRow : MonoBehaviour
{
	public Text roomNameText;
	public Text blindText;
	public Text chipsText;
	public Text totalPlayersText;
	public string roomid = "";

	void OnEnable ()
	{
		Image img = GetComponent<Image> ();
		Color c = img.color;
		c.a = .25f;
		img.color = c;

		StartCoroutine (GetRoomDetails ());

		NetworkManager.onRoomInfoReceived += HandleOnRoomInfoReceived;
	}

	void OnDisable ()
	{
		NetworkManager.onRoomInfoReceived -= HandleOnRoomInfoReceived;
	}

	public void OnJoinBtnClick ()
	{
		Debug.Log ("roomid " + roomid);
		UserManager.instance.SubscribeRoom (roomid);
	}

	private IEnumerator GetRoomDetails ()
	{
		yield return new WaitForSeconds (.1f);

		while (gameObject.activeSelf) {
			WarpClient.GetInstance ().GetLiveRoomInfo (roomid);

			yield return new WaitForSeconds (5f);
		}
	}

	private void HandleOnRoomInfoReceived (LiveRoomInfoEvent roomInfo)
	{
		if (roomid != roomInfo.getData ().getId ())
			return;

		roomid = roomInfo.getData ().getId ();

		Dictionary<string, object> rumData = roomInfo.getProperties ();

		string sbChips = rumData [Constants.ROOM_PROP_MIN_CHIPS].ToString ();

		string bbChips = "";
		string userMinChips = "";
		string userMaxChips = "";

		if (sbChips != null) {
			bbChips = (int.Parse (sbChips) * 2).ToString ();
			userMinChips = (int.Parse (sbChips) * 20).ToString ();
			userMaxChips = (int.Parse (userMinChips) * 20).ToString ();

			blindText.text = sbChips + " / " + bbChips;
			chipsText.text = userMinChips + " / " + userMaxChips;
		}

		if (roomInfo.getJoinedUsers () == null)
			totalPlayersText.text = "0 / " + roomInfo.getData ().getMaxUsers ();
		else
			totalPlayersText.text = roomInfo.getJoinedUsers ().Length + " / " + roomInfo.getData ().getMaxUsers ();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class ScrollList : MonoBehaviour {

	RoomManager rumManager;
	private VerticalLayoutGroup layout;
	public RectOffset margins;
	public GameObject itemPrefab;
	public List<JoinRoomRow> item;
	public Sprite itemBg;
	public float spacing = 2.0f;
	public float itemHeight = 0.0f;
	public float scrollWidth = 0.0f;
	public int itemCount;

	public ToggleGroup roomListToggleGroup;

	void Awake()
	{
		rumManager = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<RoomManager>();

		CreateList1 ();
	}

	void CreateList1()
	{

	}

	public void Init(TABLE t1)
	{
		CreateList (t1);
		layout = GetComponent<VerticalLayoutGroup>();
		layout.padding = margins;
		layout.spacing = spacing;

		SetScrollViewHeight ();
	}

	public void SetScrollViewHeight()
	{
		RectTransform scrollView = GetComponent<RectTransform> ();
		scrollView.sizeDelta = new Vector2 (scrollView.sizeDelta.x, item.Count * (itemHeight + spacing));
	}

	void CreateList(TABLE table)
	{
//		//int count = rumManager.tables.Count;
//		int count = GameObject.FindGameObjectWithTag ("RoomListPanel").GetComponent<RoomListScript> ().tablesList.Count;
//		Debug.Log ("create list " + count + "... " + table.minChips + "... " + table.roomId);
//
//		string sbChips = table.minChips;
//
//		string bbChips = "";			// = (int.Parse(sbChips)*2).ToString();
//		string userMinChips = "";		// = (int.Parse(sbChips)*20).ToString();
//		string userMaxChips = ""; 		// = (int.Parse(userMinChips)*20).ToString();
//
//		if (sbChips != null) {
//			bbChips = (int.Parse (sbChips) * 2).ToString ();
//			userMinChips = (int.Parse (sbChips) * 20).ToString ();
//			userMaxChips = (int.Parse (userMinChips) * 20).ToString ();
//		}
//		
//		
//		GameObject g = Instantiate(itemPrefab) as GameObject;
//		g.transform.SetParent(transform);
//		g.transform.localScale = Vector3.one;
//		g.transform.localPosition = Vector3.zero;
//		
//		item.Add(g.transform);
//		g.SetActive(true);
//		
//		layoutElement(g).minHeight = itemHeight;
//		JoinRoomRow row = g.GetComponent<JoinRoomRow>();
//		row.roomNameText.text = table.roomName;
//		row.blindText.text = sbChips+" / "+bbChips;
//		row.chipsText.text = userMinChips+" / "+userMaxChips;
//		row.totalPlayersText.text = table.totalUsers + "/" + table.maxUsers;
//		row.roomid = table.roomId;
	}

	public void InstantiateRoom(MatchedRoomsEvent matchedRooms)
	{
		foreach (JoinRoomRow t in item) {
//			if (!t.roomid.Equals (matchedRooms.getRoomsData () [item.IndexOf (t)].getId()))
				Destroy (t.gameObject);
		}
		item = new List<JoinRoomRow> ();

		roomListToggleGroup.allowSwitchOff = true;
		for (int i = 0; i < matchedRooms.getRoomsData ().Length; i++) {
			if (IsRoomCreatedOfID (matchedRooms.getRoomsData () [i].getId ()))
				continue;

			GameObject g = Instantiate(itemPrefab) as GameObject;
			g.transform.SetParent(transform);
			g.transform.localScale = Vector3.one;
			g.transform.localPosition = Vector3.zero;

			JoinRoomRow row = g.GetComponent<JoinRoomRow>();
			row.roomid = matchedRooms.getRoomsData ()[i].getId();
			row.roomNameText.text = matchedRooms.getRoomsData () [i].getName ();

			item.Add(row);
			g.SetActive(true);

			g.GetComponent<Toggle> ().isOn = false;
		}

		roomListToggleGroup.allowSwitchOff = false;
	}

	private bool IsRoomCreatedOfID(string roomID)
	{
		bool isCreated = false;
		foreach (JoinRoomRow t in item) {
			if (t.roomid.Equals (roomID))
				isCreated = true;
		}

		return isCreated;
	}
	
	public RectTransform rTransform
	{
		get
		{
			return GetComponent<RectTransform>();
		}
	}
	
	public Image GetImage(GameObject imgobj)
	{
		return imgobj.GetComponent<Image>();
	}
	
	public LayoutElement layoutElement(GameObject obg)
	{
		return obg.GetComponent<LayoutElement>();		
	}
}

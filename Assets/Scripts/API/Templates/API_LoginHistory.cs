using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_LoginHistory
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_LoginHistory"/> class.
	/// </summary>
	/// <param name="history">History.</param>
	public API_LoginHistory(string history)
	{
		JSONArray arr = new JSONArray (history);

		loginHistoryList = new List<LoginHistory> ();
		for (int i = 0; i < arr.Count (); i++) {
			loginHistoryList.Add (new LoginHistory (arr.getString (i)));
		}
	}

	public List<LoginHistory> loginHistoryList;
}

public class LoginHistory
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginHistory"/> class.
	/// </summary>
	/// <param name="history">History.</param>
	public LoginHistory(string history)
	{
		LoginHistory lh = JsonUtility.FromJson<LoginHistory> (history);
		id = lh.id;
		user_id = lh.user_id;
		ip = lh.ip;
		browser = lh.browser;
		time = lh.time;
	}

	public string id;
	public string user_id;
	public string ip;
	public string browser;
	public string time;
}
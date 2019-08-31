using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class API_SingleGameInfo
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_SingleGameInfo"/> class.
	/// </summary>
	/// <param name="gameInfo">Game info.</param>
	public API_SingleGameInfo (string gameInfo)
	{
		JSON_Object obj = new JSON_Object (gameInfo);
		id = obj.getString ("id");
		game_name = obj.getString ("game_name");
		poker_type = obj.getString ("poker_type");

		JSONArray arr = new JSONArray (obj.getString ("users"));

		users = new List<SingleGameInfoPlayer> ();
		for (int i = 0; i < arr.Count (); i++) {
			SingleGameInfoPlayer player = JsonUtility.FromJson<SingleGameInfoPlayer> (arr.getString (i));
			users.Add (player);
		}
	}

	public string id;
	public string game_name;
	public string poker_type;

	public List<SingleGameInfoPlayer> users;
}

[Serializable]
public class SingleGameInfoPlayer
{
	public double balance_chips;
	public double balance_cash;
	public string name;
	public double buy_in;
	public string flagUrl;

	public PlayerPivot pivot;
}

[Serializable]
public class PlayerPivot
{
	public string game_id;
	public string user_id;
	public string join_time;
	public string status;
	public int join_count;
	public int rebuy_counter;
	public double join_balance;
	public double leave_balance;
}
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class API_GameInfo
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_GameInfo"/> class.
	/// </summary>
	/// <param name="gameInfo">Game info.</param>
	public API_GameInfo (string gameInfo)
	{
		JSON_Object obj = new JSON_Object (gameInfo);

		total = obj.getInt ("total");
		per_page = obj.getString ("per_page");
		current_page = obj.getInt ("current_page");
		last_page = obj.getInt ("last_page");
		if (obj.has ("next_page_url"))
			next_page_url = obj.getString ("next_page_url");
		if (obj.has ("prev_page_url"))
			prev_page_url = obj.getString ("prev_page_url");
		if (obj.has ("from"))
			from = obj.getString ("from");
		if (obj.has ("to"))
			to = obj.getString ("to");

		JSONArray arr = new JSONArray (obj.getString ("data"));
		gameList = new List<GameData> ();
		for (int i = 0; i < arr.Count (); i++) {
			GameData data = new GameData (arr.getString (i));
			if (!data.game_type.Equals (APIConstants.CASH_GAME_GAME_TYPE) && data.status.Equals (APIConstants.TOURNAMENT_STATUS_FINISHED)) {
				DateTime currentTime = DateTime.Parse (data.currenrTime);
				DateTime finishedTime = DateTime.Parse (data.finished_time);

				if ((currentTime - finishedTime).Minutes < 10) {
					gameList.Add (data);
				}
			} else {
				gameList.Add (data);
			}
		}
	}

	public int total;
	public string per_page;
	public int current_page;
	public int last_page;
	public string next_page_url;
	public string prev_page_url;
	public string from;
	public string to;

	public List<GameData> gameList;
}

[Serializable]
public class GameData
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GameData"/> class.
	/// </summary>
	/// <param name="data">Data.</param>
	public GameData (string data)
	{
		GameData gameInfoObj = JsonUtility.FromJson<GameData> (data);

		id = gameInfoObj.id;
		poker_type = gameInfoObj.poker_type;
		money_type = gameInfoObj.money_type;
		game_name = gameInfoObj.game_name;
		limit = gameInfoObj.limit;
		name = gameInfoObj.name;
		game_type = gameInfoObj.game_type;
		register_from = gameInfoObj.register_from;
		start_time = gameInfoObj.start_time;
		game_speed = gameInfoObj.game_speed;
		fee = gameInfoObj.fee;
		fee_detail = gameInfoObj.fee_detail;
		small_blind = gameInfoObj.small_blind;
		prize_pool = gameInfoObj.prize_pool;
		buy_in = gameInfoObj.buy_in;
		status = gameInfoObj.status;
		status = gameInfoObj.status;
		users = gameInfoObj.users;
		minimum_players = gameInfoObj.minimum_players;
		maximum_players = gameInfoObj.maximum_players;
		joined = gameInfoObj.joined;
		currenrTime = gameInfoObj.currenrTime;
		finished_time = gameInfoObj.finished_time;
	}

	//	public string id;
	//	public string poker_type;
	//	public string money_type;
	//	public string game_name;
	//	public string limit;
	//	public string name;
	//	public string game_type;
	//	public DateTime register_from;
	//	public DateTime start_time;
	//	public string game_speed;
	//	public double fee;
	//	public string fee_detail;
	//	public double small_blind;
	//	public double prize_pool;
	//	public double buy_in;
	//	public string status;
	//	public string registered_status;
	//	public int total_enrolled;

	public string id;
	public string user_id;
	public string poker_type;
	public string money_type;
	public string game_name;
	public string limit;
	public string name;
	public string description;
	public string game_type;
	public int minimum_players;
	public int maximum_players;
	public string register_from;
	public string start_time;
	public string game_speed;
	public double fee;
	public double entry_fee;
	public string fee_detail;
	public string raise_time;
	public double small_blind;
	public double big_blind;
	public string rake_id;
	public double prize_pool;
	public double buy_in;
	public double dealer_payout;
	public string dealer_win;
	public string status;
	public string finished_time;
	public string created_at;
	public string updated_at;
	public string deleted_at;
	public int users;
	public bool joined;
	public string currenrTime;
}
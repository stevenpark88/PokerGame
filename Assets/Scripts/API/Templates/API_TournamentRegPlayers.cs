using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_TournamentRegPlayers
{
	public API_TournamentRegPlayers (string  tourPlayers)
	{
		JSON_Object obj = new JSON_Object (tourPlayers);

		id = obj.getString ("id");
		game_name = obj.getString ("game_name");
		poker_type = obj.getString ("poker_type");

		JSONArray playerArr = obj.getJSONArray ("users");

		playerList = new List<SingleGameInfoPlayer> ();
		for (int i = 0; i < playerArr.Count (); i++) {
			playerList.Add (JsonUtility.FromJson<SingleGameInfoPlayer> (playerArr.getString (i)));
		}
	}

	public string id;
	public string game_name;
	public string poker_type;

	public List<SingleGameInfoPlayer> playerList;
}
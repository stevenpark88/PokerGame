using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_TournamentAwardInfo
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_PlayerInfo"/> class.
	/// </summary>
	/// <param name="playerInfo">Player info.</param>
	public API_TournamentAwardInfo(string tournamentInfo)
	{
		JSON_Object obj = new JSON_Object (tournamentInfo);

		awardsWinnerList = new List<API_TournaAwardPlayerInfo> ();
		JSONArray arr = new JSONArray (obj.getString ("awards_winner"));
		for (int i = 0; i < arr.Count (); i++) {
			API_TournaAwardPlayerInfo player = JsonUtility.FromJson<API_TournaAwardPlayerInfo> (arr.getString (i));
			awardsWinnerList.Add (player);
		}

		game_id = obj.getString ("game_id");
	}

	public List<API_TournaAwardPlayerInfo> awardsWinnerList;

	public string game_id;
}

public class API_TournaAwardPlayerInfo
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_TournaAwardPlayerInfo"/> class.
	/// </summary>
	/// <param name="playerInfo">Player info.</param>
	public API_TournaAwardPlayerInfo(string playerInfo)
	{
		API_TournaAwardPlayerInfo player = JsonUtility.FromJson<API_TournaAwardPlayerInfo> (playerInfo);
		id = player.id;
		name = player.name;
		avtar = player.avtar;
	}

	public string id;
	public string name;
	public string avtar;
	public int rank;
	public double winning_amount;
	public float winning_percentage;
}
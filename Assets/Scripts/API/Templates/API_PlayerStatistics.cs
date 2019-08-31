using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_PlayerStatistics
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_PlayerStatistics"/> class.
	/// </summary>
	/// <param name="statistics">Statistics.</param>
	public API_PlayerStatistics(string statistics)
	{
		JSONArray arr = new JSONArray (statistics);

		statisticsList = new List<Statistics> ();
		for (int i = 0; i < arr.Count (); i++) {
			statisticsList.Add (new Statistics (arr.getString (i)));
		}
	}

	public List<Statistics> statisticsList;
}

public class Statistics
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Statistics"/> class.
	/// </summary>
	/// <param name="statistics">Statistics.</param>
	public Statistics(string statistics)
	{
		Statistics st = JsonUtility.FromJson<Statistics> (statistics);

		id = st.id;
		user_id = st.user_id;
		game_name = st.game_name;
		start_time = st.start_time;
		winning_amount = st.winning_amount;
		bet_amount = st.bet_amount;
		without_showdown = st.without_showdown;
		showdown = st.showdown;
		levels = st.levels;
	}

	public string id;
	public string user_id;
	public string game_name;
	public string start_time;
	public double winning_amount;
	public double bet_amount;
	public uint without_showdown;
	public uint showdown;
	public uint levels;
}
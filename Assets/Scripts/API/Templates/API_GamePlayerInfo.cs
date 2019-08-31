using UnityEngine;
using System.Collections;

public class API_GamePlayerInfo
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_GamePlayerInfo"/> class.
	/// </summary>
	/// <param name="playerInfo">Player info.</param>
	public API_GamePlayerInfo(string playerInfo)
	{
		API_GamePlayerInfo playerInfoObj = JsonUtility.FromJson<API_GamePlayerInfo> (playerInfo);

		player_id = playerInfoObj.player_id;
		name = playerInfoObj.name;
		total_play_chips = playerInfoObj.total_play_chips;
		total_real_chips = playerInfoObj.total_real_chips;
		buyin = playerInfoObj.buyin;
		avatar_url = playerInfoObj.avatar_url;
		country_flag_url = playerInfoObj.country_flag_url;
	}

	public string player_id;
	public string name;
	public double total_play_chips;
	public double total_real_chips;
	public double buyin;
	public string avatar_url;
	public string country_flag_url;
}
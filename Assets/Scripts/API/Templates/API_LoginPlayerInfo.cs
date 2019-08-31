using UnityEngine;
using System.Collections;

public class API_LoginPlayerInfo
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_PlayerInfo"/> class.
	/// </summary>
	/// <param name="playerInfo">Player info.</param>
	public API_LoginPlayerInfo (string playerInfo)
	{
		Debug.Log ("Login player info : " + playerInfo);
		API_LoginPlayerInfo playerInfoObj = JsonUtility.FromJson<API_LoginPlayerInfo> (playerInfo);
		JSON_Object loginResponse = new JSON_Object (playerInfo);

		token = playerInfoObj.token;
		id = playerInfoObj.id;
		name = playerInfoObj.name;
		firstname = playerInfoObj.firstname;
		lastname = playerInfoObj.lastname;
		email = playerInfoObj.email;
		balance_cash = double.Parse (loginResponse.getString ("balance_cash"));
		balance_chips = double.Parse (loginResponse.getString ("balance_chips"));
		date_of_birth = playerInfoObj.date_of_birth;
		gender = playerInfoObj.gender;
		avtar = playerInfoObj.avtar;
		daily_limit = playerInfoObj.daily_limit;
	}

	public string token;
	public string id;
	public string name;
	public string firstname;
	public string lastname;
	public string email;
	public double balance_cash;
	public double balance_chips;
	public string date_of_birth;
	public string gender;
	public string avtar;
	public double daily_limit;
}
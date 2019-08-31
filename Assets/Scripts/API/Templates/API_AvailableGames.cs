using System;
using System.Collections.Generic;

[Serializable]
public class API_AvailableGames
{
	public string status;
	public List <string> messages;
	public List <string> gameType;
	public List <string> moneyType;
}
using System;

[Serializable]
public class GameDetailResponse
{
	public Data data;

	[Serializable]
	public class Data
	{
		public int online_players;
		public int tournaments;
	}
}
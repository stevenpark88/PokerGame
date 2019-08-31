using UnityEngine;
using System.Collections;

namespace UIAccount
{
	public class Constants
	{
		public const string MESSAGE_SELF_LIMIT_TITLE = "<size=30>Select Limit</size> : I want to set my Real Money daily deposit limit to the following option.";
		public const string MESSAGE_SELF_EXCLUSION_TEMP = "<size=30>Temporary Self Exclusion :</size> I choose to be excluded from login to WhoopAss Poker website and from receiving any marketing material for the time period selected below.";
		public const string MESSAGE_SELF_EXCLUSION_PERMA = "<size=30>Permanent Self Exclusion</size> : I choose to be permanent self excluded from WhoopAss Poker website and from receiving any marketing material. Also I will be prohibited from reopening WhoopAss Poker account.";

		public const string HEX_RED_HEADER = "#4A0403FF";
		public const string HEX_GOLDEN_HEADER = "#F3D05EFF";
		public const string HEX_COLOR_RED_HEADER = "#F8D35EFF";
		public const string HEX_COLOR_LIST_VIEW_HEADER = "#bc9e50";
		public const string HEX_COLOR_GAME_LIST_ODD_ROW = "#e1c16c";
		public const string HEX_COLOR_GAME_LIST_EVEN_ROW = "#dabb69";

		public const string URL_BUY_CHIPS = APIConstants.BASE_URL + "rest-api/v1/user/" + FIELD_PLAYER_TOKEN_URL + "/buy-chips/play-money";
		public const string URL_BUY_CASH = APIConstants.BASE_URL + "rest-api/v1/user/" + FIELD_PLAYER_TOKEN_URL + "/buy-chips/real-money";
		public const string URL_PLAYER_STATISTICS = APIConstants.BASE_URL + "rest-api/v1/user/" + FIELD_PLAYER_TOKEN_URL + "/statistics";
		public const string URL_PLAYER_TRANSACTIONS = APIConstants.BASE_URL + "rest-api/v1/user/" + FIELD_PLAYER_TOKEN_URL + "/transactions";
		public const string URL_PLAYER_PROFILE = APIConstants.BASE_URL + "rest-api/v1/user/" + FIELD_PLAYER_TOKEN_URL + "/profile";
		public const string URL_PLAYER_ChipsUpdate = APIConstants.BASE_URL + "rest-api/v1/user/" + FIELD_PLAYER_TOKEN_URL + "/lastHourAchivement";

		public const string URL_RULES = APIConstants.BASE_URL + "rest-api/v1/game-rules";
		public const string URL_HELP = APIConstants.BASE_URL + "rest-api/v1/game-helps";
		public const string URL_SUPPORT = APIConstants.BASE_URL + "rest-api/v1/user/" + FIELD_PLAYER_TOKEN_URL + "/supports";
		public const string URL_PLAYER_REGISTRATION = APIConstants.BASE_URL + "rest-api/v1/register";

		public const string FIELD_PLAYER_TOKEN_URL = "[PLAYER_TOKEN]";
	}
}
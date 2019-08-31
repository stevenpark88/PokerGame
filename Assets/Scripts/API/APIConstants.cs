using UnityEngine;
using System.Collections;

public class APIConstants
{
	public const string BASE_URL = "https://whoopasspoker.com/";
	//public const string BASE_URL = "http://46.51.137.196/";
	//public const string BASE_URL = "http://35.177.136.119/";
//	public const string SOCKET_BASE_URL = "https://www.whoopasspoker.com:8888";
	public const string SOCKET_BASE_URL = "https://www.whoopasspoker.com:12346";
	//146.247.49.61
	//public const string SOCKET_BASE_URL = "http://35.177.136.119:12347";
	//public const string SOCKET_BASE_URL = "http://46.51.137.196:8888";


//	public const string BASE_URL = "http://35s.177.136.119/";
//	public const strisng SOCKET_BASE_URL = "http://35.177.136.119:8888";

	//	public const string BASE_URL = "http://192.168.2.37/whoopasspoker/";
	//	public const string SOCKET_BASE_URL = "ws://192.168.2.37:8888";

	public const string URL_GET_GAME_DETAIL = BASE_URL + "rest-api/v1/get-onlinePlayers-tournaments";

	public const string LOGIN_URL = BASE_URL + "rest-api/v1/login";
	public const string LOGIN_PLAYER_DETAIL = BASE_URL + "rest-api/v1/user-detail";
	public const string GAMES_URL = BASE_URL + "rest-api/v1/games";

	public const string URL_TIPS = BASE_URL + "rest-api/v1/help/tips";
	public const string URL_GAME_RULES = BASE_URL + "rest-api/v1/help/game-rules";

	public const string URL_UPDATE_LOGIN_STATUS = BASE_URL + "rest-api/v1/user/status";
	public const string URL_UPDATE_LOGOUT = BASE_URL + "rest-api/v1/logout";

	public const string URL_AVAILABLE_GAMES = BASE_URL + "rest-api/v1/affiliator/access";

	public const string TOURNAMENT_AWARD_URL_KEY = "/award-detail";
	public const string TOURNAMENT_REGISTERED_PLAYERS_URL_KEY = "/register-users";
	public const string JOIN_CASH_GAME_URL_KEY = "/join/cash-game";
	public const string JOIN_REGULAR_TOUR_URL_KEY = "/join/tournament-regular";
	public const string JOIN_SNG_TOUR_URL_KEY = "/join/tournament-sng";

	//	----------		UI Account		----------
	public const string USER_ACCOUNT_URL = BASE_URL + "rest-api/v1/user/account";
	public const string UPDATE_SELF_LIMIT_URL = BASE_URL + "rest-api/v1/user/self-limit";
	public const string UPDATE_SELF_EXCLUSION_URL = BASE_URL + "rest-api/v1/user/self-exclusion";

	//	----------		UI Account		----------

	public static string PLAYER_TOKEN = "";

	public const string FIELD_EMAIL = "email";
	public const string FIELD_PASSWORD = "password";
	public const string FIELD_AFFILIATE_DATA = "affiliateid";

	public const string FIELD_TOKEN = "token";
	public const string FIELD_AUTHORIZATION = "Authorization";

	public const string STATUS_AUTHORIZED = "authorized";
	public const string STATUS_UNATHORIZED = "unauthorized";
	public const string STATUS_FAILED = "failed";

	public const string FIELD_GAME_SPEED = "game_speed";
	public const string FIELD_LIMIT = "limit";
	public const string FIELD_MONEY_TYPE = "money_type";
	public const string FIELD_POKER_TYPE = "poker_type";
	public const string FIELD_GAME_TYPE = "game_type";
	public const string FIELD_TOUR_TYPE = "tour_type";
	public const string FIELD_PER_PAGE = "per_page";
	public const string FIELD_URL = "url";

	public const string FIELD_USERNAME = "username";
	public const string FIELD_CONTACT = "contact";
	public const string FIELD_SOUND = "sound";
	public const string FIELD_TIMEZONE = "time_zone";

	public const string FIELD_DAILY_LIMIT = "daily_limit";
	public const string FIELD_AFFILIATE_ID = "affiliate_id";

	public const string FIELD_EXCLUSION_TYPE = "exclusion_type";

	public const string FIELD_BUY_IN = "buy_in";

	public const string GAME_FILTER_GAME_SPEED = "filters[" + FIELD_GAME_SPEED + "]";
	public const string GAME_FILTER_LIMIT = "filters[" + FIELD_LIMIT + "]";
	public const string GAME_FILTER_MONEY_TYPE = "filters[" + FIELD_MONEY_TYPE + "]";
	public const string GAME_FILTER_POKER_TYPE = "filters[" + FIELD_POKER_TYPE + "]";
	public const string GAME_FILTER_GAME_TYPE = "filters[" + FIELD_GAME_TYPE + "]";
	public const string GAME_FILTER_TOUR_TYPE = "filters[" + FIELD_TOUR_TYPE + "]";
	public const string GAME_FILTER_PER_PAGE = "per_page";
	public const string GAME_FILTER_ORDER_BY = "order_by";
	public const string GAME_FILTER_DIRECTION = "direction";
	public const string GAME_FILTER_PAGE = "page";

	public const string KEY_REAL_MONEY = "real money";
	public const string KEY_PLAY_MONEY = "play money";

	public const string KEY_WHOOPASS = "wa";
	public const string KEY_TEXASS = "th";
	public const string KEY_TABLE = "table";

	public const string CASH_GAME_GAME_TYPE = "cash game";
	public const string REGULAR_TOUR_GAME_TYPE = "regular";
	public const string SNG_TOUR_GAME_TYPE = "sng";

	public const string STRING_WHOOPASS_GAME = "WhoopAss";
	public const string STRING_TEXASS_GAME = "Texas Holde'm";
	public const string STRING_TABLE_GAME = "WhoopAss Table";

	public const string TOURNAMENT_STATUS_RUNNING = "running";
	public const string TOURNAMENT_STATUS_PENDING = "pending";
	public const string TOURNAMENT_STATUS_REGISTERING = "registering";
	public const string TOURNAMENT_STATUS_FINISHED = "finished";

	public const int GAMES_PER_TABLE = 8;

	public const string MESSAGE_NO_GAMES_FOUND = "<color=red>No game found.</color>";
	public const string MESSAGE_NO_PLAYERS_IN_GAME = "<color=white>No player in this game.</color>";
	public const string MESSAGE_TOUR_WINNER_NOT_DECLARED = "<color=red>Winners not declared yet.</color>";
	public const string MESSAGE_TOUR_NO_REGD_PLAYERS = "<color=red>No player registered in this game.</color>";

	public const string HEX_RED_HEADER = "#4A0403FF";
	public const string HEX_GOLDEN_HEADER = "#F3D05EFF";
	public const string HEX_COLOR_RED_HEADER = "#F8D35EFF";
	public const string HEX_COLOR_LIST_VIEW_HEADER = "#bc9e50";
	public const string HEX_COLOR_GAME_LIST_ODD_ROW = "#e1c16c";
	public const string HEX_COLOR_GAME_LIST_EVEN_ROW = "#dabb69";
	public const string HEX_COLOR_YELLOW = "#F4CF58FF";
}

public enum MoneyType
{
	RealMoney,
	PlayMoney,
	All
}
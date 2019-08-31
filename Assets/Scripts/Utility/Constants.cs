using UnityEngine;
using System.Collections;

public class Constants
{
	public const bool REMOVE_BEFORE_LIVE = true;

	public static string GameID = "";

	public const bool HANDLE_MULTI_GAME_HERE = true;

	public const int RECONNECTING_TIMEOUT = 10;

	public const int RESET_GAME_DATA_AFTER = 17;

	public const int MAX_TIME_RAISE_PER_ROUND = 3;
	public const int MAX_TIMEOUT_ALLOW = 4;
	//	Will disconnect on +1
	public const int MAX_SIT_OUT_PER_GAME_ALLOW = 1;
	//	Will disconnect on +1

	public const int MIN_BET_AMOUNT = 10;
	
	public const string MESSAGE_NO_TABLE = "No table found. Tap on <b><i>Create</i></b> button to create new table.";
	public const string MESSAGE_SELECT_TABLE = "Select table and tap on <b><i>Join</i></b> button.";

	public const string MESSAGE_STRAIGHT_OR_BETTER = "Bet on STRAIGHT or BETTER for ";
	public const string MESSAGE_BUY_WHOOPASS_CARD = "Buy WhoopAss Card for ";

	public const string MESSAGE_RECONNECTING = "Connection Lost!!\nReconnecting to the network..";
	public const string MESSAGE_CONNECTION_FAIL = "You have lost connection with server.";

	public const string MESSAGE_SOMETHING_WENT_WRONG = "Something went wrong.\nTry again later.";

	public const string MESSAGE_BREAK_TIME = "It's a Break Time!!!";
	public const string MESSAGE_NOT_REGISTERED_WITH_TOURNAMENT = "You are not registered with this tournament.";

	public const string MESSAGE_CONFIRMATION = "Are you sure?";
	public const string MESSAGE_NO_ENOUGH_CHIPS = "You do not have enough chips to play";

	public const string MESSAGE_YOU_HAVE_ELIMINATED = "You are eliminated from the tournament.";
	public const string MESSAGE_INSUFFICIENT_CHIPS_ELIMINATION = "You will be eliminated from the tournament due to insufficient chips.";

	public const string MESSAGE_PLAYER_IS_SITOUT = " requested sitout.";
	public const string MESSAGE_PLAYER_IS_BACK_TO_GAME = " requested back to game.";
	public const string MESSAGE_PLAYER_IS_ELIMINATED = " is eliminated from the game.";

	public const string MESSAGE_SELECT_REBUY_AMOUNT = "Select Rebuy Amount";

	public const string MESSAGE_TABLE_GAME_TITLE = "WhoopAss Poker - Table Game";
	public const string MESSAGE_TEXASS_GAME_TITLE = "Texas Hold'em Poker";
	public const string MESSAGE_WHOOPASS_GAME_TITLE = "WhoopAss Poker";

	public const string MESSAGE_REGULAR_TOURNAMENT_TITLE = "(Regular Tournament)";
	public const string MESSAGE_SnG_TOURNAMENT_TITLE = "(SnG Tournament)";

	public const string MESSAGE_LIMIT_GAME_TITLE = "Limit";
	public const string MESSAGE_NO_LIMIT_GAME_TITLE = "No Limit";

	public const string MESSAGE_BREAKING_TABLE = "Breaking Table..";

	public const string MESSAGE_NO_INTERNET_CONNECTION = "No internet connection";

	public const string MessageStartingTournament = "Starting tournament.. Please wait..";

	public const string MESSAGE_MAX_SITOUT = "<size=18>Max sitout!!</size>";
	public const string MESSAGE_ELIMINATION_REASON_MAX_SITOUT = "<color=#F7D25DFF>You have eliminated due to max sitout.</color>";

	//	public const string DEFAULT_CURRENCY_PREFIX = "£ ";

	public const string SBAMOUNT_FIELD_SEND_GAME_TYPE = "SBAmount";
	public const string GAME_TYPE_FIELD_SEND_GAME_TYPE = "Game_Type";

	public const string WIN_REPORT_TITLE = "Win Report";

	public const string CARD_ANIMATION_TRIGGER = "animate";
	public const string PLAYER_WINNER_ANIMATION_TRIGGER = "animate";

	public const int MINIMUM_CHIP_DISPLAY = 1;
	public const int MAXIMUM_CHIPS_DISPLAY = 15;

	public const int HIDE_CHAT_TEMPLATE_AFTER = 7;

	public const long TABLE_GAME_PLAY_MIN_CHIPS = 60;
	public const double TABLE_GAME_REAL_MIN_MONEY = .6;

	public const int TABLE_GAME_RESET_TIMER = 11;
	public const string gamblingAddictionUrl = "https://www.gamcare.org.uk/";

	// ======================== Common for server and client =====================
	public const string RESPONSE_DATA_SEPARATOR = "#";
	public const string RESPONSE_FOR_DEFAULT_CARDS = "1" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_PLAYER_INFO = "2" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_ACTION = "3" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_ACTION_DONE = "4" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_ROUND_COMPLETE = "5" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_GAME_START = "6" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_GAME_COMPLETE = "7" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_BLIND_PLAYER = "8" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_DISTRIBUTE_CARD = "9" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_WINNER_INFO = "10" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_RESTART_GAME = "11" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_WA_POT_WINNER = "12" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_BLIND_AMOUNT = "13" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_REBUY = "16" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_REBUY = "17" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_BREAK_STATUS = "18" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_CHAT = "19" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_HISTORY = "20" + RESPONSE_DATA_SEPARATOR;
	public const string CONFIRM_GET_WINNER_DATA = "21" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_PLAYER_TIMEOUT = "21" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_TOURNAMENT_WINNER_INFO = "22" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_NOT_REGISTERED_IN_TOURNAMENT = "23" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_SIT_OUT = "24" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_BACK_TO_GAME = "25" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_BREAK_TABLE = "27" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_PLAYER_ELIMINATE = "28" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_ADD_CHIPS = "29" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_ELIMINATION_FROM_TOURNAMENT = "30" + RESPONSE_DATA_SEPARATOR;
	public const string RESPONSE_FOR_REBUY_ON_ELIMINATED_IN_TOURNAMENT = "31" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_PLAYER_BACK_TO_GAME_COLLECT_BLIND = "32" + RESPONSE_DATA_SEPARATOR;
	public const string REQUEST_FOR_ANTE_AND_BLIND = "100" + RESPONSE_DATA_SEPARATOR;

	public const int MIN_PLAYER_TO_START_GAME = 2;
	public const int TABLE_GAME_MAX_PLAYERS = 3;
	public const int TEXASS_GAME_MAX_PLAYERS = 9;
	public const int WHOOPASS_GAME_MAX_PLAYERS = 6;

	public const string TEXASS_SERVER_NAME = "TexassAppWarpS2";
	public const string WHOOPASS_SERVER_NAME = "WAAppWarpS2";
	public const string TABLEGAME_SERVER_NAME = "TableAppWarpS2";

	public const int TURN_TIME = 50;

	public const string FIELD_PLAYER_NAME_DEALER = "Dealer";

	public const string RESOURCE_GAMECARDS = "Cards/GameCards/";
	public const string RESOURCE_BACK_CARD = "Cards/GameCards/back_card";

	public const string FIELD_ACTION_HISTORY_GAME_STATUS = "Game_Status";
	public const string FIELD_ACTION_HISTORY_ROUND_STATUS = "Round_Status";
	public const string FIELD_ACTION_HISTORY_TURNS = "Turns";
	public const string FIELD_ACTION_HISTORY_ROUND = "Round";

	public const int MAX_CHAT_MESSAGE_LENGTH = 256;

	public const string ROOM_PROP_ROOM_NAME = "RoomName";
	public const string ROOM_PROP_GAME_TYPE = "GameType";
	public const string ROOM_PROP_TOURNAMENT = "TournamentType";
	public const string ROOM_PROP_MIN_CHIPS = "MINCHIPS";
	public const string ROOM_PROP_TURN_TIMER = "Time";
	public const string ROOM_PROP_GAME_ROOM_ID = "GameRoomID";
	public const string ROOM_PROP_TABLE_NUMBER = "TableNumber";
	public const string ROOM_PROP_STATIC_HAND = "StaticHand";

	public const string FIELD_WA_GAME_WINNER_TABLE_POT = "Table_Pot";
	public const string FIELD_WA_GAME_WINNER_WA_POT = "WA_Pot";

	public const string WGL_TABLE_GAME = "TABLE";
	public const string WGL_TEXASS_GAME = "TH";
	public const string WGL_WHOOPASS_GAME = "WA";
	public const string WGL_TH_SNG_TOURNAMENT = "TH_SNG";
	public const string WGL_TH_REGULAR_TOURNAMENT = "TH_REGULAR";
	public const string WGL_WA_SNG_TOURNAMENT = "WA_SNG";
	public const string WGL_WA_REGULAR_TOURNAMENT = "WA_REGULAR";

	public const int RED_CHIP_VALUE = 500;
	public const int GREEN_CHIP_VALUE = 50;
	public const int BLUE_CHIP_VALUE = 10;

	//	public const string BASE_URL = "http://beta.whoopasspoker.com/";

	public const string RESIZE_IMAGE_URL = APIConstants.BASE_URL + "rest-api/v1/resize/image?width=150&height=150&url=";

	public const string SOCKET_EVENT_CASH_GAME_JOIN = "onCashGameJoinEvent";
	public const string SOCKET_EVENT_CASH_GAME_LEAVE = "onCashGameLeaveEvent";

	public const string SOCKET_EVENT_SNG_ELIMINATE_PLAYER = "onEliminatePlayerEvent";
	public const string SOCKET_EVENT_SNG_LEVEL_UPDATE = "onLevelUpdateEvent";

	public const string SOCKET_EVENT_STACK_UPDATE = "onStackUpdateEvent";

	public const string SOCKET_EVENT_REGULAR_TOUR_PRICE_POOL_UPDATE = "onPricePoolUpdateEven";
	public const string SOCKET_EVENT_REGULAR_TOUR_START = "onTournamentStartedEvent";
	public const string SOCKET_EVENT_REGULAR_TOUR_FINISH = "onTournamentFinishedEvent";

	public const string SOCKET_EVENT_GAME_CREATED = "App\\Events\\GameCreated";
	public const string SOCKET_EVENT_START_SNG_GAME = "App\\Events\\StartSngGame";
	public const string SOCKET_EVENT_USER_JOINED_CASH_GAME = "App\\Events\\UserJoinedCashGame";
	public const string SOCKET_EVENT_USER_JOINED_REGULAR_TOUR = "App\\Events\\UserJoinedRegularGame";
	public const string SOCKET_EVENT_USER_JOINED_SNG_TOUR = "App\\Events\\UserJoinedSngGame";

	public const int UPDATE_LOGIN_STATUS_INTERVAL = 15;
}


public enum PLAYER_ACTION
{
	CALL = 1,
	FOLD = 2,
	RAISE = 3,
	BET = 4,
	ALLIN = 5,
	CHECK = 6,
	TIMEOUT = 7,
	ACTION_DEALER = 8,
	ACTION_FOLDED = 9,
	ACTION_PENDING = 10,
	ACTION_WA_UP = 11,
	ACTION_WA_DOWN = 12,
	ACTION_WA_NO = 13,
	ACTION_NO_TURN = 14,
	ACTION_WAITING_FOR_GAME = 15,
	SMALL_BLIND = 16,
	BIG_BLIND = 17,
	PASS_TURN = 18,
	REBUY = 19,
	BLIND_ON_BACK_TO_GAME = 22
}


public enum GAME_STATUS
{
	STOPPED = 71,
	RUNNING = 72,
	PAUSED = 73,
	RESUMED = 74,
	FINISHED = 75,
	CARD_DISTRIBUTE = 76,
	RESTART = 77
}


public enum PLAYER_STATUS
{
	ACTIVE = 1,
	WAITING = 15,
	FOLDED = 9,
	ABSENT = 17,
	SIT_OUT = 20,
	NOT_ENOUGH_CHIPS = 16,
	ELIMINATED = 21
}


public enum TABLE_GAME_ROUND
{
	START = 10,
	FIRST_BET = 11,
	SECOND_BET = 12,
	THIRD_BET = 13,
	WHOOPASS = 14,
	PLAY = 15
}

public enum WHOOPASS_GAME_ROUND
{
	START = 5,
	FIRST_FLOP = 6,
	SECOND_FLOP = 7,
	WHOOPASS_CARD = 8,
	THIRD_FLOP = 9
}

public enum TEXASS_GAME_ROUND
{
	PREFLOP = 1,
	FLOP = 2,
	TURN = 3,
	RIVER = 4
}


public enum WINNING_RANK
{
	ROYAL_FLUSH = 0,
	STRAIGHT_FLUSH = 1,
	FOUR_OF_A_KIND = 2,
	FULL_HOUSE = 3,
	FLUSH = 4,
	STRAIGHT = 5,
	THREE_OF_A_KIND = 6,
	TWO_PAIR = 7,
	ONE_PAIR = 8,
	HIGH_CARD = 9
}

public enum ROUND_STATUS
{
	ACTIVE = 1,
	PENDING = 2,
	FINISHED = 3
}

public enum POKER_GAME_TYPE
{
	TEXAS,
	WHOOPASS,
	TABLE
}

public enum TOURNAMENT_GAME_TYPE
{
	REGULAR = 0,
	SIT_N_GO_TOURNAMENT = 1,
	REGULAR_TOURNAMENT = 2
}
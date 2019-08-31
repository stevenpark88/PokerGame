using UnityEngine;
using System.Collections;

public class  GameConstant {

	// Game sprite image resources path
	public const string RES_PATH_CARDS="Cards/GameCards/";
	public const string RES_PATH_CARD_CLOSE = "back_card";
	public const string RES_PATH_BG_SELECTED = "Table/User_Selected";
	public const string RES_PATH_BG_NORMAL = "Table/User_Normal";
	public const string RES_PATH_BIG_BLIND = "Table/big_blind";
	public const string RES_PATH_SMALL_BLIND = "Table/small_blind";
	public const string RES_PATH_DEALER = "Table/dealer";
	public const string RES_PATH_SELECTED_USER = "Table/User_Selected";
	public const string RES_PATH_UNSELECTED_USER = "Table/User_Normal";
	public const string RES_PATH_WINNING_PLAYER = "Table/WA_Player_bg_selected";
	public const string RES_PATH_PLAYER_BG = "Table/WA_Player_bg";
	public const string RES_PATH_FOLDER_OTHER = "Others";
	public const string RES_PATH_CHECK_BOX = RES_PATH_FOLDER_OTHER + "/check box";
	public const string RES_PATH_CHECK_BOX_SELECTED = RES_PATH_FOLDER_OTHER + "/select check box";

	public const string UI_PATH_CHIPS = "PlayerBetChips";
	public const string UI_PATH_USER_1_CHIPS = UI_PATH_CHIPS+"/Player1Chips";
	public const string UI_PATH_USER_2_CHIPS = UI_PATH_CHIPS+"/Player2Chips";
	public const string UI_PATH_USER_3_CHIPS = UI_PATH_CHIPS+"/Player3Chips";
	public const string UI_PATH_USER_4_CHIPS = UI_PATH_CHIPS+"/Player4Chips";
	public const string UI_PATH_USER_5_CHIPS = UI_PATH_CHIPS+"/Player5Chips";
	public const string UI_PATH_USER_6_CHIPS = UI_PATH_CHIPS+"/Player6Chips";
	public const string UI_PATH_USER_7_CHIPS = UI_PATH_CHIPS+"/Player7Chips";
	public const string UI_PATH_USER_8_CHIPS = UI_PATH_CHIPS+"/Player8Chips";
	public const string UI_PATH_USER_9_CHIPS = UI_PATH_CHIPS+"/Player9Chips";

	public const string UI_PATH_PLAYER = "Players";
	public const string UI_PATH_PLAYER_1 = UI_PATH_PLAYER + "/PlayerView1";
	public const string UI_PATH_PLAYER_2 = UI_PATH_PLAYER + "/PlayerView2";
	public const string UI_PATH_PLAYER_3 = UI_PATH_PLAYER + "/PlayerView3";
	public const string UI_PATH_PLAYER_4 = UI_PATH_PLAYER + "/PlayerView4";
	public const string UI_PATH_PLAYER_5 = UI_PATH_PLAYER + "/PlayerView5";
	public const string UI_PATH_PLAYER_6 = UI_PATH_PLAYER + "/PlayerView6";
	public const string UI_PATH_PLAYER_7 = UI_PATH_PLAYER + "/PlayerView7";
	public const string UI_PATH_PLAYER_8 = UI_PATH_PLAYER + "/PlayerView8";
	public const string UI_PATH_PLAYER_9 = UI_PATH_PLAYER + "/PlayerView9";


	public const string UI_PATH_USER_BET_AMOUT = "txtBetAmt";
	public const string UI_PATH_USER_BET_CHIPS = "imgChips";
	public const string UI_PATH_USER_BET_CHIPS_POSITION = "imgChipsPosition";
	public const string UI_PATH_CARD_DESK = "cardDesk";
	public const string UI_PATH_CARD_DESK_POSITION = "cardDeskPosition";


	public const string UI_PATH_BUTTONS = "GameButtons";
	public const string UI_PATH_BUTTON_FOLD = UI_PATH_BUTTONS+"/FoldButton";
	public const string UI_PATH_BUTTON_CHECK =UI_PATH_BUTTONS+"/CheckButton";
	public const string UI_PATH_BUTTON_RAISE = UI_PATH_BUTTONS+"/RaiseButton";
	public const string UI_PATH_BUTTON_CALL = UI_PATH_BUTTONS+"/CallButton";
	public const string UI_PATH_BUTTON_BET = UI_PATH_BUTTONS+"/BetButton";
	public const string UI_PATH_BUTTON_ALLIN = UI_PATH_BUTTONS+"/AllInButton";
	public const string UI_PATH_BUTTON_REBUY = UI_PATH_BUTTONS+"/ReBuyButton";
	public const string UI_PATH_TOTAL_BET_AMOUNT = "TableChips/txtTotalBet";
	public const string UI_PATH_TABLE_CHIP_SET_1 = "TableChips/ChipsSet1";
	public const string UI_PATH_TABLE_CHIP_SET_2 = "TableChips/ChipsSet2";

	public const string UI_PATH_BET_CHIP_DETAILS = "GameButtons/Bottom/BetChipsDetails/txtBetAmnt";
	public const string UI_PATH_RESTART_PANNEL = "RestartGamePannel";
	public const string UI_PATH_RESTART_TEXT =UI_PATH_RESTART_PANNEL+ "/txtCounter";
	public const string UI_PATH_RESTART_TITLE_TEXT =UI_PATH_RESTART_PANNEL+ "/txtRestarting";

	public const string UI_PATH_DEFAULT_CARD = "DefaultCards";
	public const string UI_PATH_FIRST_FLOP = "FirstFlop";
	public const string UI_PATH_SECOND_FLOP = "SecondFlop";
	public const string UI_PATH_THIRD_FLOP = "ThirdFlop";
	public const string UI_PATH_CARD_FLOP1 = "flop1";
	public const string UI_PATH_CARD_FLOP2 = "flop2";
	public const string UI_PATH_CARD_FLOP3 = "flop3";
	public const string UI_PATH_CARD_TURN = "turn";
	public const string UI_PATH_CARD_RIVER = "river";
	public const string UI_PATH_CARD = "HandCards";
	public const string UI_PATH_CARD_1 = "/Card1";
	public const string UI_PATH_CARD_2 = "/Card2";
	public const string UI_PATH_CARD_WA_UP = UI_PATH_CARD + "/WA_Card_Up";
	public const string UI_PATH_CARD_WA_DOWN = UI_PATH_CARD + "/WA_Card_Down";
	public const string UI_PATH_POSITION = "Position";
	public const string UI_PATH_USER_INFO = "Info";
	public const string UI_PATH_USER_NAME = UI_PATH_USER_INFO+"/txtUserName";
	public const string UI_PATH_USER_CLIP = UI_PATH_USER_INFO+"/txtUserChips";
	//public const string UI_PATH_USER_BG = "UserInfo/Bg";
	public const string UI_PATH_GAME_PLAY ="GamePlayScreen";
	public const string UI_PATH_SB_BB = UI_PATH_USER_INFO+"/imgBB_SB";
	public const string UI_PATH_SB_BB_WA = UI_PATH_CARD+"/imgBB_SB";
	//public const string UI_PATH_MY_INFO = "MyData";
	public const string UI_TURN_IMAGE = "TurnImage";
	public const string UI_PATH_WA_UP_SELECTED = "WAOptions/btnUp/txtUp";
	public const string UI_PATH_WA_DOWN_SELECTED = "WAOptions/btnDown/txtDown";
	public const string UI_PATH_RANK_PANNEL =UI_PATH_USER_INFO+ "/RankPannel";
	public const string UI_PATH_RANK =UI_PATH_RANK_PANNEL+ "/txtRank";

	public const string CURRENCY = "$";

	// ========================Common for server and client=====================
	public const string RESPONSE_DATA_SEPRATOR = "#";
	public static string RESPONSE_FOR_DEFAULT_CARDS = 1+RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_PLAYERS_INFO = 2+RESPONSE_DATA_SEPRATOR;
	public static string REQUEST_FOR_ACTION = 3+RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_ACTION_DONE = 4+RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_ROUND_COMPLETE = 5+RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_GAME_START = 6+RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_GAME_COMPLETE = 7+RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_BLIEND_PLAYER = 8 + RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_DESTRIBUTE_CARD = 9 + RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_WINNIER_INFO = 10 + RESPONSE_DATA_SEPRATOR;
	public static string REQUEST_FOR_RESTART_GAME = 11 + RESPONSE_DATA_SEPRATOR;
	public static string REQUEST_FOR_WA_POT_WINNER = 12 + RESPONSE_DATA_SEPRATOR;
	public static string REQUEST_FOR_BLIEND_AMOUNT = 13 + RESPONSE_DATA_SEPRATOR;
	public static string REQUEST_FOR_RE_BUY = 16+ RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_RE_BUY_STATUS =17 + RESPONSE_DATA_SEPRATOR;
	public static string RESPONSE_FOR_BREAK_STATUS = 18 + RESPONSE_DATA_SEPRATOR;


	public const string TAG_WINER="winner";
	public const string TAG_WINER_TOTAL_BALENCE="winner_balance";
	public const string TAG_ROUND="Round";
	public const string TAG_ACTION = "Action";
	public const string TAG_BET_AMOUNT = "Bet_Amount";
	public const string TAG_TABLE_AMOUNT = "Total_Table_Amount";
	public const string TAG_CALL_AMOUNT = "Call_Amount";
	public const string TAG_PLAYER_NAME = "Player_Name";
	public const string TAG_PLAYER = "Player";
	// Texass Game Cards
	public const string TAG_CARD_FLOP_1 = "Flop1";
	public const string TAG_CARD_FLOP_2 = "Flop2";
	public const string TAG_CARD_FLOP_3 = "Flop3";
	public const string TAG_CARD_TURN = "Turn";
	public const string TAG_CARD_RIVER = "River";
	// WA game cards
	public const string TAG_CARD_FIRST_FLOP_1 = "FirstFlop1";
	public const string TAG_CARD_FIRST_FLOP_2 = "FirstFlop2";
	public const string TAG_CARD_SECOND_FLOP_1 = "SecondFlop1";
	public const string TAG_CARD_SECOND_FLOP_2 = "SecondFlop2";
	public const string TAG_CARD_THIRD_FLOP_1 = "ThirdFlop1";
	public const string TAG_CARD_THIRD_FLOP_2 = "ThirdFlop2";

	public const string TAG_CARD_PLAYER_1 = "Card1";
	public const string TAG_CARD_PLAYER_2 = "Card2";
	public const string TAG_CARD_WA = "WACard";
	public const string TAG_PLAYER_BALANCE = "Player_Balance";
	public const string TAG_PLAYER_SMALL_BLIND = "Small_Blind";
	public const string TAG_PLAYER_BIG_BLIND = "Big_Blind";
	public const string TAG_PLAYER_ACTIVE = "Player_Active";
	public const string TAG_WINNER = "winner";
	public const string TAG_WINNER_NAME = "Winner_Name";
	public const string TAG_WINNER_RANK = "Winner_Rank";
	public const string TAG_SMALL_BLIEND_AMOUNT = "SBAmount";
	public const string TAG_PLAYER_DEALER = "Player_Dealer";
	public const string TAG_WINNER_BEST_CARDS = "Winner_Best_Cards";
	public const string TAG_WINNER_TOTAL_BALENCE = "winner_balance";
	public const string TAG_WINNERS_WINNING_AMOUNT = "Winning_Amount";
	public const string TAG_GAME_STATUS = "Game_Status";
	public const string TAG_PLAYER_STATUS = "Player_Status";
	public const string TAG_GAME_TYPE = "Game_Type";
	public const string TAG_CURRENT_ROUND = "Current_Round";

	public const int GAME_TYPE_REGULAR = 0;
	public const int GAME_TYPE_TOURNAMENT_SIT_N_GO = 1;
	public const int GAME_TYPE_TOURNAMENT_REGULAR = 2;

	// All Round constants
	public const int TEXASS_ROUND_PREFLOP = 0;
	public const int TEXASS_ROUND_FLOP = 1;
	public const int TEXASS_ROUND_TURN = 2;
	public const int TEXASS_ROUND_RIVER = 3;

	// All WhoopAss round 
	public const int WA_ROUND_START= 1;
	public const int WA_ROUND_FIRST_FLOP =2;
	public const int WA_ROUND_SECOND_FLOP = 3;
	public const int WA_ROUND_WHOOPASS =4;
	public const int WA_ROUND_THIRD_FLOP = 5;


	public const int MAXIMUM_RAISE_ON_GAME = 3;
	// Round status
	public const int ROUND_STATUS_ACTIVE = 1;
	public const int ROUND_STATUS_PENDING = 2;
	public const int ROUND_STATUS_FINISH = 3;

	// Player actions
	/** Betting the same amount as BB */
	public const int ACTION_CALL = 1;
	/** Throw your cards away */
	public const int ACTION_FOLD = 2;
	/** Betting more amount as BB */
	public const int ACTION_RAISE = 3;
	/** Putting some money on middle */
	public const int ACTION_BET = 4;
	/** Putting all money on middle */
	public const int ACTION_ALL_IN = 5;
	/** Betting zero and/or to calling the current bet of zero */
	public const int ACTION_CHECK = 6;
	/** If player time is out for select action */
	public const int ACTION_TIMEOUT = 7;
	public const int ACTION_DEALER = 8;
	public const int ACTION_FOLDED = 9;
	public const int ACTION_PENDING= 10;
	public const int ACTION_WA_UP = 11;
	public const int ACTION_WA_DOWN = 12;
	public const int ACTION_WA_NO = 13;
	public const int ACTION_NO_TURN = 14;
	public const int ACTION_WAITING_FOR_GAME = 15;


	public const int STOPPED = 71;
	public const int RUNNING = 72;
	public const int PAUSED = 73;
	public const int RESUMED = 74;
	public const int FINISHED = 75;
	public const int CARD_DISTRIBUTE = 76;
	public const int RESTART = 77;

	public const int MIN_PLAYER_TO_START_GAME = 2;
	public const int WAITING_TIME=10;
	public static int TURN_TIME = 10;
//	public const float ANIM_CARD_TIME = 0.2f;
	public const float ANIM_CARD_TIME = 0.3f;
	public const float ANIM_CHIP_TIME = 1f;
	public const float ANIM_WAITING_TIME = 1f;

//	public const int TOURNAMENT_SNG_BLIND_LEVEL_TIMER = 60 * 15;
//	public const int TOURNAMENT_REGULAR_LEVEL_TIMER = 60 * 1;
//	public const int TOURNAMENT_REBUY_TIMER = 60 * 5;
//	public const int TOURNAMENT_BREAK_TIMER = 60 * 5;

	public const string GAME_TEXAS_HOLD = "Texas Hold'em";
	public const string GAME_WHOOPASS = "WhoopAss";
	public const string GAME_LIMIT = "Limit";
	public const string GAME_NO_LIMIT = "No Limit";

	public const string GAME_SPEED_TURBO  = "Turbo";
	public const string GAME_SPEED_REGULAR = "Regular";

	public const int GAME_SPEED_TURBO_TIME  = 10;
	public const int GAME_SPEED_REGULAR_TIME = 20;


}

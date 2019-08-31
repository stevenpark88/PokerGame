using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIAccount;

namespace UIAccount
{
	public class DashboardPanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES

		public SettingsPanel settingsPanel;
		public BuyChipsPanel buyChipsPanel;
		public GameHelpPanel gameHelpPanel;
		public GameRulesPanel gameRulesPanel;
		public GameTipsPanel gameTipsPanel;

		public Text txtAllGamesTitle;
		public Text txtBuyChipsTitle;
		public Text txtProfileTitle;
		public Text txtSettingsTitle;
		public Text txtGameHelpTitle;
		public Text txtGameRulesTitle;
		public Text txtGameTipsTitle;
		public Text txtSupportTitle;

		#endregion

		#region PRIVATE_VARIABLES

		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void OnEnable ()
		{
			OnSettingButtonTap ();
		}

		void OnDisable ()
		{
			if (WebViewManager.Instance != null && WebViewManager.Instance.webViewObject != null)
				WebViewManager.Instance.CloseWebview ();
		}

		#endregion

		#region DELEGATE_CALLBACKS

		#endregion

		#region PUBLIC_METHODS

		public void OnAllGameasButtonTap ()
		{
			UIManager.Instance.lobbyPanel.gameObject.SetActive (true);
			gameObject.SetActive (false);

			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = Color.white;
			txtBuyChipsTitle.color = c;
			txtGameHelpTitle.color = c;
			txtGameRulesTitle.color = c;
			txtGameTipsTitle.color = c;
			txtProfileTitle.color = c;
			txtSettingsTitle.color = c;
			txtSupportTitle.color = c;

			settingsPanel.gameObject.SetActive (false);
			buyChipsPanel.gameObject.SetActive (false);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (false);

			if (WebViewManager.Instance.webViewObject != null &&
			    WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();
		}

		public void OnBuyChipsButtonTap ()
		{
			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = c;
			txtBuyChipsTitle.color = Color.white;
			txtGameHelpTitle.color = c;
			txtGameRulesTitle.color = c;
			txtGameTipsTitle.color = c;
			txtProfileTitle.color = c;
			txtSettingsTitle.color = c;
			txtSupportTitle.color = c;

			settingsPanel.gameObject.SetActive (false);
			buyChipsPanel.gameObject.SetActive (true);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (false);

			if (WebViewManager.Instance.webViewObject != null && WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();
		}

		public void OnProfileButtonTap ()
		{
			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = c;
			txtBuyChipsTitle.color = c;
			txtGameHelpTitle.color = c;
			txtGameRulesTitle.color = c;
			txtGameTipsTitle.color = c;
			txtProfileTitle.color = Color.white;
			txtSettingsTitle.color = c;
			txtSupportTitle.color = c;

			settingsPanel.gameObject.SetActive (false);
			buyChipsPanel.gameObject.SetActive (false);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (false);

			if (WebViewManager.Instance.webViewObject != null && WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_PLAYER_PROFILE;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_PLAYER_PROFILE;
			url = url.Replace (Constants.FIELD_PLAYER_TOKEN_URL, APIConstants.PLAYER_TOKEN);

			bool canBrowsePic = false;
			canBrowsePic = (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer);
			WebViewManager.Instance.OpenWebviewWithURL (url + "?canBrowsePic=" + canBrowsePic.ToString ().ToLower ());
//			WebViewManager.Instance.OpenWebviewWithURL ("http://192.168.2.37/whoopasspoker/user/profile");
		}

		public void OnSettingButtonTap ()
		{
			settingsPanel.gameObject.SetActive (false);

			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = c;
			txtBuyChipsTitle.color = c;
			txtGameHelpTitle.color = c;
			txtGameRulesTitle.color = c;
			txtGameTipsTitle.color = c;
			txtProfileTitle.color = c;
			txtSettingsTitle.color = Color.white;
			txtSupportTitle.color = c;

			settingsPanel.gameObject.SetActive (true);
			buyChipsPanel.gameObject.SetActive (false);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (false);

			if (WebViewManager.Instance.webViewObject != null && WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();
		}

		public void OnGameHelpButtonTap ()
		{
			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = c;
			txtBuyChipsTitle.color = c;
			txtGameHelpTitle.color = Color.white;
			txtGameRulesTitle.color = c;
			txtGameTipsTitle.color = c;
			txtProfileTitle.color = c;
			txtSettingsTitle.color = c;
			txtSupportTitle.color = c;

			settingsPanel.gameObject.SetActive (false);
			buyChipsPanel.gameObject.SetActive (false);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (false);

			if (WebViewManager.Instance.webViewObject != null && WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_HELP;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_HELP;
			WebViewManager.Instance.OpenWebviewWithURL (url);
		}

		public void OnGameRulesButtonTap ()
		{
			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = c;
			txtBuyChipsTitle.color = c;
			txtGameHelpTitle.color = c;
			txtGameRulesTitle.color = Color.white;
			txtGameTipsTitle.color = c;
			txtProfileTitle.color = c;
			txtSettingsTitle.color = c;
			txtSupportTitle.color = c;

			settingsPanel.gameObject.SetActive (false);
			buyChipsPanel.gameObject.SetActive (false);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (false);

			if (WebViewManager.Instance.webViewObject != null && WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_RULES;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_RULES;
			WebViewManager.Instance.OpenWebviewWithURL (url);
		}

		public void OnGameTipsButtonTap ()
		{
			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = c;
			txtBuyChipsTitle.color = c;
			txtGameHelpTitle.color = c;
			txtGameTipsTitle.color = Color.white;
			txtGameRulesTitle.color = c;
			txtProfileTitle.color = c;
			txtSettingsTitle.color = c;
			txtSupportTitle.color = c;

			settingsPanel.gameObject.SetActive (false);
			buyChipsPanel.gameObject.SetActive (false);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (true);

			if (WebViewManager.Instance.webViewObject != null && WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();
		}

		public void OnSupportButtonTap ()
		{
			Color c = Color.white;
			ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);

			txtAllGamesTitle.color = c;
			txtBuyChipsTitle.color = c;
			txtGameHelpTitle.color = c;
			txtGameRulesTitle.color = c;
			txtGameTipsTitle.color = c;
			txtProfileTitle.color = c;
			txtSettingsTitle.color = c;
			txtSupportTitle.color = Color.white;

			settingsPanel.gameObject.SetActive (false);
			buyChipsPanel.gameObject.SetActive (false);
			gameRulesPanel.gameObject.SetActive (false);
			gameHelpPanel.gameObject.SetActive (false);
			gameTipsPanel.gameObject.SetActive (false);

			if (WebViewManager.Instance.webViewObject != null && WebViewManager.Instance.webViewObject.GetVisibility ())
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_SUPPORT;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_SUPPORT;
			url = url.Replace (Constants.FIELD_PLAYER_TOKEN_URL, APIConstants.PLAYER_TOKEN);

			WebViewManager.Instance.OpenWebviewWithURL (url);
		}

		public void OnBackButtonTap ()
		{
			UIManager.Instance.lobbyPanel.gameObject.SetActive (true);
			gameObject.SetActive (false);
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		#region COROUTINES

		#endregion
	}
}
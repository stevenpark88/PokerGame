using UnityEngine;
using System.Collections;

public class WebViewManager : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	[HideInInspector]
	public WebViewObject webViewObject;

	#endregion

	#region PRIVATE_VARIABLES

	#endregion

	public static WebViewManager Instance;

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)
		    && UIManager.Instance.dashboardPanel.gameObject.activeSelf) {
			UIManager.Instance.dashboardPanel.OnBackButtonTap ();
		}
	}

	#endregion

	#region DELEGATE_CALLBACKS

	#endregion

	#region PUBLIC_METHODS

	public void OpenWebviewWithURL (string url, bool isFullScreen = false)
	{
		Debug.Log ("Webview will open with URL  : " + url);

//		if (Application.platform != RuntimePlatform.WebGLPlayer)
//			url += "?source=" + Application.platform;
//		else
//			url += "?source=iframe";

		if (webViewObject != null)
			Destroy (webViewObject.gameObject);

		if ((Application.platform == RuntimePlatform.Android ||
		    Application.platform == RuntimePlatform.IPhonePlayer))
			StartCoroutine (OpenWebView (url, isFullScreen));
		else if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate) {
			Application.ExternalCall ("OpenInNewTab", url);
		} else
			Application.OpenURL (url);
	}

	public void CloseWebview ()
	{
		if (webViewObject != null) {
			webViewObject.SetVisibility (false);
			Destroy (webViewObject.gameObject);
		}

		#if UNITY_IPHONE || UNITY_ANDROID
		Handheld.StopActivityIndicator();
		#endif
	}

	#endregion

	#region PRIVATE_METHODS

	private IEnumerator OpenWebView (string url, bool isFullScreen)
	{
		#if UNITY_IPHONE
		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
		#elif UNITY_ANDROID
		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
		#endif

		#if UNITY_IPHONE || UNITY_ANDROID
		Handheld.StartActivityIndicator();
		#endif

		webViewObject = (new GameObject ("WebViewObject")).AddComponent<WebViewObject> ();
		webViewObject.Init (
			cb: (msg) => {
				Debug.Log (string.Format ("CallFromJS[{0}]", msg));
			},
			err: (msg) => {
				Debug.Log (string.Format ("CallOnError[{0}]", msg));

				#if UNITY_IPHONE || UNITY_ANDROID
				Handheld.StopActivityIndicator();
				#endif
			},
			ld: (msg) => {
				Debug.Log (string.Format ("CallOnLoaded[{0}]", msg));

				#if UNITY_IPHONE || UNITY_ANDROID
				Handheld.StopActivityIndicator();
				#endif

				#if !UNITY_ANDROID
				webViewObject.EvaluateJS (@"
				window.Unity = {
				call: function(msg) {
				var iframe = document.createElement('IFRAME');
				iframe.setAttribute('src', 'unity:' + msg);
				document.documentElement.appendChild(iframe);
				iframe.parentNode.removeChild(iframe);
				iframe = null;
				}
				}
				");
				#endif
			},
			enableWKWebView: true);
		webViewObject.SetMargins (10, GetTopMargin (isFullScreen), 10, 10);
		webViewObject.SetVisibility (true);

		#if !UNITY_WEBPLAYER
		if (url.StartsWith ("http")) {
			webViewObject.LoadURL (url.Replace (" ", "%20"));
		} else {
			var exts = new string[] {
				".jpg",
				".html"  // should be last
			};
			foreach (var ext in exts) {
				var html_url = url.Replace (".html", ext);
				var src = System.IO.Path.Combine (Application.streamingAssetsPath, html_url);
				var dst = System.IO.Path.Combine (Application.persistentDataPath, html_url);
				byte[] result = null;
				if (src.Contains ("://")) {  // for Android
					var www = new WWW (src);
					yield return www;
					result = www.bytes;
				} else {
					result = System.IO.File.ReadAllBytes (src);
				}
				System.IO.File.WriteAllBytes (dst, result);
				if (ext == ".html") {
					webViewObject.LoadURL ("file://" + dst.Replace (" ", "%20"));
					break;
				}
			}
		}
		#else
		if (Url.StartsWith("http")) {
		webViewObject.LoadURL(Url.Replace(" ", "%20"));
		} else {
		webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
		}
		webViewObject.EvaluateJS(
		"parent.$(function() {" +
		"   window.Unity = {" +
		"       call:function(msg) {" +
		"           parent.unityWebView.sendMessage('WebViewObject', msg)" +
		"       }" +
		"   };" +
		"});");
		#endif
	}

	private int GetTopMargin (bool isFullScreen)
	{
		int	baseTopMargin = isFullScreen ? 165 : 265;

		int newMargin = (Screen.height * baseTopMargin) / 1080;
		return newMargin;
	}

	#endregion

	#region COROUTINES

	#endregion
}
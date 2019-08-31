using System.Collections;
using System.IO;
using UnityEngine;
using System;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.UI;

public class DebugLog : MonoBehaviour
{
	//	private static bool debugLog = false;
	//#if !UNITY_EDITOR
	static string debugLog = "";
	public string Subject = "WhoopAss game log";
	public string SenderEmailId = "aistechnolabs12@gmail.com";
	public string SenderEmailIdPassword = "Alliance123";
	public string SenderEmailHost = "smtp.gmail.com";
	public int SenderEmailHostPort = 587;
	public string Recipient = "steveneworld@gmail.com";
	public string Message = "Find the attached game log...";
	public GameObject loadingViewObject;
	static string path;
	static string CorePath;
	static FileInfo filetxt;

	Text alertMessage;
	public static string DeviceStuff;
	static StreamWriter writer;

	void Start ()
	{

		path = Application.persistentDataPath + "/" + "GameLog.txt";
		Log ("Log file location : " + path);
		DeviceStuff = "---------------Device----------------\nDevice-Model=" + SystemInfo.deviceModel.ToString () + "\nDevice-OS=" + SystemInfo.operatingSystem + "\nDevice-Memory=" + SystemInfo.systemMemorySize + "\n---------------Device----------------";
		alertMessage = loadingViewObject.GetComponentInChildren<Text> ();
		Log ("Log file location : " + path);
		initLogFile ();	

	}

	// Use this for initialization
	void OnEnable ()
	{
		Application.logMessageReceivedThreaded += Log;
	}

	void OnDisable ()
	{
		Application.logMessageReceivedThreaded -= Log;
//		filetxt.Delete ();
		using (StreamWriter writer =
			       new StreamWriter (path)) {
			writer.Write ("");
		}
	}

	void initLogFile ()
	{
		Log ("Log file creating : Init >>>>>>> " + File.Exists (path));

		// Check log file is exit or not
		filetxt = new FileInfo (path);
		/*if(!filetxt.Exists)
			filetxt.Create ();
		*/
		if (!filetxt.Exists) {
			writer = filetxt.CreateText ();
		} else {
			filetxt.Delete ();
			//w = filetxt.CreateText();
			writer = filetxt.CreateText ();
		}
		Log ("Log file " + filetxt.Exists);

		writer.WriteLine (DeviceStuff);
		//#if (UNITY_ANDROID || UNITY_IOS )&& !UNITY_EDITOR 
		//		path =Application.persistentDataPath + filetxt.ToString ();
		//	#else
		path = filetxt.ToString ();
		//	#endif
		Debug.Log ("Log file created :<><><> " + filetxt.ToString ());
	}

	public void Log (string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception) {
			Log ("-----------Exception -----------------" + logString + " >> " + stackTrace);
			Log (logString);
			Log (stackTrace);
			Log ("-----------End Of Exception -----------------" + stackTrace);
		}
	}

	void OnGUI ()
	{
//		if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
		{
			if (GUI.Button (new Rect (5, 10, 70, 50), isLogEnable ? "Hide Log" : "Log")) {
				isLogEnable = !isLogEnable;
			}

			if (isLogEnable) {
//				GUI.TextArea (new Rect (10, 100, Screen.width - 20, Screen.height - 20), debugLog);
				if (debugLog.Length > 15000) {
					debugLog = debugLog.Substring (0, 10000);
				}
				GUI.Label (new Rect (10, 100, Screen.width - 20, Screen.height - 20), debugLog);
				if (GUI.Button (new Rect (85, 10, 70, 50), "Clear")) {
					debugLog = "";
				}
				if (GUI.Button (new Rect (170, 10, 70, 50), "MailLog")) {
					StartCoroutine (SendEmail ());
//					SendEmail();
				}
			}
		}
	}
	//#endif
	bool isLogEnable = false;

	public static void Log (object message, UnityEngine.Object context = null)
	{
		string log = "\n[" + System.DateTime.Now + "] " + message + "\n";
		#if UNITY_EDITOR
		Debug.Log (message, context);
		#else
			debugLog  =  log + debugLog; 
		#endif
		logWrite (log);
	}

	public static void LogWarning (object message, UnityEngine.Object context = null)
	{
		string log = "\n[" + System.DateTime.Now + "] " + "Warning :>> " + message + "\n";
		#if UNITY_EDITOR
		Debug.LogWarning (message, context);
		#else
			debugLog  = log + debugLog; 
		#endif
		logWrite (log);
	}

	public static void LogError (object message, UnityEngine.Object context = null)
	{
		string log = "\n[" + System.DateTime.Now + "] " + "Error :>> " + message + "\n";
		#if UNITY_EDITOR
		Debug.LogError (message, context);
		#else
			debugLog  =log+ debugLog; 
		#endif
		logWrite (log);
	}

	static void logWrite (string log)
	{
		if (writer != null && filetxt.Exists) {

			writer.WriteLine (log);
		} else
			debugLog = "Log file not created " + debugLog; 
	
	}

	IEnumerator SendEmail ()
	{
		
//		writer.Flush ();
		//writer.WriteLine (DeviceStuff);
		alertMessage.text = "Sending logs on mail...";
		Log ("Sending logs on mail...");
		writer.Close ();

		yield return new WaitForSeconds (2f);

		#if UNITY_IOS || UNITY_WEBGL && UNITY_EDITOR
//		loadingViewObject.GetComponent<DemoScripts>().StopAllCoroutines();

		string BodyString =DeviceStuff;
		BodyString = BodyString.Replace(" ","%20");
		String BODY = WWW.EscapeURL (BodyString);
		//String BODY =BodyString;
		Application.OpenURL ("mailto:steveneworld@gmail.com?subject="+"WhoopAssGamelog&body="+BODY);

	

		#else

		loadingViewObject.SetActive (true);
		//Hardcoded Recipient email and subject and body of the mail
		Attachment att = new Attachment (@path);

		SmtpClient client = new SmtpClient (SenderEmailHost);
		//SMTP server can be changed for gmail, yahoomail, etc., just google it up

		client.Port = SenderEmailHostPort;
		client.Host = SenderEmailHost;
		client.DeliveryMethod = SmtpDeliveryMethod.Network;
		client.UseDefaultCredentials = false;
		System.Net.NetworkCredential credentials = new System.Net.NetworkCredential (SenderEmailId, SenderEmailIdPassword);
		client.EnableSsl = true;
		client.Credentials = (System.Net.ICredentialsByHost)credentials;
	


//		try {
//			var mail = new MailMessage (SenderEmailId.Trim (), Recipient.Trim ());
//			mail.Subject = Subject;
//			mail.Body = Message;
//			//For File Attachment, more files can also be attached
//			using (StreamWriter writer = new StreamWriter (path, true)) {
//				writer.Dispose ();
//				Attachment att = new Attachment (path);
//				//tested only for files on local machine
//				mail.Attachments.Add (att);
//				Debug.Log ("Attachment is now Online");
//				alertMessage.text = "Log is attached...";
//				ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
//					return true;
//				};
//
//				client.Send (mail);
//				alertMessage.text = "Log sent..";
//				att.Dispose ();
//				Debug.Log ("Mail send.........");
//
//			}
		try {
			var mail = new MailMessage (SenderEmailId.Trim (), Recipient.Trim ());
			mail.Subject = Subject;
			mail.Body = Message;
			mail.Attachments.Add (att);
			Debug.Log ("Attachment is now Online");
			alertMessage.GetComponentInChildren<Text> ().text = "Log is attached...";
			ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
				return true;
			};

			client.Send (mail);
			alertMessage.GetComponentInChildren<Text> ().text = "Log sent..";
			att.Dispose ();
			Debug.Log ("Mail send.........");

		

		} catch (Exception ex) {
			Console.WriteLine (ex.Message);
			throw ex;
		}
//		yield return new WaitForSeconds (2f);
		loadingViewObject.SetActive (false);
//		filetxt.Delete ();	
		writer = null;
		//initLogFile ();
		#endif
	}
}
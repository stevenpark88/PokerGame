
using UnityEngine;
using System;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
//https://serverfault.com/questions/635139/how-to-fix-send-mail-authorization-failed-534-5-7-14
//https://www.linkedin.com/pulse/code-send-e-mail-attachment-from-unity-c-using-smtp-om-shridhar
//https://gist.github.com/NovaSurfer/0c24b4ab5578814d2d1d

public class MailerScript : MonoBehaviour
{
	string _sender = "";
	string _password = "";

	public MailerScript ()
	{
		_sender = "aistechnolabs12@gmail.com";
		_password = "Alliance123";
	}

	void Start ()
	{
		Debug.Log ("Send mail....");
		SendEmail ();
	}

	private void SendEmail ()
	{
		
		string path = "Assets/Resources/gamelog.txt";
		string _sender = "aistechnolabs12@gmail.com";
		string _password = "Alliance123";

		//For File Attachment, more files can also be attached
		Attachment att = new Attachment (@path);
		//tested only for files on local machine



		//Hardcoded recipient email and subject and body of the mail
		string recipient = "chirag@aistechnolabs.us";
		string subject = "This is test mail from the Unity";
		string message = "FTA...";

		SmtpClient client = new SmtpClient ("smtp.gmail.com");
		//SMTP server can be changed for gmail, yahoomail, etc., just google it up


		client.Port = 587;
		client.DeliveryMethod = SmtpDeliveryMethod.Network;
		client.UseDefaultCredentials = false;
		System.Net.NetworkCredential credentials = new System.Net.NetworkCredential (_sender, _password);
		client.EnableSsl = true;
		client.Credentials = (System.Net.ICredentialsByHost)credentials;

		try {
			var mail = new MailMessage (_sender.Trim (), recipient.Trim ());
			mail.Subject = subject;
			mail.Body = message;
			mail.Attachments.Add (att);
			Debug.Log ("Attachment is now Online");
			ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
				return true;
			};

			client.Send (mail);
			Debug.Log ("Success");
		} catch (Exception ex) {
			Console.WriteLine (ex.Message);
			throw ex;
		}
	}
}
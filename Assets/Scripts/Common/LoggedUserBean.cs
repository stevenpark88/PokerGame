using UnityEngine;
using System.Collections;

public class LoggedUserBean 
{
	public static int userId = 0;
	public static string userName = "";
	public static string fullName = "";
	public static string avatarUrl = "";
	public static int totalBalance = 0;

	public void resetData(){
		userId = 0;
		userName = "";
		fullName = "";
		avatarUrl = "";
		totalBalance = 0;
	}

}


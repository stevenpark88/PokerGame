using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserTurnAnim : MonoBehaviour {

	public GameObject[] usersArray;
	int turnUser = 0;
	float waitingTime = 20.0f;
	bool userActive = false;

	// Use this for initialization
	void Start () 
	{
		//userActive = true;

	}

	// Update is called once per frame
	void Update () 
	{
		if(userActive)
		{			
			Image img = usersArray[turnUser].transform.GetChild(2).GetComponent<Image>();
		//	img.enabled = true;
						
			img.fillAmount += 1.0f/waitingTime * Time.deltaTime;
			
			string s1 = img.fillAmount.ToString();
			string s2 = s1.Substring(0,4);
			Debug.Log(s2);
			if(s2 == "0.25")
			{
				img.color = new Color(1, 0.92f, 0.016f, 0.5f);
			}
			if(s2 == "0.50")
			{
				img.color = new Color(0, 1, 0, 0.5f);
			}
			if(s2 == "0.75")
			{
				img.color = new Color(0, 1, 1, 0.5f);
			}
			if(s2 == "0.99")
			{
				userActive = false;
				img.enabled = false;
			}
		}
	}
}

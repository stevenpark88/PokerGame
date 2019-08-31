using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
	public AnimationManager(){
	}

//	IEnumerator cardAnimation (PlayersManager playerManager)
//	{
//		yield return StartCoroutine (DistributeHandCardsToAllPlayers ());
//		yield return new WaitForSeconds (0.1f);
//	}
	
//	IEnumerator DistributeHandCardsToAllPlayers ()
//	{
//		
//		GameObject src = movingCardObjectTmp;	
//		//		GameObject dest = null;
//		//src = gameManagerGameObject.gameObject.transform.Find ("CardDeck");
//		int i = 0;
//		while (i<playersManager.getAllPlayers().Count) {
//			yield return 	StartCoroutine (MoveCardObjectToPlayer (src, playersManager.getAllPlayers () [i].getGameObject ().transform.GetChild (0).transform.GetChild (0).gameObject));
//			yield return new WaitForSeconds (0.4f);
//			playersManager.getAllPlayers () [i].getGameObject ().transform.GetChild (0).transform.GetChild (0).gameObject.SetActive (true);
//			yield return StartCoroutine (MoveCardObjectToPlayer (src, playersManager.getAllPlayers () [i].getGameObject ().transform.GetChild (0).transform.GetChild (1).gameObject));
//			yield return new WaitForSeconds (0.4f);
//			playersManager.getAllPlayers () [i].getGameObject ().transform.GetChild (0).transform.GetChild (1).gameObject.SetActive (true);
//			i++;
//		}
//		
//		movingCardObject.SetActive (false);
//		yield return new WaitForSeconds (0.2f);
//	}
//	
//	IEnumerator MoveCardObjectToPlayer (GameObject src, GameObject dest)
//	{
//		
//		movingCardObject.SetActive (true);
//		
//		movingCardObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
//		movingCardObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
//		handleAnim.GetInstanceCardAnimation ().MoveObject (movingCardObject, dest);
//		
//		//	src.SetActive (false);
//		yield return new WaitForSeconds (0.2f);
//		
//	}
}


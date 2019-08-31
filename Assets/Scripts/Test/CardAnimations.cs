using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardAnimations : MonoBehaviour {

	public List<GameObject> playerLists;
	public List<GameObject> defaultCards;
	public Sprite backCard;
//	public GameObject movePoint;
//	public GameObject moveCoinPoint;
//	public GameObject movePointSecond;
	public Vector3 distributPos;
//	public Vector3 coinPos;

	Transform trsm;
	public GameObject startDistributorPoint;
	// Use this for initialization
	void Start () {
//	
	}

	public void DefaultCardAnimation(string cardName){
		Sprite sp = null;
		GameObject gameObject = null;
		for (int i=0; i<defaultCards.Count; i++) {
			if (cardName.Equals (defaultCards [i].tag)) {
				gameObject = defaultCards [i];
				sp = defaultCards [i].gameObject.GetComponent<Image> ().sprite;
				break;
			}
		}
	
	//	MoveCardTransAnim (sp, gameObject);
	//	FlipCardAnim (sp, gameObject);
		StartCoroutine (MoveAndFlipCardAnim(sp, gameObject));
	}


	public void FlipCardAnim(Sprite sp,GameObject gameObject) {
		gameObject.GetComponent<Image>().sprite = backCard;
		//	yield return new WaitForSeconds (0.0f);
		iTween.RotateBy(gameObject, iTween.Hash("y", .25, "easeType", "easeInOutBack", "loopType", "none", "delay", .4));
		gameObject.GetComponent<Image>().sprite = sp;
		gameObject.transform.Rotate (new Vector3(0f,-90f,0f));
		
	}
	
	IEnumerator MoveAndFlipCardAnim(Sprite sp,GameObject gameObject) {
		gameObject.GetComponent<Image>().sprite = backCard;
		yield return new WaitForSeconds (0.0f);
		iTween.MoveBy(gameObject, iTween.Hash("x",50, "easeType", "easeInOutExpo", "loopType", "none", "delay", .1));
		yield return new WaitForSeconds (0.0f);
		iTween.RotateBy(gameObject, iTween.Hash("y", .25, "easeType", "easeInOutBack", "loopType", "none", "delay", .4));
		gameObject.GetComponent<Image>().sprite = sp;
		gameObject.transform.Rotate (new Vector3(0f,-90f,0f));
	}



	public void MoveObject(GameObject src,GameObject dest) {
		iTween.MoveTo (src, iTween.Hash ("position", dest.transform.position, "easetype", iTween.EaseType.easeInOutBack, "time", 0.5f));
	}

	public void MoveObjectInDirection(GameObject gameObject,int distance,string dir) {
		iTween.MoveBy(gameObject, iTween.Hash(dir,distance, "easeType", "easeInOutExpo", "loopType", "none", "delay", .1));
		
	}

//	public void FlipObject(GameObject gameObject) {
//	    Sprite tempSprite = gameObject.GetComponent<Image> ().sprite;
//		gameObject.GetComponent<Image>().sprite = backCard;
//		movePoint.transform.localPosition = distributPos;
//		iTween.RotateBy(gameObject, iTween.Hash("y", .25, "easeType", "easeInOutBack", "loopType", "none", "delay", .4));
//		gameObject.transform.Rotate (new Vector3(0f,-90f,0f));
//		gameObject.GetComponent<Image>().sprite = tempSprite;
//	}



	// Update is called once per frame
	void Update () {
	
	}
}

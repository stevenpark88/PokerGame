using UnityEngine;
using System.Collections;

public class Spiner : MonoBehaviour {
	public Vector3 spindir;
	public Transform spiningimage;
	public static GameObject obj;
	public GameObject obj2;
	void Awake(){
		Init(gameObject);
	}
	public static void Init(GameObject g){
		obj=g;
		g.SetActive(false);
		g.GetComponent<Spiner>().obj2.SetActive(true);
	}
	void Update () {
		spiningimage.Rotate(spindir,Space.World);
	}

	public static void StartStop(bool start){
		if(obj!=null)obj.SetActive(start);
	}
}

using UnityEngine;
using System.Collections;

public class DEBUG : MonoBehaviour {
	private static DEBUG inst;
	public string msg;
	public static void Log(object obj){
		//return;
		if(inst==null){

			GameObject g=new GameObject("GUILogs");
			DontDestroyOnLoad(g);
			inst=g.AddComponent<DEBUG>();
		}
	
		inst.msg+=obj.ToString()+"\n";
	} 

	void OnGUI(){

//		GUI.Box(new Rect(0.0f,0.0f,Screen.width,Screen.height-100.0f),msg);
//
//		if(GUI.Button(new Rect(0.0f,Screen.height-50,Screen.width,50.0f),"Clear")){
//			Destroy(gameObject);
//		}
	}
}

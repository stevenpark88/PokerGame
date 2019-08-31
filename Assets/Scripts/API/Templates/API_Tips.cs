using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_Tips
{
	public API_Tips(string tips)
	{
		JSONArray arr = new JSONArray (tips);

		tipList = new List<Tip> ();

		for (int i = 0; i < arr.Count (); i++) {
			Debug.Log (arr.getString (i));
			tipList.Add (new Tip (arr.getString (i)));
		}
	}

	public List<Tip> tipList;
}

public class Tip
{
	public Tip(string tip)
	{
		Tip t = JsonUtility.FromJson<Tip> (tip);

		id = t.id;
		title = t.title;
		category = t.category;
		description = t.description;
		created_at = t.created_at;
		updated_at = t.updated_at;
		deleted_at = t.deleted_at;
	}

	public string id;
	public string title;
	public string category;
	public string description;
	public string created_at;
	public string updated_at;
	public string deleted_at;
}
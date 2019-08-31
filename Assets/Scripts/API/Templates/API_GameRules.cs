using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_GameRules
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_GameRules"/> class.
	/// </summary>
	/// <param name="rule">Rule.</param>
	public API_GameRules(string rule)
	{
		JSONArray arr = new JSONArray (rule);

		ruleList = new List<Rule> ();
		for (int i = 0; i < arr.Count (); i++) {
			Debug.Log (arr.getString (i));
			ruleList.Add (new Rule (arr.getString (i)));
		}
	}

	public List<Rule> ruleList;
}

public class Rule
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Rule"/> class.
	/// </summary>
	/// <param name="rule">Rule.</param>
	public Rule(string rule)
	{
		Rule t = JsonUtility.FromJson<Rule> (rule);

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
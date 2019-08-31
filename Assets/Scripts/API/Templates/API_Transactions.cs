using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_Transactions
{
	/// <summary>
	/// Initializes a new instance of the <see cref="API_Transactions"/> class.
	/// </summary>
	/// <param name="transactions">Transactions.</param>
	public API_Transactions(string transactions)
	{
		JSON_Object obj = new JSON_Object (transactions);

		total = obj.getInt ("total");
		total = obj.getInt ("per_page");
		total = obj.getInt ("current_page");
		total = obj.getInt ("last_page");
		next_page_url = obj.getString ("next_page_url");
		prev_page_url = obj.getString ("prev_page_url");
		from = obj.getString ("from");
		to = obj.getString ("to");

		JSONArray arr = obj.getJSONArray ("data");
		transactionList = new List<Transactions> ();
		for (int i = 0; i < arr.Count (); i++) {
//			transactionList.Add (new Transactions (arr.getString (i)));
			transactionList.Add (JsonUtility.FromJson<Transactions> (arr.getString (i)));
		}
	}

	public int total;
	public int per_page;
	public int current_page;
	public int last_page;
	public string next_page_url;
	public string prev_page_url;
	public string from;
	public string to;

	public List<Transactions> transactionList;
}

public class Transactions
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Transactions"/> class.
	/// </summary>
	/// <param name="transaction">Transaction.</param>
	public Transactions(string transaction)
	{
		
	}

	public string id;
	public string user_id;
	public string txn_id;
	public double txn_amount;
	public string chip_type;
	public string txn_type;
	public string cheque_no;
	public double chips_amount;
	public string payment_method;
	public string status;
	public string created_at;
	public string updated_at;
	public string deleted_at;
	public string md_approval_status;
	public string md_routing;
	public string md_currency;
	public string md_approval_code;
	public string md_reference3;
	public string md_card_brand;
	public string md_comment;
	public string md_card_expirydate;
	public string md_tracking_number;
	public string md_signature;
	public string md_fraud_score;
	public string md_cv2_status;
	public string md_timestamp;
	public string md_masked_card_number;
	public string md_reference2;
	public string md_submitter;
	public string md_card_scheme;
	public string md_reference;
	public string md_amount;
	public string md_status;
	public string md_card_holdername;
}
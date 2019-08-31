using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RadioButtons:MonoBehaviour
{
	public string defaultValue;
	
	public UnityEvent onValueChanged;
	[NonSerialized] public string currentValue;
	
	public void ForceToValue(string v)
	{
		_oneRadioButtonLiterallyClicked(v);
	}
	
	///////////////////////////////////////////////////////////////////
	
	void Awake()
	{
		_setup();
		_unselectAll();
		_selectDefault();
	}
	
	//// private ...
	
	void Start()
	{
	}
	
	private void _setup()
	{
		int k=0;
		var tts = gameObject.GetComponentsInChildren<Transform>();
		foreach ( Transform tt in tts )
		{
			if ( tt.GetComponent<Button>() )
			{
				Button bb = tt.GetComponent<Button>();
				string val = tt.name;
				Debug.Log("\t\t\tIn RadioButtons " +gameObject.name +" there's a button: " +val);
				bb.onClick.AddListener(delegate { _oneRadioButtonLiterallyClicked(val); });
				k++;
			}
		}
		Debug.Log("\t\t\tIn RadioButtons " +gameObject.name +" there are count buttons: " +k);
		if (k==0) Debug.Log("\t\t\tYOU HAVE AN EMPTY RadioButton Panel, named " +gameObject.name);
	}
	
	public void _oneRadioButtonLiterallyClicked(string v)
	{
		_unselectAll();
		_selectValue(v);
		onValueChanged.Invoke();
	}
	
	/// set the whole group ...
	
	private void _unselectAll()
	{
		var tts = gameObject.GetComponentsInChildren<Transform>();
		foreach ( Transform tt in tts )
		{
			if ( tt.GetComponent<Button>() )
			{
				Button bb = tt.GetComponent<Button>();
				_unselectedStyle(bb);
			}
		}
	}
	
	private void _selectValue(string v)    // select this named button; and set the currentValue
	{
		var tts = gameObject.GetComponentsInChildren<Transform>();
		foreach ( Transform tt in tts )
		{
			if ( tt.GetComponent<Button>() )
			{
				if ( tt.name == v )
				{
					Button bb = tt.GetComponent<Button>();
					_selectedStyle(bb);
					currentValue = tt.name;
					return;
				}
			}
		}
		Debug.Log("\t\t\tNON-EXISTENT VALUE " +v +" for RadioButton Panel, named " +gameObject.name);
		_selectDefault();
	}
	
	private void _selectDefault()    // if dev has not set default in editor, use first one
	{
		if ( defaultValue == "" )
		{
			_selectFirstOne();
			return;
		}
		
		var tts = gameObject.GetComponentsInChildren<Transform>();
		foreach ( Transform tt in tts )
		{
			if ( tt.GetComponent<Button>() )
			{
				if ( tt.name == defaultValue )
				{
					Button bb = tt.GetComponent<Button>();
					_selectedStyle(bb);
					currentValue = tt.name;
					return;
				}
			}
		}
		Debug.Log("\t\t\tNON-EXISTENT DEFAULT VALUE on RadioButton Panel, named " +gameObject.name);
		currentValue = "";
	}
	
	private void _selectFirstOne()
	{
		var tts = gameObject.GetComponentsInChildren<Transform>();
		foreach ( Transform tt in tts )
		{
			if ( tt.GetComponent<Button>() )
			{
				Button bb = tt.GetComponent<Button>();
				_selectedStyle(bb);
				currentValue = tt.name;
				return;
			}
		}
		Debug.Log("\t\t\tYOU HAVE AN EMPTY RadioButton Panel, named " +gameObject.name);
		currentValue = "";
	}
	
	/// set colors on Button ...
	
	private void _unselectedStyle(Button bb)        // ie, "white"
	{
		ColorBlock cb = bb.colors;
		cb.normalColor = Color.white;
		cb.highlightedColor = Color.white;
		bb.colors = cb;
	}
	
	private void _selectedStyle(Button bb)        // ie, "blue"
	{
		ColorBlock cb = bb.colors;
		cb.normalColor = Color.blue;
		cb.highlightedColor = Color.blue;
		bb.colors = cb;
	}
}


/*
For testing, make a script UnitTest, attach to say the camera.
On your RadioButtons (say, "motorSpeed"), be sure to set the
OnValueChanged callback to
 
using UnityEngine;
 
public class UnitTest:MonoBehaviour
    {
    void Awake()
        {
        InvokeRepeating("_teste",3.0f,3.0f);
        }
 
    void _teste()
        {
        GameObject.Find("motorSpeed").GetComponent<RadioButtons>().ForceToValue("c");
        // "c" is one of the values (that is, the .name) of one of your buttons
        }
 
    public void ClickedExample()
        {
        Debug.Log("Testing: motorSpeed changed to .. "+
            GameObject.Find("motorSpeed").GetComponent<RadioButtons>().currentValue );
        }
    }
*/
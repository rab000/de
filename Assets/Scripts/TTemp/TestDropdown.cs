using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestDropdown : MonoBehaviour {

	public Dropdown d1;

	// Use this for initialization
	void Start () {
		Dropdown.OptionData op1 = new Dropdown.OptionData ();
		op1.text = "a1";
		d1.options.Add (op1);

		Dropdown.OptionData op2 = new Dropdown.OptionData ();
		op2.text = "a2";
		d1.options.Add (op2);

		Dropdown.OptionData op3 = new Dropdown.OptionData ();
		op3.text = "a3";
		d1.options.Add (op3);

		d1.onValueChanged.AddListener (OnChange);
	}
	
	// Update is called once per frame

	public void OnChange(int index){
		Debug.Log ("--->index:"+index);
	}

	void Update () {
		
		Debug.Log ("--->"+d1.options[d1.value].text);
	}
}

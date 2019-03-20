using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SQLiteDatabase;

public class Prototype : MonoBehaviour {
	
	// Variable that stores the console messages
	public string console = "<b>Console:</b>\r\n\r\n";
	
	
	// Function invoked when the program starts
	void Start()
    {
		
		log("== INIT config ==");
		log(" ");


		GameObject.Find("Screens").transform.Find("Console").gameObject.SetActive(true);
		log("    > Console set to active");

		
		
		
		log(" ");
		log("== END config ==");
		log(" ");

    }
	
	// Function invoked at the start of every frame
	void Update()
    {

		
	

    }
	
	//
	public void log(string msg){
		 
		Debug.Log(msg);

		console = console + msg + "\r\n";
		
			
		
		
	}
	
}



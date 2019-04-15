using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class chat : MonoBehaviour
{
	
	// Variable that stores the whole chat
	public string chat_text = "";
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void send_button() {
		
		string input_text =  GameObject.Find("InputField").GetComponent<TMP_InputField>().text;
		 		 
		chat_text += "sent: "+input_text+"\r\n";
		
		update_chat();
		
		Debug.Log("Hi! You pressed send!");
		 
	}
	
	public void button1() {
		
		chat_text += ">>> Button1 clicked\r\n";
		
		update_chat();
		 
	}
	
	public void button2() {
		
		chat_text += ">>> Button 2 clicked\r\n";
		
		update_chat();
		 
	}
	
	public void button3() {
		
		chat_text += ">>> Button 3 clicked\r\n";
		
		update_chat();
		 
	}
	
	private void update_chat() {
		
		GameObject.Find("Chat_text").gameObject.transform.GetComponent<TextMeshProUGUI>().text = chat_text;
				 
	}
	
	
	
}

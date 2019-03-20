using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SQLiteDatabase;

public class Main_script : MonoBehaviour
{
	
    // Variable that stores the console messages
	public string console = "<b>Console:</b>\r\n\r\n";
	
	// console position control : 0 = outside the screen / 1 = inside the screen
	public int console_position = 0;
	
	// COntrols which screen on the lobby is beeing shown
	public string lobby_screen = "";
	
	// Variables to store Screen Size (used to control the resize of the screen window)
	public float screen_width;
	public float screen_height;
	
	// Main Database
	SQLiteDB db = SQLiteDB.Instance;
	
	
	// Function invoked when the program starts
	void Start()
    {
		
		log("== INIT config ==");
		log(" ");

			GameObject.Find("Screens").transform.Find("Console").gameObject.SetActive(true);
			log("    > Console set to active");

			GameObject.Find("Screens/Console").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Screen.height);
			log("    > Console Positioned to : 0," + Screen.height+" ");

			console_position = 0;
			log("    > Console Position set to " + console_position);
			
			Application.targetFrameRate = 30;
			log("    > FrameRate set to " + Application.targetFrameRate);
			
			screen_width = Screen.width;
			log("    > screen_width set to " + screen_width);
			
			screen_height = Screen.height;
			log("    > screen_height set to " + screen_height);
			
			// MAIN DATABASE
			
			db.DBName = "Main.db";
			db.DBLocation = Application.persistentDataPath;
			bool result = db.CreateDatabase(db.DBName, false); // false = do not overwrite if it exists already
			
			if(result == true){
				
				log("    > Main DB CREATED at: " + Application.persistentDataPath);		
				
			}else{
			
				log("    > Main DB creation error: " + result);		
							
			}
			
			
		log(" ");
		log("== END config ==");
		log(" ");

			// Initiate the app showing the lobby
			lobby("Show");
			
			

		
    }
	
	// Shows one screen from the lobby
	// This function is called either inside this code or directly from buttons (onclick method)
	public void lobby(string screen){

		log("== INIT Lobby ==");
		log(" ");
		
			if(screen == "Show"){
				
				GameObject.Find("Screens").transform.Find("Login").gameObject.SetActive(true);
				log("    > Login set to active");
				
				GameObject.Find("Screens").transform.Find("Create_account").gameObject.SetActive(true);
				log("    > Create_account set to active");

				GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(true);
				log("    > Restore_account set to active");
				
				// position screens of the Lobby
				
				GameObject.Find("Screens/Login").GetComponent<RectTransform>().anchoredPosition = new Vector2( 0, 0 );
				log("    > Login position to ("+( Screen.width * -1 )+",0)");
				
				GameObject.Find("Screens/Create_account").GetComponent<RectTransform>().anchoredPosition = new Vector2( Screen.width, 0 );
				log("    > Create_account position to ("+Screen.width+",0)");
				
				GameObject.Find("Screens/Restore_account").GetComponent<RectTransform>().anchoredPosition = new Vector2( Screen.width, 0);
				log("    > Restore_account position to ("+Screen.width+",0)");
							
				lobby_screen = "Login";
				log("    > lobby screen set to 'Login'");
				
				focus("Input_login");
								
			}else if(screen == "Hide"){
				
				GameObject.Find("Screens").transform.Find("Login").gameObject.SetActive(false);
				log("    > Login set to inactive");
				
				GameObject.Find("Screens").transform.Find("Create_account").gameObject.SetActive(false);
				log("    > Create_account set to inactive");

				GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(false);
				log("    > Restore_account set to inactive");
				
				lobby_screen = "";
				log("    > lobby screen set to ''");
				
			}else{
													
				// ht stores parameters for the tween animation
				Hashtable ht;
			
				if(lobby_screen != "" && lobby_screen != screen){
					
					ht = new Hashtable();
					
					ht.Add("time", 0.3);
					ht.Add("easetype", "easeOutCirc");
					
					if(lobby_screen == "Login"){
						
						ht.Add("x", ( Screen.width *  -1 ) ); // left of the screen
						
													
					}else if(lobby_screen == "Create_account"){
						
						ht.Add("x", ( Screen.width * 1 ) ); // right of the screen
																
					}else if(lobby_screen == "Restore_account"){
						
						ht.Add("x", ( Screen.width * 1 ) ); // right of the screen
											
					}else{
						
						log("    > ERROR [#lobby001] : Current lobby Unknown: " + lobby_screen);
						
					}
					
									
					iTween.MoveTo(GameObject.Find(lobby_screen), ht);
					
					log("    > Lobby Screen Tween : "+lobby_screen);
					
					
											
				}
				
				if(lobby_screen != screen){
					
					ht = new Hashtable();
					
					ht.Add("time", 0.3);
					ht.Add("easetype", "easeOutCirc");
					ht.Add("x", 0 );
					
					if(screen == "Login" || screen == "Create_account" || screen == "Restore_account" ){
					
						iTween.MoveTo(GameObject.Find(screen), ht);
						
						lobby_screen = screen;
					
						log("    > Screen Tween : "+lobby_screen);
						
						if(lobby_screen == "Login"){
							
							focus("Input_login");
							
						}
						
						if(lobby_screen == "Create_account"){
							
							focus("Input_new_login");
							
						}
						
						if(lobby_screen == "Restore_account"){
							
							focus("Input_email");
							
						}
										
					}else{
						
						log("    > ERROR [#lobby002] : Unknown screen: "+screen);
						
					}
										
				}
								
			}
		
		log(" ");
		log("== END Lobby ==");
		log(" ");
	
	}
	
	// Function invoked at the start of every frame
	void Update()
    {
			
		// Detects if the the screen has been resized
		
		int flag  = 0;
		
		if(screen_height != Screen.height){
			
			log("== INIT screen resize ==");
			log(" ");
			
				flag = 1;
			
		}else{
			
			if(screen_width != Screen.width){
			
				log("== INIT screen resize ==");
				log(" ");

					flag = 1;
				
			}
			
		}
		
		// If the screen has been resized reposition the screens been shown
		if(flag == 1){
			
				// Console
				
				if(console_position == 0){
					
					GameObject.Find("Screens/Console").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Screen.height);
					log("    > Console Positioned to : 0," + Screen.height+" ");
					
				}else if(console_position == 1){
					
					GameObject.Find("Screens/Console").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
					log("    > Console Positioned to : 0,0");
										
				}
				
				if(lobby_screen != ""){
					
					lobby(lobby_screen);
										
				}			
				
				screen_width = Screen.width;
				log("    > screen_width set to " + screen_width);
							
				screen_height = Screen.height;
				log("    > screen_height set to " + screen_height);
			
				
			log(" ");
			log("== END screen resize ==");
			log(" ");	
			
			
			
		}
		
		// detect F1 key pressed for console
		if (Input.GetKeyDown(KeyCode.F1))
        {
			
			log(" ");
			log("== F1 key pressed ==");
			log(" ");
			
			// ht stores parameters for the tween animation
			Hashtable ht = new Hashtable();

			ht.Add("time", 0.3);
			ht.Add("easetype", "easeOutCirc");
			
			if(console_position == 1){
				
				ht.Add("y", ( Screen.height * 2) );
				
				console_position = 0;
				
			}else{
				
				ht.Add("y", ( Screen.height * 1));
				
				console_position = 1;
								
			}
			
			iTween.MoveTo(GameObject.Find("Console"), ht);
			
        }		
		
    }
	
	// Log Console Function
	public void log(string msg){
		 
		Debug.Log(msg);

		console = console + msg + "\r\n";
		
		if(GameObject.Find("Console") != null){
			
			GameObject.Find("Console/Text").GetComponent<TMP_InputField>().text = console;	
					
		}
				
	}
	
	// UI support functions
	
	// Helper function to set focus on input field
	public void focus(string target){
				
		GameObject.Find(target).GetComponent<TMP_InputField>().Select();
		GameObject.Find(target).GetComponent<TMP_InputField>().ActivateInputField();
				
	}
	
	public void getvalue(string target){
				
				
	}
	
	
}

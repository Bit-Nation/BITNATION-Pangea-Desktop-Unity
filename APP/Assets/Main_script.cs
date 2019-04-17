using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SQLiteDatabase;
using SimpleJSON;

public class Main_script : MonoBehaviour
{
	
	// production enviroment : 1 = true / 0 = false (test enviroment)
	private int production = 0;
	
		
	
    // Variable that stores the console messages
	private string console = "<b>Console:</b>\r\n\r\n";
		
	// console position control : 0 = outside the screen / 1 = inside the screen
	private int console_position = 0;
	
	// COntrols which screen on the lobby is beeing shown
	private string lobby_screen = "";
	
	// Variables to store Screen Size (used to control the resize of the screen window)
	private float screen_width;
	private float screen_height;
	
	
	
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
			
			if(result == true)
			{
				
				log("    > Main DB CREATED at: " + Application.persistentDataPath);		
				
			}
			else
			{
			
				log("    > Main DB creation error: " + result);		
							
			}
			
			
		log(" ");
		log("== END config ==");
		log(" ");

			// Initiate the app showing the lobby
			lobby("Login");
			
			// hides the message screen
			message("hide", "");

		
    }
	
	// Shows one screen from the lobby
	// This function is called either inside this code or directly from buttons (onclick method)
	public void lobby(string screen)
	{

		log("== INIT Lobby ==");
		log(" ");
									
				GameObject.Find("Screens").transform.Find("Login").gameObject.SetActive(false);
				log("    > Login set to inactive");
				
				GameObject.Find("Screens").transform.Find("Create_account").gameObject.SetActive(false);
				log("    > Create_account set to inactive");

				GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(false);
				log("    > Restore_account set to inactive");
				
				// This shows the proper screen and sets focus to the proper input field
				// If you pass Hide or any other parameter then it will not show any screen from the lobby
						
				if(screen == "Login")
				{
					
					GameObject.Find("Screens").transform.Find("Login").gameObject.SetActive(true);
					log("    > Login set to active");
					
					focus("Input_login");
					log("    > Focus set to Input_login");
					
				}
				else if(screen == "Create")
				{
					
					GameObject.Find("Screens").transform.Find("Create_account").gameObject.SetActive(true);
					log("    > Create_account set to active");
					
					focus("Input_name");
					log("    > Focus set to Input_name");
										
				}
				else if(screen == "Restore")
				{
					
					GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(true);
					log("    > Restore_account set to active");
					
					focus("Input_email");
					log("    > Focus set to Input_email");
					
					
				}
				
			
		log(" ");
		log("== END Lobby ==");
		log(" ");
	
	}
	
	
	// Shows messages on top of all other screens
	public void message(string screen, string message)
	{
		
		log("== INIT message ==");
		log(" ");
		
			if(screen == "hide")
			{
				
				GameObject.Find("Screens").transform.Find("Message").gameObject.SetActive(false);
				log("    > Message set to inactive");
								
			}
			else
			{
				
				// Activate the main message screen with black background
				
				GameObject.Find("Screens").transform.Find("Message").gameObject.SetActive(true);
				log("    > Message set to active");
									
				// Sets all sub screen to inactive
				
				GameObject.Find("Screens/Message").transform.Find("Loading").gameObject.SetActive(false);
				log("    > Message/Loading set to inactive");				
				
				if(screen == "loading")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Loading").gameObject.SetActive(true);
					log("    > Message/Loading set to active");		
					
					GameObject.Find("Screens/Message/Loading/Message").GetComponent<TextMeshProUGUI>().text = message;
										
				}
								
			}
					
		log(" ");
		log("== END message ==");
		log(" ");
		
	}
		
	// Function invoked at the start of every frame
	void Update()
    {
			
		// Detects if the the screen has been resized
		
		int flag  = 0;
		
		if(screen_height != Screen.height)
		{
			
			log("== INIT screen resize ==");
			log(" ");
			
				flag = 1;
			
		}
		else
		{
			
			if(screen_width != Screen.width)
			{
			
				log("== INIT screen resize ==");
				log(" ");

					flag = 1;
				
			}
			
		}
		
		// If the screen has been resized reposition the screens been shown
		if(flag == 1)
		{
			
				// Console
				
				if(console_position == 0)
				{
					
					GameObject.Find("Screens/Console").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Screen.height);
					log("    > Console Positioned to : 0," + Screen.height+" ");
					
				}
				else if(console_position == 1)
				{
					
					GameObject.Find("Screens/Console").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
					log("    > Console Positioned to : 0,0");
										
				}
				
				if(lobby_screen != "")
				{
					
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
			
			if(console_position == 1)
			{
				
				ht.Add("y", ( Screen.height * 2) );
				
				console_position = 0;
				
			}
			else
			{
				
				ht.Add("y", ( Screen.height * 1));
				
				console_position = 1;
								
			}
			
			iTween.MoveTo(GameObject.Find("Console"), ht);
			
        }		
		
    }
	
	
	// Click Functions
	
	public void login()
	{
		
		message("loading", "Connecting to server ...");
		
		string json = "{\"function\":\"login\"}";		
				
		StartCoroutine(apicom(json));
			
	}
	
	public void tab(string tab_id)
	{
		
		// clears everything

		log("== INIT Tab ==");
		log(" ");

			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_contacts").gameObject.SetActive(false);
			log("    > Sidebar_contacts set to inactive");
			
			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_nations").gameObject.SetActive(false);
			log("    > Sidebar_nations set to inactive");
			
			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_settings").gameObject.SetActive(false);
			log("    > Sidebar_settings set to inactive");
			
			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_menu").gameObject.SetActive(false);
			log("    > Sidebar_menu set to inactive");
				
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab1").GetComponent<Image>().color = new Color(0.9137256f, 0.9137256f, 0.9137256f, 1f);
			log("    > Tab1 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab1").GetComponent<Image>().color);
			
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab2").GetComponent<Image>().color = new Color(0.9137256f, 0.9137256f, 0.9137256f, 1f);
			log("    > Tab2 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab2").GetComponent<Image>().color);
			
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab3").GetComponent<Image>().color = new Color(0.9137256f, 0.9137256f, 0.9137256f, 1f);
			log("    > Tab2 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab3").GetComponent<Image>().color);
			
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab4").GetComponent<Image>().color = new Color(0.9137256f, 0.9137256f, 0.9137256f, 1f);
			log("    > Tab2 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab4").GetComponent<Image>().color);
			
				
		
		if(tab_id == "Tab1")
		{
			
			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_menu").gameObject.SetActive(true);
			log("    > Sidebar_menu set to active");
			
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab1").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			log("    > Tab1 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab1").GetComponent<Image>().color);
			
			
			
		}
		else if(tab_id == "Tab2")
		{
			
			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_contacts").gameObject.SetActive(true);
			log("    > Sidebar_contacts set to active");
			
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab2").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			log("    > Tab1 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab2").GetComponent<Image>().color);
			
			
		}
		else if(tab_id == "Tab3")
		{
			
			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_nations").gameObject.SetActive(true);
			log("    > Sidebar_nations set to active");
			
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab3").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			log("    > Tab1 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab3").GetComponent<Image>().color);
			
		}
		else if(tab_id == "Tab4")
		{
			
			GameObject.Find("Screens/Main/Sidebar").transform.Find("Sidebar_settings").gameObject.SetActive(true);
			log("    > Sidebar_settings set to active");
			
			GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab4").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			log("    > Tab1 color set to : "+GameObject.Find("Screens/Main/Sidebar/Tab_bar/Tab4").GetComponent<Image>().color);
			
		}
		
		
	}
		
	
	// Api Response functions
	
	private void login_response(SimpleJSON.JSONNode r)
	{
		
		// message("loading", "Updating information...");
		
		lobby("Hide");
		
		message("hide", "");
				
		GameObject.Find("Screens").transform.Find("Main").gameObject.SetActive(true);
		log("    > Main set to active");
			
		tab("Tab1");
		
	}
	
	
	// Communication functions
	
	// Api com function
	IEnumerator apicom(string json)
    {
		
		log("== INIT apicom ==");
		log(" ");
		
		string api_url = "";
		
		// Sets the api URl according to the production enviroment
		
		if(production == 1)
		{
			
			api_url = "https://n1.nortrix.net/apps/bitnation/api.php";
			
		}
		else if(production == 0)
		{
			
			api_url = "https://n1.nortrix.net/apps/bitnation/api.php";
			
		}
		else
		{
			
			log("    > ERROR [#apicom001] : Invalid production enviroment : " + production);
						
		}
		
		// Put is used to send raw data. (Json string in our case)
		// The other possible method is POST as variables in a form.
		
		WWWForm form = new WWWForm();
        form.AddField("json", json);
		
        using (UnityWebRequest www = UnityWebRequest.Post(api_url, json))
        {
           
			yield return www.Send();
			
			if (www.isNetworkError || www.isHttpError)
            {
				
				log("    > ERROR [#apicom002] : Network error : " + www.error);
			
				//// SHOW ERROR MESSAGE SCREEN HERE!
                
            }
			else
			{
				
				// Show results as text
                log("    > Apicom response : " + www.downloadHandler.text);
			
				// r stores the response from the api call inside an array
                var r = JSON.Parse(www.downloadHandler.text);
				
				if (r == null)
                {

					// Returns null on empty or malformatteed json string
					log("    > ERROR [#apicom003] : Unknown response error.");
			
                }
				else
				{
					
					if (r["error"] == "yes")
                    {
					
						//// SHOW ERROR MESSAGE SCREEN HERE!
                					
					}
					else
					{
						
						if(r["function"] == "login")
						{
														
							login_response(r); // response
							
						}
						else
						{
							
							log("    > ERROR [#apicom003] : Unknown response error.");
							
							//// SHOW ERROR MESSAGE SCREEN HERE!
							
							// test change
                									
						}
						
						
						
					}
					
				}
						
				
			}// end yield
		
        } // end request
    
		log(" ");
		log("== END apicom ==");
		log(" ");
			
	}

	
	// Log Console Function
	public void log(string msg)
	{
		 
		Debug.Log(msg);

		console = console + msg + "\r\n";
		
		if(GameObject.Find("Console") != null)
		{
			
			GameObject.Find("Console/Text").GetComponent<TMP_InputField>().text = console;	
					
		}
				
	}
	
	// UI support functions
	
	// Helper function to set focus on input field
	public void focus(string target)
	{
				
		GameObject.Find(target).GetComponent<TMP_InputField>().Select();
		GameObject.Find(target).GetComponent<TMP_InputField>().ActivateInputField();
				
	}
	
	public string getvalue(string target)
	{
				
		return GameObject.Find(target).GetComponent<TMP_InputField>().text;
		
	}
	
	
}

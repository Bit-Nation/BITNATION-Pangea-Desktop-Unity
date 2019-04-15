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
			lobby("Show");
			
			

		
    }
	
	// Shows one screen from the lobby
	// This function is called either inside this code or directly from buttons (onclick method)
	public void lobby(string screen)
	{

		log("== INIT Lobby ==");
		log(" ");
		
			if(screen == "Show")
			{
				
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
								
			}
			else if(screen == "Hide")
			{
				
				GameObject.Find("Screens").transform.Find("Login").gameObject.SetActive(false);
				log("    > Login set to inactive");
				
				GameObject.Find("Screens").transform.Find("Create_account").gameObject.SetActive(false);
				log("    > Create_account set to inactive");

				GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(false);
				log("    > Restore_account set to inactive");
				
				lobby_screen = "";
				log("    > lobby screen set to ''");
				
			}
			else
			{
													
				// ht stores parameters for the tween animation
				Hashtable ht;
			
				if(lobby_screen != "" && lobby_screen != screen)
				{
					
					ht = new Hashtable();
					
					ht.Add("time", 0.3);
					ht.Add("easetype", "easeOutCirc");
					
					if(lobby_screen == "Login")
					{
						
						ht.Add("x", ( Screen.width *  -1 ) ); // left of the screen
						
													
					}
					else if(lobby_screen == "Create_account")
					{
						
						ht.Add("x", ( Screen.width * 1 ) ); // right of the screen
																
					}
					else if(lobby_screen == "Restore_account")
					{
						
						ht.Add("x", ( Screen.width * 1 ) ); // right of the screen
											
					}
					else
					{
						
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
					
					if(screen == "Login" || screen == "Create_account" || screen == "Restore_account" )
					{
					
						iTween.MoveTo(GameObject.Find(screen), ht);
						
						lobby_screen = screen;
					
						log("    > Screen Tween : "+lobby_screen);
						
						if(lobby_screen == "Login")
						{
							
							focus("Input_login");
							
						}
						else if(lobby_screen == "Create_account")
						{
							
							focus("Input_new_login");
							
						}						
						else if(lobby_screen == "Restore_account")
						{
							
							focus("Input_email");
							
						}
										
					}
					else
					{
						
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
	
	// API communication protocol
	
	/*
	
	StartCoroutine(comunica2(node, "sys_c:remove_id|profile:" + reader.GetStringValue(node) + "|hash:" + reader.GetStringValue(node + "_hash"))); //   comunica2(node, "profile:"+profile+"|password:"+password+""); // nodeid, vars
	
	var resp = JSON.Parse(v);

	int result = db.Update("UPDATE profiles SET  " + resp["node"] + "='', " + resp["node"] + "_hash=''  WHERE id = '" + profile_atual + "'");

	
	*/
	
	// Click Functions
	
	public void login()
	{
				
		string json = "{\"function\":\"login\"}";		
				
		StartCoroutine(apicom(json));
			
	}
	
	
	// Return functions
	
	private void login_response(SimpleJSON.JSONNode r)
	{
		
		log(" ***************** CHEGOU NO LOGIN RESPONSE : "+r["error_message"]);
		
		
	}
	
	
	// COM functions
	
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
							
							log("    > ERROR 1 [#apicom003] : Unknown response error.");
							
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
		
		/*
		
			****** TRASH STUFF (here to remember if necessary later)
			
			string api_url = "";

			WWWForm form = new WWWForm();
			form.AddField("json", vars);
			WebRequest.Put(string url, string data);
			
			____________
			
			

		   yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log("COM RESULT: "+www.downloadHandler.text);

                var resp = JSON.Parse(www.downloadHandler.text);

                // Debug.Log("TYPE: "+resp.GetType());

                if (resp == null)
                {

                    // Debug.Log("FIM : NULL ");

                    view_message("Message", "", "<b>ERROR:</b>", "Unknown server error.", "", "Ok");


                }
                else {

                    if (resp["error"] != null)
                    {

                        if (resp["error_message1"] != null && resp["error_message2"] != null)
                        {

                            view_message("Message", "", "<b>ERROR:</b>", resp["error_message1"], resp["error_message2"], "Ok");

                        }
                        else
                        {

                            view_message("Message", "", "<b>ERROR:</b>", "Bad Server answer", "", "Ok");

                        }

                    }
                    else {

                        if (resp["sys_c"] == null)
                        {

                            view_message("Message", "", "<b>ERROR:</b>", "Invalid server response.", "", "Ok");

                        }
                        else
                        {

                            if (resp["sys_c"] == "identity_added")
                            {

                                identity_added(www.downloadHandler.text);
                            }
                            else if (resp["sys_c"] == "profile_removed")
                            {

                                delete_id_ok(www.downloadHandler.text);
                            }
                            else if (resp["sys_c"] == "profile_updated")
                            {

                                Debug.Log("Profile Updated");

                            }
                            else if (resp["sys_c"] == "contact_added")
                            {

                                contact_added(www.downloadHandler.text);

                            }
                            else if (resp["sys_c"] == "notifications_back")
                            {

                                notifications_back(www.downloadHandler.text);

                            }
                            else if (resp["sys_c"] == "app_response")
                            {

                                if (resp["back"] == "deploy")
                                {

                                    new_dapp_received(www.downloadHandler.text);
                                    dapp_log(www.downloadHandler.text);

                                }
                                else if (resp["back"] == "action")
                                {

                                    dapp_log(www.downloadHandler.text);

                                }


                            }
                            else if (resp["sys_c"] == "get_dapp")
                            {

                                if (resp["response"] == "OK")
                                {

                                    string flag1 = "0";

                                    DBReader reader = db.Select("Select * from contacts WHERE atlantis_id ='" + resp["id"] + "'");

                                    while (reader != null && reader.Read())
                                    {
                                        flag1 = "1";
                                    };

                                    if (flag1 == "0")
                                    {

                                        int result2 = db_lastid("contacts");

                                        db.Insert("INSERT INTO contacts VALUES ('" + result2 + "','" + resp["name"] + "','2','" + resp["execnode1"] + "','" + resp["execnode2"] + "','" + resp["execnode3"] + "','" + resp["id"] + "') ");

                                   
                                    }

                                }

                                if (add_dapp == "0") {

                                    add_dapp = "1";

                                } else if (add_dapp == "1") {

                                    add_dapp = "2";

                                } else if (add_dapp == "2") {

                                    add_dapp = "3";

                                }

                                if (add_dapp == "3") {

                                    string flag1 = "0";

                                    Debug.Log("ID: " + resp["id"] + "");

                                    DBReader reader = db.Select("Select * from contacts WHERE atlantis_id ='" + resp["id"] + "'");

                                    while (reader != null && reader.Read())
                                    {
                                        flag1 = "1";
                                    };

                                    if (flag1 == "0")
                                    {

                                        view_message("Message", "", "DAPP not found.", "", "", "Ok");

                                    }
                                    else
                                    {

                                        view_message("Message", "", "DAPP added to your", "address book.", "", "Ok");

                                    }

                                    atualiza_address();
                                    novo_contato();
                                    atualiza_inbox();


                                }

                            }
                            else
                            {

                                view_message("Message", "", "<b>ERROR:</b>", "Invalid server call.", "[" + resp["sys_c"] + "]", "Ok");

                            }

                            // view_message("Message", "", "<b>Resposta ok!</b>", "", "", "Ok");

                        }


                    }

                    

                    // Debug.Log("FIM : " + resp["teste1"]);
                    
                }


                //// Verificação de erros




                // Or retrieve results as binary data
                // byte[] results = www.downloadHandler.data;
            }
			
			
		*/
	
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

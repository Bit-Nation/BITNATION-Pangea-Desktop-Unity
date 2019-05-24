using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SQLiteDatabase;
using SimpleJSON;
using System.Text;
using System.IO;  
using System.Xml; 
using System.Xml.Serialization; 
using UnityEngine.SceneManagement;

public class Main_script : MonoBehaviour
{
	
	// app version
	private string app_version = "1.0";
	
	// production enviroment : 1 = true / 0 = false (test enviroment)
	private int production = 0;
	
	
	// PREFABS
	
	public Transform page_image;
	public Transform page_text;
	public Transform page_input;
	public Transform page_button;
	public Transform page_bar;
	public Transform page_area;
	public Transform area_nation;
	
	
	
    // Variable that stores the console messages
	private string console = "Console:\r\n\r\n";
		
	// console position control : 0 = outside the screen / 1 = inside the screen
	private int console_position = 0;
	
	// Controls which screen on the lobby is beeing shown
	private string lobby_screen = "";
	
	// Controls which screen on the main screen is beeing shown
	private string main_screen = "";
	
	
	// Variables to store Screen Size (used to control the resize of the screen window)
	private float screen_width;
	private float screen_height;
	
	
	private string token;
	
	
	// Main Database
	SQLiteDB db = SQLiteDB.Instance;
		
	// Api com
	
	string current_apicom = "";
	
	// Function invoked when the program starts
	void Start()
    {
		
		log("== INIT config ==\r\n");
		
			GameObject.Find("Screens").transform.Find("Console").gameObject.SetActive(true);
			log("    > Console set to active");

			GameObject.Find("Screens/Console").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Screen.height);
			log("    > Console Positioned to : 0," + Screen.height+" ");

			console_position = 0;
			log("    > Console Position set to " + console_position);
			
			Application.targetFrameRate = 30;
			log("    > FrameRate set to " + Application.targetFrameRate);
			
			QualitySettings.vSyncCount = 0;
			log("    > vSyncCount set to " + QualitySettings.vSyncCount);
	
			QualitySettings.asyncUploadTimeSlice = 32;
			QualitySettings.asyncUploadBufferSize = 32;
			QualitySettings.asyncUploadPersistentBuffer = true;
			
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
				
				create_tables();
				
			}
			else
			{
			
				log("    > Main DB creation error: " + result);		
							
			}
			
			
		log("\r\n== END config ==\r\n");
		
			// Create main pages (Settings, account, wallets, etc...
			create_pages();
					
			// Initiate the app showing the lobby
			lobby("Login");
			
			// hides the message screen
			message("hide", "");
			
			// hides the Main Screen
			GameObject.Find("Screens").transform.Find("Main").gameObject.SetActive(false);
			log("    > Main set to inactive");

			
			
			//// TESTS
			
			// downloadImage("test_image.png");		
			
			
    }
	
	// Shows one screen from the lobby
	// This function is called either inside this code or directly from buttons (onclick method)
	public void lobby(string screen)
	{

		log("== INIT Lobby ==\r\n");
									
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
					
					focus("Input_new_citzen_id"); // Input_new_name1
					log("    > Focus set to Input_new_citzen_id");
										
				}
				else if(screen == "Restore")
				{
					
					GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(true);
					log("    > Restore_account set to active");
					
					focus("Input_email");
					log("    > Focus set to Input_email");
					
					
				}
				
			
		log("\r\n== END Lobby ==\r\n");
		
	}
		
	// Shows messages on top of all other screens
	private void message(string screen, string message)
	{
		
		log("== INIT message ==\r\n");
		
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

				GameObject.Find("Screens/Message").transform.Find("Error").gameObject.SetActive(false);
				log("    > Message/Error set to inactive");						
				
				GameObject.Find("Screens/Message").transform.Find("Version").gameObject.SetActive(false);
				log("    > Message/Version set to inactive");						
				
				if(screen == "loading")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Loading").gameObject.SetActive(true);
					log("    > Message/Loading set to active");		
					
					GameObject.Find("Screens/Message/Loading/Message").GetComponent<TextMeshProUGUI>().text = message;
										
				}
				else if(screen == "error")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Error").gameObject.SetActive(true);
					log("    > Message/Error set to active");		
					
					GameObject.Find("Screens/Message/Error/Message").GetComponent<TextMeshProUGUI>().text = message;
										
				}
				else if(screen == "version")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Version").gameObject.SetActive(true);
					log("    > Message/Version set to active");		
										
				}
								
			}
					
		log("\r\n== END message ==\r\n");
		
	}
	
	public void loadpage(string name){
		
		log("== INIT loadpage ==\r\n");
		
		log("    > Page name: " + name);

		XmlDocument xmlDoc = new XmlDocument(); 
		
		try
        {
			
			xmlDoc.Load( Application.persistentDataPath + "/"+name+".txt" ); 
					
        }
        catch (Exception e)
        {
            
			log("\r\n**********\r\n ERROR : File not found. ("+name+")\r\n**********\r\n");
		
			return;
		
        }
		
		// variables used in the page load
		
		Transform obj;
		Texture2D texture;
		string first_field = ""; // used to give focus to the first input field created
		
		XmlNodeList pagelist = xmlDoc.GetElementsByTagName("page");
		
		foreach (XmlNode pageinfo in pagelist) {
			
			if(pageinfo.Attributes["title"] != null){
			
				set_text("Screens/Main/Main_content_page/Title/Title_text", pageinfo.Attributes["title"].Value);
				
				log("    > Page title set to: "+get_text("Screens/Main/Main_content_page/Title/Title_text")); 

				
			}else{
				
				set_text("Screens/Main/Main_content_page/Title/Title_text", "#error");
				
				log("\r\n**********\r\n WARNING : Page title not found. (page:"+name+")\r\n**********\r\n");
		
			}
			
			clear("Screens/Main/Main_content_page/Content/Viewport/Content");
						
			XmlNodeList pagecontent = pageinfo.ChildNodes;
			
			foreach (XmlNode pageitems in pagecontent) {
								
				// log(" PAGE :::::: Child name: "+pageitems.Name+" - id : "+pageitems.Attributes["id"].Value+" ");
		
				if(pageitems.Name == "image")
				{
					
					obj = Instantiate(page_image, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);

					// change the name of the image for reference

					if(pageitems.Attributes["id"] != null)
					{
						
						obj.name = "page_" + pageitems.Attributes["id"].Value;
											
					}else
					{
						
						obj.name = "page_image";
												
					}
					
					// change the image sprite
					
					if(pageitems.Attributes["src"] != null)
					{
						
						texture = new Texture2D(2, 2);
						texture.LoadImage(loadImage(pageitems.Attributes["src"].Value));
							
						obj.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f));
											
					}
					
					if(pageitems.Attributes["sprite"] != null)
					{
						
							
						obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/"+pageitems.Attributes["sprite"].Value);
											
					}
										
					if(pageitems.Attributes["link_page"] != null)
					{
						
						obj.GetComponent<Button>().onClick.AddListener( delegate { loadpage(pageitems.Attributes["link_page"].Value); });
					
					}
					
					if(pageitems.Attributes["align"] != null)
					{
						
						if(pageitems.Attributes["align"].Value == "left")
						{
							
							obj.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
						
						}
						else if(pageitems.Attributes["align"].Value == "center")
						{
							
							obj.GetComponent<RectTransform>().pivot = new Vector2((float)(0.5), 0);
												
						}
						else if(pageitems.Attributes["align"].Value == "right")
						{
							
							obj.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
													
						}
						
																	
					}
					
					log("    > Page image created"); 

								
				}
				else if(pageitems.Name == "area")
				{
					
					obj = Instantiate(page_area, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);

					if(pageitems.Attributes["id"] != null)
					{
						
						// obj.name = "page_" + pageitems.Attributes["id"].Value;
						
						obj.name = "page_"+Time.unscaledTime+"_" + pageitems.Attributes["id"].Value;
						
						// clear("Screens/Main/Main_content_page/Content/Viewport/Content/page_"+pageitems.Attributes["id"].Value+"/Scroll View/Viewport/Content");
											
					}else
					{
						
						obj.name = "page_area";
												
					}
											
					log("    > Page area created"); 
					
				}
				else if(pageitems.Name == "area_nation")
				{
					
					obj = Instantiate(area_nation, new Vector3(0, 0, 0), Quaternion.identity);
					
					// obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);

					if(pageitems.Attributes["id"] != null)
					{
						
						obj.name = "page_" + pageitems.Attributes["id"].Value;
											
					}else
					{
						
						obj.name = "page_area_nations";
												
					}
					
					if(pageitems.Attributes["target"] == null)
					{
						
						log("\r\n**********\r\n Error : Area_nation target not provided.\r\n**********\r\n");
								
					}else{
						
						log("    > TARGET : "+pageitems.Attributes["target"].Value+"");
						
						
						// obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content/page_"+pageitems.Attributes["target"].Value+"/ScrollView/Viewport/Content").gameObject.transform, true);
						
						obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content/page_"+Time.unscaledTime+"_"+pageitems.Attributes["target"].Value+"/ScrollView/Viewport/Content").gameObject.transform, true);
						
						/*
						if(pageitems.Attributes["link_page"] != null)
						{
							
							obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener( delegate { loadpage(pageitems.Attributes["link_page"].Value); });
						
						}
						*/
						
						obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = pageitems.InnerText.Replace("{l}", "<").Replace("{b}", ">");
												
						log("    > Page area_nation created"); 
						
					}
									
				}
				else if(pageitems.Name == "bar")
				{
					
					obj = Instantiate(page_bar, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);

					obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = pageitems.InnerText.Replace("{l}", "<").Replace("{b}", ">");
										
					
					if(pageitems.Attributes["height"] != null)
					{
						
							
						obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, float.Parse(pageitems.Attributes["height"].Value));
											
					}else{
						
						obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 120);
												
					}											
										
					log("    > Page bar created"); 
				
				}
				else if(pageitems.Name == "button")
				{
					
					obj = Instantiate(page_button, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);

					if(pageitems.Attributes["link_page"] != null)
					{
						
						obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener( delegate { loadpage(pageitems.Attributes["link_page"].Value); });
					
					}
					
					if(pageitems.Attributes["label"] != null)
					{
						
						obj.transform.Find("Button/Label").GetComponent<TextMeshProUGUI>().text = pageitems.Attributes["label"].Value;
											
					}else
					{
						
						obj.transform.Find("Button/Label").GetComponent<TextMeshProUGUI>().text = "#error";
																		
					}
					
					log("    > Page button created"); 

					
				}
				else if(pageitems.Name == "input")
				{
					
					obj = Instantiate(page_input, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);
					
					// change name of the input field for reference
					
					if(pageitems.Attributes["id"] != null)
					{
						
						obj.transform.Find("Input").name = "page_" + pageitems.Attributes["id"].Value;
								
						if(first_field == ""){
												
							first_field = pageitems.Attributes["id"].Value;
							
						}
								
					}else
					{
						
						obj.transform.Find("Input").name = "page_input";
												
					}
					
					// change the text of the label 
					
					if(pageitems.Attributes["label"] != null)
					{
						
						obj.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = pageitems.Attributes["label"].Value;
						
											
					}else
					{
						
						obj.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "#error";
											
					}
										
					log("    > Page input created"); 
					
				}
				else if(pageitems.Name == "text")
				{
					
					obj = Instantiate(page_text, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);

					// change thte text

					obj.GetComponent<TextMeshProUGUI>().text = pageitems.InnerText.Replace("{l}", "<").Replace("{b}", ">");
										
					// Change the name of the text for reference
					
					if(pageitems.Attributes["id"] != null)
					{
						
						obj.name = "page_" + pageitems.Attributes["id"].Value;
											
					}else
					{
						
						obj.name = "page_text";
												
					}
					
					// change text alignment
									
					if(pageitems.Attributes["align"] != null)
					{
						
						if(pageitems.Attributes["align"].Value == "left")
						{
							
							obj.GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Left;
						
						}
						else if(pageitems.Attributes["align"].Value == "center")
						{
							
							obj.GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Center;
													
						}
						else if(pageitems.Attributes["align"].Value == "right")
						{
							
							obj.GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Right;
													
						}
						else if(pageitems.Attributes["align"].Value == "justified")
						{
							
							obj.GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Justified;
													
						}
																	
					}
					
					if(pageitems.Attributes["size"] != null)
					{
									
						obj.GetComponent<TextMeshProUGUI>().fontSize = float.Parse(pageitems.Attributes["size"].Value);
											
					}
					
					log("    > Page text created"); 

					
				}				
				
			}			
			
		}
		
		// give focus
		
		if(first_field != ""){
			
			focus("page_"+first_field);
			
		}
		
		// log("\r\n== Page spot 1: "+GameObject.Find("page_spot1").gameObject+" ==\r\n");
		
		

		log("\r\n== END loadpage ==\r\n");
				
	}
	
	// Function invoked at the start of every frame
	void Update()
    {
			
		// Detects if the the screen has been resized
		
		int flag  = 0;
		
		if(screen_height != Screen.height)
		{
			
			log("== INIT screen resize ==\r\n");
			
				flag = 1;
			
		}
		else
		{
			
			if(screen_width != Screen.width)
			{
			
				log("== INIT screen resize ==\r\n");
				
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
					
					// lobby(lobby_screen);
										
				}

				if(main_screen != ""){
					
					GameObject.Find("Screens/Main/Main_content_page").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), Screen.height);
					log("    > Main_content_page size changed to : "+GameObject.Find("Screens/Main/Main_content_page").GetComponent<RectTransform>().sizeDelta);
					
					GameObject.Find("Screens/Main/Main_content_page/Title").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), (float)(43.0));
					log("    > Main_content/Title size changed to : "+GameObject.Find("Screens/Main/Main_content_page/Title").GetComponent<RectTransform>().sizeDelta);
					
					GameObject.Find("Screens/Main/Main_content_page/Content").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), (float)(Screen.height-43.0));
					log("    > Main_content/Content size changed to : "+GameObject.Find("Screens/Main/Main_content_page/Content").GetComponent<RectTransform>().sizeDelta);
					
					
				}
				
				screen_width = Screen.width;
				log("    > screen_width set to " + screen_width);
							
				screen_height = Screen.height;
				log("    > screen_height set to " + screen_height);
			
				
			log("\r\n== END screen resize ==\r\n");
			
		}
		
		// detect F1 key pressed for console
		if (Input.GetKeyDown(KeyCode.F1))
        {
			
			log("\r\n== F1 key pressed ==\r\n");
			
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
	
	// ***********************************
	// 		Interactive Functions
	// ***********************************
	
	public void close_message()
	{
		
		message("hide", "");
				
	}
		
	// Logs out and resets the app
	public void logout()
	{
		
		SceneManager.LoadScene( SceneManager.GetActiveScene().name );
				
	}
	
	// Click on the login button
	public void login()
	{
		
		message("loading", "Connecting to server ...");
		
		string json = "{\"request\": \"login\",\"username\": \""+get_value("Input_login")+"\",\"password\": \""+get_value("Input_password")+"\"} ";		
			
		// {\"function\":\"login\"}
		
		// current_apicom = "login";
			
		StartCoroutine(apicom(json,"/auth/login"));
			
	}
	
	// 
	public void new_user()
	{
		
		if(get_value("Input_new_password") != get_value("Input_new_repassword")){
			
			message("error", "Retyped password mismatch.");
		
			return;
			
		}
		
		message("loading", "Connecting to server ...");
		
		string json = "{\"request\": \"new_user\","+
		
			"\"username\": \""+get_value("Input_new_name")+"\","+
			
			"\"citzen_id\": \""+get_value("Input_new_citzen_id")+"\","+
			
			"\"password\": \""+get_value("Input_new_password")+"\","+
			
			"\"email\": \""+get_value("Input_new_email")+"\""+
						
		"} ";		
				
		StartCoroutine(apicom(json,""));
			
	}
	
	public void update_app()
	{
		
		Application.OpenURL("http://tse.bitnation.co/");
				
	}	
	
	// Change the text from the create new user screen
	public void change_id()
	{
		
		set_text("Text_full_citzen_id", get_value("Input_new_citzen_id")+"@bitnation");
		
	}
		
	// Click on the login button
	public void retrieve()
	{
		
		message("loading", "Connecting to server ...");
		
		string json = "{ \"request\": \"retrieve\",\"email\": \""+get_value("Input_email")+"\" } ";		
			
		StartCoroutine(apicom(json,""));
			
	}		
		
	public void tab(string tab_id)
	{
		
		// clears everything

		log("== INIT Tab ==\r\n");
		
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
		
		log("\r\n== END Tab ==\r\n");
		
	}
	
	public void menu_page(string name){
		
		if(name == "settings")
		{
			
			loadpage("settings");
			
		}
		else if(name == "account")
		{
			
			loadpage("account");
						
		}
		else if(name == "wallets")
		{
			
			loadpage("wallets");
						
		}
		else if(name == "townhall")
		{
			
			loadpage("townhall");
						
		}
		else if(name == "nations")
		{
			
			loadpage("nations");
						
		}
		else if(name == "govmarketplace")
		{
			
			loadpage("govmarketplace");
						
		}
		else if(name == "proposals")
		{
			
			loadpage("proposals");
						
		}
		else if(name == "constitution")
		{
			
			loadpage("constitution");
						
		}
		else
		{
			
			log("\r\n**********\r\n Error : Invalid Page ("+name+")\r\n**********\r\n");
			
		}
		
		
	}
	
	
	// ***********************************
	// 		API Response Functions
	// ***********************************
	
	private void login_response(SimpleJSON.JSONNode r)
	{
		
		
		log("== INIT login_response ==\r\n");
		
		log("    > Response Status: " + r["status"]);
		
			token = r["token"];
			log("    > Token set to : "+token);
			
			//// User configuration
			
			// user id
			
			// DBReader reader = db.Select("Select * from contacts WHERE atlantis_id ='" + resp["id"] + "'");
				
			
			/*
			
			//// Load the main screen
					
			lobby("Hide");
		
			message("hide", "");
					
			GameObject.Find("Screens").transform.Find("Main").gameObject.SetActive(true);
			log("    > Main set to active");
			
			GameObject.Find("Screens/Main/Main_content_page").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), Screen.height);
			log("    > Main_content_page size changed to : "+GameObject.Find("Screens/Main/Main_content_page").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens/Main/Main_content_page/Title").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), 43);
			log("    > Main_content/Title size changed to : "+GameObject.Find("Screens/Main/Main_content_page/Title").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens/Main/Main_content_page/Content").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), (float)(Screen.height-43.0));
			log("    > Main_content/Content size changed to : "+GameObject.Find("Screens/Main/Main_content_page/Content").GetComponent<RectTransform>().sizeDelta);
				
			main_screen = "Home";
			
			tab("Tab1");
			
			loadpage("townhall");
						
			*/
		
		log("\r\n== END login_response ==\r\n");
					
	}
	
	// ***********************************
	// 		Comunication Functions
	// ***********************************
	
	// Api com function
	IEnumerator apicom(string json, string function)
    {
		
		log("== INIT apicom ==\r\n");
		
		
		string api_url = "";
		
		// Sets the api URl according to the production enviroment
		
		if(production == 1)
		{
			
			// api_url = "http://bitnation-backend.herokuapp.com/auth/login";
			api_url = "http://bitnationapi.azurewebsites.net/api.php";
			
		}
		else if(production == 0)
		{
			
			api_url = "http://bitnationapi.azurewebsites.net/api.php";
			
		}
		else
		{
			
			log("    > ERROR [#apicom001] : Invalid production enviroment : " + production);
						
		}
		
		// Put is used to send raw data. (Json string in our case)
		// The other possible method is POST as variables in a form.
		
		WWWForm form = new WWWForm();
		
        form.AddField("json", json);
		form.AddField("app_version", app_version);
		// form.headers ["email"] = "teste";
		// form.headers ["password"] = "teste";
		
		/*
		Dictionary<string, string> headers = form.headers;
		headers["email"] = "teste";
		headers["password"] = "teste";
		*/
		
        using (UnityWebRequest www = UnityWebRequest.Post(api_url, form))
        {
           
		   
		   
			yield return www.Send();
			
			log("    > API request sent.");
			
			
			log("\r\n== END apicom ==\r\n");
		
			
			if (www.isNetworkError || www.isHttpError)
            {
				
				log("    > ERROR [#apicom002] : Network error : " + www.error);
			
				//// SHOW ERROR MESSAGE SCREEN HERE!
                
            }
			else
			{
				
				log("== INIT apicom response ==\r\n");
		
					
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
					
					
					if (r["error"] == "true")
                    {
					
						//// SHOW ERROR MESSAGE SCREEN HERE!
                		
						message("error", r["error_message"]);
											
						
					}
					else if (r["error"] == "version")
                    {
					
						//// SHOW ERROR MESSAGE SCREEN HERE!
                		
						message("version", r["error_message"]);
											
						
					}
					else
					{
						
						if(r["request"] == "login")
						{
							
							login_response(r);
							
						}
						else if(r["request"] == "new_user")
						{
							
							lobby("Login");
							message("error", "New user sucessfully created!");
													
						}
						else if(r["request"] == "retrieve")
						{
							
							message("error", "Check your email with the information.");
													
						}
						else
						{
							
							message("error", r["error_message"]);
							
							log("    > ERROR [#apicom003] : Unknown response error.");
							
							
						}
												
					}					
					
					log("\r\n== END response ==\r\n");
					
				}
						
				
			}// end yield
		
        } // end request
    
		// log("\r\n== END apicom ==\r\n");
			
	}

	// Image functions

	private void downloadImage(string image)
	{
		
		log("\r\n== Load Image : "+image+"\r\n");

		
		string api_url = "";
		
		if(production == 1)
		{
			
			api_url = "https://n1.nortrix.net/apps/bitnation/";
			
		}
		else if(production == 0)
		{
			
			api_url = "https://n1.nortrix.net/apps/bitnation/";
			
		}
		else
		{
			
			log("\r\n**********\r\n ERROR [#downloadimage001] : Invalid production enviroment : "+production+"\r\n**********\r\n");
								
		}
		
		
		WWW www = new WWW(api_url+image);
		StartCoroutine(_downloadImage(www, image));
		
	}
	
	private IEnumerator _downloadImage(WWW www, string name)
	{
		
		yield return www;

		//Check if we failed to send
		if (string.IsNullOrEmpty(www.error))
		{
		
			File.WriteAllBytes(Application.persistentDataPath + "/"+name, www.bytes);
			
			log("\r\n== Image downloaded : "+name+"\r\n");
					
		}
		else
		{
			
			log("\r\n**********\r\n ERROR [#downloadimage002] : Download error: "+www.error+"\r\n**********\r\n");
		
		}
		
	}
	
	byte[] loadImage(string image)
	{
		byte[] dataByte = null;

		if (!File.Exists(Application.persistentDataPath+"/"+image))
		{
			
			log("\r\n**********\r\n ERROR [#loadimage001] : File does not exist: "+image+"\r\n**********\r\n");
			return null;
			
		}

		try
		{
			
			dataByte = File.ReadAllBytes(Application.persistentDataPath+"/"+image);
			
			log("\r\n== Image loaded : "+name+"\r\n");
						
		}
		catch (Exception e)
		{
			
			log("\r\n**********\r\n ERROR [#loadimage002] : Failed To Load Data from : "+image+" \r\n "+e.Message+"\r\n**********\r\n");
			
		}

		return dataByte;
	}
	
	// Log Console Function
	private void log(string msg)
	{
		 
		Debug.Log(msg);

		console = console + msg + "\r\n";
		
		if(GameObject.Find("Console") != null)
		{
			
			GameObject.Find("Console/Text").GetComponent<TMP_InputField>().text = console;	
					
		}
				
	}
	
	// ***********************************
	// 		Support Functions
	// ***********************************
	
	// Helper function to set focus on input field
	private void focus(string target)
	{
		if( GameObject.Find(target).GetComponent<TMP_InputField>() != null )
        {
        	GameObject.Find(target).GetComponent<TMP_InputField>().Select();
			GameObject.Find(target).GetComponent<TMP_InputField>().ActivateInputField();
		}
        else
        {
        	log("\r\n**********\r\n ERROR : Focus Object not found: "+target+"\r\n**********\r\n");
		}
				
	}
	
	// Get the value of an input field
	private string get_value(string target)
	{
				
		return GameObject.Find(target).GetComponent<TMP_InputField>().text;
		
	}
	
	// Get the value of an input field
	private void set_text(string target, string text)
	{
		
		if(GameObject.Find(target).GetComponent<TextMeshProUGUI>() != null){
			
			GameObject.Find(target).GetComponent<TextMeshProUGUI>().text = text;
		
		}
		else
		{
			
			log("\r\n**********\r\n ERROR : Text Object not found: "+target+"\r\n**********\r\n");
			
		}		
		
	}

	
	// Get the text from a text area
	private string get_text(string target)
	{
		
			return GameObject.Find(target).GetComponent<TextMeshProUGUI>().text;
		
		
	}
	
	// removes all child objects
	private void clear(string target)
	{
		
			
		if(GameObject.Find(target).gameObject.transform != null){
			
			Transform obj = GameObject.Find(target).gameObject.transform;

			foreach (Transform child in obj)
			{
				GameObject.Destroy(child.gameObject);
			}
			
		}
		else
		{
			
			log("\r\n**********\r\n ERROR : Object not found: "+target+"\r\n**********\r\n");
						
		}		
		
		
		
		
	}
	
	// ***********************************
	// 		Configuration Functions
	// ***********************************
	
	// This function creates the main pages of the system, like settings, wallets, etc...
	private void create_pages()
	{
		
		log("== INIT create_pages ==\r\n");
		
		string text = "";
		
		if(loadfile("townhall") == null){
			
			
			text = 
			
				"<page title=\"Townhall\">"+
				
				"<image id=\"banner\" sprite=\"banner_townhall\" align=\"center\"></image>"+
				"<bar>{l}b{b}# News and Updates from Bitnation{l}/b{b}</bar>"+
				"<text align=\"justified\" size=\"18\">"+
				"\r\n"+
				"02/05/2019 - Desktop app updated!\r\n\r\n"+
				"Aenean vitae nulla sed enim vehicula tristique. Suspendisse ornare a erat at viverra. Aliquam consectetur sagittis ultricies. Donec vitae blandit orci. Fusce interdum finibus neque id posuere. Nulla ac lorem quam. Maecenas ut molestie libero, gravida finibus risus. Duis elementum mollis lacus, et varius tortor hendrerit commodo. Cras bibendum, sem vitae consectetur bibendum, ipsum tortor efficitur elit, sed ultrices nisi libero vitae urna. Suspendisse et venenatis ex. Phasellus et aliquam diam. Curabitur imperdiet pretium nisi eu tincidunt."+
				"\r\n\r\n"+
				"</text>"+
				
				"<bar>{l}b{b}# Spotlight From Gov Marketplace{l}/b{b}</bar>"+
				
				"<area id=\"gov_spotlight\"></area>"+
			
				"<area_nation id=\"spot1\" target=\"gov_spotlight\">Bit Crypto Courses</area_nation>"+
				
				"<area_nation id=\"spot2\" target=\"gov_spotlight\">Bitnation T-shirts</area_nation>"+
				
				"<area_nation id=\"spot3\" target=\"gov_spotlight\">Bitnation Mug</area_nation>"+
				
				"<area_nation id=\"spot4\" target=\"gov_spotlight\">Bitnation Phone Covers</area_nation>"+
				
				
				"<bar>{l}b{b}# Nation Spotlight{l}/b{b}</bar>"+
				
				"<area id=\"nation_spotlight\"></area>"+
				
				"<area_nation target=\"nation_spotlight\">Catalunya</area_nation>"+
				
				"<area_nation target=\"nation_spotlight\">Howingas</area_nation>"+
				
				"<area_nation target=\"nation_spotlight\">Hawaian Nation</area_nation>"+
				
				"<area_nation target=\"nation_spotlight\">Ideias Radicais</area_nation>"+
				
				
				"</page>";
			
			savefile("townhall", text);
			
			log("    > Page Townhall created");

			
		}
		
		if(loadfile("govmarketplace") == null){
			
			
			text = 
			
				"<page title=\"GovMarketplace\">"+
				
					"<bar>{l}b{b}# Gov Marketplace Spotlight{l}/b{b}</bar>"+
					
					
				
				"</page>";
			
			savefile("govmarketplace", text);
			
			log("    > Page Govmarketplace created");

			
		}
		
		if(loadfile("nations") == null){
			
			
			text = 
			
				"<page title=\"Nations\">"+
				
				"<image id=\"banner\" sprite=\"banner_create_nation\" align=\"center\" link_page=\"nations_new1\"></image>"+
				
				"<bar>{l}b{b}# Nations Spotlight{l}/b{b}</bar>"+
				
				"<area id=\"nation_spotlight\"></area>"+
				
				"<area_nation target=\"nation_spotlight\">Catalunya</area_nation>"+
				
				"<area_nation target=\"nation_spotlight\">Howingas</area_nation>"+
				
				"<area_nation target=\"nation_spotlight\">Hawaian Nation</area_nation>"+
				
				"<area_nation target=\"nation_spotlight\">Ideias Radicais</area_nation>"+
				
				
				"<bar>{l}b{b}# All Nations{l}/b{b}</bar>"+
				
				
				"</page>";
			
			savefile("nations", text);
			
			log("    > Page Nations created");

			
		}
		
		if(loadfile("nations_new1") == null){
			
			
			text = 
			
				"<page title=\"Start a new Nation\">"+
				
				"<text align=\"left\" size=\"18\">"+
				"\r\n{l}b{b}First Select your nation plan:{l}/b{b}"+
				"\r\n\r\n"+
				"</text>"+
				
				"<image id=\"plan1\" sprite=\"plan1\" align=\"left\" link_page=\"nations_new1\"></image>"+
				
				"<image id=\"plan2\" sprite=\"plan2\" align=\"left\" link_page=\"nations_new2\"></image>"+
				
				"<image id=\"plan3\" sprite=\"plan3\" align=\"left\" link_page=\"nations_new3\"></image>"+
				
				"<image id=\"plan4\" sprite=\"plan4\" align=\"left\" link_page=\"nations_new2\"></image>"+
				
				"<image id=\"plan5\" sprite=\"plan5\" align=\"left\" link_page=\"nations_new2\"></image>"+
				
				
				"<button label=\"Go Back\" link_page=\"nations\" vars=\"name\"></button>"+
								
				"</page>";
			
			savefile("nations_new1", text);
			
			log("    > Page Nations_new1 created");

			
		}
		
		if(loadfile("proposals") == null){
			
			
			text = 
			
				"<page title=\"Proposals\">"+
				
				"<bar>{l}b{b}# Proposals{l}/b{b}</bar>"+
				
				"</page>";
			
			savefile("proposals", text);
			
			log("    > Page Proposals created");

			
		}
		
		if(loadfile("constitution") == null){
			
			
			text = 
			
				"<page title=\"Constitution\">"+
				
				"<bar>{l}b{b}# Article 1{l}/b{b}</bar>"+
				
				"<text align=\"justified\" size=\"18\">"+
				"Aenean vitae nulla sed enim vehicula tristique. Suspendisse ornare a erat at viverra. Aliquam consectetur sagittis ultricies. Donec vitae blandit orci. Fusce interdum finibus neque id posuere. Nulla ac lorem quam. Maecenas ut molestie libero, gravida finibus risus. Duis elementum mollis lacus, et varius tortor hendrerit commodo. Cras bibendum, sem vitae consectetur bibendum, ipsum tortor efficitur elit, sed ultrices nisi libero vitae urna. Suspendisse et venenatis ex. Phasellus et aliquam diam. Curabitur imperdiet pretium nisi eu tincidunt."+
				"</text>"+
				
				"<bar>{l}b{b}# Article 2{l}/b{b}</bar>"+
				
				"<text align=\"justified\" size=\"18\">"+
				"Aenean vitae nulla sed enim vehicula tristique. Suspendisse ornare a erat at viverra. Aliquam consectetur sagittis ultricies. Donec vitae blandit orci. Fusce interdum finibus neque id posuere. Nulla ac lorem quam. Maecenas ut molestie libero, gravida finibus risus. Duis elementum mollis lacus, et varius tortor hendrerit commodo. Cras bibendum, sem vitae consectetur bibendum, ipsum tortor efficitur elit, sed ultrices nisi libero vitae urna. Suspendisse et venenatis ex. Phasellus et aliquam diam. Curabitur imperdiet pretium nisi eu tincidunt."+
				"</text>"+
				
				"<bar>{l}b{b}# Article 3{l}/b{b}</bar>"+
				
				"<text align=\"justified\" size=\"18\">"+
				"Aenean vitae nulla sed enim vehicula tristique. Suspendisse ornare a erat at viverra. Aliquam consectetur sagittis ultricies. Donec vitae blandit orci. Fusce interdum finibus neque id posuere. Nulla ac lorem quam. Maecenas ut molestie libero, gravida finibus risus. Duis elementum mollis lacus, et varius tortor hendrerit commodo. Cras bibendum, sem vitae consectetur bibendum, ipsum tortor efficitur elit, sed ultrices nisi libero vitae urna. Suspendisse et venenatis ex. Phasellus et aliquam diam. Curabitur imperdiet pretium nisi eu tincidunt."+
				"</text>"+
				
				"<bar>{l}b{b}# Article 4{l}/b{b}</bar>"+
				
				"<text align=\"justified\" size=\"18\">"+
				"Aenean vitae nulla sed enim vehicula tristique. Suspendisse ornare a erat at viverra. Aliquam consectetur sagittis ultricies. Donec vitae blandit orci. Fusce interdum finibus neque id posuere. Nulla ac lorem quam. Maecenas ut molestie libero, gravida finibus risus. Duis elementum mollis lacus, et varius tortor hendrerit commodo. Cras bibendum, sem vitae consectetur bibendum, ipsum tortor efficitur elit, sed ultrices nisi libero vitae urna. Suspendisse et venenatis ex. Phasellus et aliquam diam. Curabitur imperdiet pretium nisi eu tincidunt."+
				"</text>"+
				
				"<bar>{l}b{b}# Article 5{l}/b{b}</bar>"+
				
				"<text align=\"justified\" size=\"18\">"+
				"Aenean vitae nulla sed enim vehicula tristique. Suspendisse ornare a erat at viverra. Aliquam consectetur sagittis ultricies. Donec vitae blandit orci. Fusce interdum finibus neque id posuere. Nulla ac lorem quam. Maecenas ut molestie libero, gravida finibus risus. Duis elementum mollis lacus, et varius tortor hendrerit commodo. Cras bibendum, sem vitae consectetur bibendum, ipsum tortor efficitur elit, sed ultrices nisi libero vitae urna. Suspendisse et venenatis ex. Phasellus et aliquam diam. Curabitur imperdiet pretium nisi eu tincidunt."+
				"</text>"+
				
				
				"</page>";
			
			savefile("constitution", text);
			
			log("    > Page Proposals created");

			
		}
		
		if(loadfile("settings") == null){
			
			
			text = 
			
				"<page title=\"Settings\">"+
				"<text align=\"left\" size=\"24,5\">"+
				"This is the settings page"+
				"</text>"+
				"<input label=\"Name\" id=\"name\"></input>"+
				"<button label=\"My Button\" link_page=\"wallets\" vars=\"name\"></button>"+
				"</page>";
			
			savefile("settings", text);
			
			log("    > Page Settings created");

			
		}
		
		if(loadfile("wallets") == null){
			
			
			text = 
			
				"<page title=\"Wallets\">"+
				"<text align=\"left\" size=\"24,5\">"+
				"My Wallets"+
				"</text>"+
				"</page>";
			
			savefile("wallets", text);
			
			log("    > Page Wallets created");

			
		}
		
		if(loadfile("account") == null){
			
			
			text = 
			
				"<page title=\"My Account\">"+
				"<text align=\"left\" size=\"24,5\">"+
				"My Account"+
				"</text>"+
				"</page>";
			
			savefile("account", text);
			
			log("    > Page Account created");

			
		}
		
		
		log("\r\n== END create_pages ==\r\n");
		
		
	}
	
	private void create_tables()
	{
		
		log("\r\n== INIT create_tables ==\r\n");
		
			/// TABLE : Users

			DBSchema schema = new DBSchema("users");

			schema.AddField("id", SQLiteDB.DB_DataType.DB_VARCHAR, 9, false, true, true);
			schema.AddField("id_user", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("last_update", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
				   
			bool result = db.CreateTable(schema); // create table

			log("    > Users table created? "+result);
			
			/// TABLE : Users_nations

			schema = new DBSchema("users_nations");

			schema.AddField("id", SQLiteDB.DB_DataType.DB_VARCHAR, 9, false, true, true);
			schema.AddField("id_user", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("id_nation", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("last_update", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("name", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
				   
			result = db.CreateTable(schema); // create table

			log("    > Users_nations table created? "+result);
			
			/// TABLE : Nations_structure

			schema = new DBSchema("nations_structure");

			schema.AddField("id", SQLiteDB.DB_DataType.DB_VARCHAR, 9, false, true, true);
			schema.AddField("id_user", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("id_nation", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("status", SQLiteDB.DB_DataType.DB_VARCHAR, 1, false, false, false);
			schema.AddField("menu_name", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			schema.AddField("menu_type", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			schema.AddField("menu_icon", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			schema.AddField("menu_order", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
				   
			result = db.CreateTable(schema); // create table

			log("    > nations_structure table created? "+result);
			

		log("\r\n== END create_tables ==");
		
		
	}
	
	
	// Saves a txt File
	// TODO: link to the database
	private void savefile(string name, string text)
	{
		
		System.IO.File.WriteAllText( Application.persistentDataPath + "/"+name+".txt", text);
		
	}
	
	// Load txt file
	private string loadfile(string name)
	{
		
		string response = "";
		
		try
        {
			
            StreamReader r = File.OpenText(  Application.persistentDataPath + "/"+name+".txt" ); 
			response = r.ReadToEnd(); 
			r.Close(); 
			
        }
        catch (Exception e)
        {
            
			log("\r\n**********\r\n Warning : File not found. ("+name+")\r\n**********\r\n");

			response = null;
			
        }
		
		return response;
	   
	}
	
	
}

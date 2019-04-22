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
	
	// PREFABS
	
	public Transform page_image;
	public Transform page_text;
	
	
	// production enviroment : 1 = true / 0 = false (test enviroment)
	private int production = 0;
	
		
	
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
	
	
	
	// Main Database
	SQLiteDB db = SQLiteDB.Instance;
	
	
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
			
			
		log("\r\n== END config ==\r\n");
		
			// Initiate the app showing the lobby
			lobby("Login");
			
			// hides the message screen
			message("hide", "");
			
			// hides the Main Screen
			GameObject.Find("Screens").transform.Find("Main").gameObject.SetActive(false);
			log("    > Main set to inactive");


			//// TESTS
			
			downloadImage("test_image.png");		
			
			
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
				
			
		log("\r\n== END Lobby ==\r\n");
		
	}
		
	// Shows messages on top of all other screens
	public void message(string screen, string message)
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
				
				if(screen == "loading")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Loading").gameObject.SetActive(true);
					log("    > Message/Loading set to active");		
					
					GameObject.Find("Screens/Message/Loading/Message").GetComponent<TextMeshProUGUI>().text = message;
										
				}
								
			}
					
		log("\r\n== END message ==\r\n");
		
	}
	
	private void loadpage(string name){
		
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
		
		XmlNodeList pagelist = xmlDoc.GetElementsByTagName("page");
		
		foreach (XmlNode pageinfo in pagelist) {
			
			if(pageinfo.Attributes["title"].Value != null){
			
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

					if(pageitems.Attributes["id"] != null)
					{
						
						obj.name = "page_" + pageitems.Attributes["id"].Value;
											
					}else
					{
						
						obj.name = "page_image";
												
					}
					
					if(pageitems.Attributes["src"] != null)
					{
						
						texture = new Texture2D(2, 2);
						texture.LoadImage(loadImage(pageitems.Attributes["src"].Value));
							
						obj.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f));
											
					}
					
					if(pageitems.Attributes["link"] != null)
					{
						
						obj.GetComponent<Button>().onClick.AddListener( delegate { loadpage(pageitems.Attributes["link"].Value); });
					
					}
								
				}
				else if(pageitems.Name == "text")
				{
					
					obj = Instantiate(page_text, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(GameObject.Find("Screens/Main/Main_content_page/Content/Viewport/Content").gameObject.transform, false);

					

					obj.GetComponent<TextMeshProUGUI>().text = pageitems.InnerText.Replace("{l}", "<").Replace("{b}", ">");
					
					if(pageitems.Attributes["id"].Value != null)
					{
						
						obj.name = "page_" + pageitems.Attributes["id"].Value;
											
					}else
					{
						
						obj.name = "page_text";
												
					}
									
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
					
				}				
				
			}			
			
		}

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
					
					GameObject.Find("Screens/Main/Main_content_page/Title").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), Screen.height);
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
	
	// Click Functions
	
	// Logs out and resets the app
	public void logout()
	{
		
		SceneManager.LoadScene( SceneManager.GetActiveScene().name );
				
	}
	
	public void login()
	{
		
		message("loading", "Connecting to server ...");
		
		string json = "{\"function\":\"login\"}";		
				
		StartCoroutine(apicom(json));
			
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
		
	
	// Api Response functions
	
	private void login_response(SimpleJSON.JSONNode r)
	{
		
		// message("loading", "Updating information.. teest.");
		
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
		
		loadpage("page1");
			
		
	}
	
	
	// Communication functions
	
	// Api com function
	IEnumerator apicom(string json)
    {
		
		log("== INIT apicom ==\r\n");
		
		
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
    
		log("\r\n== END apicom ==\r\n");
			
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
	
	// Support functions
	
	// Helper function to set focus on input field
	private void focus(string target)
	{
				
		GameObject.Find(target).GetComponent<TMP_InputField>().Select();
		GameObject.Find(target).GetComponent<TMP_InputField>().ActivateInputField();
				
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
	
	private string get_text(string target)
	{
		
			return GameObject.Find(target).GetComponent<TextMeshProUGUI>().text;
		
		
	}
	
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
            
			log("\r\n**********\r\n ERROR : File not found. ("+name+")\r\n**********\r\n");

			response = null;
			
        }
		
		return response;
	   
	}
	
	
}

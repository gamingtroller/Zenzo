using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class control : MonoBehaviour {
public Text DisplayText;
public Text inputText;

    /* Arcade Classes */

    // Unique Items
    public class Item
    {
        public string id;   // Unique ID of the item (Combats duplication, makes unique items possible).
        public string name;  // Name of an item (Safe to duplicate on other items).
        public double value; // The forged ZNZ value of the unique item.
        public GameObject obj;   // In-game object of the unique item.
    }

    /* Gameplay Mechanic variables (User, items, spawn points, ect) */

    [Header("API")]
    [Tooltip("User Authentication Key")]
    public string ApiKey = "";
    [Tooltip("The Main API Endpoint URL")]
    public string EndpointMaster = "https://arcade.zenzo.io/api/v1/";

    public InputField ApiKey_Input;
    public Text ArcadeStatus;
    public Text UserGUIBalance;
    public string EndpointKey;
 
        

  
    // User Information
    private string Username = "";
    private string UserId = "";
    private float UserBalance = 0;

 



    // Send a Raw and Custom POST request to the Arcade API
    IEnumerator Post(string endpoint, string json)
    {
        var uwr = new UnityWebRequest(EndpointMaster + endpoint, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        // Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error sending to Arcade: " + uwr.error);
            
        }
        else
        {
            var data = JSON.Parse(uwr.downloadHandler.text);
            //Debug.Log("Processing Arcade Signal: " + uwr.downloadHandler.text);
            ArcadeStatus.text = data.ToString();
            if (endpoint == "ping")
            {
                ArcadeStatus.text ="Arcade API is Active and a connection has been established.";
            }
            else if (endpoint == "account")
            {
                try
                {
                    //Debug.Log("User is " + data["content"]["username"].Value + " with ID " + data["content"]["id"].Value + ", pulling user's forge items.");
                    Username = data["content"]["username"].Value;
                    UserId = data["content"]["id"].Value;
                    //Debug.Log("User has " + data["content"]["balance"].Value + " ZNZ");
                    if (data["content"]["balance"].AsFloat > 0)
                    {
                        UserBalance = data["content"]["balance"].AsFloat;
                        UserGUIBalance.text = "ZNZ: " + UserBalance.ToString();
                    }
                   
                }
                catch (Exception e)
                {
                    ArcadeStatus.text = "Arcade Status: Invalid Key";
                    ArcadeStatus.text="Arcade: Unable to parse user object, probably an incorrect API Key.";
                   
                   
                }
            }
            

        }
    }

    public void Sync()
    {
        //Debug.Log("Arcade: Syncing...");
       
        if (ApiKey_Input.text.Length == 64)
            ApiKey = ApiKey_Input.text;
        StartCoroutine(Post(EndpointKey, "api_key=" + ApiKey));
    }



    public void CheckBalance()
    {
        Sync();

    }

}



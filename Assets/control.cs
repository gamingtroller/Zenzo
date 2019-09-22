using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class control : MonoBehaviour {


     
    public string ApiKey = "";
    
    public string EndpointMaster = "https://arcade.zenzo.io/api/v1/";
    public string EndpointKey;

    public InputField ApiKey_Input;
    public Text Status;
    public Text BalanceText;
    public Text UsernameText;
    public Text IDText;
    public Text AddressText;
    public Text EmailText;
    public Image imageT;
  
 
        

  
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
          
            if (endpoint == "ping")
            {
                Status.text ="Arcade API is Active and a connection has been established.";
            }
            else if (endpoint == "account")
            {
                try
                {

                    Debug.Log(data);

                    //Debug.Log("User has " + data["content"]["balance"].Value + " ZNZ");

                   


                    if (data["content"]["balance"].AsFloat > 0)
                    {
                        UserBalance = data["content"]["balance"].AsFloat;
                        BalanceText.text = "ZNZ: " + UserBalance.ToString();
                    }

                    if (data["content"]["username"] != null)
                    {
                        UsernameText.text = data["content"]["username"].ToString();

                    }

                    if (data["content"]["email"] != null)
                    {
                        EmailText.text = data["content"]["email"].ToString();

                    }

                    if (data["content"]["id"] != null)
                    {
                        IDText.text = data["content"]["id"].ToString();

                    }

                    if (data["content"]["address"] != null)
                    {
                        AddressText.text = data["content"]["address"].ToString();

                    }







                    Status.text = "Data Retrieved Successfully";



                }
                catch (Exception e)
                {
                    Status.text = "Arcade Status: Invalid Key";
                    Status.text="Arcade: Unable to parse user object, probably an incorrect API Key.";
                   
                   
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



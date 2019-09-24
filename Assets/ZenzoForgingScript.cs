using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class ZenzoForgingScript : MonoBehaviour
{





    public InputField apiKey;
    public string apikey;
    public InputField nameItem;
    public InputField imageLink;
    public InputField amountZNZ;
    public Text Balance;
    public Text Status;
    public string EndpointMaster = "https://arcade.zenzo.io/api/v1/";
    private float UserBalance;
    

    public GameObject forgeButtonActivate;


    private void Awake()
    {
        forgeButtonActivate.SetActive(false);
    }


    //the post function to process web callbacks
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
                Status.text = "Arcade API is Active and a connection has been established.";
            }
            else if (endpoint == "account")
            {
                try
                {

                 
                    if (data["content"]["balance"].AsFloat > 0)
                    {
                        UserBalance = data["content"]["balance"].AsFloat;
                        Balance.text = "ZNZ: " + UserBalance.ToString();
                    }
                                                                  

                    
                    
                    Status.text = "Data Retrieved Successfully";
                    forgeButtonActivate.SetActive(true);
                    
                  
                }
                catch (Exception e)
                {
                    
                    Status.text = "Arcade: Unable to parse user object, probably an incorrect API Key.";


                }
            }


        }
    }




  //the login button that checks the user balance
    public void Login()
    {
        if (apiKey.text.Length == 64)
            apikey = apiKey.text;
        StartCoroutine(Post("account", "api_key=" + apikey));

    }



    //the forging button that forge the input 
    public void Forge()
    {

        if (UserBalance <= 1)
            Status.text = "Insufficient Balance!";
        else
            StartCoroutine(Post("forgecreate", "api_key=" + apikey + "&title=M82A1&value=50&image=https://cdn.discordapp.com/attachments/543638439652753409/624233950419877909/M82A1.png"));
    }


}

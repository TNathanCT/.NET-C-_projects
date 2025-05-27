using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Web;
using System.Xml;
using System.Linq;
using System.Text.Json.Nodes;
using Newtonsoft.Json;


[System.Serializable] public class HeartbeatData
	{
		public string email;
		public string game;
		public string os_version;
		public string game_version;
		public string device_id;
		public string firebase_token;
	}



public class APIHandler : MonoBehaviour{
    public static APIHandler instance;
    public static float requestTimer; //PROBABLY SHOULDN'T USE SAME TIMER INCASE SIMULTANEOUS REQUESTS INCREMENT IT
    public static float timeoutLimit = 15.00f;
    public delegate object CustomResponseHandlerDelegate(UnityWebRequest targetRequest);

    public static string apiRoot { //TO ALLOW FOR EASIER SWITCHING BETWEEN STAGING AND PRODUCTION 
  		get {
			  if (!string.IsNullOrWhiteSpace(AppStartHook.APIRootOverride)) {
				  return AppStartHook.APIRootOverride;
			  } else {
				  //return productionAPIURL;

				  // ADD THE /api/v2 AT THE END OF EACH APIROOT
				  return "https://staging.apiroot.com/api/v2";//"https://bedbug-splendid-smoothly.ngrok-free.app/api/v2"; //
				  //return "https://bedbug-splendid-smoothly.ngrok-free.app/api/v2";
			  }
		  }
	  }



    public JsonSerializerSettings jsonSerializerConfig;// = new JsonSerializerSettings{
    //    NullValueHandling = NullValueHandling.Ignore,
    //};
    public static float maxRequestWait{
        get{
            if (AppStartHook.internetConnection){ 
                //USE TIMEOUT LIMIT IF INTERNET CONNECTION IS AVAILABLE, OTHERWISE FORCE TO 0 TO CAUSE AN IMMEDIATE TIMEOUT ERROR
                return timeoutLimit;
            }else{
                return 0f;
            }
        }
    }
    public static string offlineProfilePrefixGeneric = "offline-";//DEC2022 - PREFIX CHANGED TO USE HYPHEN INSTEAD OF UNDERSCORE "Offline_";
    public static string offlineProfilePrefix = "offline_pb";
    public static string offlineProfilePrefixETB = "offline_pbe";
    public static string v6_apichanges_version2string = "version=2"; //FOR SETTING API REQUESTS TO USE VERSION 2 REQUESTS ON THE API (V6 KIDS APP+)
    public static string statsFallbackDirectory{
        get{
            return string.Concat(Application.persistentDataPath + "/stats/");
        }
    }


     public void SendHeartbeatRequest(GameObject rObj, string rFunction, HeartbeatData version){
        string JsonStr = JsonUtility.ToJson (version);
		    string requestString = apiRoot + "/account/heartbeat";
        StartCoroutine(SendAPIRequest<TokenSetupJSON>(rObj,rFunction,RequestType.POST,requestString, JsonStr));
    }

}


//Another script runs this but putting it here for convenience sake

public static void PostLoginStart(bool showLoadingLayer = false)
    {			
		UserAccount.instance.SendVersionRequest (); //SEND REQUEST TO HEARTBEAT ENDPOINT WHEN GOING TO MAIN MENU 
		if (AppStartHook.currentApp == AppStartHook.AppType.ADULTAPP){
			if (GlobalUIController.isLoading){
				GlobalUIController.HideLoadingLayer();
			}
			

		  }
        if (AppStartHook.currentApp == AppStartHook.AppType.KIDSAPP)
        {
            //GO STRAIGHT TO MAIN MENU IN PBAPP
            // Check if user was autologged in please!!!!
            if(showLoadingLayer)
                GlobalUIController.ShowLoadingLayer();
            UICanvas.SendMessageUpwards("StartGamesMenu");
        }
    }

////////////////////////////

public void SendVersionRequest()
	{	
		APIHandler.HeartbeatData version = new APIHandler.HeartbeatData ();
		version.game_version = AppStartHook.appVersion;
		version.os_version = SystemInfo.operatingSystem;
		version.game = Application.productName;
		version.device_id = deviceID;
		version.email = playerEmail;
		//if device denies notifications or have no token, send as null or empty string.
		version.firebase_token = fcmDeviceToken;
		//if the sending is successful,{"response": "successfully saved user data for heartbeat {user.id}"}
		//or if it fails {"response": "failed to save user data for heartbeat {user.id}"}
		if (AppStartHook.currentApp != AppStartHook.AppType.ADULTAPP){
			apiConnect.SendHeartbeatRequest(this.gameObject,"DeviceInfoUpdateResponse",version);		
		}


		
		if (AppStartHook.currentApp == AppStartHook.AppType.ADULTAPP && !System.String.IsNullOrWhiteSpace(fcmDeviceToken)){
			PB_AdultAPIHandler.DeviceInfo currentDeviceInfo = new  PB_AdultAPIHandler.DeviceInfo();
			currentDeviceInfo.device_id = SystemInfo.deviceUniqueIdentifier;
			currentDeviceInfo.device_model = SystemInfo.deviceModel;
			currentDeviceInfo.device_os = SystemInfo.operatingSystem;				
			currentDeviceInfo.firebase_token = fcmDeviceToken;

			apiConnect_PBA.UpdateDeviceInfo(this.gameObject,"DeviceInfoUpdateResponse",currentDeviceInfo);
		}
	}




[System.Serializable] public struct SubscriptionUserInfoUpdateData{
        public string first_name;
        public string last_name;
        public string email;
    }


public IEnumerator SubscriptionManagementUserInfoUpdateRequest(GameObject rObj, string rFunction, string user, SubscriptionUserInfoUpdateData updateData,bool passwordCheck = false,string oldPassword = ""){
        if (!AppStartHook.internetConnection)
        { //CHECK FOR INTERNET CONNECTION
            SetMessage(rObj, rFunction, -1); //FAIL THE REQUEST IF NO INTERNET CONNECTION AVAILABLE
            yield break;
        }
        if (!string.IsNullOrEmpty(AppStartHook.APIUserIDOverride) && AppStartHook.apiOverrideReadOnly){
            //IF API OVERRIDE IS SET TO READ ONLY, FAIL ANY POST/DELETE REQUESTS
            SetMessage(rObj, rFunction, -1); 
            yield break;            
        }
        float thisRequestTimer = 0f;
        if (passwordCheck){ //ONLY DO PASSWORD CHECK FIRST IF IT IS REQUESTED WHEN THE FUNCTION IS CALLED
            bool oldPasswordValid = false;
            //FIRST CHECK IF OLD PASSWORD IS VALID
            // ======================================        
                //BUILD THE REQUEST URL
                AccountLogin login = new AccountLogin ();
                login.username = UserAccount.instance.playerEmail;
                login.password = oldPassword;
                //convert to Json format
                string PL_JsonStr = JsonUtility.ToJson (login);
                string loginURL = string.Concat(ORIGIN.api ,"/account/login" ,"?game_name=" , currentGameID);
                UnityWebRequest loginRequest = BuildBasicWebRequest(RequestType.POST,loginURL,PL_JsonStr,false);
                AsyncOperation loginRequestOp = loginRequest.Send();
                while (!loginRequestOp.isDone){ yield return null; }
                if (GenericRequestSuccessCheck(loginRequest)){
                    oldPasswordValid = true;
                }
            // ======================================
            if (!oldPasswordValid){
                SetMessage(rObj,rFunction,-9);
                yield break; //STOP ATTEMPTED UPDATE HERE IF OLD PASSWORD DOES NOT WORK
            }
        }
        string JsonStr = JsonUtility.ToJson(updateData);
        string requestString = ORIGIN.api + "/subscription_management/" + user;
        UnityWebRequest subManUserInfoUpdateReq = BuildBasicWebRequest(RequestType.POST,requestString,JsonStr);
        AsyncOperation requestOp = subManUserInfoUpdateReq.Send();
        while (!requestOp.isDone){ yield return null; }
        object result = null;
        string jsonRes = subManUserInfoUpdateReq.downloadHandler.text;
		if (GenericRequestSuccessCheck(subManUserInfoUpdateReq)){
            result = jsonRes;//1;//data;
		}else{
            string rStatus = subManUserInfoUpdateReq.responseCode.ToString();
            if (rStatus[0] == cachedChar4){ //BAD
                try{
				    responseFailJSON data = JsonUtility.FromJson<responseFailJSON> (jsonRes);                
                    if (!PBDebug.pbSettingsProductionBuild){PBDebug.Log (data.detail);}
                    result = data;
                }catch{
                    if (jsonRes.Contains("Email is already in use")){
                        result = -3;
                    }
                }
			}else if (rStatus[0] == cachedChar5){  
                result = -1;
			}else{
                result = -2;
			}
        }
        //PREPARE MESSAGE TO RETURN RESULT TO CALLING FUNCTION
        SetMessage(rObj, rFunction, result);
    }


public void UpdateSubscriptionManagementUserInfo(string user,GameObject rObj, string rFunction, SubscriptionUserInfoUpdateData updatedData,bool checkPassword = false,string oldPassword = ""){
        StartCoroutine(SubscriptionManagementUserInfoUpdateRequest(rObj, rFunction, user , updatedData,checkPassword,oldPassword));
    }

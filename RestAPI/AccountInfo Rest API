  public AccountInfo(AccountInfoJSON data){
            token = data.token;
            user_id = data.user_id;
            email = data.email;
            first_name = data.first_name;
            last_name = data.last_name;
            temporary = data.temporary;
            confirmed = data.confirmed;
            language = data.language;
            role = data.role;
            subscriptions = new AccountInfo.subscriptionType[data.subscriptions.Length];
            for (int s = 0; s < data.subscriptions.Length; s++){
                subscriptions[s] = new AccountInfo.subscriptionType();
                subscriptions[s].title = data.subscriptions[s].title;//name;
                subscriptions[s].subscription_id = data.subscriptions[s].subscription_id;//amount;
                subscriptions[s].quantity = data.subscriptions[s].quantity;
                subscriptions[s].status = data.subscriptions[s].status;
                subscriptions[s].includes_dental = data.subscriptions[s].includes_dental;
                subscriptions[s].next_charge_due_at = data.subscriptions[s].next_charge_due_at;
                subscriptions[s].for_adults = data.subscriptions[s].for_adults;
                subscriptions[s].access_level = data.subscriptions[s].access_level; 
                subscriptions[s].created_at = data.subscriptions[s].created_at; 
                subscriptions[s].end_date = data.subscriptions[s].end_date; 
                subscriptions[s].store = data.subscriptions[s].store;
                subscriptions[s].custom_id = data.subscriptions[s].custom_id;
                subscriptions[s].apple_latest_receipt = data.subscriptions[s].apple_latest_receipt; 
                subscriptions[s].price = data.subscriptions[s].price;
                subscriptions[s].latest_price = data.subscriptions[s].latest_price; 
            }
            coupon = new AccountInfo.infoCoupon();
            coupon.amount = data.coupon.amount;
            coupon.code  = data.coupon.code;
            coupon.link  = data.coupon.link;
            coupon.text  = data.coupon.text;
            coupon.reward_amount = data.coupon.reward_amount;

            referral_message = new AccountInfo.referralMessage();
            referral_message.amount = data.referral_message.amount;
            referral_message.amount_percents  = data.referral_message.amount_percents;
            referral_message.banner_message  = data.referral_message.banner_message;
            referral_message.currency  = data.referral_message.currency;//GBP or EUR
            referral_message.language  = data.referral_message.language;//en or de 
            referral_message.link = data.referral_message.link;
            referral_message.shared_message  = data.referral_message.shared_message;
            referral_message.coupon_code =  data.referral_message.coupon_code;
            referral_message.valid_till = ConvertDateFromString(data.referral_message.valid_till);
            if (!string.IsNullOrWhiteSpace(data.coupon.valid_till)){
                coupon.valid_till  = ConvertDateFromString(data.coupon.valid_till);
            }
            subscription_end = ConvertDateFromString(data.subscription_end);
            agreed_to_terms_on = ConvertDateFromString(data.agreed_to_terms_on);
            agreed_to_marketing_on = ConvertDateFromString(data.agreed_to_marketing_on);
            registration_start = ConvertDateFromString(data.registration_start);
            contact_email_address_support = data.contact_email_address_support;
            contact_email_address_feedback = data.contact_email_address_feedback;
            times_declined_marketing = data.times_declined_marketing;
            targed_duration = data.targed_duration;
        //    named_infoes = new Dictionary<string,AccountInfo.infoData>();//[data.named_infoes.Length];
            List<AccountInfo.infoData> infoList = new List<AccountInfo.infoData>();//[data.named_infoes.Length];
            for (int p = 0; p < data.named_infoes.Length; p++){
            //    AccountInfo.infoData pbData
                if (!string.IsNullOrWhiteSpace(data.named_infoes[p].info_id) && data.named_infoes[p].info_id.ToUpper().StartsWith("PB") ){ //FIRST CHECK info ID IS PRESENT AND VALID (STARTS WITH 'PB') BEFORE COPYING OVER DATA
                    AccountInfo.infoData newinfo = new AccountInfo.infoData();
                    newinfo.name = data.named_infoes[p].name;                   
                    newinfo.info_id = data.named_infoes[p].info_id;
                    Color pbColour;
                    if (!string.IsNullOrWhiteSpace(data.named_infoes[p].colour) && ColorUtility.TryParseHtmlString(string.Concat("#",data.named_infoes[p].colour),out pbColour) ){ //CHECK IF VALUE FOR COLOUR IS VALID
                        newinfo.colour = pbColour;// ColorHelper.HexToColor(data.named_infoes[p].colour);
                    }else{ //IF NO VALID HEXCODE IS SAVED AS COLOUR, ASSIGN ONE
                        if (p < UserAccount.defaultPBColours.Length){
                            newinfo.colour = UserAccount.defaultPBColours[p];
                        }else{
                            newinfo.colour = CorporateColors.infoLightGrey;
                        }
                    }
                //    named_infoes.Add(data.named_infoes[p].info_id,pbData);
                    infoList.Add(newinfo);
                }
            }
            named_infoes = infoList.ToArray();
        }

    }

/////////////////////////////
 public AccountInfo(){
            //INITIALISE THESE ARRAYS SO THEY AREN'T NULL
            named_infoes = new infoData[0];
            subscriptions = new subscriptionType[0];
        }
/////////////////////////////

 public class AccountInfoUpdateJSON{
        public int target_duration;
        public int language;
        public infoDataJSON[] named_infoes; 
    }
////////////////////////////////
    [System.Serializable] public struct FeaturedGameJSON
    {
        public string featured_game;
        public int week;
        public int year;
    }

    public class AccountTargetDurationJSON
    {
        public int target_duration;
    }
    [System.Serializable] public struct infoDataJSON{
        public string info_id;
        public string name;
        public string colour;
    }

    public class AccountInfoUpdateJSON{
        public int target_duration;
        public int language;
        public infoDataJSON[] named_infoes; 
    }

    [System.Serializable] public struct TokenSetupJSON{
        public string access_token;
        public string token_type;
        public int expires_days;
    }



    [System.Serializable] public struct AccountInfoJSON{

        public string token;
        public string user_id;
        public string email;
        public string first_name;
        public string last_name;
        public bool temporary;
        public bool confirmed;
        public int language;
        public string role;
        [System.Serializable] public struct subscriptionType{
            // public string name;
            // public float amount;
            // public string currency;
            public string subscription_id;
            public string title;
            public int quantity;
            public string status;
            public bool includes_dental;
            public string next_charge_due_at;
            public bool for_adults;
            public string access_level;
            public string created_at;
            public string end_date;
            public string store;
            public string custom_id;
            public string apple_latest_receipt;
            public float price;
            public float latest_price;
        }

        [System.Serializable] public struct couponDataStruct{
                public float amount;
                public string reward_amount;
                public string code;
                public string link; 
                public string text;
                public string valid_till;
        }

        [System.Serializable] public struct referralMessage{
                public int amount;
                public string amount_percents;
                public string banner_message;
                public string currency;
                public string language;
                public string link;
                public string shared_message;
                public string coupon_code;
                public string valid_till;
        }
        public referralMessage referral_message;
        public couponDataStruct coupon;
        public subscriptionType[] subscriptions;
        public string subscription_end;
        public string agreed_to_terms_on;
        public string agreed_to_marketing_on;
        public int times_declined_marketing;
        public int targed_duration;
        public string registration_start;
        public string contact_email_address_support;
        public string contact_email_address_feedback;
        public infoDataJSON[] named_infoes;
    }


    public void RecoverLoginAccountDetailsWithToken(GameObject rObj, string rFunction)  {
        string requestString = RegistManager.apiRoot + "/account/info";//?user_id=";
        StartCoroutine(SendAPIRequest<AccountInfoJSON>(rObj,rFunction,RequestType.GET,requestString,string.Empty,true,-1,false,null,CustomResponseHandler_AccountInfoRequest));
    }

    public void UpdateAccountInfo(string user, GameObject rObj, string rFunction)
    {
        AccountInfoUpdateJSON accountUpdate = new AccountInfoUpdateJSON();
        accountUpdate.target_duration = Chronos.playTime;
        accountUpdate.language = (int)PBCommonLocalizationDataFile.mainLanguage.Language;
        if (UserAccount.instance.accountInfo != null){
            accountUpdate.named_infoes = new infoDataJSON[UserAccount.instance.accountInfo.named_infoes.Length];
            for (int p = 0; p < UserAccount.instance.accountInfo.named_infoes.Length; p++){
                accountUpdate.named_infoes[p].info_id = UserAccount.instance.accountInfo.named_infoes[p].info_id;
                accountUpdate.named_infoes[p].name = UserAccount.instance.accountInfo.named_infoes[p].name;
                accountUpdate.named_infoes[p].colour = ColorUtility.ToHtmlStringRGBA(UserAccount.instance.accountInfo.named_infoes[p].colour);
            }
        }
        string JsonStr = JsonUtility.ToJson(accountUpdate);
        string requestString = RegistManager.apiRoot + "/account/info";//?user_id=" + user;
        StartCoroutine(SendAPIRequest<AccountInfoJSON>(rObj,rFunction,RequestType.POST,requestString,JsonStr,true,-1,false,null,CustomResponseHandler_AccountInfoRequest));
    }


 public void RetrieveAccountInfo(GameObject rObj, string rFunction,bool raw = false){

       // string requestString = RegistManager.apiRoot + "/account/as_user_token?user_email="+user_email+ "&game_name=" + currentGameID; 
        string requestString = RegistManager.apiRoot + "/account/info";//?user_id=" + user + "&game_name=" + currentGameID; 
        StartCoroutine(SendAPIRequest<AccountInfoJSON>(rObj,rFunction,RequestType.GET,requestString,string.Empty,true,-1,false,null, raw ? null : CustomResponseHandler_AccountInfoRequest));
    }

public object CustomResponseHandler_AccountInfoRequest(UnityWebRequest targetRequest){
		object result = null;
        string jsonRes = string.Empty;
        if (GenericRequestSuccessCheck(targetRequest)){   //GOOD
            jsonRes = targetRequest.downloadHandler.text;
        }//IF REQUEST FAILS BUT LOCAL BACKUP AVAILABLE, RESTORE THAT BEFORE RETURNING ERRORS IF POSSIBLE
        else if(targetRequest.url.Contains(UserAccount.instance.user_id) && PlayerPrefs.HasKey("AccountInfoBackup") ){
            //IF AN ERROR OCCURS AND BACKUP ACCOUNT INFO IS PRESENT IN PLAYERPREFS, RESTORE DATA FROM BACKUP
            jsonRes = PlayerPrefs.GetString("AccountInfoBackup");PBDebug.Log("RESTORING ACCOUNT INFO FROM BACKUP: " +jsonRes);
        } 


        if (!string.IsNullOrEmpty(jsonRes)){
            Debug.Log("Go tHere");
            AccountInfoJSON data = JsonUtility.FromJson<AccountInfoJSON> (jsonRes);

            result = ConvertAccountInfoFromJSON(data);
        }else{
            Debug.Log("Go Here");
            result = RequestGenericFailTypeCheck(targetRequest);
        }
        return result;     
    }




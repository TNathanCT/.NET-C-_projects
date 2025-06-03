[Serializable] public struct EvolutionDate{
        public string phase_0;
        public string phase_1;
        public string phase_2;
        public string phase_3;
    }
    
    [Serializable] public struct TamagotchiDetails{
        public int evolution_phase;
        public EvolutionDate evolution_dates;
        public itemDetails[] items;
        [Serializable]
        public struct itemDetails{
            public string name;
            public string type;
            public string place;
            public bool is_active;
            public string source;
            public string created_at;
        }
    }

    [Serializable] public struct TamagotchiTokenTransactionResponse{
        public int pet_token_balance;
    }
    
    [Serializable] public struct TamagotchiGetRewardList_CollectedRewards{ //SO COLLECTED_REWARDS CAN BE PARSED SEPARATELY FROM MAIN TAMAGOTCHIGETREWARDLIST STRUCT IF NEEDED
        public string collected_rewards;
    }
    [Serializable] public struct TamagotchiGetRewardList{
        [Serializable] public struct rewardItem{
            public int _id;
            public string type;
            public string name;
            public bool claimed;
        }
        public rewardItem[] current_reward; //TODAY'S REWARD
        public rewardItem[][] collected_rewards; //REWARDS FOR THE CURRENT WEEK
        public int current_streak; //NUM OF CLAIMED DAYS
        public int current_day; //CURRENT DAY OF WEEK
    }

    [Serializable] public struct TamagotchiProfileData{
        public string user_id;
        public string email;
        public string profile;
        public int pet_token_balance;
        public TamagotchiDetails tamagotchi;
        public TamagotchiGetRewardList rewards;
        
        public plantsDetails plants;
        [Serializable]
        public struct plantsDetails{
            public int numberoftreeplanted;
        }
    }

    [Serializable]
    public struct TamagotchiSkinIAP{
        
        [Serializable]
        public struct ItemStruct{
            [SerializeField] public string name;
            [SerializeField] public string type;
            [SerializeField] public string place;
        }
        
        public ItemStruct item;
        public TamagotchiSkinIAP(string itemname, string typestring, string position){
            item.name = itemname;
            item.type = typestring;
            item.place = position;
        }
    }

    [Serializable]
    public struct TamagotchiEvolutionStateUpdate{
        [SerializeField] public int evolution_phase;
        [SerializeField] public bool evolution_change;
        
        public TamagotchiEvolutionStateUpdate(int evo, bool change){
            evolution_phase = evo;
            evolution_change = change;
        }
    }

    [Serializable]
    public struct TamagotchiRewardsIAP{
        
        [Serializable]
        public struct rewardstruct{
            public bool has_streaked;
            public bool has_reset;
        }

        public rewardstruct rewards;

        public TamagotchiRewardsIAP(bool ongoing, bool hasreset){
            rewards = new rewardstruct();            
            rewards.has_streaked = ongoing;
            rewards.has_reset = hasreset;
        }
    }

    [Serializable]
    public struct ItemStruct{
        [SerializeField] public string name;
        [SerializeField] public string type;
        [SerializeField] public int price;
        [SerializeField] public string place;

        public ItemStruct(string itemname, string typeitem, int pricetag, string position){          
            name = itemname;
            type = typeitem;
            price = pricetag;
            place = position;
        }
    } 

    [Serializable]
    public struct TamagotchiIAP{
        public ItemStruct item;

        public TamagotchiIAP(string itemname, string typeitem, int pricetag, string position){
            item = new ItemStruct();            
        
            item.name = itemname;
            item.type= typeitem;
            item.price = pricetag;
            item.place = position;

        }
    }

    [Serializable] public struct TamagotchiBrushDates{
        public System.DateTime date;
        public int amount_brushes;
    }

    [Serializable] public struct TamagotchiTokenIAP{
        public int pet_tokens;
        public TamagotchiTokenIAP(int token){
            pet_tokens = token;
        }
    }

    [Serializable]
    public struct TamagotchiFoodEatenIAP{
        [Serializable]
        public struct ItemStruct{
            [SerializeField] public string name;
        }
        public ItemStruct item;
        public TamagotchiFoodEatenIAP(string foodname){
            item = new ItemStruct();
            item.name = foodname;
        }
    }




// ===================== VIRTUAL PET pet =====================================
    
    public void RetrieveTamagotchiData(string user,GameObject rObj,string rFunction,string profile){
        string requestString = origin.apiRoot + "/game_profile/" + user + "/"+ profile +"?"+ v6_apichanges_version2string;
        StartCoroutine(SendAPIRequest<TamagotchiProfileData>(rObj,rFunction,RequestType.GET,requestString,string.Empty,true));    
    }
    public void GetTamagotchiBrushingDatesData (GameObject rObj,string rFunction,string user,string gameID, string profile = ""){
        string requestString = origin.apiRoot + "/game_profile/brush_data/" + gameID + "/"+ UserAccount.instance.activeProfile;
		StartCoroutine(SendAPIRequest<TamagotchiBrushDates>(rObj,rFunction,RequestType.GET,requestString,string.Empty,true,-1,false,null,CustomResponseHandler_GetTamagotchiBrushingDatesData));
    }

    public void PostTamagotchiEvolutionState(GameObject rObj, string rFunction, string user, TamagotchiEvolutionStateUpdate purchaseData){
        string JsonStr = JsonUtility.ToJson(purchaseData);
        string requestString = origin.apiRoot + "/game_profile/tamagotchi/evolution/"+UserAccount.instance.user_id +"/"+UserAccount.instance.activeProfile;// + requestURL;
        StartCoroutine(SendAPIRequest<TamagotchiDetails>(rObj,rFunction,RequestType.POST,requestString,JsonStr));
    }     

    public void SendTamagotchiFoodData(GameObject rObj, string rFunction, string user, TamagotchiFoodEatenIAP purchaseData){        
        string JsonStr = JsonUtility.ToJson(purchaseData);
        string requestString = origin.apiRoot + "/game_profile/tamagotchi/items/delete/"+UserAccount.instance.user_id +"/"+UserAccount.instance.activeProfile;
        StartCoroutine(SendAPIRequest<TamagotchiDetails>(rObj,rFunction,RequestType.POST,requestString,JsonStr));
    }

    public void PostTamagotchiItemTransaction(GameObject rObj, string rFunction, string user, ItemStruct purchaseData){ //FOR SINGLE ITEM TRANSACTIONS
        ItemStruct[] singleArray = new ItemStruct[]{purchaseData};
        PostTamagotchiItemTransaction(rObj, rFunction, user, singleArray);
    }


    public void PostTamagotchiItemTransaction(GameObject rObj, string rFunction, string user, ItemStruct[] purchaseData){ //FOR ADDING MULTIPLE ITEMS (LIKE ON SETTING UP NEW PROFILES)
        string JsonStr = "[ "; //BUILD JSON FROM ARRAY OF ItemStruct ITEMS
        for (int s = 0; s < purchaseData.Length; s++ ){
            if (s > 0){ JsonStr += " , ";}
            JsonStr += JsonUtility.ToJson (purchaseData[s]);
        }
        JsonStr += " ]";
        string requestString = origin.apiRoot + "/game_profile/tamagotchi/items/"+UserAccount.instance.user_id +"/"+UserAccount.instance.activeProfile;  
        StartCoroutine(SendAPIRequest<TamagotchiDetails>(rObj,rFunction,RequestType.POST,requestString,JsonStr));        
    }

    public void SendpetTokens(GameObject rObj, string rFunction, string user, TamagotchiTokenIAP purchaseData){        
        string JsonStr = JsonUtility.ToJson(purchaseData);
        string requestString = origin.apiRoot + "/game_profile/pet_tokens/"+UserAccount.instance.user_id +"/"+UserAccount.instance.activeProfile;         
        StartCoroutine(SendAPIRequest<TamagotchiTokenTransactionResponse>(rObj,rFunction,RequestType.POST,requestString,JsonStr));  
        
    }


    public void SendNewTamagotchiDisplayData(GameObject rObj, string rFunction, string user, TamagotchiSkinIAP purchaseData){        
        string JsonStr = JsonUtility.ToJson(purchaseData);
        string requestString = origin.apiRoot + "/game_profile/tamagotchi/items/activate_skin/"+UserAccount.instance.user_id +"/"+UserAccount.instance.activeProfile;// + requestURL;
        StartCoroutine(SendAPIRequest<TamagotchiDetails>(rObj,rFunction,RequestType.POST,requestString,JsonStr));
    }
   
    public void RetrieveTamagotchiRewardData(GameObject rObj, string rFunction, string user ){
        string requestString = origin.apiRoot + "/game_profile/rewards/"+UserAccount.instance.user_id +"/"+UserAccount.instance.activeProfile +"?"+ v6_apichanges_version2string;
        StartCoroutine(SendAPIRequest<TamagotchiGetRewardList>(rObj,rFunction,RequestType.GET,requestString,string.Empty,true));
    }

    public void SendNewTamagotchiRewardData(GameObject rObj, string rFunction, string user, TamagotchiRewardsIAP purchaseData){        
        string JsonStr = string.Empty;//JsonUtility.ToJson(purchaseData);
        string requestString = origin.apiRoot + "/game_profile/rewards/"+UserAccount.instance.user_id +"/"+UserAccount.instance.activeProfile +"?"+ v6_apichanges_version2string;// + requestURL;
        StartCoroutine(SendAPIRequest<TamagotchiGetRewardList>(rObj,rFunction,RequestType.POST,requestString,JsonStr,true,-1,false,null,CustomResponseHandler_SendNewTamagotchiRewardData));
    }



 public object CustomResponseHandler_SendNewTamagotchiRewardData(UnityWebRequest targetRequest){
        object result = null;
        if (targetRequest.downloadHandler.text.Contains("reward of today already obtained")){ //IF REWARD CANNOT BE CLAIMED BECAUSE IT HAS BEEN CLAIMED TODAY ALREAD
            result = -9; //RETURN -9 INSTEAD OF STRUCT TO TRIGGER ERROR MESSAGE
        }else if (GenericRequestSuccessCheck(targetRequest)){//status.StartsWith ("2"))
            result = JsonConvert.DeserializeObject<TamagotchiGetRewardList> (targetRequest.downloadHandler.text,jsonSerializerConfig);
        }else{
            result = RequestGenericFailTypeCheck(targetRequest);
        }
        return result;
    }

 public object CustomResponseHandler_GetTamagotchiBrushingDatesData(UnityWebRequest targetRequest){
        object result = null;
        string jsonRes = targetRequest.downloadHandler.text;
        if (GenericRequestSuccessCheck(targetRequest)){   //GOOD
            string[] jsonResArray = jsonRes.Replace("{","").Replace("}","").Split(','); //SPLIT JSON INTO STRING ARRAY USING COMMA AS DELIMITER
            List<TamagotchiBrushDates> retrievedBrushDates = new List<TamagotchiBrushDates>();
            foreach(string item in jsonResArray){
                if (!string.IsNullOrWhiteSpace(item)){
                    string[] itemSplit = item.Split(':');
                    TamagotchiBrushDates newDate = new TamagotchiBrushDates();
                    System.DateTime.TryParse(itemSplit[0].Replace("\"","").Trim(),out newDate.date); 
                    newDate.amount_brushes = int.Parse (itemSplit[1]);
                    retrievedBrushDates.Add(newDate);
                }
            }
            result = retrievedBrushDates.ToArray();// newData;
        }else{
            result = RequestGenericFailTypeCheck(targetRequest);
        }
        return result;
    }

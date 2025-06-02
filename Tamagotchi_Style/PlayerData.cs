using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using ADK.Mobile.UIKit;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class Tamagotchi_PlayerData : MonoBehaviour
{
    public List<Tamagotchi_ItemLibrary.itemTypeList> itemLibrary;
    public static Tamagotchi_PlayerData instance;
    public enum itemType{
        main_rug, main_frame, main_plant, main_shelf, main_wall, main_floor,    
        kitchen_fridge, kitchen_wall, kitchen_floor, kitchen_table,
        clothing_vest, clothing_hat, clothing_accessory, clothing_babypacifier, clothing_babyhat, clothing_babyclothes,
        bathroom_sink, bathroom_shelf, bathroom_tub, bathroom_pot, bathroom_rug, bathroom_wall, bathroom_floor,
        food, evolution
    };
    public int ownedCoins, activatedInitialItems;
    public TMP_Text coinText_TMP;
    public GameObject loadingScreen;
    public List<GameObject> thingsBought = new List<GameObject>();
    public List<GameObject> itemChosentoDisplay = new List<GameObject>();
    // public List<GameObject> itemChosentoDisplay{
    //     set{
    //         m_itemChosentoDisplay = value;
    //     //    ForceMinimumItemBaseline(); //ENSURE ITEM CHOSEN TO DISPLAY ALWAYS CONTAINS ONE OF EACH ITEM TYPE (IN CASE ERROR OCCURS WHEN SETTING/CHANGING INDIVIDUAL ITEMS)
    //     }
    //     get{
    //         if (m_itemChosentoDisplay.Count > 0){ //DON'T AUTO SET BASELINE ITEMS IF MAIN LIST IS EMPTY AS THAT IS USED TO DETERMINE WHEN TO SET INITIAL DEFAULT SET
    //             ForceMinimumItemBaseline(); //ENSURE ITEM CHOSEN TO DISPLAY ALWAYS CONTAINS ONE OF EACH ITEM TYPE (IN CASE ERROR OCCURS WHEN SETTING/CHANGING INDIVIDUAL ITEMS)
    //         }
    //         return m_itemChosentoDisplay;
    //     }
    // }                
    //public int numberthingsbought;
    //public int displaythings;

    public List<GameObject> defaultItems = new List<GameObject>(), defaultPetBodyItems = new List<GameObject>(); //DEFAULT Pet BODY ITEMS ARE THE NON-CLOTHED DEFAULT PARTS THAT CAN BE REPLACED/COVERED WITH CLOTHING ITEMS WHEN AVAILABLE
    //public List<string> foodinDataBase = new List<string>();

    public Dictionary<string, int> amountofFoodBought; 
    //public Dictionary<string, int> orderFoodBoughtin;
    //public bool amountisEmpty;
    //public bool orderisEmpty;
    public Canvas mainCanvas;
    public GameObject Pet;
    public int amountrunning;
    public int evoStateValidBrush = 0; //HOW MANY VALID SESSIONS IN THE CURRENT EVOLUTION STATE
    public List<string> dailyrewardnames = new List<string>();
    public static SDKAPIManager.TamagotchiProfileData localCachedData; //SO THE DATA CAN BE RETRIEVED IF THE INSTANCE IS NOT AVAILABLE
    //public SDKAPIManager.TamagotchiProfileData lastSyncedData;
    public System.DateTime lastEvoDate = System.DateTime.MinValue, lastSyncTime = System.DateTime.MinValue;
    public Button coinAmountButton;
    public GameObject Pethead;
    public Dictionary<System.DateTime, int> datesandtimesbrushedDictionary; 
    public int framecount = -1;
    public int outfitcount = -1;
    public int tablecount = -1;
    public string itemname;
    public bool run, rolling;
    public bool opening = true, requireInitialItemActivation = false;
    public List<string> tags = new List<string>();

    
    List<AsyncOperationHandle<IList<GameObject>>> libraryGameObjectLoaders;
    public void Awake(){
        StopAllCoroutines();
        rolling = true;
        amountrunning = 0;
        instance = this;
        loadingScreen.SetActive(true);        
//        this.GetComponent<Tamagotchi_MarketSliderManager>().Initialise(); //ENSURE MARKETSLIDERMANAGER INSTANCE IS INITIALISED FIRST BEFORE ATTEMPTING TO ACCESS IT BELOW
        amountofFoodBought= new Dictionary<string, int>(){};
        datesandtimesbrushedDictionary = new Dictionary<System.DateTime, int>(){};
        coinAmountButton = coinText_TMP.transform.parent.gameObject.AddComponent<Button>();
        coinAmountButton.onClick.AddListener(()=> ShowCoinTutorial(true));
        StartCoroutine(LoadItemLibrary());
    }

    public IEnumerator LoadItemLibrary(){
        //LOAD THE ITEM LIBRARY SCRIPTABLE OBJECT TO SET UP WHAT ITEMS ARE CURRENTLY AVAILABLE IN THE APP.
        AsyncOperationHandle<Tamagotchi_ItemLibraryManifest> libraryManifestLoader = Addressables.LoadAssetAsync<Tamagotchi_ItemLibraryManifest>("Assets/AddressableGroupManifest/tamagotchi/tamagotchi_ItemLibrary_Manifest.asset");
        yield return libraryManifestLoader;//.Completed += () => {//Assets/ScriptableObjects/Tamagotchi/tamagotchi_ItemLibrary.asset").Completed += (itemLibraryData) => {
        if (!gameDebug.gameSettingsProductionBuild) Debug.Log("Item Library Info Loaded Successfully");
        itemLibrary = new List<Tamagotchi_ItemLibrary.itemTypeList>(); 
        libraryGameObjectLoaders = new List<AsyncOperationHandle<IList<GameObject>>>();
        foreach(Tamagotchi_ItemLibraryManifest.addressableItemTypeList libraryItemSet in libraryManifestLoader.Result.itemLibraryList){
            //CONVERT PATHS TO PREFABS AND LOAD EACH PREFAB VIA ADDRESSABLES SYSTEM BEFORE CACHING IN ITEM LIBRARY
            Tamagotchi_ItemLibrary.itemTypeList nextItemSet = new Tamagotchi_ItemLibrary.itemTypeList();
            nextItemSet.type = libraryItemSet.type;
            nextItemSet.itemPrefabs = new GameObject[libraryItemSet.itemPrefabPaths.Length]; //CREATE ARRAY WITH SPACE FOR ALL GAMEOBJECTS TO BE LOADED FROM PATHS
            int nx = 0;
            AsyncOperationHandle<IList<GameObject>> nextPrefabLoader = Addressables.LoadAssetsAsync<GameObject>(libraryItemSet.itemPrefabPaths,resultObj =>{
                //yield return nextPrefabLoader; //WAIT FOR FILE TO LOAD BEFORE CONTINUING TO NEXT FILE IN LOOP, LOAD FILES SEQUENTIALLY
                nextItemSet.itemPrefabs[nx] = resultObj; //STORE LOADED PREFAB REFERENCE IN ITEMPREFABS ARRAY FOR THIS ITEM TYPE IN ITEM LIBRARY LIST
                if(libraryItemSet.defaultAreaItemPath.Contains(resultObj.name)){//,StringComparison.OrdinalIgnoreCase)){
                    //IF THE PATH FOR THIS GAMEOBJECT MATCHES THE PATH FOR THE DEFINED DEFAULT GAMEOBJECT FOR THIS ITEM TYPE, 
                    //ALSO SAVE THE REFERENCE TO LOADED GAMEOBJECT AS THE DEFAULT ITEM IN THE NEXTITEMSET
                    nextItemSet.defaultAreaItem = resultObj;
                }                
                nx++;
            },Addressables.MergeMode.Union);
            yield return nextPrefabLoader;
            libraryGameObjectLoaders.Add(nextPrefabLoader);
            itemLibrary.Add(nextItemSet);
        }
        Addressables.Release(libraryManifestLoader); //LIBRAY MANIFEST ASSET NO LONGER NEEDED AFTER LOADING THE CONTAINED GAMEOBJECT REFERENCES AND ITEM LISTS
        //CONTINUE WITH THE INITIALISATION AFTER ITEM LIBRARY HAS FINISHED LOADING
        //UserAccount.apiConnect.GetRewardsList(this.gameObject,"GetRewardsListResult", UserAccount.instance.user_id);
        //UserAccount.apiConnect.RetrieveTamagotchiRewardData(this.gameObject,"RetrieveRewardStateResponse",UserAccount.instance.user_id);
        RefreshProfileData();
    }

    public void RefreshProfileData(){
        
        GlobalUIController.ShowLoadingLayer();
        UserAccount.apiConnect.RetrieveTamagotchiData(UserAccount.instance.user_id,this.gameObject,"RetrieveTamagotchiDataResult", UserAccount.instance.activeProfile);///""); 
    }

    public void UnloadItemLibrary(){
        for(int g = 0; g < libraryGameObjectLoaders.Count; g++){
            Addressables.Release(libraryGameObjectLoaders[g]);
            //libraryGameObjectLoaders[g] = null;
        }
        libraryGameObjectLoaders = new List<AsyncOperationHandle<IList<GameObject>>>();
    }
    public void AddPetcoinsResult(object result){

    }

    public void ForceMinimumItemBaseline(){ //ENSURE ITEM CHOSEN TO DISPLAY LIST ALWAYS CONTAINS ONE OF EACH ITEM TYPE 
        List<SDKAPIManager.ItemStruct> initialItems = new List<SDKAPIManager.ItemStruct>(); //SET BLANK LIST TO BE USED IN REQUEST
        foreach(GameObject item in defaultItems){
            bool hasItemType = false;
            for(int icd = 0; icd < thingsBought.Count; icd++){ //LOOP THROUGH CURRENT ITEMS LIST TO CHECK IF THIS TAG EXISTS
                if(thingsBought[icd].tag == item.tag){ //IF ITEM OF THIS TAG TYPE ALREADY EXISTS IN ITEM TO DISPLAY LIST, SKIP THIS ONE
                    hasItemType = true;
                }
            }
            if (!hasItemType){ //MAKE SURE THE MISSING ITEMS ARE ADDED TO THE PROFILE'S ITEMS VIA THE API 
                thingsBought.Add(item);   
                itemChosentoDisplay.Add(item); //IF AN ITEM OF THIS TAG TYPE IS NOT FOUND IN THE ITEM CHOSEN TO DISPLAY LIST, ADD THE ITEM OF THE CORRESPONDING TYPE FROM THE DEFAULT ITEM LIST
                thingsBought.Add(item);            
                SDKAPIManager.ItemStruct data = new SDKAPIManager.ItemStruct(item.name, "skin", 0,item.gameObject.tag);    //PRICE FOR ALL DEFAULT ITEMS SHOULD BE 0 REGARDLESS OF WHAT PRICE ON INDIVIDUAL ITEM OBJECT IS SET TO           
                initialItems.Add(data); //ADD EACH ITEM TO LIST
            }
        }
        if (initialItems.Count > 0){
            requireInitialItemActivation = true;
            UserAccount.apiConnect.PostTamagotchiItemTransaction(this.gameObject, "RetrieveTamagotchiDataResult",UserAccount.instance.activeProfile,initialItems.ToArray()); //CONVERT ITEM LIST TO ARRAY
        }else{
            FinishSetup();
        }
    }

    
    public void ActivateIniitalItems(){
        foreach(GameObject item in defaultItems){
            SDKAPIManager.TamagotchiDisplaySkinIAP data = new SDKAPIManager.TamagotchiDisplaySkinIAP(item.name, "skin", item.tag);
            UserAccount.apiConnect.SendNewTamagotchiDisplayData(this.gameObject, "ActivateInitialItemResult", UserAccount.instance.activeProfile,data);
        }
    }
    
    public void ActivateInitialItemResult(object result){ 
        //DO NOTHING WITH THE RESPONSE FOR NOW
    }

    public void RetrieveTamagotchiDataResult(object result){ //WAIT FOR RESULT FROM GETTING Pet PET DATA
        if (result is SDKAPIManager.TamagotchiProfileData){
            SDKAPIManager.TamagotchiProfileData newData = (SDKAPIManager.TamagotchiProfileData)result;
            localCachedData = newData;
            ownedCoins = localCachedData.Pet_coins_balance;
            UpdateCoinText(coinText_TMP);
        //    egg.GetComponent<Tamagotchi_EvolutionScript>().Initialise();
            this.GetComponent<Tamagotchi_MarketSliderManager>().Initialise(); //ENSURE MARKETSLIDERMANAGER INSTANCE IS INITIALISED FIRST BEFORE ATTEMPTING TO ACCESS IT BELOW
        //    if (requireInitialItemActivation){ //SET THE NEWLY ADDED ITEMS AS ACTIVE
        //        ActivateIniitalItems();
        //    }
            //SET UP DEFAULT ITEMS LIST 
            defaultItems = new List<GameObject>();//defaultPetBodyItems); //CREATE NEW DEFAULT ITEMS LIST STARTING WITH THE DEFAULT Pet BODY ITEMS
            foreach(Tamagotchi_ItemLibrary.itemTypeList libraryItem in itemLibrary){
                if(libraryItem.defaultAreaItem != null){ //IF A DEFAULT ITEM IS DEFINED FOR THIS AREA, FIND IT IN THE MARKETSLIDERMANAGER LISTS
                    try{
                        //FOR EACH ITEM TYPE IN THE LIBRARY LIST, FIRST FIND THE CORRESPONDING LIST FOR THE CURRENT ITEM TYPE IN ALLLISTS. THEN FIND THE GAMEOBJECT THAT MATCHES THE OBJECT NAME OF THE DEFAULT ITEM NAME FOR THAT ITEM TYPE
                        defaultItems.Add(Tamagotchi_MarketSliderManager.instance.allLists.Find(targetList => targetList.area == libraryItem.type).objectList.Find(targetItem => targetItem.name == libraryItem.defaultAreaItem.name));
                    }catch {
                        //ERROR WHEN TRYING TO FIND DEFAULT ITEM REFERENCE , SKIP THIS ITEM?
                    }
                }
            }
            LoadTamagotchiData(localCachedData.tamagotchi);    
        }else if(result is SDKAPIManager.TamagotchiDetails){
            localCachedData.tamagotchi = (SDKAPIManager.TamagotchiDetails)result;
            LoadTamagotchiData(localCachedData.tamagotchi);
        }else{ //IF GETTING Pet PET DATA FAILS
                string errorCode = "gameK_tamagotchi_E-PR1"; //MY little Pet - PROFILE RETRIEVAL ERROR 1
                string errorText = gameCommontranslationDataFile.Gettranslation("_ServerResponse_Error_Generic") + "\n"+ errorCode +"\n\n" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (AppSystem.sharedUI.gameLoaderSplashObject.activeSelf){ //IF SPLASH SCREEN IS STILL ACTIVE, SHOW ERROR MESSAGE ON SPLASH SCREEN
                // AppSystem.sharedUI.SetGameLoaderSplashText(translationDataFile.Gettranslation("_GameLoaderSplashScreen_LoadingText_MissingProfile").Replace("%#NAME#%",UserAccount.instance.activeProfile),true);											
				// AppSystem.sharedUI.gameLoaderSplash_loadingText.color = Color.red;    
                AppSystem.sharedUI.SetGameLoaderSplashText(errorText);
                AppSystem.sharedUI.gameLoaderSplash_loadingText.color = Color.red;
            }else{ //OTHERWISE SHOW POPUP
                GlobalUIController.ShowServerFailedResponseError(this.gameObject, "DataRetrievalFailResponse",errorCode);
            }
        }
    }

    public void SettingUpMarketClothes(int change){
        for (int d = 0; d < Tamagotchi_MarketSliderManager.instance.PetTypes.Count; d++){
            Tamagotchi_MarketSliderManager.instance.PetTypes[d].SetActive(change == d);
        }
        //USE TAMAGOTCHI_ACCESSORIESSCRIPT INSTANCE TO SET CLOTHING ITEMS INSTEAD
        Tamagotchi_MarketSliderManager.instance.PetTypes[change].GetComponent<Tamagotchi_AccessoriesScript>().ChangeClothesOnPet();
    
        for(int i = 0 ; i < change - 1; i++){
            if(!thingsBought.Contains(Tamagotchi_MarketSliderManager.instance.PetTypes[i].gameObject)){
                thingsBought.Add(Tamagotchi_MarketSliderManager.instance.PetTypes[i].gameObject);
            }
        }
        if(Tamagotchi_PlayerData.instance.Pet != null){
            Tamagotchi_PlayerData.instance.Pet.GetComponent<Tamagotchi_AccessoriesScript>().ChangeClothesOnPet();
        }else{
            
        }
    }

    public void DataRetrievalFailResponse(){
        AppSystem.instance.ReturnToMain();
    }


    public void LoadSavedList(SDKAPIManager.TamagotchiDetails.itemDetails item)
    {
        foreach(Tamagotchi_MarketSliderManager.AreaList areaList in Tamagotchi_MarketSliderManager.instance.allLists){
            //THIS COULD CAUSE PROBLEMS IF MULTIPLE ITEMS OF  DIFFERENT TYPES HAVE IDENTICAL NAMES AS IT LOOKS 
            foreach(GameObject areaItemObj in areaList.objectList){
                if (!thingsBought.Contains(areaItemObj) && string.Equals(areaItemObj.name, item.name,StringComparison.OrdinalIgnoreCase)){
                    thingsBought.Add(areaItemObj);
                }
            }
        }
        RemoveDuplicates();
    }

    public void RemoveDuplicates(){
        thingsBought = thingsBought.Distinct().ToList();
        itemChosentoDisplay = itemChosentoDisplay.Distinct().ToList();
        Tamagotchi_MarketSliderManager.instance.frameList.objectList = Tamagotchi_MarketSliderManager.instance.frameList.objectList.Distinct().ToList();
        Tamagotchi_MarketSliderManager.instance.tableList.objectList = Tamagotchi_MarketSliderManager.instance.tableList.objectList.Distinct().ToList();
        Tamagotchi_MarketSliderManager.instance.vesteList.objectList = Tamagotchi_MarketSliderManager.instance.vesteList.objectList.Distinct().ToList();
        Tamagotchi_MarketSliderManager.instance.hatList.objectList =  Tamagotchi_MarketSliderManager.instance.hatList.objectList.Distinct().ToList();
        Tamagotchi_MarketSliderManager.instance.accessoryList.objectList = Tamagotchi_MarketSliderManager.instance.accessoryList.objectList.Distinct().ToList();
    }

    public void SetUpDefaultItems(){
        List<SDKAPIManager.ItemStruct> initialItems = new List<SDKAPIManager.ItemStruct>(); //SET BLANK LIST TO BE USED IN REQUEST
        for(var i = 0; i < defaultItems.Count; i++){        //ADD DEFAULT ITEMS TO NEW PROFILE
            thingsBought.Add(defaultItems[i]);            
            SDKAPIManager.ItemStruct data = new SDKAPIManager.ItemStruct(defaultItems[i].name, "skin", 0,defaultItems[i].gameObject.tag);    //PRICE FOR ALL DEFAULT ITEMS SHOULD BE 0 REGARDLESS OF WHAT PRICE ON INDIVIDUAL ITEM OBJECT IS SET TO           
            initialItems.Add(data); //ADD EACH ITEM TO LIST
        }          
        amountrunning = 1;         
        //amountrunning++;  //INCREMENT AMOUNT RUNNING COUNTER FOR EACH ITERATION
        UserAccount.apiConnect.PostTamagotchiItemTransaction(this.gameObject, "SendNewTamagotchiDataResult",UserAccount.instance.activeProfile,initialItems.ToArray()); //CONVERT ITEM LIST TO ARRAY
        DisplayDefaultItems();
    }
    //public void PostRefresh
    public void SendNewTamagotchiDataResult(object result){
        if (result is SDKAPIManager.TamagotchiDetails){  
            localCachedData.tamagotchi = (SDKAPIManager.TamagotchiDetails)result;
            Tamagotchi_PlayerData.instance.amountrunning = 0;// Tamagotchi_PlayerData.instance.amountrunning - 1; 
        }else{ //ERROR IN INITIALISING DEFAULT ITEMS
            string errorCode = "gameK_tamagotchi_E-PI1"; //MY little Pet - PROFILE INITIALISATION ERROR 1
            string errorText = gameCommontranslationDataFile.Gettranslation("_ServerResponse_Error_Generic") + "\n"+ errorCode +"\n\n" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (AppSystem.sharedUI.gameLoaderSplashObject.activeSelf){ //IF SPLASH SCREEN IS STILL ACTIVE, SHOW ERROR MESSAGE ON SPLASH SCREEN
                AppSystem.sharedUI.SetGameLoaderSplashText(errorText);
                AppSystem.sharedUI.gameLoaderSplash_loadingText.color = Color.red;
            }else{ //OTHERWISE SHOW POPUP
                GlobalUIController.ShowServerFailedResponseError(this.gameObject, "DataRetrievalFailResponse",errorCode);
            }
        }
    }

    public void DisplayDefaultItems(){ //SET DEFAULT ITEMS AS CURRENT ACTIVE ITEMS 
        for(var i = 0; i < defaultItems.Count; i++){
            defaultItems[i].gameObject.SetActive(true);
            itemChosentoDisplay.Add(defaultItems[i]);
        //    Tamagotchi_KitchenRoomScript.kitcheninstance.SetUpKitchen(itemChosentoDisplay);
            //DONT NEED TO SEND SEPARATE REQUESTS TO ACTIVATE NEWLY ADDED ITEMS AS THEY ARE ALREADY ACTIVE AFTER ADDING VIA ENDPOINT
        //        amountrunning++; //INCREMENT AMOUNT RUNNING COUNTER FOR EACH ITERATION
        //    SDKAPIManager.TamagotchiDisplaySkinIAP datachosen = new SDKAPIManager.TamagotchiDisplaySkinIAP(itemChosentoDisplay[i].name, itemChosentoDisplay[i].gameObject.tag, itemChosentoDisplay[i].gameObject.tag); 
        }
        StartCoroutine(CheckifReady());
    }
    
    public void SetDisplayItemResult(object result){
        if (result is SDKAPIManager.TamagotchiDetails){            
            localCachedData.tamagotchi = (SDKAPIManager.TamagotchiDetails)result;
            Tamagotchi_PlayerData.instance.amountrunning = Tamagotchi_PlayerData.instance.amountrunning - 1; 
        }
    }


    IEnumerator CheckifReady(){
        while (amountrunning > 0){
        //yield return new WaitForEndOfFrame();
        //while(rolling == true){
            yield return null;
        }            
        //CHECK BRUSH SESSIONS FOR EVOLUTION PROGRESS BEFORE STARTING FINISHING SETUP
        UserAccount.apiConnect.GetTamagotchiBrushingDatesData(this.gameObject, "GetTamagotchiBrushingDatesDataResult", UserAccount.instance.user_id, UserAccount.instance.user_id, UserAccount.instance.activeProfile);
    }

    public void GetTamagotchiBrushingDatesDataResult(object result){
      if (result is SDKAPIManager.TamagotchiBrushDates[]){
        SDKAPIManager.TamagotchiBrushDates[] newData = (SDKAPIManager.TamagotchiBrushDates[])result;
           instance.datesandtimesbrushedDictionary = new Dictionary<System.DateTime,int>();
           evoStateValidBrush = 0; //RESET TO 0 BEFORE COUNTING VALID BRUSHES
            for(int d = 0; d < newData.Length; d++){ //LOOP THROUGH DATA FROM API AND ADD DATES AND BRUSH AMOUNTS TO DICTIONARY
                instance.datesandtimesbrushedDictionary.Add(newData[d].date,newData[d].amount_brushes); //IF DATE IS ON OR AFTER LAST EVOLUTION DATE
                if (newData[d].date >= lastEvoDate){
                    evoStateValidBrush += Mathf.Clamp(newData[d].amount_brushes,0,2); //ADD SESSIONS TO VALID SESSIONS TOTAL (CLAMPED TO 2 PER DATE)
                }
            }
        }else{ //IF BRUSH SESSIONS AREN'T RETRIEVED, SET SESSION COUNTER TO 0
            instance.datesandtimesbrushedDictionary = new Dictionary<System.DateTime,int>();
            evoStateValidBrush = 0;
        }
        //IF THIS REQUEST FAILED, STILL CONTINUE
        Tamagotchi_PlayerData.instance.loadingScreen.SetActive(false);
        ForceMinimumItemBaseline(); ////ENSURE ITEM CHOSEN TO DISPLAY LIST ALWAYS CONTAINS ONE OF EACH ITEM TYPE 

    }


    public void FinishSetup(){
    //    SetUpFoodOwned();
        this.GetComponent<Tamagotchi_PetHungerScript>().Initialise();
        GlobalUIController.HideLoadingLayer();
        Tamagotchi_NavigateGameScript.navscript.KitchenRoom.SetActive(true);
        Tamagotchi_NavigateGameScript.navscript.KitchenRoom.GetComponent<Tamagotchi_KitchenRoomScript>().Initialise(); //ENSURE KITCHENSCRIPT IS INITIALISED 
        Tamagotchi_EvolutionScript.evoinstance.RefreshCurrentState();  
        AppSystem.HideLoadingScreenAfterAtlasLoad();	
		    PlayerPrefs.SetInt("OfflineCoins"+UserAccount.instance.user_id, 0);
        StartCoroutine(HideObjects());
    }

    public void LoadItemsFromAPIResponse(SDKAPIManager.TamagotchiDetails data, bool foodOnly = false){ //HAVE THIS SEPARATE SO IT CAN BE CALLED INDEPENDENTLY FROM OTHER SCRIPTS
        if (!foodOnly){  //FOOD ONLY - WHEN ONLY THE FOOD ITEMS FROM THE RESPONSE SHOULD BE UPDATED AND OTHER ITEMS SHOULD BE LEFT AS IS
            itemChosentoDisplay = new List<GameObject>(); //CLEAR EXISTING LISTS BEFORE POPULATING WITH DATA FROM BACKEND
        }
        //foodinDataBase = new List<string>();
        amountofFoodBought = new Dictionary<string, int>(); //RESET DICTIONARY
        for(int i = 0; i < data.items.Length; i++){
            if (!foodOnly){        
                if(data.items[i].is_active == true && !string.Equals(data.items[i].type,"Food",StringComparison.OrdinalIgnoreCase)){
                    itemname = data.items[i].name;
                    if(thingsBought.Find(x=>x.name == itemname) != null){
                        if(itemChosentoDisplay.Find(x=>x.name == itemname) == null ){
                            itemChosentoDisplay.Add(thingsBought.Find(x=>x.name == itemname));         
                        }
                    }
                }
            }
            //FOR EACH 'FOOD' ITEM IN THE USER PROFILE'S ITEMS LIST, ADD/INCREMENT COUNTERS FOR NUMBER OF EACH FOOD TYPE IN THEIR POSSESSION 
            if(string.Equals(data.items[i].type,"Food",StringComparison.OrdinalIgnoreCase)){
                if(amountofFoodBought.ContainsKey(data.items[i].name)){
                    amountofFoodBought[data.items[i].name]++;
                }else{
                    amountofFoodBought.Add(data.items[i].name, 1);
                }
            }
        }
    }

    public void LoadTamagotchiData(SDKAPIManager.TamagotchiDetails data, bool inStartupSequence = true){
        localCachedData.tamagotchi = data; //UPDATE 'TAMAGOTCHI' COMPONENT OF localCachedData IF THIS IS CALLED WITHOUT RETRIEVETAMAGOTCHIDATARESULT
        string[] evoDateString = new string[]{data.evolution_dates.phase_0,data.evolution_dates.phase_1,data.evolution_dates.phase_2,data.evolution_dates.phase_3};
        if(!string.IsNullOrEmpty(data.evolution_dates.phase_3) || data.evolution_phase == 3){ //IF A DATE IS PRESENT FOR PHASE_3, SET THAT AS LAST EVOLUTION DATE
            Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved = 3;
        }else if(!string.IsNullOrEmpty(data.evolution_dates.phase_2)){ //IF A DATE IS PRESENT FOR PHASE_2, SET THAT AS LAST EVOLUTION DATE
            Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved = 2;
        }else if(!string.IsNullOrEmpty(data.evolution_dates.phase_1)){ //IF A DATE IS PRESENT FOR PHASE_1, SET THAT AS LAST EVOLUTION DATE
            Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved = 1;
        }else if(!string.IsNullOrEmpty(data.evolution_dates.phase_0)){ //IF A DATE IS PRESENT FOR PHASE_0, SET THAT AS LAST EVOLUTION DATE
            Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved = 0;
        }
        lastEvoDate = SDKAPIManager.ConvertDateFromString(evoDateString[Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved]);
        Tamagotchi_EvolutionScript.evoinstance.fillgauge.transform.parent.gameObject.SetActive(Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved < 3);
        lastSyncTime = DateTime.Now; //LOG THE LAST TIME THE DATA WAS SYNCED FROM THE API
        if(data.items.Length != 0){
            thingsBought = new List<GameObject>(); //RESET THINGS BOUGHT LIST
            for(int i = 0; i < data.items.Length; i++){
                LoadSavedList(data.items[i]); //POPULATE THE 'THINGS BOUGHT' LIST
            }

            if(!BrushSdkSettings.IsProductionBuild) Debug.LogWarning("EXISTING ITEM DATA");
            LoadItemsFromAPIResponse(data); 
            amountrunning = 0;
            if(Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved == 3){
                for(int i = 0; i < 3; i++){
                    if (thingsBought.Contains(Tamagotchi_MarketSliderManager.instance.PetTypes[i])){
                        Tamagotchi_PlayerData.instance.thingsBought.Add(Tamagotchi_MarketSliderManager.instance.PetTypes[i]);
                    }
                }
            }
            if (inStartupSequence){
                StartCoroutine(CheckifReady());
            }
        }else{
            if(!BrushSdkSettings.IsProductionBuild) Debug.LogWarning("INITIALISE NEW ITEM DATA");
            if(Tamagotchi_EvolutionScript.evoinstance.lastevodatesaved == 3){
                for(int i = 0; i < 3; i++){
                    Tamagotchi_PlayerData.instance.thingsBought.Add(Tamagotchi_MarketSliderManager.instance.PetTypes[i]);
                }
            }
            SetUpDefaultItems();
        }
    }

    public void PrepareAssetsForBathMinigame(){
        for(var i = 0; i < itemChosentoDisplay.Count; i++){ 
            if(itemChosentoDisplay[i].tag == "Tub"){
                PlayerPrefs.SetString("Tub", itemChosentoDisplay[i].name);
            }

            if(itemChosentoDisplay[i].tag == "BathroomShelf"){
                PlayerPrefs.SetString("ShelfBath", itemChosentoDisplay[i].GetComponent<Image>().sprite.name);
            }
        }
    }

    public IEnumerator HideObjects(){
    //    this.gameObject.GetComponent<Tamagotchi_Translation>().StartTranslations();
//    while(opening == true){ //WHY?
        Tamagotchi_MarketSliderManager.instance.DisableAllObjects();
        yield return new WaitForEndOfFrame();
    //    ForceMinimumItemBaseline(); ////ENSURE ITEM CHOSEN TO DISPLAY LIST ALWAYS CONTAINS ONE OF EACH ITEM TYPE 
        for(var i = 0; i < itemChosentoDisplay.Count; i++){ 
            itemChosentoDisplay[i].gameObject.SetActive(true);
        }
        Tamagotchi_MarketSliderManager.instance.confirmationpurchaseObj.SetActive(false);

        if(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase != 0){
            Tamagotchi_PlayerData.instance.Pet.GetComponent<Tamagotchi_AccessoriesScript>().ChangeClothesOnPet();
        }
        yield return new WaitForEndOfFrame();
    }

    public void LeaveFoodStore(){
    //    StartCoroutine(WaitbeforeLeaving());
    }

    public IEnumerator WaitbeforeLeaving(){
    //    yield return new WaitForSeconds(1f);
   //     SetUpFoodOwned();
        yield break;
    }
   
    public void FoodhasbeenEaten(string foodname, int amountowned, int foodontableforreference){
        SDKAPIManager.TamagotchiFoodEatenIAP data = new SDKAPIManager.TamagotchiFoodEatenIAP(foodname);
        //SEND API REQUEST TO DEDUCT FOOD ITEM FROM USER PROFILE AND THEN REFRESH DATA AFTER RESPONSE
        UserAccount.apiConnect.SendTamagotchiFoodData(this.gameObject, "UpdateFoodItemResponse", UserAccount.instance.activeProfile,data);
        // //SHOW LOADING OVERLAY WHILE WAITING FOR RESPONSE 
        GlobalUIController.ShowInvisibleLoadingLayer();
    }

    public void UpdateFoodItemResponse(object result){ //REFRESH FOOD LISTS AFTER API RESPONSE HAS BEEN RECIEVED
        if (result is SDKAPIManager.TamagotchiDetails){
            SDKAPIManager.TamagotchiDetails updatedData = (SDKAPIManager.TamagotchiDetails)result;
            LoadItemsFromAPIResponse(updatedData,true);
            //SetUpFoodOwned();
        }else{
            string errorCode = "gameK_tamagotchi_E-CFI1"; //MY little Pet - CONSUME FOOD ITEM ERROR 1
            GlobalUIController.ShowServerFailedResponseError(this.gameObject, "DataRetrievalFailResponse",errorCode);
        }
        GlobalUIController.HideLoadingLayer();
    }

    public void AddCoins(){
        ownedCoins += 10;
        SDKAPIManager.TamagotchicoinsIAP data = new SDKAPIManager.TamagotchicoinsIAP(ownedCoins);               
        UserAccount.apiConnect.SendPetcoinss(this.gameObject, "AddPetcoinsResult", UserAccount.instance.activeProfile,data);
        UpdateCoinText(coinText_TMP);
    }

    public void RemoveCoins(int amount){
        ownedCoins -= amount;
        UpdateCoinText(coinText_TMP);
    }

    public void UpdateCoinText(TMP_Text coinText){
        coinText.text = ownedCoins.ToString();
    }


    public void ShowCoinTutorial(bool forceShow = false){
        List<ToolTipSystem.ToolTipConfig> tts = new List<ToolTipSystem.ToolTipConfig>(); //SET BLANK LIST
            int tID = 19;
            if (!GlobalUIController.tooltipUI.toolTipActive && (!GameData.Instance.globalTutorialsViewed.Contains(tID) || forceShow)){ //IF FORCE SHOW SET TO TRUE, SKIP CHECK FOR IF TOOLTIP ITEM HAS BEEN VIEWED BEFORE
                RectTransform targetObject = coinText_TMP.transform.parent.gameObject.GetComponent<RectTransform>();
                //ONLY SHOW IF THIS TUTORIAL STEP HAS NOT BEEN VIEWED BEFORE
                Vector2 winPos = Vector2.zero;
                Vector2 windowSize = new Vector2(600f,300f);
                ToolTipSystem.maskShape cutoutShape = ToolTipSystem.maskShape.SQUARE;
                Rect highlightRect = new Rect();
                bool showPointer = true;
                string tutorialText = "";

                winPos = new Vector2(0,500); //-820);
                cutoutShape = ToolTipSystem.maskShape.SQUARE; //CUTOUT SHAPE - THE SHAPE OF THE NON COVERED AREA
                windowSize = new Vector2(550f,220f); //SET THE SIZE OF THE TOOLTIP WINDOW
                showPointer = true;
                             
                tutorialText = translationDataFile.Gettranslation("_MenuTutorial_" + (tID).ToString()); //SET TOOLTIP TEXT BASED ON ID NUMBER (THIS SEQUENCE STARTS AT ID 17)

                highlightRect = ToolTipSystem.GetHighlightRect(targetObject);
                winPos = new Vector2(highlightRect.x,winPos.y); //SET WINDOW POSITION TO SAME X POSITION AS HIGHLIGHT RECT

                //ADD EACH TOOLTIP STRUCT ITEM TO THE LIST OF ITEMS TO BE SHOWN AS PART OF THIS SEQUENCE (ONCE ONE ITEM IS DISMISSED, THE NEXT ITEM IN THE SEQUENCE WILL BE SHOWN AUTOMATICALLY)
                tts.Add(new ToolTipSystem.ToolTipConfig(tutorialText,winPos,windowSize,showPointer,highlightRect,cutoutShape,this.gameObject)); //SET 'TRUE' AS 4TH VALUE IF POINTER IS TO BE SHOWN
                
                if (!GameData.Instance.globalTutorialsViewed.Contains(tID)){
                    GameData.Instance.globalTutorialsViewed.Add(tID); //LOG EACH TOOLTIP ITEM AS VIEWED ONCE ADDED TO THE SEQUENCE OF ITEMS TO BE SHOWN
                }
            }
           
		if (tts.Count > 0){
			GlobalUIController.tooltipUI.ShowTooltip(tts);
            GameData.Instance.SAVE();		
		}
        
    }

    // void Update(){
    //     if(Pethead == null){
    //         if(Pet != null){
    //             if(Pet.GetComponent<Tamagotchi_AdultPetAnimations>() != null){
    //                 Pethead = Pet.GetComponent<Tamagotchi_AdultPetAnimations>().head.gameObject;
    //             }else if(Pet.GetComponent<Tamagotchi_BabyPetAnimation1>() != null){
    //                 Pethead = Pet.GetComponent<Tamagotchi_BabyPetAnimation1>().head.gameObject;
    //             }
    //         }
    //     }
    // }
}


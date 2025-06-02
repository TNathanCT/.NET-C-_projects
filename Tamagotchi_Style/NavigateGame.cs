using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.PlayerLoop;



public class Tamagotchi_NavigateGameScript : MonoBehaviour
{
    public GameObject LivingRoom, KitchenRoom, Bathroom, DailyRewardsPopup;
    public GameObject EditButtonsLivingRoom, EditButtonsBathroom, EditButtonsKitchen, EditDayoWardrobe, editscenebutton, exitButton;
    public Button openRewardsButton;   
    public GameObject displayButtons;
    public List<GameObject> editSceneSectionButtons = new List<GameObject>(); //EDIT ROOM BUTTONS AT TOP OF SCREEN
    public List<GameObject> DisplayRoomButtons = new List<GameObject>(); //ROOM SELECT BUTTONS NEAR BUTTOM OF SCREEN (LIVING ROOM, BATHROOM, KITCHEN)
    List<GameObject> roomEditButtonGroups; //THE GROUPS OF EDIT ITEM BUTTONS FOR EACH ROOM
    TMP_Text openRewardsButtonLabel;
    public TMP_Text bathroomPlayHeader;
    public int editpage;
    //public GameObject notificationPage;
    public static Tamagotchi_NavigateGameScript navscript;
    IEnumerator rewardIconTextRefresh;

    public GameObject bathroomMirror;
  
    void Start(){
        PlayerPrefs.SetInt("Upsell", 0); 
        navscript = this;
        roomEditButtonGroups = new List<GameObject>(){EditButtonsLivingRoom,EditButtonsBathroom,EditButtonsKitchen};
        if(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase == 0){
            Tamagotchi_NavigateGameScript.navscript.editSceneSectionButtons[3].GetComponent<Button>().interactable = false;
            Tamagotchi_NavigateGameScript.navscript.editSceneSectionButtons[2].GetComponent<Button>().interactable = false;
            Tamagotchi_NavigateGameScript.navscript.editSceneSectionButtons[1].GetComponent<Button>().interactable = false;
            Tamagotchi_NavigateGameScript.navscript.DisplayRoomButtons[1].GetComponent<Button>().enabled = false;
            Tamagotchi_NavigateGameScript.navscript.DisplayRoomButtons[2].GetComponent<Button>().enabled = false;                      
            Tamagotchi_NavigateGameScript.navscript.DisplayRoomButtons[1].transform.GetChild(0).gameObject.GetComponent<Button>().enabled = false;
            Tamagotchi_NavigateGameScript.navscript.DisplayRoomButtons[2].transform.GetChild(0).gameObject.GetComponent<Button>().enabled = false;
            Tamagotchi_NavigateGameScript.navscript.DisplayRoomButtons[1].transform.GetChild(3).gameObject.GetComponent<Button>().enabled = false;
        }
        openRewardsButtonLabel = openRewardsButton.GetComponentInChildren<TMP_Text>(true);
        Tamagotchi_AudioManager.audioinstace.StartLivingRoomMusic();
    }   

    public void DisplayEditSceneSectionButtons(){
        for(int i = 0; i < editSceneSectionButtons.Count; i++){
            editSceneSectionButtons[i].SetActive(true);
        }
        editscenebutton.SetActive(false);
    }

    public void DisableEditSceneSectionButtons(){
        for(int i = 0; i < editSceneSectionButtons.Count; i++){
            editSceneSectionButtons[i].GetComponent<Tamagotchi_EditbuttonScript>().isOn = false;
            editSceneSectionButtons[i].SetActive(false);
            Tamagotchi_MarketSliderManager.instance.DisableotherEditButtons();
        }
        editscenebutton.SetActive(true);
    }

    public void GotoEdit(){
        //Tamagotchi_DayoHungerScript.hungerinstance.HoursPassedSinceLastSession();
        //Tamagotchi_PlayerData.instance.Dayo.GetComponent<Tamagotchi_AccessoriesScript>().SetDirtDisplay(100);
        Tamagotchi_EvolutionScript.evoinstance.babyDayopositions[0].SetActive(false); 
        Tamagotchi_AudioManager.audioinstace.StartStoreMusic();
        Tamagotchi_MarketSliderManager.instance.backButton.SetActive(true);
        exitButton.SetActive(false);
        editscenebutton.SetActive(false);
        if(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase >= 1){
            editSceneSectionButtons[1].GetComponent<Button>().enabled = true;
            editSceneSectionButtons[2].GetComponent<Button>().enabled = true;
            editSceneSectionButtons[3].GetComponent<Button>().enabled = true;
            DisplayRoomButtons[2].SetActive(true);
            DisplayRoomButtons[1].SetActive(true);
        }else{}
        //Tamagotchi_MarketSliderManager.instance.ShowEditView();
        switch(editpage){
            case 0:
                displayButtons.SetActive(false);
                editSceneSectionButtons[0].GetComponent<Tamagotchi_EditbuttonScript>().isOn = true;
                GoToEditLivingRoom();
                break;
            case 1:
                displayButtons.SetActive(false);
                editSceneSectionButtons[3].GetComponent<Tamagotchi_EditbuttonScript>().isOn = true;
                GoToEditBathroom();
                break;
            case 2:
                displayButtons.SetActive(false);
                editSceneSectionButtons[1].GetComponent<Tamagotchi_EditbuttonScript>().isOn = true;
                GoToEditKitchen();
                break;
        }
    }

    public IEnumerator UpdateRewardText(){
        while (LivingRoom.activeInHierarchy){
            if (!Tamagotchi_PlayerData.localCachedData.rewards.collected_rewards[Tamagotchi_PlayerData.localCachedData.rewards.current_day-1][0].claimed){
                //IF REWARD FOR TODAY IS NOT CLAIMED YET, SHOW CLAIM REWARD TEXT ON DAILY REWARDS BUTTON
                openRewardsButtonLabel.text =  translationDataFile.Gettranslation("_Tamagotchi_ClaimYourReward");
            }else{ //IF REWARD IS CLAIMED, SHOW COUNTDOWN TIL TOMORROW (WHEN NEXT REWARD SHOULD BE AVAILABLE)
                TimeSpan timeLeft = (DateTime.Today.AddDays(1) - DateTime.Now);
                openRewardsButtonLabel.text = string.Concat(timeLeft.Hours.ToString(),"h " , timeLeft.Minutes.ToString(),"min " , timeLeft.Seconds.ToString(),"s");
            }    
            yield return new WaitForSeconds(1f); //WAIT ONE SECOND BEFORE UPDATING AGAIN
        }
    }

    public void ShowEditPageTutorialSequence(bool forceShow = false){
        List<ToolTipSystem.ToolTipConfig> initialToolTipSequence = new List<ToolTipSystem.ToolTipConfig>(); //SET BLANK LIST

        // INITIAL TOOL TIP SEQUENCE : STEPS 5-6
        for (int t = 0; t < 2; t++){ //SETUP TOOL TIP STRUCT FOR EACH TOOLTIP ITEM
            int tID = 21 + t; //SET GLOBAL TOOLTIP ID NUMBER (THIS SEQUENCE STARTS AT 21)
            if (!GlobalUIController.tooltipUI.toolTipActive && (!GameData.Instance.globalTutorialsViewed.Contains(tID) || forceShow)){ //IF FORCE SHOW SET TO TRUE, SKIP CHECK FOR IF TOOLTIP ITEM HAS BEEN VIEWED BEFORE
                RectTransform targetObject  = EditButtonsLivingRoom.transform.GetChild(0).GetComponent<RectTransform>(); //= target[n];
                //ONLY SHOW IF THIS TUTORIAL STEP HAS NOT BEEN VIEWED BEFORE
                Vector2 winPos = Vector2.zero;
                Vector2 windowSize = new Vector2(600f,300f);
                ToolTipSystem.maskShape cutoutShape = ToolTipSystem.maskShape.SQUARE;
                Rect highlightRect = new Rect();
                bool showPointer = true;
                string tutorialText = "";
                switch(t){
                    case 0: //EDIT BUTTON
                        targetObject = EditButtonsLivingRoom.transform.GetChild(1).GetComponent<RectTransform>(); //SET TARGET OBJECT TO FRAME EDIT BUTTON OBJECT
                        cutoutShape = ToolTipSystem.maskShape.SQUARE; //CUTOUT SHAPE - THE SHAPE OF THE NON COVERED AREA
                        windowSize = new Vector2(420f,275f); //SET THE SIZE OF THE TOOLTIP WINDOW		
                        showPointer = true;                
                        highlightRect = ToolTipSystem.GetHighlightRect(targetObject);
                        winPos = new Vector2(highlightRect.x,highlightRect.y - 400f); //SET WINDOW POSITION TO SAME X POSITION AND BELOW HIGHLIGHT RECT

                        break;
                    case 1: //EDIT ROOM BUTTONS
                        targetObject = Tamagotchi_NavigateGameScript.navscript.editSceneSectionButtons[1].GetComponent<RectTransform>();
                        cutoutShape = ToolTipSystem.maskShape.SQUARE; //CUTOUT SHAPE - THE SHAPE OF THE NON COVERED AREA
                        windowSize = new Vector2(350f,275f); //SET THE SIZE OF THE TOOLTIP WINDOW
                        showPointer = true;                            
                        if (targetObject != null && targetObject.GetType() == typeof(RectTransform)){
                            RectTransform targetRect = (RectTransform)targetObject;
                            RectTransform tempRect = GameObject.Instantiate(targetRect.gameObject,targetRect.position,Quaternion.identity,Tamagotchi_PlayerData.instance.mainCanvas.transform).GetComponent<RectTransform>();
                            tempRect.SetParent(AppSystem.sharedUI.transform); 
                            Vector3 originalPos = tempRect.localPosition;
                            tempRect.pivot = GlobalUIController.UIAnchorRef.MidCenter; //SET RECT PIVOT TO CENTRE OF OBJECT AND ANCHOR TO TO CENTRE OF SCREEN
                            tempRect.anchorMax = GlobalUIController.UIAnchorRef.MidCenter;
                            tempRect.anchorMin = GlobalUIController.UIAnchorRef.MidCenter;//BottomLeft;
                            tempRect.localPosition = originalPos;

                            //CREATE A DUPLICATE OF THE RECTTRANSFORM TO ENSURE THAT THE ANCHORS ARE SET CORRECTLY, WHICH WILL BE USED AS THE BASIS FOR THE TARGET HIGHLIGHT RECT
                            highlightRect.Set (
                                tempRect.anchoredPosition.x,// != 0 ? (tempRect.anchoredPosition.x - (AppStartHook.deviceCanvasSize.x/2f)) : tempRect.anchoredPosition.x, //+ (AppStartHook.deviceCanvasSize.x/2f),
                                tempRect.anchoredPosition.y ,//+ (AppStartHook.deviceCanvasSize.y/2f), //+ (AppStartHook.deviceCanvasSize.y/2f) - (tempRect.rect.height/2f),
                                ((tempRect.rect.width+10f)*3)+20f, //MULTIPLY BY 3 SO HIGHLIHGHT RECT COVERS ALL 3 EDIT ROOM BUTTONS //ADD 20PX BUFFER TO WIDTH AND HEIGHT TO GIVE EXTRA SPACE AROUND THE HIGHLIGHT TARGET ITEM
                                tempRect.rect.height+20f //ADD 20PX BUFFER TO WIDTH AND HEIGHT TO GIVE EXTRA SPACE AROUND THE HIGHLIGHT TARGET ITEM
                            );

                            winPos = new Vector2(highlightRect.x,highlightRect.y - 400f); //SET WINDOW POSITION TO SAME X POSITION AND BELOW HIGHLIGHT RECT
                            Destroy(tempRect.gameObject);	
                        } 
                        break;
                    default:
                        break;
                }                                
                tutorialText = translationDataFile.Gettranslation("_MenuTutorial_" + (tID).ToString()); //SET TOOLTIP TEXT BASED ON ID NUMBER (THIS SEQUENCE STARTS AT ID 17)
            	
                //ADD EACH TOOLTIP STRUCT ITEM TO THE LIST OF ITEMS TO BE SHOWN AS PART OF THIS SEQUENCE (ONCE ONE ITEM IS DISMISSED, THE NEXT ITEM IN THE SEQUENCE WILL BE SHOWN AUTOMATICALLY)
                initialToolTipSequence.Add(new ToolTipSystem.ToolTipConfig(tutorialText,winPos,windowSize,showPointer,highlightRect,cutoutShape,this.gameObject)); //SET 'TRUE' AS 4TH VALUE IF POINTER IS TO BE SHOWN
                
                if (!GameData.Instance.globalTutorialsViewed.Contains(tID)){
                    GameData.Instance.globalTutorialsViewed.Add(tID); //LOG EACH TOOLTIP ITEM AS VIEWED ONCE ADDED TO THE SEQUENCE OF ITEMS TO BE SHOWN
                }
            }
        }   
		if (initialToolTipSequence.Count > 0){
			GlobalUIController.tooltipUI.ShowTooltip(initialToolTipSequence);		
		}
        GameData.Instance.SAVE();
    }

    public void LeaveEdit(){
        Tamagotchi_AudioManager.audioinstace.StartLivingRoomMusic();
        if(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase != 0 && Tamagotchi_PlayerData.instance.Dayo!=null){
            Tamagotchi_PlayerData.instance.Dayo.SetActive(true);
        }        
        if(Tamagotchi_MarketSliderManager.instance.sliderOpen){
            Tamagotchi_MarketSliderManager.instance.ChangeSection(Tamagotchi_NavigateGameScript.navscript.editpage); //REFRESH SECTION BEFORE CLOSING EDIT MODE 
            Tamagotchi_MarketSliderManager.instance.SliderActionButton(-1); //FORCE CLOSE ITEM SELECT SLIDER
        }
        switch(editpage){
            case 0: //LIVING ROOM
            case 3: //WARDROBE <- DO SAME AS LIVING ROOM WHEN EXITING EDIT MODE
                GoToDisplayLivingRoom();
                break;
           case 1:
                GoToDisplayBathroom();
                break;
            case 2:
                GoToDisplayKitchen();
                break;     
        }
    }
    
    public void ChosenDisplayButton(){
        DisplayRoomButtons[editpage].GetComponent<Image>().enabled = true;
        for(int i = 0 ; i < DisplayRoomButtons.Count; i++){
            if(i != editpage){
                DisplayRoomButtons[i].GetComponent<Image>().enabled = false;
            }
        }
    }
    public void ShowEditPage(int targetPage){
        editpage = targetPage;
        //HIDE DISPLAY MODE ELEMENTS
        if(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase == 0){
            Tamagotchi_PlayerData.instance.Dayo = GameObject.Find("DayoEgg");
        }
        if(Tamagotchi_PlayerData.instance.Dayo != null){
            Tamagotchi_PlayerData.instance.Dayo.SetActive(false);
        }        
        Tamagotchi_MarketSliderManager.instance.topIcons.GetComponent<Image>().enabled = true;
        editscenebutton.SetActive(false);
        exitButton.SetActive(false);
        KitchenRoom.SetActive(editpage == 2);
        LivingRoom.SetActive(editpage == 0);
        Bathroom.SetActive(editpage == 1);

        Tamagotchi_MarketSliderManager.instance.backButton.SetActive(true);

        EditButtonsBathroom.SetActive(targetPage == 1);
        EditButtonsLivingRoom.SetActive(targetPage == 0);
        EditButtonsKitchen.SetActive(targetPage == 2);        

        foreach(GameObject editButtonGroup in roomEditButtonGroups){
            foreach(Tamagotchi_EditbuttonScript editButton in editButtonGroup.GetComponentsInChildren<Tamagotchi_EditbuttonScript>(true)){
                //ONLY SHOW EDIT BUTTONS FOR AREAS WHERE THE USER HAS MORE THAN ONE ITEM AVAILABLE
                editButton.gameObject.SetActive(Tamagotchi_MarketSliderManager.instance.allLists.Find(aList => aList.area == editButton.editType).objectList.Count > 1);
            }
        }
        if (editpage != 3){
            Tamagotchi_MarketSliderManager.instance.TamagotchiShopSections[3].SetActive(false); //HIDE WARDROBE AREA IF ACTIVE
        }
        DisplayEditSceneSectionButtons();
    }    
    public void ShowDisplayMode(int targetMode){
        editpage = targetMode;
        Tamagotchi_MarketSliderManager.instance.topIcons.GetComponent<Image>().enabled = false;
        displayButtons.SetActive(true);
        LivingRoom.SetActive(editpage == 0);
        Bathroom.SetActive(editpage == 1);
        KitchenRoom.SetActive(editpage == 2); 
        Tamagotchi_MarketSliderManager.instance.TamagotchiShopSections[3].SetActive(false); //HIDE WARDROBE AREA IF ACTIVE
        exitButton.SetActive(true);
        editscenebutton.SetActive(true);    
        //HIDE EDIT MODE COMPONENTS    
        EditButtonsLivingRoom.SetActive(false);
        EditButtonsBathroom.SetActive(false);
        EditButtonsKitchen.SetActive(false);
        Tamagotchi_MarketSliderManager.instance.backButton.SetActive(false);
        //SET MAIN DISPLAY CHANGE AREA BUTTONS 
        ChosenDisplayButton();
        DisableEditSceneSectionButtons();
    }
    public void GoToEditLivingRoom(){
        ShowEditPage(0);
        openRewardsButton.gameObject.SetActive(false); //HIDE REWARDS BUTTON IN EDIT MODE
        ShowEditPageTutorialSequence(); //SHOW EDIT PAGE TUTORIAL SEQUENCE IF NEEDED
    }

    public void GoToEditBathroom(){
        bathroomMirror.GetComponent<Button>().enabled = false;
        ShowEditPage(1);
    }

    public void GoToEditKitchen(){
        ShowEditPage(2);
        //HIDE DAYO AND FOOD BUTTON WHEN GOING INTO KITCHEN EDIT MODE
        Tamagotchi_KitchenRoomScript.kitcheninstance.dayoArea.SetActive(false);
        Tamagotchi_KitchenRoomScript.kitcheninstance.foodTableArea.SetActive(false); 
    }

     public void GoToDisplayBathroom(){
        ShowDisplayMode(1);
        bathroomMirror.GetComponent<Button>().enabled = true;
        Tamagotchi_EvolutionScript.evoinstance.babyDayopositions[0].SetActive(false);  
        bathroomPlayHeader.text = translationDataFile.Gettranslation("_StartScreen_PlayHeader");
        if(Tamagotchi_PlayerData.instance.Dayo != null){
            MoveDayo(Bathroom.transform.Find("Room"));
        }
     
        if(PlayerPrefs.GetString("LastScene") != "Tamagotchi_BathGame"){
            ShowBathroomTutorialSequence(true); //FORCE BATHROOM TOOLTIP TO APPEAR EVERY SINGLE TIME BATHROOM PAGE IS OPENED VIA THE BUTTONS
        }else{
           GlobalUIController.tooltipUI.DismissTutorial();
        }
        StartCoroutine(Tamagotchi_BathroomScript.bathroominstance.SwingPlant());
    }

    public void GoToDisplayLivingRoom(){
        ShowDisplayMode(0);
        if(Tamagotchi_PlayerData.instance.Dayo != null){
            MoveDayo(LivingRoom.transform.Find("Room"));
        }
        DailyRewardsPopup.SetActive(false);
        openRewardsButton.gameObject.SetActive(true);

        //SET COROUTINE TO UPDATE REWARD ICON TEXT WHILE LIVING ROOM IS ACTIVE
        if (rewardIconTextRefresh != null){ 
            StopCoroutine(rewardIconTextRefresh);
        }
        rewardIconTextRefresh = UpdateRewardText();
        StartCoroutine(rewardIconTextRefresh);

        if (Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase > 0){ //IF NOT EGG
            Tamagotchi_PlayerData.instance.SettingUpMarketClothes(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase-1); //SET CLOTHES 
            Tamagotchi_PlayerData.instance.Dayo.GetComponent<Tamagotchi_AccessoriesScript>().SetDirtDisplay(); //SET DIRT VISIBILITY
            Tamagotchi_EvolutionScript.evoinstance.babyDayopositions[0].SetActive(false);  
        }
    }
    public void OpenDailyRewards(){
        DailyRewardsPopup.SetActive(true);
    }

    public void GoToDisplayKitchen(){
        ShowDisplayMode(2);
        Tamagotchi_EvolutionScript.evoinstance.babyDayopositions[1].SetActive(false); //?? WHATS THIS FOR?...
        // if(currentDayo != null && Tamagotchi_EvolutionScript.evoinstance.evolutionstage > 1){
        //     for(int i = 0; i < Tamagotchi_KitchenRoomScript.kitcheninstance.accessorylist.Count; i++){ 
        //         Tamagotchi_KitchenRoomScript.kitcheninstance.accessorylist[i] = currentDayo.transform.Find("Head/Eyes/Accessory").GetChild(i).gameObject;
        //     }
        // }
        Tamagotchi_KitchenRoomScript.kitcheninstance.SetUpKitchen();//Tamagotchi_PlayerData.instance.itemChosentoDisplay);
    }

    public void MoveDayo(Transform room){ //MOVE THE MAIN DAYO INSTANCE BETWEEN ROOMS LIKE LIVING ROOM / BATHROOM AS NEEDED 
        Tamagotchi_PlayerData.instance.Dayo.SetActive(true); 
        Tamagotchi_PlayerData.instance.Dayo.transform.SetParent(room); //SET MAIN DAYO INSTANCE'S PARENT TRANSFORM TO TARGET ROOM TRANSFORM PROVIDED AS PARAMETER
        //START THE ANIMATIONS FOR THE TARGET DAYO INSTANCE WHETHER THE CURRENT EVOLTUION STAGE IS BABY OR ADULT/FANTASY
        if(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase == 1 &&Tamagotchi_PlayerData.instance.Dayo.GetComponent<Tamagotchi_BabyDayoAnimation1>() != null ){
            Tamagotchi_PlayerData.instance.Dayo.GetComponent<Tamagotchi_BabyDayoAnimation1>().StartMainSceneAnimations();
        }else if(Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase == 2 || Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase == 3){
            Tamagotchi_PlayerData.instance.Dayo.GetComponent<Tamagotchi_AdultDayoAnimations>().StartMainSceneAnimations();
        }
        for(int i = 0; i < Tamagotchi_PlayerData.instance.Dayo.GetComponent<Tamagotchi_HeartEffects>().hearts.Length; i++){
            Tamagotchi_PlayerData.instance.Dayo.GetComponent<Tamagotchi_HeartEffects>().hearts[i].gameObject.transform.parent = Tamagotchi_PlayerData.instance.Dayo.transform;
        }
    }

    public void ShowTutorial(){ //THIS IS NEVER CALLED?...
        ShowKitchenTutorialSequence(); //SHOW KITCHEN TOOLTIPS IF NEEDED
    }

    public void ShowKitchenTutorialSequence(bool forceShow = false){
        List<ToolTipSystem.ToolTipConfig> kitchenTipSequence = new List<ToolTipSystem.ToolTipConfig>(); //SET BLANK LIST
        // BABY TOOL TIP SEQUENCE : STEPS 10
        
        for (int t = 0; t < 1; t++){ //SETUP TOOL TIP STRUCT FOR EACH TOOLTIP ITEM
            int tID = 26 + t; //SET GLOBAL TOOLTIP ID NUMBER (THIS SEQUENCE STARTS AT 26)
            if (!GlobalUIController.tooltipUI.toolTipActive && (!GameData.Instance.globalTutorialsViewed.Contains(tID) || forceShow)){ //IF FORCE SHOW SET TO TRUE, SKIP CHECK FOR IF TOOLTIP ITEM HAS BEEN VIEWED BEFORE
                RectTransform targetObject  = Tamagotchi_KitchenRoomScript.kitcheninstance.emptyplate.GetComponent<RectTransform>();//= target[n];
                //ONLY SHOW IF THIS TUTORIAL STEP HAS NOT BEEN VIEWED BEFORE
                Vector2 winPos = Vector2.zero;
                Vector2 windowSize = new Vector2(600f,300f);
                ToolTipSystem.maskShape cutoutShape = ToolTipSystem.maskShape.SQUARE;
                Rect highlightRect = new Rect();
                bool showPointer = true;
                string tutorialText = "";
                switch(t){
                    case 0: //MINIGAME BUTTON
                        winPos = new Vector2(0,-520); //-820);
                        targetObject = Tamagotchi_KitchenRoomScript.kitcheninstance.emptyplate.GetComponent<RectTransform>();
                        cutoutShape = ToolTipSystem.maskShape.SQUARE; //CUTOUT SHAPE - THE SHAPE OF THE NON COVERED AREA
                        windowSize = new Vector2(350f,220f); //SET THE SIZE OF THE TOOLTIP WINDOW		
                        showPointer = true;
                        break;
                    default:
                        break;
                }                                
                tutorialText = translationDataFile.Gettranslation("_MenuTutorial_" + (tID).ToString()); //SET TOOLTIP TEXT BASED ON ID NUMBER (THIS SEQUENCE STARTS AT ID 17)
                highlightRect = ToolTipSystem.GetHighlightRect(targetObject);
                winPos = new Vector2(highlightRect.x,winPos.y); //SET WINDOW POSITION TO SAME X POSITION AS HIGHLIGHT RECT
                
                //ADD EACH TOOLTIP STRUCT ITEM TO THE LIST OF ITEMS TO BE SHOWN AS PART OF THIS SEQUENCE (ONCE ONE ITEM IS DISMISSED, THE NEXT ITEM IN THE SEQUENCE WILL BE SHOWN AUTOMATICALLY)
                kitchenTipSequence.Add(new ToolTipSystem.ToolTipConfig(tutorialText,winPos,windowSize,showPointer,highlightRect,cutoutShape,this.gameObject)); //SET 'TRUE' AS 4TH VALUE IF POINTER IS TO BE SHOW
                if (!GameData.Instance.globalTutorialsViewed.Contains(tID)){
                    GameData.Instance.globalTutorialsViewed.Add(tID); //LOG EACH TOOLTIP ITEM AS VIEWED ONCE ADDED TO THE SEQUENCE OF ITEMS TO BE SHOWN
                }
            }
        }
           
		if (kitchenTipSequence.Count > 0){
			GlobalUIController.tooltipUI.ShowTooltip(kitchenTipSequence);		
		}
        GameData.Instance.SAVE();
    }

    public void ShowBathroomTutorialSequence(bool forceShow = false){

        if (forceShow == true){ //IF FORCE SHOW SET TO TRUE, SKIP CHECK FOR IF TOOLTIP ITEM HAS BEEN VIEWED BEFORE
                List<ToolTipSystem.ToolTipConfig> bathroomTipSequence = new List<ToolTipSystem.ToolTipConfig>(); //SET BLANK LIST
            int tID = 29; //SET GLOBAL TOOLTIP ID NUMBER (THIS SEQUENCE STARTS AT 29)
            //GET THE LOCATION OF THE BATHROOM TUB 
            RectTransform targetObject  = Tamagotchi_PlayerData.instance.itemChosentoDisplay.Find(activeItem => activeItem.tag == "Tub").GetComponent<RectTransform>();//= target[n];
            string tutorialText = "";
            Rect highlightRect = new Rect();
            Vector2 winPos = new Vector2(0,-520); //-820);
            ToolTipSystem.maskShape cutoutShape = ToolTipSystem.maskShape.SQUARE; //CUTOUT SHAPE - THE SHAPE OF THE NON COVERED AREA
            Vector2 windowSize = new Vector2(350f,220f); //SET THE SIZE OF THE TOOLTIP WINDOW		
            bool showPointer = true;
               
            tutorialText = translationDataFile.Gettranslation("_MenuTutorial_" + (tID).ToString()); //SET TOOLTIP TEXT BASED ON ID NUMBER (THIS SEQUENCE STARTS AT ID 17)
            highlightRect = ToolTipSystem.GetHighlightRect(targetObject);
            winPos = new Vector2(highlightRect.x,winPos.y); //SET WINDOW POSITION TO SAME X POSITION AS HIGHLIGHT RECT

            //ADD EACH TOOLTIP STRUCT ITEM TO THE LIST OF ITEMS TO BE SHOWN AS PART OF THIS SEQUENCE (ONCE ONE ITEM IS DISMISSED, THE NEXT ITEM IN THE SEQUENCE WILL BE SHOWN AUTOMATICALLY)
            bathroomTipSequence.Add(new ToolTipSystem.ToolTipConfig(tutorialText,winPos,windowSize,showPointer,highlightRect,cutoutShape,this.gameObject)); //SET 'TRUE' AS 4TH VALUE IF POINTER IS TO BE SHOW
            if (!GameData.Instance.globalTutorialsViewed.Contains(tID)){
                GameData.Instance.globalTutorialsViewed.Add(tID); //LOG EACH TOOLTIP ITEM AS VIEWED ONCE ADDED TO THE SEQUENCE OF ITEMS TO BE SHOWN
            }
        
        if (bathroomTipSequence.Count > 0){
            GlobalUIController.tooltipUI.ShowTooltip(bathroomTipSequence);		
        }
        GameData.Instance.SAVE();
        } 
    }

    public void OpenMiniGame(){
        PlayerPrefs.SetInt("EvolutionStage", Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase);
        StopAllCoroutines();
        DisableAllInstances();
        AppSystem.LoadSceneAfterClean("Tamagotchi_Game");
    }
    
    public void OpenBathMiniGame(){
        PlayerPrefs.SetInt("EvolutionStage", Tamagotchi_PlayerData.localCachedData.tamagotchi.evolution_phase);
        Tamagotchi_PlayerData.instance.PrepareAssetsForBathMinigame();
        StopAllCoroutines();
        DisableAllInstances();
        AppSystem.LoadSceneAfterClean("Tamagotchi_BathGame");
    }

    public void DisableAllInstances(){
        Tamagotchi_AudioManager.audioinstace = null;
        Tamagotchi_BathroomScript.bathroominstance = null;
    //    Tamagotchi_DayoEatingAnimation.dayoanim = null;
        Tamagotchi_DayoHungerScript.hungerinstance = null;
        Tamagotchi_EvolutionScript.evoinstance = null;
        Tamagotchi_HeartEffects.effectinstance = null;
        Tamagotchi_KitchenRoomScript.kitcheninstance = null;
        Tamagotchi_MarketSliderManager.instance = null;
        Tamagotchi_NavigateGameScript.navscript = null;
        //Tamagotchi_RotateBackandForth.instance = null;
        DailyRewardSystem.Tamagotchi_DailyRewardsScript.instance = null;
        //Tamagotchi_PlayerData.instance = null;
        Tamagotchi_PlayerData.instance.UnloadItemLibrary(); //CLEAR LOADERS FOR ADDRESSABLES 
        Resources.UnloadUnusedAssets();
    }


    public void QuitGame(){
        StopAllCoroutines();
        DisableAllInstances();
        
        Tamagotchi_PlayerData.instance = null;
        AppSystem.instance.ReturnToMain();
    }

    public void GoToUpsellPage(){
        PlayerPrefs.SetInt("Upsell", 1);
        AppSystem.instance.TamagotchitoUpsellPage();
    }

    public GameObject referpage;
    public void CloseReferPage(){
        referpage.SetActive(false);
    }
}

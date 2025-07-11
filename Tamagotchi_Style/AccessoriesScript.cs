using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoriesScript : MonoBehaviour
{
    public enum SubBodyPart {Head,LeftLeg,RightLeg,Torso,Tail,LeftArm,RightArm,Back};
    public bool isBaby; //TO DETERMINE WETHER TO USE BABY CLOTHING ITEMS OR ADULT CLOTHING ITEMS
    [System.Serializable] public class BodyArea{
        public Tamagotchi_PlayerData.itemType type; //WHAT ITEM TYPE THIS AREA WILL CONTAIN
        public Transform areaContainer; //THE PARENT OBJECT WHERE THE ITEMS WILL BE PLACED
    }
    [System.Serializable] public class SubBodyPartArea{
        public SubBodyPart bodyPart;
        public GameObject targetObject;
        public Sprite defaultSprite;
        public void ResetSprite(){ //RESETS SPRITE TO DEFAULT
            if (targetObject.GetComponent<Image>() != null){
                targetObject.GetComponent<Image>().sprite = defaultSprite;
            }
        }
    }
    public Dictionary<Tamagotchi_PlayerData.itemType,List<GameObject>> clothingList; //DICTIONARY OF CLOTHING ITEMS AVAILABLE FOR EACH SECTION
    public List<BodyArea> itemBodyAreas = new List<BodyArea>(); 
    public List<SubBodyPartArea> subBodyPartAreas = new List<SubBodyPartArea>();
    public List<Image> dirtStains;
    public List<Sprite> pajamaOutfit = new List<Sprite>();
    public Color currentDirtImageColour;
    bool initialised = false;
    void Awake(){
        Initialise();
    }

    public void Initialise(){
        if (!initialised){
            for(int bp = 0; bp < subBodyPartAreas.Count; bp++){
                //LOG THE DEFAULT SPRITES FOR EACH OF THE SUB BODYPARTS FOR WHEN THE BODY PARTS NEED TO BE RESET
                try{
                    subBodyPartAreas[bp].defaultSprite = subBodyPartAreas[bp].targetObject.GetComponent<Image>().sprite;
                }catch{ 
                    //IF GETTING THE IMAGE ELEMENT FAILS (IF THE AREA DOES NOT HAVE AN IMAGE COMPONENT, DO NOTHING AND LEAVE DEFAULT SPRITE BLANK)
                }
            }
            clothingList = new Dictionary<Tamagotchi_PlayerData.itemType,List<GameObject>>();
            currentDirtImageColour = ColorHelper.HexToColor("3c3a38"); //DEFAULT DIRT COLOUR - GREY
            initialised = true;
        }
    }   
    public void DisplayDefaultObj(Tamagotchi_PlayerData.itemType targetAreaType, List<GameObject> targetObjList){
        //SHOW THE DEFAULT ITEM FOR THE TARGET AREA
        DisplayObj(targetAreaType, targetObjList, Tamagotchi_PlayerData.instance.itemLibrary.Find(itemType => itemType.type == targetAreaType).defaultAreaItem);
    }

    public void DisplayObj(Tamagotchi_PlayerData.itemType targetAreaType, List<GameObject> targetObjList, GameObject overrideObj = null){ //OVERRIDE OBJECT TO BE USED TO SHOW CLOTHING BASED ON SPECIFIED ITEM AND NOT FROM CURRENT CHOSEN ITEMS;
        Initialise(); //CHECK IF INITIALISED BEFORE ATTEMPTING TO DISPLAYOBJ
        //FIND THE TARGET TRANSFORM CORRESPONDING TO THE CORRECT BODY PART AREA FOR WHERE THE ITEM SHOULD BE PLACED
        Transform targetArea = itemBodyAreas.Find(areaObj => areaObj.type == targetAreaType).areaContainer;
        if (!clothingList.ContainsKey(targetAreaType)){
            clothingList.Add(targetAreaType,new List<GameObject>()); //IF A LIST FOR THE CURRENT AREA TYPE IS NOT ALREADY IN THE CLOTHING LIST, ADD A NEW ENTRY BEFORE CONTINUING
        }
        foreach(GameObject otherItem in clothingList[targetAreaType]){
            otherItem.SetActive(false); //HIDE ALL OTHER CLOTHING ITEMS IN LIST
        }
        foreach(GameObject obj in targetObjList){ //LOOP THROUGH ALL OBJECTS IN THE TARGET LIST
            //IF THIS CURRENT OBJECT IN THE TARGET LIST IS FOUND IN THE LIST OF ITEMS CURRENTLY SET TO BE DISPLAYED, CREATE/SHOW INSTANCE OF TARGET OBJECT
            GameObject targetObj = Tamagotchi_PlayerData.instance.itemChosentoDisplay.Find( itemObj => string.Equals(itemObj.name, obj.name, System.StringComparison.OrdinalIgnoreCase));
            if (overrideObj != null){
                targetObj = overrideObj;
            }

            if (targetObj != null && targetArea != null){
                GameObject existingObjInstance = clothingList[targetAreaType].Find( exObj => string.Equals(exObj.name ,targetObj.name, System.StringComparison.OrdinalIgnoreCase) ); //CHECK IF AN INSTANCE OF THE TARGET GAMEOBJECT WITH MATCHING NAME ALREADY EXISTS IN THE CLOTHINGINSTANCEOBJECTS LIST
                if (existingObjInstance != null){ //IF INSTANCE OF TARGET CLOTHING ITEM EXISTS IN DICTIONARY, USE THAT
                    existingObjInstance.SetActive(true); //ENABLE TARGET ITEM
                }else{ //OTHERWISE CREATE NEW INSTANCE OF OBJECT WHEN IT IS NOT ALREAY PRESENT
                    GameObject itemCopy = GameObject.Instantiate(targetObj, targetArea.position, Quaternion.identity, targetArea);
                    itemCopy.SetActive(true);
                    itemCopy.name = targetObj.name;
                    itemCopy.transform.localPosition = Vector3.zero;//targetObj.transform.localPosition;
                    itemCopy.transform.localScale = Vector3.one;//targetObj.transform.localScale;
                    itemCopy.transform.localEulerAngles = Vector3.zero; //RESET ROTATION IN CASE PARENT OBJECT IS ROTATING DURING INSTANTIATION
                    RectTransform objRect = itemCopy.GetComponent<RectTransform>();
                    clothingList[targetAreaType].Add(itemCopy);
                    targetObj = null;
                }
            }
            if(overrideObj != null){
                break;
            }
        }
    }

    public void ResetClothes(){
        if (isBaby){
            DisplayDefaultObj(Tamagotchi_PlayerData.itemType.clothing_babyclothes, Tamagotchi_MarketSliderManager.instance.babypetclothes.objectList);
            DisplayDefaultObj(Tamagotchi_PlayerData.itemType.clothing_babyhat, Tamagotchi_MarketSliderManager.instance.babypethat.objectList);
            DisplayDefaultObj(Tamagotchi_PlayerData.itemType.clothing_babypacifier, Tamagotchi_MarketSliderManager.instance.passyList.objectList);
        }else{
            DisplayDefaultObj(Tamagotchi_PlayerData.itemType.clothing_vest, Tamagotchi_MarketSliderManager.instance.vesteList.objectList);
            DisplayDefaultObj(Tamagotchi_PlayerData.itemType.clothing_hat, Tamagotchi_MarketSliderManager.instance.hatList.objectList);
            DisplayDefaultObj(Tamagotchi_PlayerData.itemType.clothing_accessory, Tamagotchi_MarketSliderManager.instance.accessoryList.objectList);
        }
    }

    public void DisplayVeste(){
        if (isBaby){
            DisplayObj(Tamagotchi_PlayerData.itemType.clothing_babyclothes, Tamagotchi_MarketSliderManager.instance.babypetclothes.objectList);
        }else{
            DisplayObj(Tamagotchi_PlayerData.itemType.clothing_vest, Tamagotchi_MarketSliderManager.instance.vesteList.objectList);
        }
    }

    public void DisplayHat(){
        if (isBaby){
            DisplayObj(Tamagotchi_PlayerData.itemType.clothing_babyhat, Tamagotchi_MarketSliderManager.instance.babypethat.objectList);
        }else{
            DisplayObj(Tamagotchi_PlayerData.itemType.clothing_hat, Tamagotchi_MarketSliderManager.instance.hatList.objectList);
        }
    }

    public void DisplayAccessory(){
        if (isBaby){
            DisplayObj(Tamagotchi_PlayerData.itemType.clothing_babypacifier, Tamagotchi_MarketSliderManager.instance.passyList.objectList);
        }else{
            DisplayObj(Tamagotchi_PlayerData.itemType.clothing_accessory, Tamagotchi_MarketSliderManager.instance.accessoryList.objectList);
        }
    }

    public void SetDirtDisplay(float amount = -1f){ //0 = CLEAN | 100 = DIRTY
        if (amount == -1f){ //IF NO VALUE PROVIDED, DEFAULT TO HYGINEAMOUNT FROM HUNGER SCRIPT
            amount = Tamagotchi_petHungerScript.hungerinstance.hygieneamout;
        }
      
       float imagealpha = 0;
        if(amount <= 45f){
            imagealpha = 100 - (float)amount;
        }
        else if(amount > 45f){
            imagealpha = 0;
        }
        //imagealpha = amount/100;
        currentDirtImageColour.a = (imagealpha/10)*0.1f;
        for(int j = 0; j <= dirtStains.Count-1; j++){
           dirtStains[j].color = currentDirtImageColour;
        }
    }

    public void ResetSubBodyPart(SubBodyPart targetPart){
        subBodyPartAreas.Find(sbp => sbp.bodyPart == targetPart).ResetSprite();
    }

    public void ChangeClothesOnpet(){
        //SET THE CLOTHES SELECTION ON THIS pet TO THE CURRENTLY ACTIVE CLOTHES SELECTED BY THE USER
        DisplayAccessory();
        DisplayHat();
        DisplayVeste();
    }


}

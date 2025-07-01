using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tamagotchi_Translation : MonoBehaviour
{

    public TMP_Text imFull;
    public Text subscriptionbannertext;
    public TMP_Text dailyrewarddescription;
    public TMP_Text dailyrewardbonustitle;
    public TMP_Text canclaimreward;
    public TMP_Text claimbuttontext;
//    public TMP_Text losestreaktext; //MOVED TO DAILY REWARDS SCRIPT
//    public TMP_Text keepstreak;
//    public TMP_Text loseit;
    //public TMP_Text notificationtext;

    public TMP_Text subscribeto;
    public TMP_Text subscribepointone;
    public TMP_Text subscribepointtwo;
    public TMP_Text subscribepointthree;
    public TMP_Text learnmoretext;
    public TMP_Text rewardtoptext;
    public TMP_Text playminigameText;

    //public Text confirmationpurchasetext;
    //public Text yesconfirmtext;
    //public Text nothankyouconfirm;




    public void StartTranslations(){
        //imFull.text = LocalizationDataFile.GetLocalization("_Tamagtochi_Imful"); 
        subscriptionbannertext.text = LocalizationDataFile.GetLocalization("_Tamagotchi_SubscriberExclusive");
      //  dailyrewarddescription.text = LocalizationDataFile.GetLocalization("_Tamagotchi_DailyRewardRules");
        //dailyrewardbonustitle.text = LocalizationDataFile.GetLocalization("_Tamagotchi_DailyLoginBonus");
       // canclaimreward.text = LocalizationDataFile.GetLocalization("_Tamagotchi_ClaimYourReward");
        //claimbuttontext.text = LocalizationDataFile.GetLocalization("_Tamagotchi_ClaimButton");
        //losestreaktext.text = "5" + " " + LocalizationDataFile.GetLocalization("_Tamagotchi_DaysBonusStreakLost");//MOVED TO DAILY REWARDS SCRIPT
        //keepstreak.text = LocalizationDataFile.GetLocalization("_Tamagotchi_KeepItFor") + " 200";
        //loseit.text = LocalizationDataFile.GetLocalization("_Tamagotchi_LoseIt");  
        //notificationtext.text = LocalizationDataFile.GetLocalization("_Tamagotchi_NotEnoughCoins");    
    
        subscribeto.text = LocalizationDataFile.GetLocalization("_MainMenu_Alert_SubscriptionRequired_Text_1");
        learnmoretext.text = LocalizationDataFile.GetLocalization("_MainMenu_Alert_SubscriptionRequired_Confirm");
        subscribepointone.text = LocalizationDataFile.GetLocalization("_SettingsPage_Reminder_UpsellHeader");
        subscribepointtwo.text = LocalizationDataFile.GetLocalization("_PARENTAPPNEWS2");
        subscribepointthree.text = LocalizationDataFile.GetLocalization("_MenuTutorial_3_B");
        //playminigameText.text = LocalizationDataFile.GetLocalization("_StartScreen_PlayHeader");
        //rewardtoptext.text = LocalizationDataFile.GetLocalization("_Tamagotchi_TodayYouHaveEarned");
    } 




}

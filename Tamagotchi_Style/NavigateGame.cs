using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomNavigator : MonoBehaviour
{
    public GameObject roomLiving, roomKitchen, roomBath, popupDailyRewards;
    public GameObject uiEditLiving, uiEditBath, uiEditKitchen, wardrobeUI, sceneEditBtn, quitBtn;
    public Button btnOpenRewards;
    public GameObject uiRoomSelector;

    public List<GameObject> sectionEditButtons = new();
    public List<GameObject> roomSwitchButtons = new();
    private List<GameObject> roomEditGroups;
    private TMP_Text labelRewardButton;
    public TMP_Text headerBathPlay;
    public GameObject bathMirror;

    public int currentEditRoomIndex;
    public static RoomNavigator Instance;
    private IEnumerator rewardLabelUpdater;

    void Start()
    {
        PlayerPrefs.SetInt("Upsell", 0);
        Instance = this;
        roomEditGroups = new() { uiEditLiving, uiEditBath, uiEditKitchen };

        if (PlayerData.local.tamagotchi.evolution_phase == 0)
        {
            LockAdvancedRooms();
        }

        labelRewardButton = btnOpenRewards.GetComponentInChildren<TMP_Text>(true);
        AudioService.Instance.PlayMainTheme();
    }

    private void LockAdvancedRooms()
    {
        foreach (var idx in new[] { 1, 2, 3 })
        {
            sectionEditButtons[idx].GetComponent<Button>().interactable = false;
        }
        for (int i = 1; i < 3; i++)
        {
            var roomBtn = roomSwitchButtons[i];
            roomBtn.GetComponent<Button>().enabled = false;
            roomBtn.transform.GetChild(0).GetComponent<Button>().enabled = false;
            roomBtn.transform.GetChild(3).GetComponent<Button>().enabled = false;
        }
    }

    public void ShowSectionButtons()
    {
        foreach (var btn in sectionEditButtons)
            btn.SetActive(true);
        sceneEditBtn.SetActive(false);
    }

    public void HideSectionButtons()
    {
        foreach (var btn in sectionEditButtons)
        {
            btn.GetComponent<RoomEditButton>().isActive = false;
            btn.SetActive(false);
            MarketSlider.Instance.DeactivateAll();
        }
        sceneEditBtn.SetActive(true);
    }

    public void EnterEditMode()
    {
        AudioService.Instance.PlayStoreTheme();
        MarketSlider.Instance.backButton.SetActive(true);
        quitBtn.SetActive(false);
        sceneEditBtn.SetActive(false);

        if (PlayerData.local.tamagotchi.evolution_phase >= 1)
        {
            for (int i = 1; i <= 2; i++)
                sectionEditButtons[i].GetComponent<Button>().enabled = true;

            roomSwitchButtons[1].SetActive(true);
            roomSwitchButtons[2].SetActive(true);
        }

        DisplayRoom(currentEditRoomIndex, isEdit: true);
    }

    private void DisplayRoom(int index, bool isEdit)
    {
        currentEditRoomIndex = index;
        if (isEdit)
        {
            roomLiving.SetActive(index == 0);
            roomBath.SetActive(index == 1);
            roomKitchen.SetActive(index == 2);

            uiEditLiving.SetActive(index == 0);
            uiEditBath.SetActive(index == 1);
            uiEditKitchen.SetActive(index == 2);

            foreach (var group in roomEditGroups)
            {
                foreach (var btn in group.GetComponentsInChildren<RoomEditButton>(true))
                {
                    btn.gameObject.SetActive(MarketSlider.Instance.HasMultipleItems(btn.editCategory));
                }
            }
        }
        else
        {
            uiRoomSelector.SetActive(true);
            roomLiving.SetActive(index == 0);
            roomBath.SetActive(index == 1);
            roomKitchen.SetActive(index == 2);

            uiEditLiving.SetActive(false);
            uiEditBath.SetActive(false);
            uiEditKitchen.SetActive(false);

            MarketSlider.Instance.backButton.SetActive(false);
            quitBtn.SetActive(true);
            sceneEditBtn.SetActive(true);

            HighlightActiveRoomButton();
            HideSectionButtons();
        }
    }

    private void HighlightActiveRoomButton()
    {
        for (int i = 0; i < roomSwitchButtons.Count; i++)
        {
            var image = roomSwitchButtons[i].GetComponent<Image>();
            image.enabled = (i == currentEditRoomIndex);
        }
    }

    public void OpenRewardsPopup()
    {
        popupDailyRewards.SetActive(true);
    }

    public void NavigateToLivingRoom()
    {
        DisplayRoom(0, isEdit: false);
        btnOpenRewards.gameObject.SetActive(true);

        if (rewardLabelUpdater != null)
            StopCoroutine(rewardLabelUpdater);

        rewardLabelUpdater = RefreshRewardLabel();
        StartCoroutine(rewardLabelUpdater);
    }

    private IEnumerator RefreshRewardLabel()
    {
        while (roomLiving.activeInHierarchy)
        {
            var rewards = PlayerData.local.rewards;
            var today = rewards.current_day - 1;
            if (!rewards.collected_rewards[today][0].claimed)
            {
                labelRewardButton.text = Translation.Get("_Tamagotchi_ClaimYourReward");
            }
            else
            {
                var remaining = DateTime.Today.AddDays(1) - DateTime.Now;
                labelRewardButton.text = $"{remaining.Hours}h {remaining.Minutes}min {remaining.Seconds}s";
            }
            yield return new WaitForSeconds(1f);
        }
    }

}
public class RoomEditButton : MonoBehaviour { public string editCategory; public bool isActive; }
public static class AudioService { public static AudioService Instance = new(); public void PlayMainTheme() {} public void PlayStoreTheme() {} }
public static class Translation { public static string Get(string key) => key; }
public static class MarketSlider { public static MarketSlider Instance = new(); public Button backButton; public void DeactivateAll() {} public bool HasMultipleItems(string category) => true; }
public static class PlayerData { public static LocalData local = new(); public class LocalData { public Tamagotchi tamagotchi = new(); public RewardData rewards = new(); } }
public class Tamagotchi { public int evolution_phase; }
public class RewardData { public int current_day = 1; public List<List<RewardEntry>> collected_rewards = new() { new() { new RewardEntry() } }; }
public class RewardEntry { public bool claimed; }

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class TamagotchiManager : MonoBehaviour
{
    public List<GameObject> thingsBought = new();
    public List<GameObject> itemChosentoDisplay = new();
    public List<GameObject> defaultItems = new();
    public List<Tamagotchi_ItemLibrary.itemTypeList> itemLibrary = new();
    public List<AsyncOperationHandle<IList<GameObject>>> libraryGameObjectLoaders = new();
    public TMP_Text coinText_TMP;
    public bool requireInitialItemActivation = false;
    public int amountrunning;
    public SDKAPIManager.TamagotchiProfileData localCachedData;

    public IEnumerator LoadItemLibrary()
    {
        AsyncOperationHandle<Tamagotchi_ItemLibraryManifest> libraryManifestLoader = Addressables.LoadAssetAsync<Tamagotchi_ItemLibraryManifest>("Assets/AddressableGroupManifest/tamagotchi/tamagotchi_ItemLibrary_Manifest.asset");
        yield return libraryManifestLoader;
        itemLibrary = new();
        libraryGameObjectLoaders = new();
        foreach (var libraryItemSet in libraryManifestLoader.Result.itemLibraryList)
        {
            Tamagotchi_ItemLibrary.itemTypeList nextItemSet = new Tamagotchi_ItemLibrary.itemTypeList();
            nextItemSet.type = libraryItemSet.type;
            nextItemSet.itemPrefabs = new GameObject[libraryItemSet.itemPrefabPaths.Length];
            int nx = 0;
            AsyncOperationHandle<IList<GameObject>> nextPrefabLoader = Addressables.LoadAssetsAsync<GameObject>(libraryItemSet.itemPrefabPaths, resultObj =>
            {
                nextItemSet.itemPrefabs[nx] = resultObj;
                if (libraryItemSet.defaultAreaItemPath.Contains(resultObj.name))
                {
                    nextItemSet.defaultAreaItem = resultObj;
                }
                nx++;
            }, Addressables.MergeMode.Union);
            yield return nextPrefabLoader;
            libraryGameObjectLoaders.Add(nextPrefabLoader);
            itemLibrary.Add(nextItemSet);
        }
        Addressables.Release(libraryManifestLoader);
        RefreshProfileData();
    }

    public void ForceMinimumItemBaseline()
    {
        List<SDKAPIManager.ItemStruct> initialItems = new();
        foreach (GameObject item in defaultItems)
        {
            bool hasItemType = thingsBought.Any(x => x.tag == item.tag);
            if (!hasItemType)
            {
                thingsBought.Add(item);
                itemChosentoDisplay.Add(item);
                SDKAPIManager.ItemStruct data = new(item.name, "skin", 0, item.tag);
                initialItems.Add(data);
            }
        }
        if (initialItems.Count > 0)
        {
            requireInitialItemActivation = true;
            UserAccount.apiConnect.PostTamagotchiItemTransaction(this.gameObject, "RetrieveTamagotchiDataResult", UserAccount.instance.activeProfile, initialItems.ToArray());
        }
        else
        {
            FinishSetup();
        }
    }

    public void RetrieveTamagotchiDataResult(object result)
    {
        if (result is SDKAPIManager.TamagotchiProfileData newData)
        {
            localCachedData = newData;
            ownedCoins = newData.Pet_coins_balance;
            UpdateCoinText(coinText_TMP);
            GetComponent<Tamagotchi_MarketSliderManager>().Initialise();

            defaultItems = new();
            foreach (var libraryItem in itemLibrary)
            {
                if (libraryItem.defaultAreaItem != null)
                {
                    try
                    {
                        var match = Tamagotchi_MarketSliderManager.instance.allLists.Find(x => x.area == libraryItem.type)
                            .objectList.Find(item => item.name == libraryItem.defaultAreaItem.name);
                        defaultItems.Add(match);
                    }
                    catch { }
                }
            }
            LoadTamagotchiData(localCachedData.tamagotchi);
        }
        else if (result is SDKAPIManager.TamagotchiDetails details)
        {
            localCachedData.tamagotchi = details;
            LoadTamagotchiData(details);
        }
        else
        {
            string errorCode = "gameK_tamagotchi_E-PR1";
            string errorText = gameCommontranslationDataFile.Gettranslation("_ServerResponse_Error_Generic") + "\n" + errorCode + "\n\n" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            if (AppSystem.sharedUI.gameLoaderSplashObject.activeSelf)
            {
                AppSystem.sharedUI.SetGameLoaderSplashText(errorText);
                AppSystem.sharedUI.gameLoaderSplash_loadingText.color = Color.red;
            }
            else
            {
                GlobalUIController.ShowServerFailedResponseError(this.gameObject, "DataRetrievalFailResponse", errorCode);
            }
        }
    }

    public void RemoveDuplicates()
    {
        thingsBought = thingsBought.Distinct().ToList();
        itemChosentoDisplay = itemChosentoDisplay.Distinct().ToList();
        var slider = Tamagotchi_MarketSliderManager.instance;
        slider.frameList.objectList = slider.frameList.objectList.Distinct().ToList();
        slider.tableList.objectList = slider.tableList.objectList.Distinct().ToList();
        slider.vesteList.objectList = slider.vesteList.objectList.Distinct().ToList();
        slider.hatList.objectList = slider.hatList.objectList.Distinct().ToList();
        slider.accessoryList.objectList = slider.accessoryList.objectList.Distinct().ToList();
    }

    public void SetUpDefaultItems()
    {
        List<SDKAPIManager.ItemStruct> initialItems = new();
        foreach (var item in defaultItems)
        {
            thingsBought.Add(item);
            SDKAPIManager.ItemStruct data = new(item.name, "skin", 0, item.tag);
            initialItems.Add(data);
        }
        amountrunning = 1;
        UserAccount.apiConnect.PostTamagotchiItemTransaction(this.gameObject, "SendNewTamagotchiDataResult", UserAccount.instance.activeProfile, initialItems.ToArray());
        DisplayDefaultItems();
    }

    public void ShowCoinTutorial(bool forceShow = false)
    {
        List<ToolTipSystem.ToolTipConfig> tts = new();
        int tID = 19;
        if (!GlobalUIController.tooltipUI.toolTipActive && (!GameData.Instance.globalTutorialsViewed.Contains(tID) || forceShow))
        {
            RectTransform targetObject = coinText_TMP.transform.parent.GetComponent<RectTransform>();
            Vector2 winPos = new(0, 500);
            Vector2 windowSize = new(550f, 220f);
            ToolTipSystem.maskShape cutoutShape = ToolTipSystem.maskShape.SQUARE;
            Rect highlightRect = ToolTipSystem.GetHighlightRect(targetObject);
            string tutorialText = translationDataFile.Gettranslation("_MenuTutorial_" + tID);
            winPos = new Vector2(highlightRect.x, winPos.y);
            tts.Add(new ToolTipSystem.ToolTipConfig(tutorialText, winPos, windowSize, true, highlightRect, cutoutShape, this.gameObject));
            if (!GameData.Instance.globalTutorialsViewed.Contains(tID))
            {
                GameData.Instance.globalTutorialsViewed.Add(tID);
            }
        }
        if (tts.Count > 0)
        {
            GlobalUIController.tooltipUI.ShowTooltip(tts);
            GameData.Instance.SAVE();
        }
    }

    // Placeholder stubs
    private void RefreshProfileData() {}
    private void FinishSetup() {}
    private void UpdateCoinText(TMP_Text text) {}
    private void LoadTamagotchiData(object tamagotchiData) {}
    private void DisplayDefaultItems() {}
}

// Assume these class definitions exist elsewhere in your code:
public static class SDKAPIManager
{
    public struct ItemStruct
    {
        public string name;
        public string category;
        public int price;
        public string tag;
        public ItemStruct(string name, string category, int price, string tag)
        {
            this.name = name;
            this.category = category;
            this.price = price;
            this.tag = tag;
        }
    }
    public class TamagotchiProfileData
    {
        public int Pet_coins_balance;
        public TamagotchiDetails tamagotchi;
    }
    public class TamagotchiDetails {}
}

public class UserAccount
{
    public static UserAccount instance = new();
    public string activeProfile;
    public APIConnect apiConnect = new();
    public class APIConnect
    {
        public void PostTamagotchiItemTransaction(GameObject go, string callback, string profile, SDKAPIManager.ItemStruct[] items) {}
    }
}

public class AppSystem
{
    public static SharedUI sharedUI = new();
    public class SharedUI
    {
        public GameObject gameLoaderSplashObject;
        public TMP_Text gameLoaderSplash_loadingText;
        public void SetGameLoaderSplashText(string text) {}
    }
}

public class GlobalUIController
{
    public static TooltipUI tooltipUI = new();
    public static void ShowServerFailedResponseError(GameObject go, string id, string errorCode) {}

    public class TooltipUI
    {
        public bool toolTipActive;
        public void ShowTooltip(List<ToolTipSystem.ToolTipConfig> toolTips) {}
    }
}

public static class ToolTipSystem
{
    public enum maskShape { SQUARE }
    public struct ToolTipConfig
    {
        public ToolTipConfig(string text, Vector2 pos, Vector2 size, bool pointer, Rect rect, maskShape shape, GameObject go) {}
    }
    public static Rect GetHighlightRect(RectTransform rectTransform) => new();
}

public static class GameData
{
    public static GameData Instance = new();
    public List<int> globalTutorialsViewed = new();
    public void SAVE() {}
}

public class Tamagotchi_ItemLibrary
{
    public class itemTypeList
    {
        public string type;
        public GameObject[] itemPrefabs;
        public GameObject defaultAreaItem;
    }
}

public class Tamagotchi_ItemLibraryManifest : ScriptableObject
{
    public List<addressableItemTypeList> itemLibraryList;
    public class addressableItemTypeList
    {
        public string type;
        public string[] itemPrefabPaths;
        public string defaultAreaItemPath;
    }
}

public class Tamagotchi_MarketSliderManager : MonoBehaviour
{
    public static Tamagotchi_MarketSliderManager instance = new();
    public List<SliderList> allLists;
    public SliderList frameList, tableList, vesteList, hatList, accessoryList;
    public void Initialise() {}
    public class SliderList
    {
        public string area;
        public List<GameObject> objectList;
    }
}

public static class gameCommontranslationDataFile
{
    public static string Gettranslation(string key) => key;
}

public static class translationDataFile
{
    public static string Gettranslation(string key) => key;
}

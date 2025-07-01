using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Tamagotchi_ItemLibrary", menuName = "My Pet Assets/Item Library", order = 1)]
public class Tamagotchi_ItemLibrary : ScriptableObject //STORES REFERENCES OF EACH CUSTOMISABLE OBJECT CURRENTLY AVAILABLE (REGARDLESS OF WHETHER IT IS ABLE TO BE PURCHASED YET)
{
    [Serializable] public struct itemTypeList{
        public Tamagotchi_PlayerData.itemType type;
        public GameObject[] itemPrefabs;  

        public GameObject defaultAreaItem; //THE DEFAULT ITEM FOR THE TARGET AREA WHEN A USER HAS NO OTHER ITEMS      
    }

    public List<itemTypeList> itemLibraryList;
}


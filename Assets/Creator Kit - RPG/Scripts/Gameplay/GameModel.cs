using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPGM.Core;
using RPGM.Gameplay;
using RPGM.UI;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

namespace RPGM.Gameplay
{
    /// <summary>
    /// This class provides all the data you need to control and change gameplay.
    /// </summary>
    [Serializable]
    public class GameModel
    {
        public CharacterController2D player;
        public DialogController dialog;
        public DiaLogmanager dialogmanager;//我的
        public InputController input;
        public InventoryController inventoryController;
        public MusicController musicController;
        public PackageManager packageManager;

        Dictionary<GameObject, HashSet<string>> conversations = new Dictionary<GameObject, HashSet<string>>();

        Dictionary<string, int> inventory = new Dictionary<string, int>();
        Dictionary<string, Dictionary<string, object>> itemData = new Dictionary<string, Dictionary<string, object>>();//我的
        Dictionary<string, Sprite> inventorySprites = new Dictionary<string, Sprite>();

        HashSet<string> storyItems = new HashSet<string>();

        public IEnumerable<string> InventoryItems => inventory.Keys;

        public Sprite GetInventorySprite(string name)
        {
            Sprite s;
            inventorySprites.TryGetValue(name, out s);
            return s;
        }

        public int GetInventoryCount(string name)
        {
            int c;
            inventory.TryGetValue(name, out c);
            return c;
        }

        public void AddInventoryItem(InventoryItem item)
        {
            int c = 0;
            inventory.TryGetValue(item.name, out c);
            c += item.count;
            inventorySprites[item.name] = item.sprite;
            inventory[item.name] = c;
            inventoryController.Refresh();
        }
        public void AddItems(ItemsMark item)//我的
        {
            Dictionary<string, object> innerDict = new Dictionary<string, object>();
            innerDict["Introduction"] = item.Introduction;
            innerDict["Sprite_real"] = item.Sprite_real;
            innerDict["ID"]= item.ID;
            innerDict["ShowInPackage"] = item.ShowInPackage;

            if (itemData.TryGetValue(item.ItemName, out Dictionary<string, object> c))
            {
                c.TryGetValue("Count", out object cont);
                innerDict["Count"] = item.Count + (int)cont;
            }
            else
            {
                innerDict["Count"] = item.Count;
            }
            
            // 将内部字典添加到外部字典中
            itemData[item.ItemName] = innerDict;
            packageManager.AddItem(item.ItemName);
        }

        public bool HasInventoryItem(string name, int count = 1)
        {
            int c = 0;
            inventory.TryGetValue(name, out c);
            return c >= count;
        }
        public bool HaveItem(string name, int count = 1)
        {
            Dictionary<string, object> c = new Dictionary<string, object>();
            if (itemData.TryGetValue(name, out c))
            {
                // 从内部字典中获取物品数量
                if (c.TryGetValue("Count", out object countObj) && countObj is int)
                {
                    return (int)countObj >= count;
                }
            }
            return false;
        }

        public bool RemoveInventoryItem(InventoryItem item, int count)
        {
            int c = 0;
            inventory.TryGetValue(item.name, out c);
            c -= count;
            if (c < 0) return false;
            inventory[item.name] = c;
            inventoryController.Refresh();
            return true;
        }
        public bool RemoveItem(string name, int count = 1)
        {
            Dictionary<string, object> c = new Dictionary<string, object>();
            if (itemData.TryGetValue(name, out c))
            {
                if (c.TryGetValue("Count", out object countObj) && countObj is int)
                {
                    if ((int)countObj > count)
                    {
                        c["Count"] = ((int)countObj - count);
                        itemData[name] = c;
                        return true;
                    }
                    else if ((int)countObj == count)
                    {
                        itemData.Remove(name);
                        return true;
                    }
                }
            }
            return false;
        }

        public void RegisterStoryItem(string ID)
        {
            storyItems.Add(ID);
        }

        public bool HasSeenStoryItem(string ID)
        {
            return storyItems.Contains(ID);
        }

        public void RegisterConversation(GameObject owner, string id)
        {
            if (!conversations.TryGetValue(owner, out HashSet<string> ids))
                conversations[owner] = ids = new HashSet<string>();
            ids.Add(id);
        }

        public bool HasHadConversationWith(GameObject owner, string id)
        {
            if (!conversations.TryGetValue(owner, out HashSet<string> ids))
                return false;
            return ids.Contains(id);
        }

        public bool HasMet(GameObject owner)
        {
            return conversations.ContainsKey(owner);
        }
        public List<string> GetItemsList()
        {
            var sortedKeys = itemData.Keys
            .OrderBy(key => (int)itemData[key]["ID"])
            .ToList();
            return sortedKeys;
        }
        public object GetItemDate(string name,string info)
        {
            Dictionary<string, object> c = new Dictionary<string, object>();
            if (itemData.TryGetValue(name, out c))
            {
                return c[info];
            }
            return null;
        }
        public Dictionary<string, object> GetItemDic(string name)
        {
            return itemData[name];
        }
    }
}
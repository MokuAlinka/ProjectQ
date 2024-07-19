using RPGM.Core;
using RPGM.Gameplay;
using RPGM.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ItemsMark : MonoBehaviour
{
    public string ItemName;
    public string Introduction;
    public string FindingText;
    public int Count = 1;
    public int ID;
    public Sprite Sprite_thu;
    public Sprite Sprite_real;

    GameModel model = Schedule.GetModel<GameModel>();
    void OnEnable()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite_thu;
    }
    public void OnTriggerEnter2D(Collider2D collider)
    {
        model.dialogmanager.DialogBar("获取物品", FindingText);
        model.AddItems(this);
        gameObject.SetActive(false);
    }
}

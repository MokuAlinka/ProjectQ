using RPGM.Core;
using RPGM.Gameplay;
using RPGM.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaLogTirgger : MonoBehaviour
{
    public TextAsset dialogDataFile;
    public bool NeedConfirm = false;

    GameModel model = Schedule.GetModel<GameModel>();
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!NeedConfirm)
        {
            if (other.CompareTag("Player"))
            {
                model.dialogmanager.ReadText(dialogDataFile);
                model.dialogmanager.StartDiaLog();
            }
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (NeedConfirm && other.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.Z))
            {
                if(model.input.state == InputController.State.CharacterControl)
                {
                    model.dialogmanager.ReadText(dialogDataFile);
                    model.dialogmanager.StartDiaLog();
                }
            }
        }
    }
}

using RPGM.Core;
using RPGM.Gameplay;
using UnityEngine;

namespace RPGM.UI
{
    /// <summary>
    /// Sends user input to the correct control systems.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        public float stepSize = 1f;
        GameModel model = Schedule.GetModel<GameModel>();

        public enum State
        {
            CharacterControl,
            DialogControl,
            PackageControl,
            Pause
        }

        State state;

        public void ChangeState(State state) => this.state = state;

        void Update()
        {
            switch (state)
            {
                case State.CharacterControl:
                    CharacterControl();
                    break;
                case State.DialogControl:
                    DialogControl();
                    break;
                case State.PackageControl:
                    PackageControl();
                    break;

            }
        }

        void DialogControl()
        {
            model.player.nextMoveCommand = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                model.dialogmanager.SelectOption(-1);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                model.dialogmanager.SelectOption(+1);
            if (Input.GetKeyDown(KeyCode.Z))
                model.dialogmanager.ConfirmOption();
            
        }

        void CharacterControl()
        {
            Vector3 moveDirection = Vector3.zero;
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection += Vector3.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection += Vector3.down;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection += Vector3.left;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection += Vector3.right;
            }

            if (moveDirection != Vector3.zero)
            {
                model.player.nextMoveCommand = moveDirection * stepSize;
            }
            else
            {
                model.player.nextMoveCommand = Vector3.zero;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                model.packageManager.PackageChange();
                ChangeState(InputController.State.PackageControl);
            }
        }
        void PackageControl()
        {
            model.player.nextMoveCommand = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                model.packageManager.HorizontalSelection(-1);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                model.packageManager.HorizontalSelection(1);
            else if (Input.GetKey(KeyCode.UpArrow))
                model.packageManager.VerticalSelection(-1);
            else if (Input.GetKey(KeyCode.DownArrow))
                model.packageManager.VerticalSelection(1);
            if (Input.GetKeyDown(KeyCode.Z))
                model.packageManager.ConfirmOption();
            else if (Input.GetKeyDown(KeyCode.X))
                model.packageManager.DisableOption();
            if (Input.GetKeyDown(KeyCode.B))
            {
                model.packageManager.PackageChange();
                ChangeState(InputController.State.CharacterControl);
            }       
        }
    }
}
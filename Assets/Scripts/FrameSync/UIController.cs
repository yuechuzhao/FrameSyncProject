using System.Collections;
using Client.Library.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.FrameSync {
    public class UIController: EntityBase {
        protected override int ThisType {
            get { return EntityConsts.EntityType.InputController; }
        }

        protected override void OnCreate(object param = null) {

            foreach (Transform childButton in transform) {
                var button = childButton.GetComponent<Button>();
                button.onClick.AddListener(() => {
                    string buttonName = button.gameObject.name;
                    Send(EntityConsts.Message.PLAY_OPERATION, new Hashtable() {
                        {"operation", buttonName}
                    });
                });
            }
        }

    }
}
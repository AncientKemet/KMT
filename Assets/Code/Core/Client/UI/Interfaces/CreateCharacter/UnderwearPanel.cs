using System.Collections.Generic;
using Libaries.Generic.Managers;
using Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.UI.Interfaces.CreateCharacter
{
    public class UnderwearPanel : MonoBehaviour {


        public ColorButton CButton;
        

        public void Setup()
        {
            foreach (var component in GetComponentsInChildren<SelectionButton>())
            {
                if (component != CButton)
                    Destroy(component.gameObject);
            }

            Vector3 c_local_pos = CButton.transform.localPosition;
            Vector3 c_local_sc = CButton.transform.localScale;
            Vector3 c_size = CButton.GetComponent<BoxCollider>().size;

            int x = 0;
            int y = 0;
            int index = 0;

            foreach (Color color in HumanModelConfig.I.UnderWearColors)
            {
                ColorButton h = ((GameObject)Instantiate(CButton.gameObject)).GetComponent<ColorButton>();

                h.transform.parent = CButton.transform.parent;
                h.transform.localPosition = c_local_pos + new Vector3(x * c_size.x * c_local_sc.x, y * c_size.y * c_local_sc.y);
                h.transform.localScale = c_local_sc;

                h.ColorSprite.color = color;
                h.gameObject.SetActive(true);

                h.Index = index++;
                h.CharAction = CharacterChangePacket.CharAction.UnderwearColor;

                x++;

                if (x == 4)
                {
                    x = 0;
                    y--;
                }
            }

            CButton.gameObject.SetActive(false);
        }
    }
}

using Libaries.Generic.Managers;
using Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.UI.Interfaces.CreateCharacter
{
    public class EyePanel : MonoBehaviour {


        public EyeButton HButton;
        public ColorButton CButton;

        public void Setup(int gender)
        {
            foreach (var component in GetComponentsInChildren<SelectionButton>())
            {
                if (component != HButton && component != CButton)
                    Destroy(component.gameObject);
            }

            Vector3 h_local_pos = HButton.transform.localPosition;
            Vector3 h_local_sc = HButton.transform.localScale;
            Vector3 h_size = HButton.GetComponent<BoxCollider>().size;
            Vector3 c_local = CButton.transform.localPosition;

            int x = 0;
            int y = 0;
            int index = 0;

            var coll = HumanModelConfig.I.MaleFaceTextures;
            if (gender == 0)
                coll = HumanModelConfig.I.FemaleFaceTextures;

            for (int id = 0; id < coll.Count; id++)
            {
                var tex = coll[id];
                if (tex != null)
                {
                    EyeButton h = ((GameObject)Instantiate(HButton.gameObject)).GetComponent<EyeButton>();

                    h.transform.parent = HButton.transform.parent;
                    h.transform.localPosition = h_local_pos + new Vector3(x * h_size.x * h_local_sc.x, y * h_size.y * h_local_sc.y);
                    h.transform.localScale = h_local_sc;

                    h.EyeRender.material = new Material(h.EyeRender.material);
                    h.EyeRender.material.mainTexture = tex;
                    h.gameObject.SetActive(true);
                    h.Index = index++;
                    h.CharAction = CharacterChangePacket.CharAction.FaceType;

                    x++;

                    if (x == 2)
                    {
                        x = 0;
                        y--;
                    }
                }
            }

            Vector3 c_local_pos = CButton.transform.localPosition;
            Vector3 c_local_sc = CButton.transform.localScale;
            Vector3 c_size = CButton.GetComponent<BoxCollider>().size;


            x = 0;
            y = 0;
            index = 0;

            foreach (Color color in HumanModelConfig.I.FaceColors)
            {
                ColorButton h = ((GameObject)Instantiate(CButton.gameObject)).GetComponent<ColorButton>();

                h.transform.parent = CButton.transform.parent;
                h.transform.localPosition = c_local_pos + new Vector3(x * c_size.x * c_local_sc.x, y * c_size.y * c_local_sc.y);
                h.transform.localScale = c_local_sc;

                h.ColorSprite.color = color;
                h.gameObject.SetActive(true);
                h.Index = index++;
                h.CharAction = CharacterChangePacket.CharAction.FaceColor;

                x++;

                if (x == 4)
                {
                    x = 0;
                    y--;
                }
            }

            x = 0;
            y = 0;

            HButton.gameObject.SetActive(false);
            CButton.gameObject.SetActive(false);
        }
    }
}

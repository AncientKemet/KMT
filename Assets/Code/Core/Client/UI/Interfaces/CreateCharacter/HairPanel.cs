using Libaries.Generic.Managers;
using Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.UI.Interfaces.CreateCharacter
{
    public class HairPanel : MonoBehaviour
    {

        public HairButton HButton;
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
            Vector3 c_local_pos = CButton.transform.localPosition;
            Vector3 c_local_sc = CButton.transform.localScale;
            Vector3 c_size = CButton.GetComponent<BoxCollider>().size;


            HButton.gameObject.SetActive(true);
            CButton.gameObject.SetActive(true);

            int index = 0;
            int x = 0;
            int y = 0;

            var coll = HumanModelConfig.I.MaleHairs;
            if (gender == 0)
                coll = HumanModelConfig.I.FemaleHairs;

            for (int id = 0; id < coll.Count; id++)
            {
                var meshRenderer = coll[id];
                var enableEars = HumanModelConfig.I.MaleHairsEars[id];
                if (meshRenderer != null)
                {
                    HairButton h = ((GameObject)Instantiate(HButton.gameObject)).GetComponent<HairButton>();

                    h.transform.parent = HButton.transform.parent;
                    h.transform.localPosition = h_local_pos + new Vector3(x * h_size.x * h_local_sc.x, y * h_size.y * h_local_sc.y);
                    h.transform.localScale = h_local_sc;

                    h.HairFilter.mesh = meshRenderer.GetComponent<MeshFilter>().mesh;
                    h.HairRender.material = new Material(h.HairRender.material);
                    h.HairRender.material.mainTexture = meshRenderer.material.mainTexture;

                    h.EarRender.enabled = enableEars;
                    h.gameObject.SetActive(true);
                    h.Index = index;
                    h.CharAction = CharacterChangePacket.CharAction.HairType;
                    
                    x++;
                    index++;

                    if (x == 2)
                    {
                        x = 0;
                        y--;
                    }
                }
            }

            x = 0;
            y = 0;
            index = 0;

            foreach (Color color in HumanModelConfig.I.HairsColors)
            {
                ColorButton h = ((GameObject)Instantiate(CButton.gameObject)).GetComponent<ColorButton>();

                h.transform.parent = CButton.transform.parent;
                h.transform.localPosition = c_local_pos + new Vector3(x * c_size.x * c_local_sc.x, y * c_size.y * c_local_sc.y);
                h.transform.localScale = c_local_sc;

                h.ColorSprite.color = color;
                h.gameObject.SetActive(true);
                
                h.Index = index;
                h.CharAction = CharacterChangePacket.CharAction.HairColor;
                
                x++;
                index++;
                if (x == 4)
                {
                    x = 0;
                    y--;
                }
            }
            
            HButton.gameObject.SetActive(false);
            CButton.gameObject.SetActive(false);
        }
    }
}

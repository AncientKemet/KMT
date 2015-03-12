using Client.UI.Interfaces;
using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Controls
{
    [RequireComponent(typeof(Clickable))]
    public class ButtonDescribtion : MonoBehaviour
    {
        [Multiline(5)]
        public string Text;

        void Start ()
        {
            GetComponent<Clickable>().OnMouseIn += () => DescriptionInterface.I.Show("", Text);
            GetComponent<Clickable>().OnMouseOff += () => DescriptionInterface.I.Hide();
        }
    }
}

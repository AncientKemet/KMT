using Code.Core.Client.UI.Controls;
using Shared.StructClasses;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class SkillButton : InterfaceButton
    {

        [SerializeField] private tk2dTextMesh _XpLabel;
        [SerializeField] private tk2dTextMesh _textMesh;
        public Levels.Skills type;

        private int _level;

        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                _textMesh.text = "" + value;
            }
        }

        public int RemainingExp { get; set; }

        protected override void Start()
        {
            base.Start();
            OnMouseIn += () =>
            {
                DescriptionInterface.I.Show(type.ToString()+" [ "+Level+" ]","Click to view details.");
                _XpLabel.text = "Current: "+(Levels.GetExperience(_level+1) - RemainingExp)+" xp.\nNext: "+Levels.GetExperience(_level+1)+" xp.\nRequired: "+RemainingExp+" xp.";
            };
            OnMouseOff += () =>
            {
                _XpLabel.text = "";
            };
        }
    }
}

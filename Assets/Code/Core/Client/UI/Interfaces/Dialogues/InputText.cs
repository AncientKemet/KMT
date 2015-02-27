using System;
using Client.UI.Controls.Inputs;
using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Interfaces.Dialogues
{
    public class InputText : DialogueInterface
    {

        [SerializeField]
        private TextField TextField;
        [SerializeField]
        private tk2dTextMesh DefaultText;

        [SerializeField] private Clickable ContinueButton,CloseButton;

        public Action<string> OnFinish;

        public Action OnCancel;

        public static InputText Create(string defaultText, Action<string> onFinish)
        {
            var i = Create<InputText>();
            i.OnFinish += onFinish;
            i.DefaultText.text = defaultText;
            i.CloseButton.OnLeftClick += i.Close;
            i.ContinueButton.OnLeftClick += i.Continue;
            return i;
        }

        public override void Continue()
        {
            if(OnFinish != null)
                OnFinish(TextField.Text);
            else
                Debug.LogError("Useless input text dialogue.");
            
            base.Continue();
        }
    }
}

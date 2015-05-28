using System.Collections.Generic;
using Client.Net;
using Client.UI.Controls;
using Client.UI.Controls.Inputs;
using Client.UI.Interfaces.CreateCharacter;
using Client.UI.Scripts;
using Client.Units;
using Code.Core.Client.Controls.Camera;
using Code.Core.Client.UI.Controls;
using Libaries.IO;
using Libaries.Net.Packets.ForClient;
using Libaries.Net.Packets.ForServer;

namespace Client.UI.Interfaces
{
    public class CreateCharacterInterface : UIInterface<CreateCharacterInterface>
    {
        public TextButton HairButton,EyesButton,UnderwearButton,SkinButton;
        public TextField NameField;
        public SkinPanel SkinPanel;
        public EyePanel EyePanel;
        public UnderwearPanel UnderwearPanel;
        public HairPanel HairPanel;

        public Clickable FemaleButton, MaleButton;

        public static CharacterCustomalizationDataPacket Data
        {
            get { return _data; }
            set
            {
                _data = value;
                I.LoadFromJson(Data.JsonObject);
            }
        }

        public List<int> UnlockedMaleHairs = new List<int>();
        public List<int> UnlockedFemaleHairs = new List<int>();
        public List<int> UnlockedHairColors = new List<int>();
        public List<int> UnlockedEyes = new List<int>();
        public List<int> UnlockedEyeColors = new List<int>();
        public List<int> UnlockedSkinTypes = new List<int>();
        public List<int> UnlockedSkinColors = new List<int>();
        public List<int> UnlockedUnderwearColors = new List<int>();
        private static CharacterCustomalizationDataPacket _data;

        public int Gender = 1;

        private void LoadFromJson(JSONObject o)
        {
            LoadListFromJson(ref UnlockedMaleHairs, "UnlockedMaleHairs", o);
            LoadListFromJson(ref UnlockedFemaleHairs, "UnlockedFemaleHairs", o);
            LoadListFromJson(ref UnlockedHairColors, "UnlockedHairColors", o);
            LoadListFromJson(ref UnlockedEyes, "UnlockedEyes", o);
            LoadListFromJson(ref UnlockedEyeColors, "UnlockedEyeColors", o);
            LoadListFromJson(ref UnlockedSkinTypes, "UnlockedSkinTypes", o);
            LoadListFromJson(ref UnlockedSkinColors, "UnlockedSkinColors", o);
            LoadListFromJson(ref UnlockedUnderwearColors, "UnlockedUnderwearColors", o);
        }

        private void OnDisable()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            CameraController.Instance.CameraToObjectDistance = 8;
            CameraController.Instance.CameraY = 13f;
        }

        protected override void OnStart()
        {
            base.OnStart();

            Gender = PlayerUnit.MyPlayerUnit.Display.Model;

            PlayerUnit.MyPlayerUnit.Display.OnModelChange += i =>
            {
                if(CreateCharacterInterface.IsNull)
                    return;
                CreateCharacterInterface.I.Gender = i;
                CreateCharacterInterface.I.HairPanel.Setup(Gender);
                CreateCharacterInterface.I.SkinPanel.Setup();
                CreateCharacterInterface.I.EyePanel.Setup(Gender);
                CreateCharacterInterface.I.UnderwearPanel.Setup();
            };

            CameraController.Instance.CameraToObjectDistance = 4;
            CameraController.Instance.CameraY = 2f;
            CameraController.Instance.rotation = 1.2f;

            ClosePanels();
            SetDefaultLook();
            FemaleButton.OnLeftClick += SetDefaultLook;
            FemaleButton.OnLeftClick += () => ClientCommunicator.Instance.SendToServer(new CharacterChangePacket()
            {
                Action = CharacterChangePacket.CharAction.Gender,
                value = 1
            });
            MaleButton.OnLeftClick += SetDefaultLook;
            MaleButton.OnLeftClick += () => ClientCommunicator.Instance.SendToServer(new CharacterChangePacket()
            {
                Action = CharacterChangePacket.CharAction.Gender,
                value = 0
            });

            HairButton.OnLeftClick += () =>
            {
                ClosePanels();
                HairPanel.gameObject.SetActive(true);
            };

            EyesButton.OnLeftClick += () =>
            {
                ClosePanels();
                EyePanel.gameObject.SetActive(true);
            };
            
            SkinButton.OnLeftClick += () =>
            {
                ClosePanels();
                SkinPanel.gameObject.SetActive(true);
            };

            UnderwearButton.OnLeftClick += () =>
            {
                ClosePanels();
                UnderwearPanel.gameObject.SetActive(true);
            };

        }

        private void SetDefaultLook()
        {
            ClientCommunicator.Instance.SendToServer(new CharacterChangePacket()
            {
                Action = CharacterChangePacket.CharAction.HairType,
                value = 1
            });
            ClientCommunicator.Instance.SendToServer(new CharacterChangePacket()
            {
                Action = CharacterChangePacket.CharAction.FaceType,
                value = 0
            });
        }

        private void ClosePanels()
        {
            HairPanel.gameObject.SetActive(false);
            SkinPanel.gameObject.SetActive(false);
            EyePanel.gameObject.SetActive(false);
            UnderwearPanel.gameObject.SetActive(false);
        }

        private void LoadListFromJson(ref List<int> _ref, string fieldName, JSONObject o)
        {
            if (o.HasField(fieldName))
            {
                JSONObject listJson = o.GetField(fieldName);

                int count = int.Parse(listJson.GetField("Count").str);
                _ref = new List<int>(count);

                for (int i = 0; i < count; i++)
                {
                    _ref.Add(int.Parse(listJson.GetField("" + i).str));
                }
            }
        }
    }
}

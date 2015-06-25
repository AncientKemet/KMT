using System;
using System.Collections.Generic;
using Libaries.IO;
using Shared.StructClasses;

namespace Server.Model.Extensions.PlayerExtensions
{
    public class PlayerLevels : EntityExtension
    {

        private Dictionary<Levels.Skills, int> _remainingXp = new Dictionary<Levels.Skills, int>();
        private Dictionary<Levels.Skills, int> _currentLevels = new Dictionary<Levels.Skills, int>();

        public event Action<Levels.Skills, int> OnLevelUp;

        public int CombatLevel { get; private set; }
        public int TotalLevel { get; private set; }

        public void AddExperience(Levels.Skills skill, int amount)
        {
            //Maximum at once levels up = 100
            for (int i = 0; i < 100; i++)
            {
                if (_remainingXp[skill] <= amount)
                {
                    _currentLevels[skill] ++;
                    if(OnLevelUp != null)
                    OnLevelUp(skill, _currentLevels[skill]);
                    amount -= _remainingXp[skill];
                    _remainingXp[skill] = Levels.GetRemainingXp(_currentLevels[skill], _remainingXp[skill]);
                }
                else
                {
                    break;
                }
            }
            _remainingXp[skill] -= amount;
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            foreach (var v in Enum.GetValues(typeof (Levels.Skills)))
            {
                _remainingXp.Add((Levels.Skills) v,83);
                _currentLevels.Add((Levels.Skills)v, 1);
            }
        }

        public override void Progress(float time)
        {
        }

        public override void Serialize(JSONObject j)
        {
            JSONObject levels = ToJsonObject();

            j.AddField("Levels", levels);
        }

        public override void Deserialize(JSONObject j)
        {
            JSONObject levels = j.GetField("Levels");
            if (levels != null)
            {
                foreach (var i in _currentLevels)
                {
                    _remainingXp[i.Key] = int.Parse(levels.GetField("xp" + i.Key).str);
                }
                foreach (var i in _remainingXp)
                {
                    _currentLevels[i.Key] = int.Parse(levels.GetField(i.Key.ToString()).str);
                }
            }
            RecalculateTotalLevel();
        }

        private void RecalculateTotalLevel()
        {
            TotalLevel = 0;
            foreach (var i in _currentLevels)
            {
                TotalLevel += i.Value;
            }
        }

        public JSONObject ToJsonObject()
        {
            JSONObject levels = new JSONObject();

            foreach (var i in _remainingXp)
            {
                levels.AddField("xp" + i.Key.ToString(), i.Value + "");
            }
            foreach (var i in _currentLevels)
            {
                levels.AddField(i.Key.ToString(), i.Value + "");
            }
            return levels;
        }
    }
}

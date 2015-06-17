using System.Collections;
using UnityEngine;

namespace Shared.SharedTypes
{
    public abstract class MinimapEvent
    {
        public MinimapEventType Type;
        public abstract string GetText();
        public abstract int GetIconId();
    }

    public enum MinimapEventType
    {
        Experience,
        People,
        Other
    }
}
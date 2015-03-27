using Server.Model.Entities.Human;
using UnityEngine;
using System.Collections;

public class NpcCC : NpcSpawnExtension
{
    public int faceType, faceColor, skinColor, hairType, hairColor;

    public override void Apply(NPC n)
    {
        n.Display.Facetype = faceType;
        n.Display.FaceColor = faceColor;
        n.Display.SkinColor = skinColor;
        n.Display.Hairtype = hairType;
        n.Display.HairColor = hairColor;
    }
}

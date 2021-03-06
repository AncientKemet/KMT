﻿using Server.Model.Entities.Human;

namespace Server.Model.Content.Spawns.NpcSpawns
{
    public class NpcCC : NpcSpawnExtension
    {
        public int faceType, faceColor, skinColor, hairType, hairColor;

        public override void Apply(NPC n)
        {
            base.Apply(n);
            n.Display.FaceType = faceType;
            n.Display.FaceColor = faceColor;
            n.Display.SkinColor = skinColor;
            n.Display.Hairtype = hairType;
            n.Display.HairColor = hairColor;
        }
    }
}

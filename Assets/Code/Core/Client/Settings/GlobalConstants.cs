﻿using Code.Libaries.Generic;

namespace Code.Core.Client.Settings
{
    public class GlobalConstants : MonoSingleton<GlobalConstants> {

        public int MAX_UNIT_AMOUNT = 1024*64;
        public ushort STATIC_UNIT_OFFSET = 1024 * 32;
    }
}

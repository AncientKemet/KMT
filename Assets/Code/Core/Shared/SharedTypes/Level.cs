﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shared.StructClasses
{
    public class Levels
    {
        public enum Skills
        {
            Attack=0,
            Strenght=1,
            Ranged=2,
            Wisdom=3,
            Combat=9,
            Woodcutting=10,
            Mining=11,
            Blacksmithing=12,
            Tailoring=13,
            Jewelery=14,
            Cooking=15,
            Hunting=16,
            Pottering=17,
            Farming = 18,
            Crafting = 19,
        }

        public static int GetExperience(int level)
        {
            return TotalXpTable[level];
        }

        public static int GetRemainingXp(int level, int currentXp)
        {
            return XpPerLevelTable[level] - currentXp;
        }

        public static int[] XpPerLevelTable = new[]{0,//1
                                                    83,//2
                                                    93,//3
                                                    103,//4
                                                    114,//5
                                                    127,//6
                                                    141,//7
                                                    155,//8
                                                    172,//9
                                                    190,//10
                                                    210,//11
                                                    231,//12
                                                    255,//13
                                                    282,//14
                                                    310,//15
                                                    342,//16
                                                    377,//17
                                                    416,//18
                                                    458,//19
                                                    505,//20
                                                    557,//21
                                                    613,//22
                                                    676,//23
                                                    745,//24
                                                    821,//25
                                                    905,//26
                                                    997,//27
                                                    1100,//28
                                                    1212,//29
                                                    1336,//30
                                                    1473,//31
                                                    1624,//32
                                                    1791,//33
                                                    1975,//34
                                                    2179,//35
                                                    2403,//36
                                                    2650,//37
                                                    2923,//38
                                                    3224,//39
                                                    3557,//40
                                                    3924,//41
                                                    4330,//42
                                                    4777,//43
                                                    5271,//44
                                                    5816,//45
                                                    6417,//46
                                                    7082,//47
                                                    7815,//48
                                                    8625,//49
                                                    9518,//50
                                                    10505,//51
                                                    11594,//52
                                                    12796,//53
                                                    14124,//54
                                                    15589,//55
                                                    17208,//56
                                                    18994,//57
                                                    20966,//58
                                                    23143,//59
                                                    25547,//60
                                                    28201,//61
                                                    31131,//62
                                                    34367,//63
                                                    37938,//64
                                                    41882,//65
                                                    46235,//66
                                                    51042,//67
                                                    56349,//68
                                                    62208,//69
                                                    68678,//70
                                                    75820,//71
                                                    83706,//72
                                                    92412,//73
                                                    102025,//74
                                                    112638,//75
                                                    124355,//76
                                                    137293,//77
                                                    151576,//78
                                                    167347,//79
                                                    184759,//80
                                                    203983,//81
                                                    225208,//82
                                                    248642,//83
                                                    274516,//84
                                                    303082,//85
                                                    334622,//86
                                                    369445,//87
                                                    407892,//88
                                                    450341,//89
                                                    497208,//90
                                                    548955,//91
                                                    606087,//92
                                                    669165,//93
                                                    738810,//94
                                                    815703,//95
                                                    900601,//96
                                                    994334,//97
                                                    1097826,//98
                                                    1212089,//99
                                                    };
        public static int[] TotalXpTable = new[]{0,//0
                                            0,//1
                                            83,//2
                                            176,//3
                                            279,//4
                                            393,//5
                                            520,//6
                                            661,//7
                                            816,//8
                                            988,//9
                                            1178,//10
                                            1388,//11
                                            1619,//12
                                            1874,//13
                                            2156,//14
                                            2466,//15
                                            2808,//16
                                            3185,//17
                                            3601,//18
                                            4059,//19
                                            4564,//20
                                            5121,//21
                                            5734,//22
                                            6410,//23
                                            7155,//24
                                            7976,//25
                                            8881,//26
                                            9878,//27
                                            10978,//28
                                            12190,//29
                                            13526,//30
                                            14999,//31
                                            16623,//32
                                            18414,//33
                                            20389,//34
                                            22568,//35
                                            24971,//36
                                            27621,//37
                                            30544,//38
                                            33768,//39
                                            37325,//40
                                            41249,//41
                                            45579,//42
                                            50356,//43
                                            55627,//44
                                            61443,//45
                                            67860,//46
                                            74942,//47
                                            82757,//48
                                            91382,//49
                                            100900,//50
                                            111405,//51
                                            122999,//52
                                            135795,//53
                                            149919,//54
                                            165508,//55
                                            182716,//56
                                            201710,//57
                                            222676,//58
                                            245819,//59
                                            271366,//60
                                            299567,//61
                                            330698,//62
                                            365065,//63
                                            403003,//64
                                            444885,//65
                                            491120,//66
                                            542162,//67
                                            598511,//68
                                            660719,//69
                                            729397,//70
                                            805217,//71
                                            888923,//72
                                            981335,//73
                                            1083360,//74
                                            1195998,//75
                                            1320353,//76
                                            1457646,//77
                                            1609222,//78
                                            1776569,//79
                                            1961328,//80
                                            2165311,//81
                                            2390519,//82
                                            2639161,//83
                                            2913677,//84
                                            3216759,//85
                                            3551381,//86
                                            3920826,//87
                                            4328718,//88
                                            4779059,//89
                                            5276267,//90
                                            5825222,//91
                                            6431309,//92
                                            7100474,//93
                                            7839284,//94
                                            8654987,//95
                                            9555588,//96
                                            10549922,//97
                                            11647748,//98
                                            12859837//99
                                            };

        public const float DamageXpRatio = 30;

#if UNITY_EDITOR
        [MenuItem("Kemet/Levels/TotalXpTable")]
        public static void PrintXPTableTotal()
        {
            string s = "{";
            int xp = 0;
            for (int lvl = 1; lvl < 100; lvl++)
            {
                if (lvl >= 2)
                    xp += (int)(lvl + 67 * Mathf.Pow(2, (float)lvl / 7));
                s += xp + ",//" + lvl + "\n";
            }
            Debug.Log(s + "};");
        }
        [MenuItem("Kemet/Levels/XpTable")]
        public static void PrintXPTable()
        {
            string s = "{";
            int xp = 0;
            for (int lvl = 1; lvl < 100; lvl++)
            {
                if (lvl >= 2)
                    xp = (int)(lvl + 67 * Mathf.Pow(2, (float)lvl / 7));
                s += xp + ",//" + lvl + "\n";
            }
            Debug.Log(s + "};");
        }
#endif
    }
}

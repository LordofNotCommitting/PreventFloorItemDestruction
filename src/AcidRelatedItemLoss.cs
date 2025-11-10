using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PreventFloorItemDestruction
{
    [HarmonyPatch(typeof(ToxicController), nameof(ToxicController.DamageItems))]
    public class AcidRelatedItemLoss
    {
        public static bool Prefix(CellPosition pos, ItemOnFloor itemOnFloor)
        {
            return false;
        }
    }
}
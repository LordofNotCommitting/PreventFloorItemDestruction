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
    [HarmonyPatch(typeof(FireController), nameof(FireController.AddDamage))]
    public class FireRelatedItemLoss
    {
        public static bool Prefix(ref FireController __instance , CellPosition position, int duration, bool isPlayerFire)
        {
            ItemOnFloor itemOnFloor = __instance._itemsOnFloor.Get(position);
            if (itemOnFloor != null && !itemOnFloor.Storage.Empty)
            {
                __instance._itemsCache.Clear();
                __instance._itemsCache.AddRange(itemOnFloor.Storage.Items);
                int num = 0;
                foreach (BasePickupItem basePickupItem2 in __instance._itemsCache)
                {
                    for (int i = 0; i < Data.Global.FireItemConvertsFrom.Count; i++)
                    {
                        if (Data.Global.FireItemConvertsFrom[i].Equals(basePickupItem2.Id))
                        {
                            num++;
                            itemOnFloor.Storage.Remove(basePickupItem2, true);
                            BasePickupItem basePickupItem3 = SingletonMonoBehaviour<ItemFactory>.Instance.CreateForInventory(Data.Global.FireItemConvertsTo[i], false);
                            basePickupItem3.StackCount = basePickupItem2.StackCount;
                            itemOnFloor.Storage.AddItemAndReshuffleOptional(basePickupItem3);
                            break;
                        }
                    }
                }
                __instance._itemsCache.Clear();
            }

            MineEntity mineEntity = __instance._mapEntities.Get<MineEntity>(position.X, position.Y);
            if (mineEntity != null)
            {
                mineEntity.ForceTrigger();
            }
            Creature creature = __instance._creatures.GetCreature(position.X, position.Y);
            if (creature == null || creature.CreatureData.EffectsController.HasAnyEffect<WoundEffectBurnImmune>())
            {
                return false;
            }
            creature.ActivateTakeBurnTrigger();
            if (PerkSystem.GetPerkParameterBool(creature.CreatureData, "BBurnImmune"))
            {
                return false;
            }
            if (isPlayerFire && !(creature is Player))
            {
                __instance._creatures.Player.ActivatePutFireTrigger();
                duration += __instance._creatures.Player.CreatureData.FireDurationBonus;
            }
            creature.CreatureData.EffectsController.Add(new BurningEffect(duration), true);

            return false;

        }
    }
}
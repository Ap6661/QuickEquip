using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using FrooxEngine.CommonAvatar;
using System;

namespace QuickEquip
{
    public class QuickEquip : ResoniteMod
    {
        public override string Name => "QuickEquip";
        public override string Author => "APnda";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/ap6661/QuickEquip/";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.apnda.QuickEquip");
            harmony.PatchAll();
        }
		
        [HarmonyPatch(typeof(FrooxEngine.UIX.Button), "RunPressed")]
        class QuickEquipPatch
        {
            public static void Postfix(FrooxEngine.UIX.Button __instance)
            {
                if (__instance.World.IsUserspace())
                {
                    if (__instance.Slot.GetComponent<FrooxEngine.CloudValueVariable<Uri>>() != null &&
                        __instance.Slot.GetComponent<FrooxEngine.Comment>() != null &&
                        __instance.Slot.GetComponent<FrooxEngine.Comment>().Text.Value == "QuickEquip")
                    {
                        World world = __instance.Engine.WorldManager.FocusedWorld;
                        world.RunSynchronously(delegate
                        {
                            if (world.CanSwapAvatar() && world.Permissions.CheckAll((AvatarObjectPermissions m) => m.Tags.List.Count == 0))
                            {
                                Slot s = world.RootSlot.LocalUserSpace.AddSlot("AvatarSpawn");
                                s.StartTask(async delegate
                                {
                                    await s.LoadObjectAsync(__instance.Slot.GetComponent<FrooxEngine.CloudValueVariable<Uri>>().Value.Value);
                                    s = s.GetComponent<InventoryItem>()?.Unpack() ?? s;
                                    world.RunInUpdates(1, delegate
                                    {
                                        AvatarManager registeredComponent = world.LocalUser.Root.GetRegisteredComponent<AvatarManager>();
                                        registeredComponent.ClearEquipped();
                                        registeredComponent.Equip(s);
                                    });
                                });
                            }
                        });
                    }
                }
            }
        }
    }
}

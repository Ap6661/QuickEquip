using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using SkyFrost.Base;

namespace QuickEquip
{
    public class QuickEquip : ResoniteMod
    {
        public override string Name => "QuickEquip";
        public override string Author => "APnda";
        public override string Version => "1.1.0";
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

                if (!__instance.World.IsUserspace() || !__instance.Cloud.Session.IsUserSignedIn)
                {
                    return;
                }
                if (!(__instance.Slot.GetComponent<FrooxEngine.CloudValueVariable<Uri>>() != null &&
                        __instance.Slot.GetComponent<FrooxEngine.Comment>() != null &&
                        __instance.Slot.GetComponent<FrooxEngine.Comment>().Text.Value == "QuickEquip"))
                {
                    return;
                }

                World focusedWorld = __instance.Engine.WorldManager.FocusedWorld;

                __instance.World.Coroutines.StartTask(async delegate
                {
                    Uri recordUri = __instance.Slot.GetComponent<FrooxEngine.CloudValueVariable<Uri>>().Value.Value;
                    if (!(recordUri == null))
                    {
                        CloudResult<FrooxEngine.Store.Record> recordResult = await __instance.Engine.RecordManager.FetchRecord(recordUri);
                        if (recordResult.IsOK)
                        {
                            focusedWorld.TryEquipAvatar(recordResult.Entity);
                        }
                    }
                });

            }
        }
    }
}

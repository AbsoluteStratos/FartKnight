using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Mono.Security.X509.X520;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using UObject = UnityEngine.Object;

namespace FartKnight
{
    // Main Mod Class
    // IGlobalSettings enable the global settings in GlobalSettings.cs
    // https://prashantmohta.github.io/ModdingDocs/saving-mod-data.html
    // ICustomMenuMod is an interface for the mod options menu (in ModMenu.cs)

    public class FartKnight : Mod, IGlobalSettings<GlobalSettings>, ICustomMenuMod
    {

        internal static FartKnight instance;
        internal static GlobalSettings GS = new GlobalSettings();
        new public string GetName() => "Fart Knight";
        public override string GetVersion() => "0.1.0";

        public override void Initialize()
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            On.HeroController.Awake += new On.HeroController.hook_Awake(this.OnHeroControllerAwake);
        }
        public void OnHeroUpdate()
        {
            // Here we use the Player Action to detect the input
            // This WasPressed is defined in the subclass `OneAxisInputControl`
            if (GS.KeyBinds.Action.WasPressed)
            {
                Modding.Logger.Log("Fart Key Pressed", FartKnight.GS.LogLevel);
                HeroController.instance.GetComponent<FartHandler>().Run();
            }
        }

        private void OnHeroControllerAwake(On.HeroController.orig_Awake orig, HeroController self)
        {
            orig.Invoke(self);
            self.gameObject.AddComponent<KnightHandler>();
            self.gameObject.AddComponent<FartHandler>();
        }

        void IGlobalSettings<GlobalSettings>.OnLoadGlobal(GlobalSettings s)
        {
            GS = s;
        }

        GlobalSettings IGlobalSettings<GlobalSettings>.OnSaveGlobal()
        {
            return GS;
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            return ModMenu.CreateMenuScreen(modListMenu).Build();
        }
            

        public bool ToggleButtonInsideMenu => false;
    }
}
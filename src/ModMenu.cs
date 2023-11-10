using GlobalEnums;
using UnityEngine.UI;
using Modding;
using Modding.Menu;
using Modding.Menu.Config;
using Satchel.BetterMenus;

using System;
using UnityEngine;
using MenuButton = Satchel.BetterMenus.MenuButton;
using InControl;

namespace FartKnight
{
    public static class ModMenu
    {
        public static MappableKey myKey;
        public static InControl.PlayerAction Action;

        // This is based on the debug UI menu
        // https://github.com/TheMulhima/HollowKnight.DebugMod/blob/master/Source/ModMenu.cs#L12
        // Typically people use Satchel to not deal with this complexity
        // https://prashantmohta.github.io/ModdingDocs/Satchel/BetterMenus/better-menus.html
        public static MenuBuilder CreateMenuScreen(MenuScreen modListMenu)
        {
            Action<MenuSelectable> CancelAction = selectable => UIManager.instance.UIGoToDynamicMenu(modListMenu);
            return new MenuBuilder(UIManager.instance.UICanvas.gameObject, "Fart Knight Menu")
                .CreateTitle("Fart Knight Menu", MenuTitleStyle.vanillaStyle)
                .CreateContentPane(RectTransformData.FromSizeAndPos(
                    new RelVector2(new Vector2(1920f, 903f)),
                    new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -60f)
                    )
                ))
                .CreateControlPane(RectTransformData.FromSizeAndPos(
                    new RelVector2(new Vector2(1920f, 259f)),
                    new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -502f)
                    )
                ))
                .SetDefaultNavGraph(new GridNavGraph(1))
                .AddContent(
                    RegularGridLayout.CreateVerticalLayout(105f),
                    c =>
                    {
                        c.AddKeybind(
                                "Fart Key",
                                FartKnight.GS.KeyBinds.Action,
                                new KeybindConfig
                                {
                                    Label = "Fart Key"
                                },
                                out var myKey
                            )
                        // https://github.com/PrashantMohta/GrubTrain/blob/master/options/Menu.cs#L122
                         .AddHorizontalOption(
                                "SFX",
                                new HorizontalOptionConfig
                                {
                                    Label = "Sound Effects",
                                    Options = new[] {  "On", "Off" },
                                    ApplySetting = (_, i) => { FartKnight.GS.EnableSound = (i == 0); },
                                    RefreshSetting = (s, _) =>
                                        s.optionList.SetOptionTo(FartKnight.GS.EnableSound ? 0 : 1),
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                    Style = HorizontalOptionStyle.VanillaStyle,
                                    Description = new DescriptionInfo
                                    {
                                        Text = "Let it rip"
                                    }
                                }, out var AutoBackupSelector)
                         .AddTextPanel(
                                "Spacer",
                                new RelVector2(new Vector2(1000.0f, 50f)),
                                new TextPanelConfig
                                {
                                    Text = "",
                                    Size = 1
                                }
                         )
                         .AddTextPanel(
                                "DebugTitle",
                                new RelVector2(new Vector2(1000.0f, 32f)),
                                new TextPanelConfig
                                {
                                    Text = "Debug Options",
                                    Size = 32
                                }
                         )
                        .AddHorizontalOption(
                                "Debug",
                                new HorizontalOptionConfig
                                {
                                    Label = "Logging",
                                    Options = new[] { "Off", "On" },
                                    ApplySetting = (_, i) => {
                                        if (i == 0) {
                                            FartKnight.GS.LogLevel = LogLevel.Fine;
                                        }
                                        else
                                        {
                                            FartKnight.GS.LogLevel = LogLevel.Info;
                                        }},
                                    RefreshSetting = (s, _) =>
                                        s.optionList.SetOptionTo(FartKnight.GS.EnableSound ? 1 : 0),
                                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                                    Style = HorizontalOptionStyle.VanillaStyle,
                                    Description = new DescriptionInfo
                                    {
                                        Text = "Elevate Mod Logs from Fine to Info"
                                    }
                                }, out var LogLevelToggle);
                    })
                .AddControls(
                    new SingleContentLayout(new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -64f)
                    )), c => c.AddMenuButton(
                        "BackButton",
                        new MenuButtonConfig
                        {
                            Label = "Back",
                            CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                            SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(modListMenu),
                            Style = MenuButtonStyle.VanillaStyle,
                            Proceed = true
                        }));
        }
    }
}
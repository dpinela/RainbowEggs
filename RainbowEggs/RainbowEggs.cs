using Modding;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.UIDefs;
using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using RandomizerMod.Menu;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.Settings;
using RandomizerMod.RC;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using System.Collections.Generic;

namespace RainbowEggs
{
    public class RainbowEggs : Mod, IGlobalSettings<RandoSettings>
    {
        private static List<Egg> Eggs = new()
        {
            new()
            {
                Name = "Red Egg",
                Description = "A bright red egg of an unknown creature. Smells strongly of cherries.\n\nEdibility uncertain.",
                Sprite = "RedEgg.png"
            },
            new()
            {
                Name = "Orange Egg",
                Description = "An orange egg of a mysterious creature. Smells strongly of oranges.\n\nGenerally considered very sweet.",
                Sprite = "OrangeEgg.png"
            },
            new()
            {
                Name = "Yellow Egg",
                Description = "A yellow egg of a strange creature. Smells strongly of lemons.\n\nGenerally considered sour but tasty.",
                Sprite = "YellowEgg.png"
            },
            new()
            {
                Name = "Green Egg",
                Description = "A green egg of an unknown creature. Emits a powerful smell of melon.\n\nGenerally considered watery but sweet.",
                Sprite = "GreenEgg.png"
            },
            new()
            {
                Name = "Cyan Egg",
                Description = "A cyan egg of a mysterious creaure. Emits a powerful smell of fresh air.\n\nGenerally considered too insubstantial to eat.",
                Sprite = "CyanEgg.png"
            },
            new()
            {
                Name = "Blue Egg",
                Description = "A blue egg of a strange creature. Emits a powerful smell of blueberries.\n\nGenerally considered very soft and round.",
                Sprite = "BlueEgg.png"
            },
            new()
            {
                Name = "Purple Egg",
                Description = "A purple egg of an unknown creature. Emits a powerful smell of grapes.\n\nGenerally considered sweet but also crunchy.",
                Sprite = "PurpleEgg.png"
            },
            new()
            {
                Name = "Pink Egg",
                Description = "A pink egg of a mysterious creature. Emits a powerful smell of lychees.\n\nGenerally considered rather strange-tasting.",
                Sprite = "PinkEgg.png"
            },
            new()
            {
                Name = "Trans Egg",
                Description = "A multicoloured egg of a strange creature. Emits a powerful aura of transness.\n\nGenerally considered blessed.",
                Sprite = "TransEgg.png"
            },
            new()
            {
                Name = "Rainbow Egg",
                Description = "A multicoloured egg of a mysterious creature. Emits a powerful aura of pride.\n\nGenerally considered quite beautiful.",
                Sprite = "RainbowEgg.png"
            },
            new()
            {
                Name = "Arcane Eg",
                Description = "Mysterious stone eg from before the birth of Halluwnest.\n\nRellic from the anciont past. Thls item now holds little value exceqt for those dedicated to hiztory.",
                Sprite = "BlackEgg.png"
            }
        };

        private RandoSettings Settings = new();

        public void OnLoadGlobal(RandoSettings rs)
        {
            Settings = rs;
        }

        public RandoSettings OnSaveGlobal() => Settings;

        public override void Initialize()
        {
            foreach (var egg in Eggs)
            {
                var item = new IntItem()
                {
                    fieldName = "rancidEggs",
                    amount = 1,
                    name = egg.InternalName,
                    UIDef = new MsgUIDef()
                    {
                        name = new BoxedString(egg.Name),
                        shopDesc = new BoxedString(egg.Description),
                        sprite = new EmbeddedSprite() { key = egg.Sprite }
                    }
                };
                Finder.DefineCustomItem(item);
            }

            RequestBuilder.OnUpdate.Subscribe(17f, ColorizeEggs);
            RCData.RuntimeLogicOverride.Subscribe(50, DefineLogicItems);
            RandomizerMenuAPI.AddMenuPage(BuildMenu, BuildButton);
            SettingsLog.AfterLogSettings += LogRandoSettings;
        }

        private static IEnumerable<string> NRandomEggs(System.Random rng, int n)
        {
            for (var i = 0; i < n; i++)
            {
                yield return Eggs[rng.Next(Eggs.Count)].InternalName;
            }
        }

        private void ColorizeEggs(RequestBuilder rb)
        {
            if (Settings.ColourizeRancidEggs)
            {
                var rng = new System.Random(rb.gs.Seed);
                rb.ReplaceItem("Rancid_Egg", n => NRandomEggs(rng, n));
            }
        }

        private static void DefineLogicItems(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            var value = new TermValue(lmb.GetTerm("RANCIDEGGS"), 1);
            foreach (var egg in Eggs)
            {
                lmb.AddItem(new SingleItem(egg.InternalName, value));
            }
        }

        private MenuPage SettingsPage;

        private void BuildMenu(MenuPage landingPage)
        {
            SettingsPage = new MenuPage(GetName(), landingPage);
            var factory = new MenuElementFactory<RandoSettings>(SettingsPage, Settings);
            new VerticalItemPanel(SettingsPage, new(0, 300), 75f, true, factory.Elements);
        }

        private bool BuildButton(MenuPage landingPage, out SmallButton settingsButton)
        {
            settingsButton = new(landingPage, GetName());
            settingsButton.AddHideAndShowEvent(landingPage, SettingsPage);
            return true;
        }

        private void LogRandoSettings(LogArguments args, TextWriter w)
        {
            w.WriteLine("Logging RainbowEggs settings:");
            w.WriteLine(JsonUtil.Serialize(Settings));
        }

        public override string GetVersion() => "1.0";
    }
}

using Modding;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.UIDefs;
using ItemChanger.Tags;
using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using RandomizerMod.Menu;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
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
            AbstractItem.ModifyItemGlobal += args =>
            {
                if (args.Item.GetTag(out EggColorTag eggtag))
                {
                    args.Item = Finder.GetItem(eggtag.ReplacementEggName);
                }
            };

            RequestBuilder.OnUpdate.Subscribe(17f, ColorizeEggs);
            RandomizerMenuAPI.AddMenuPage(_ => {}, BuildButton);
            SettingsLog.AfterLogSettings += LogRandoSettings;
        }

        // By performing egg replacements this way, the placements and hash are the same regardless
        // of whether this mod is in use or not. Furthermore, since the tag is only stored locally
        // and not synced, ItemSync will work even if some but not all the players in a room are using
        // this mod; eggs will appear with their respective colors for players that have eggs colorized,
        // and as regular Rancid Eggs for those that do not.
        private void ColorizeEggs(RequestBuilder rb)
        {
            if (Settings.ColorizeRancidEggs)
            {
                var rng = new System.Random(rb.gs.Seed);
                rb.EditItemRequest("Rancid_Egg", info =>
                {
                    info.realItemCreator = (factory, _) =>
                    {
                        var item = factory.MakeItem("Rancid_Egg");
                        var tag = item.AddTag<EggColorTag>();
                        tag.ReplacementEggName = Eggs[rng.Next(Eggs.Count)].InternalName;
                        return item;
                    };
                });
            }
        }

        private const float ConsecutiveClickInterval = .5f;
        private const int ClicksToToggleSHINY = 7;

        private bool BuildButton(MenuPage landingPage, out SmallButton settingsButton)
        {
            var button = new SmallButton(landingPage, GetName());
            var shiny = button.Text.gameObject.AddComponent<SHINY>();

            void UpdateButtonColor()
            {
                if (Settings.ColorizeRancidEggs)
                {
                    button.Text.color = Colors.TRUE_COLOR;
                    shiny.enabled = Settings.SHINY;
                }
                else         
                {
                    shiny.enabled = false;
                    button.Text.color = Colors.DEFAULT_COLOR;
                }
            }

            UpdateButtonColor();
            var numConsecutiveClicks = 0;
            var timeOfLastClick = float.NegativeInfinity;
            button.OnClick += () =>
            {
                Settings.ColorizeRancidEggs = !Settings.ColorizeRancidEggs;
                var now = UnityEngine.Time.time;
                if (now - timeOfLastClick > ConsecutiveClickInterval)
                {
                    numConsecutiveClicks = 0;
                }
                timeOfLastClick = now;
                numConsecutiveClicks++;
                if (numConsecutiveClicks == ClicksToToggleSHINY)
                {
                    Settings.SHINY = !Settings.SHINY;
                    numConsecutiveClicks = 0;
                }
                UpdateButtonColor();
            };
            settingsButton = button;
            return true;
        }

        private void LogRandoSettings(LogArguments args, TextWriter w)
        {
            w.WriteLine("Logging RainbowEggs settings:");
            w.WriteLine(JsonUtil.Serialize(Settings));
        }

        public override string GetVersion() => "1.1.1";
    }
}

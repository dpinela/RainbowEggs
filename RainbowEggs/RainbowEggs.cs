using Modding;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.UIDefs;
using RandomizerMod.Settings;
using RandomizerMod.RC;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using System.Collections.Generic;

namespace RainbowEggs
{
    public class RainbowEggs : Mod
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
            }
        };

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
        }

        private static IEnumerable<string> NRandomEggs(System.Random rng, int n)
        {
            for (var i = 0; i < n; i++)
            {
                yield return Eggs[rng.Next(Eggs.Count)].InternalName;
            }
        }

        private static void ColorizeEggs(RequestBuilder rb)
        {
            var rng = new System.Random(rb.gs.Seed);
            rb.ReplaceItem("Rancid_Egg", n => NRandomEggs(rng, n));
        }

        private static void DefineLogicItems(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            var value = new TermValue(lmb.GetTerm("RANCIDEGGS"), 1);
            foreach (var egg in Eggs)
            {
                lmb.AddItem(new SingleItem(egg.InternalName, value));
            }
        }

        public override string GetVersion() => "1.0";
    }
}

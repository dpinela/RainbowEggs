using ItemChanger;
using UnityEngine;

namespace RainbowEggs
{
    internal class EmbeddedSprite : ISprite
    {
        public string key;

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => EmbeddedSprites.Get(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
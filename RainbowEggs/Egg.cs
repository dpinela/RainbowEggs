namespace RainbowEggs
{
    internal struct Egg
    {
        public string Sprite;
        public string Name;
        public string Description;

        public string InternalName => Name.Replace(" ", "_");
    }
}
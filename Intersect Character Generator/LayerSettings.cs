using JetBrains.Annotations;

namespace Intersect.CharacterGenerator
{
    public class LayerSettings
    {
        [NotNull] public Layer Layer { get; }

        internal LayerSettings([NotNull] Layer layer)
        {
            Layer = layer;
        }

        public string Graphic
        {
            get => Layer.Selected;
            set => Layer.Selected = value;
        }

        public int Hue
        {
            get => Layer.Hue;
            set => Layer.Hue = value;
        }

        public int HueIntensity
        {
            get => Layer.Saturation;
            set => Layer.Saturation = value;
        }

        public int Alpha
        {
            get => Layer.Alpha;
            set => Layer.Alpha = value;
        }

        public bool RandomizationLocked
        {
            get => Layer.RandomizationLocked;
            set => Layer.RandomizationLocked = value;
        }
    }
}

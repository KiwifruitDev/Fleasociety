using Microsoft.Xna.Framework;

namespace Fleasociety
{
    public interface IStation : IObject
    {
        public int order { get; set; }
        public bool attention { get; set; }
        public Color color { get; set; }
    }
}

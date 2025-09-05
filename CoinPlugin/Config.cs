using System.ComponentModel;

namespace CoinPlugin
{
    public class Config
    {
        [Description("Random chance that a player will receive a coin when spawning in (0-100)")]
        public int SpawnCoinChance { get; set; } = 15;
    }
}
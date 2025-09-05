// --- [ Modification Notice ] ---
// This file has been added by gamendegamer321.

using System.ComponentModel;

namespace CoinPlugin
{
    public class Config
    {
        [Description("Random chance that a player will receive a coin when spawning in (0-100)")]
        public int SpawnCoinChance { get; set; } = 15;

        [Description("This will increase the chance of nothing happening " +
                     "by adding it multiple times to the possible effects." +
                     "Keep in mind, restricting other effects will already make it more likely to get nothing.")]
        public int NothingCount { get; set; } = 1;

        [Description("Warhead effect can only be activated after x seconds, " +
                     "if it activates before it will do nothing instead")]
        public int MinSecondsForWarhead { get; set; } = 120;

        [Description("Whether to check whether an SCP is allowed to be teleported to surface")]
        public bool ScpCanTeleportToSurface { get; set; } = true;

        [Description("Max players that can be on surface for an SCP to get teleported there." +
                     "Use -1 to always allow it (Setting ignored when ScpCanTeleportToSurface is set to ture)")]
        public int ScpTeleportToSurfaceMaxPlayers { get; set; } = 5;

        [Description("Whether a coin throw can be cancelled by deselecting the coin before the effect triggers")]
        public bool CanCancelCoin { get; set; } = true;

        [Description("Max amount of times skeleton effect can be picked," +
                     "when picked it will do nothing instead of turning the player into a skeleton")]
        public int Scp3114Limit = 2;

        [Description("Minimum time in seconds between allowing the zombie effect. " +
                     "If the effect triggers to early, nothing will happen.")]
        public int MinSecondsForZombie = 30;
    }
}
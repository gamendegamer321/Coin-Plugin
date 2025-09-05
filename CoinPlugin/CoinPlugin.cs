// --- [ Modification Notice ] ---
// This file has been modified by gamendegamer321.
// View the original version at: https://github.com/BorkoAXT/Coin-Plugin/blob/master/FirstLabApiPlugin/CoinPlugin.cs

using System;
using System.Linq;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;
using Random = System.Random;

namespace CoinPlugin
{
    public class CoinPlugin : Plugin
    {
        public override string Author { get; } = "BorkoAXT";

        public override string Name { get; } = "Coin Plugin";

        public override string Description { get; } = "A plugin which allows coins to have unique effects when thrown";

        public override Version Version { get; } = new Version(0, 0, 7, 1);

        public override Version RequiredApiVersion { get; } =
            new Version(LabApi.Features.LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            PlayerEvents.FlippedCoin += OnCoinThrow;
        }

        public override void Disable()
        {
            PlayerEvents.FlippedCoin -= OnCoinThrow;
        }

        private static readonly Random Random = new Random();

        private static readonly ItemType[] ScpItems =
            { ItemType.MicroHID, ItemType.ParticleDisruptor, ItemType.Jailbird, ItemType.GunSCP127 };

        private static readonly string[] Effects =
        {
            "Amnesia", "Slowness", "Blindness", "Flashed", "Deafened", "Asphyxiated", "Bleeding", "Bleeding",
            "Blindness", "Exhausted", "Hemorrhage", "Invisibility", "SCP-1853", "Poisoned", "Bodyshot Reduction",
            "Damage Reduction", "Movement boost"
        };

        private static readonly string[] DisplayNames = { "Unlucky", "Coin Victim", "Gambler", "Unfamily guy" };


        private static void SwitchRole(Player ev)
        {
            var role = RoleTypeId.None;
            switch (ev.Role)
            {
                case RoleTypeId.NtfSergeant:
                    role = RoleTypeId.ChaosRifleman;
                    break;
                case RoleTypeId.NtfPrivate:
                    role = RoleTypeId.ChaosMarauder;
                    break;
                case RoleTypeId.NtfCaptain:
                    role = RoleTypeId.ChaosRepressor;
                    break;
                case RoleTypeId.NtfSpecialist:
                    role = RoleTypeId.ChaosConscript;
                    break;
                case RoleTypeId.ChaosConscript:
                    role = RoleTypeId.NtfSpecialist;
                    break;
                case RoleTypeId.ChaosRepressor:
                    role = RoleTypeId.NtfCaptain;
                    break;
                case RoleTypeId.ChaosMarauder:
                    role = RoleTypeId.NtfPrivate;
                    break;
                case RoleTypeId.ChaosRifleman:
                    role = RoleTypeId.NtfSergeant;
                    break;
                case RoleTypeId.Scientist:
                    role = RoleTypeId.ClassD;
                    break;
                case RoleTypeId.ClassD:
                    role = RoleTypeId.Scientist;
                    break;
                case RoleTypeId.FacilityGuard:
                    role = RoleTypeId.ClassD;
                    break;
            }

            if (role == RoleTypeId.None) return;

            ev.SetRole(role, flags: RoleSpawnFlags.AssignInventory);
            ev.SendHint($"<size=25><color=blue>[Coin Flip]</color>\nYour new class is: {ev.Role.ToString()}</size>", 5);
        }

        private static void SetToZombie(Player ev)
        {
            ev.SetRole(RoleTypeId.Scp0492, flags: RoleSpawnFlags.AssignInventory);
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou wake up feeling a bit.. weird</size>", 5);
        }

        private static void SetToSkeleton(Player ev)
        {
            ev.SetRole(RoleTypeId.Scp3114, flags: RoleSpawnFlags.AssignInventory);
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou look like a pencil with limbs!</size>", 5);
        }

        private static void GrantAnScpWeapon(Player ev)
        {
            if (!ev.IsInventoryFull)
            {
                ev.AddItem(ScpItems[Random.Next(ScpItems.Length)]);
                ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou have been given a random SCP weapon!</size>",
                    5);
            }
            else
            {
                ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nUnfortunately, your inventory was full!</size>",
                    5);
            }
        }

        private static void ThrowGrenade(Player ev)
        {
            var grenade = (TimedGrenadeProjectile)Pickup.Create(ItemType.GrenadeHE, ev.Position);
            grenade.Base.ServerActivate();
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nWatch you feet!</size>", 5);
        }

        private static void StartWarhead(Player ev)
        {
            if (Round.Duration.TotalMinutes >= 2)
            {
                Warhead.Start();
                ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nA warfare has started!</size>", 5);
            }
            else
            {
                Nothing(ev);
            }
        }

        private static void Nothing(Player ev)
        {
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nNothing happened!</size>", 5);
        }

        private static void ClearInventory(Player ev)
        {
            ev.ClearInventory();
            ev.SendHint(
                "<size=25><color=blue>[Coin Flip]</color>\nYou remember not taking your dementia pills today!</size>",
                5);
        }

        private static void SetMaxHp(Player ev)
        {
            var randomMaxHealth = Random.Next(100, 200);
            ev.MaxHealth = randomMaxHealth;
            ev.SendHint($"<size=25><color=blue>[Coin Flip]</color>\nYour new max health is {randomMaxHealth}</size>",
                5);
        }

        private static void GiveEffect(Player ev)
        {
            var randomEffect = Random.Next(Effects.Length);
            ev.ReferenceHub.playerEffectsController.ChangeState(Effects[randomEffect], 10, 10f);
            ev.SendHint(
                $"<size=25><color=blue>[Coin Flip]</color>\nYou have been diagnosed with {Effects[randomEffect]} for 10 seconds!</size>",
                5);
        }

        private static void KillPlayer(Player ev)
        {
            ev.Kill("You skipped a heartbeat!");
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou skipped a heartbeat!</size>", 5);
        }

        private static void SetRandomHp(Player ev)
        {
            var randomHealth = Random.Next(1, 100);
            ev.Health = randomHealth;
            ev.SendHint($"<size=25><color=blue>[Coin Flip]</color>\nYou are now at {randomHealth} health!</size>", 5);
        }

        private static void HighestTierCard(Player ev)
        {
            var highestLevel = ItemType.None;
            Item highestCard = null;
            foreach (var item in ev.Items.Where(x => x.Category == ItemCategory.Keycard))
            {
                if ((int)item.Type <= (int)highestLevel) continue;

                highestLevel = item.Type;
                highestCard = item;
            }

            if (highestLevel == ItemType.None || highestCard == null)
            {
                if (ev.IsInventoryFull)
                {
                    ev.SendHint(
                        "<size=25><color=blue>[Coin Flip]</color>\nYou don't have enough inventory space!</size>", 5);
                }
                else
                {
                    ev.AddItem(ItemType.KeycardScientist);
                    ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou have been given a cool card!</size>", 5);
                }

                return;
            }

            if (highestLevel == ItemType.KeycardO5)
            {
                ev.SendHint(
                    "<size=25><color=blue>[Coin Flip]</color>\nYou already have the highest level keycard!</size>", 5);
                return;
            }

            ev.RemoveItem(highestCard);
            ev.AddItem((ItemType)((int)highestLevel + 1));
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYour card has been upgraded</size>", 5);
        }

        private static void DisableLights(Player ev)
        {
            Map.TurnOffLights(15f);
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou haven't paid the electricity bill!</size>", 5);
        }

        private static void DisableElevators(Player ev)
        {
            Elevator.LockAll();
            Timing.CallDelayed(15f, Elevator.UnlockAll);
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nThe elevators have gone under maintenance</size>",
                5);
        }

        private static void SwapPositions(Player ev)
        {
            var players = Player.List.Where(x => x != ev).ToList();
            var randomPlayer = players[Random.Next(players.Count)];

            var evPosition = ev.Position;
            var victimPosition = randomPlayer.Position;

            ev.Position = victimPosition;
            randomPlayer.Position = evPosition;

            ev.SendHint(
                "<size=25><color=blue>[Coin Flip]</color>\nYou have switched positions with another player!</size>", 5);
            randomPlayer.SendHint(
                "<size=25><color=blue>[Coin Flip]</color>\nYou have switched positions with another player!</size>", 5);
        }


        private static void TeleportToRandomRoom(Player ev)
        {
            var nonEndingRooms = Map.Rooms
                .Where(x => x.Shape == RoomShape.Endroom || x.Shape == RoomShape.Undefined)
                .ToArray();
            var randomRoom = nonEndingRooms[Random.Next(nonEndingRooms.Length)];
            ev.Position = randomRoom.Position + Vector3.up * 0.2f;
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou have been teleported to a random room</size>",
                5);
        }

        private static void TeleportARandomScp(Player ev)
        {
            var scps = Player.List.Where(x => x.Team == Team.SCPs && x.Role != RoleTypeId.Scp0492).ToList();
            if (scps.Count != 0)
            {
                var randomScp = scps[Random.Next(scps.Count)];

                randomScp.Position = ev.Position;
            }

            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nSomething big has come for you!</size>", 5);
        }

        private static void DropAllItems(Player ev)
        {
            ev.DropAllItems();
            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou slipped!</size>", 5);
        }

        private static void ChangeName(Player ev)
        {
            ev.DisplayName = DisplayNames[Random.Next(DisplayNames.Length)];
            ev.SendHint($"<size=25><color=blue>[Coin Flip]</color>\nYour new name is {ev.DisplayName}</size>", 5);
            Timing.CallDelayed(300f, () => ev.DisplayName = null);
        }

        private static void DropManyMedkits(Player ev)
        {
            for (var i = 0; i < 10; i++)
            {
                if (ev.IsInventoryFull) break;

                var medkit = Pickup.Create(ItemType.Medkit, ev.Position, ev.Rotation);
                if (medkit != null)
                {
                    NetworkServer.Spawn(medkit.GameObject);
                }
            }


            ev.SendHint("<size=25><color=blue>[Coin Flip]</color>\nYou praying to RNGESUS has payed off!</size>", 5);
        }

        private static void OnCoinThrow(PlayerFlippedCoinEventArgs ev)
        {
            Timing.CallDelayed(5, () =>
            {
                var player = ev.Player;
                var num = Random.Next(1, 22);
                switch (num)
                {
                    case 1:
                        SwitchRole(player);
                        break;
                    case 2:
                        SetToZombie(player);
                        break;
                    case 3:
                        SetToSkeleton(player);
                        break;
                    case 4:
                        GrantAnScpWeapon(player);
                        break;
                    case 5:
                        ThrowGrenade(player);
                        break;
                    case 6:
                        StartWarhead(player);
                        break;
                    case 7:
                        Nothing(player);
                        break;
                    case 8:
                        ClearInventory(player);
                        break;
                    case 9:
                        SetMaxHp(player);
                        break;
                    case 10:
                        GiveEffect(player);
                        break;
                    case 11:
                        KillPlayer(player);
                        break;
                    case 12:
                        SetRandomHp(player);
                        break;
                    case 13:
                        HighestTierCard(player);
                        break;
                    case 14:
                        DisableLights(player);
                        break;
                    case 15:
                        DisableElevators(player);
                        break;
                    case 16:
                        DropManyMedkits(player);
                        break;
                    case 17:
                        SwapPositions(player);
                        break;
                    case 18:
                        TeleportToRandomRoom(player);
                        break;
                    case 19:
                        TeleportARandomScp(player);
                        break;
                    case 20:
                        DropAllItems(player);
                        break;
                    case 21:
                        ChangeName(player);
                        break;
                }

                Timing.CallDelayed(0.1f, () => player.RemoveItem(ev.CoinItem));
            });
        }
    }
}
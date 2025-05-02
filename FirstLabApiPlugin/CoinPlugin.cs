using LabApi.Features.Wrappers;
using System;
using LabApi.Loader.Features.Plugins;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using UnityEngine;
using ThrowableItem = LabApi.Features.Wrappers.ThrowableItem;
using LabApi.Events.Handlers;
using MEC;

namespace FirstLabApiPlugin
{
    public class CoinPlugin : Plugin
    {
        public override string Author { get; } = "BorkoAXT";

        public override string Name { get; } = "Coin Plugin";

        public override string Description { get; } = "A plugin which allows coins to have unique effects when thrown";

        public override Version Version { get; } = new Version(0, 0, 5, 0);

        public override Version RequiredApiVersion { get; } = new Version(LabApi.Features.LabApiProperties.CompiledVersion);

        public System.Random random = new System.Random();

        /* random hp - done
             * switch class - done
             * card - done
             * effect - done
             * throwgrenade
             * kill player - done
             * set role as zombie - done
             * set role as 3114 - done
             * nothing - done
             * clear inv - done
             * maxhp - done
             * grant a random scp item - done
             * warhead - done
        */
        public override void Enable()
        {
            PlayerEvents.FlippedCoin += OnCoinThrow;
        }
        public override void Disable()
        {
            PlayerEvents.FlippedCoin -= OnCoinThrow;
        }

        public void SwitchRole(Player ev)
        {
            Vector3 evPosition = ev.Position;
            switch (ev.Role)
            {
                case PlayerRoles.RoleTypeId.NtfSergeant: ev.SetRole(PlayerRoles.RoleTypeId.ChaosRifleman); break;
                case PlayerRoles.RoleTypeId.NtfPrivate: ev.SetRole(PlayerRoles.RoleTypeId.ChaosMarauder); break;
                case PlayerRoles.RoleTypeId.NtfCaptain: ev.SetRole(PlayerRoles.RoleTypeId.ChaosRepressor); break;
                case PlayerRoles.RoleTypeId.NtfSpecialist: ev.SetRole(PlayerRoles.RoleTypeId.ChaosConscript); break;

                case PlayerRoles.RoleTypeId.ChaosConscript: ev.SetRole(PlayerRoles.RoleTypeId.NtfSpecialist); break;
                case PlayerRoles.RoleTypeId.ChaosRepressor: ev.SetRole(PlayerRoles.RoleTypeId.NtfCaptain); break;
                case PlayerRoles.RoleTypeId.ChaosMarauder: ev.SetRole(PlayerRoles.RoleTypeId.NtfPrivate); break;
                case PlayerRoles.RoleTypeId.ChaosRifleman: ev.SetRole(PlayerRoles.RoleTypeId.NtfSergeant); break;

                case PlayerRoles.RoleTypeId.Scientist: ev.SetRole(PlayerRoles.RoleTypeId.ClassD); break;
                case PlayerRoles.RoleTypeId.ClassD: ev.SetRole(PlayerRoles.RoleTypeId.Scientist); break;

                case PlayerRoles.RoleTypeId.FacilityGuard: ev.SetRole(PlayerRoles.RoleTypeId.ClassD); break;
            }
            ev.Position = evPosition;
            ev.SendBroadcast($"Your new class is: {ev.Role.ToString()}", 5);
        }
        public void SetToZombie(Player ev)
        {
            Vector3 evPosition = ev.Position;
            ev.SetRole(PlayerRoles.RoleTypeId.Scp0492);
            ev.Position = ev.Position;
            ev.SendBroadcast("You wake up feeling a bit.. weird", 3);
        }
        public void SetToSkeleton(Player ev)
        {
            Vector3 evPosition = ev.Position;
            ev.SetRole(PlayerRoles.RoleTypeId.Scp3114);
            ev.Position = ev.Position;
            ev.SendBroadcast("You look like a pencil with limbs!", 5);
        }
        public void GrantAnScpWeapon(Player ev)
        {
            byte[] scpItems = { 16, 47, 50, 62 };
            sbyte _ = (sbyte)random.Next(scpItems.Length);
            if (!ev.IsInventoryFull)
            {
                Server.RunCommand($"give {ev.DisplayName} {_}");
                ev.SendBroadcast("You have been given a random SCP weapon!", 5);
            }
            else
            {
                ev.SendBroadcast("Unfortunately, your inventory was full!", 3);
            }

        }
        public void ThrowGrenade(Player ev)
        {
            if (!ev.IsInventoryFull)
            {
                var grenade = ev.AddItem(ItemType.GrenadeHE);
                if (grenade is ThrowableItem throwable)
                {
                    throwable.DropItem();
                    ev.SendBroadcast("Watch your feet! Oh, it appears to be disarmed", 5);
                }
                else
                {
                    ev.SendBroadcast("Failed to throw grenade!", 5);
                }
            }
        }
        public void StartWarhead(Player ev)
        {
            Warhead.Start();
            ev.SendBroadcast("A warfare has started!", 3);
        }
        public void Nothing(Player ev)
        {
            ev.SendBroadcast("Nothing happened!", 3);
        }
        public void ClearInventory(Player ev)
        {
            ev.ClearInventory();
            ev.SendBroadcast("You remember not taking your dementia pills today!", 5);
        }
        public void SetMaxHp(Player ev)
        {
            byte randomMaxHealth = (byte)random.Next(100, 200);
            ev.MaxHealth = randomMaxHealth;
            ev.SendBroadcast($"Your new max health is {randomMaxHealth}", 3);
        }
        public void GiveEffect(Player ev)
        {
            List<string> effects = new List<string>() { "Amnesia", "Slowness", "Blindness", "Flashed", "Deafened", "Asphyxiated", "Bleeding", "Bleeding", "Blindness", "Exhausted", "Hemorrhage", "Invisibility", "SCP-1853", "Poisoned", "Bodyshot Reduction", "Damage Reduction", "Movement boost" };
            ushort randomEffect = (ushort)random.Next(effects.Count);
            ev.ReferenceHub.playerEffectsController.ChangeState(effects[randomEffect], 10, 10f);
            ev.SendBroadcast($"You have been diagnosed with {effects[randomEffect]} for 10 seconds!", 5);
        }
        public void KillPlayer(Player ev)
        {
            ev.Kill();
            ev.SendBroadcast("You skipped a heartbeat!", 3);
        }
        public void SetRandomHp(Player ev)
        {
            byte randomHealth = (byte)random.Next(1, 100);
            ev.Health = randomHealth;
            ev.SendBroadcast($"You are now at {randomHealth} health!", 3);
        }
        public void HighestTierCard(Player ev)
        {
            sbyte highestKeycardId = -1;
            ItemType highestLevel = 0;
            foreach (var item in ev.ReferenceHub.inventory.UserInventory.Items)
            {
                if (item.Value.name.StartsWith("Keycard"))
                {
                    if ((int)item.Value.ItemTypeId > (int)highestLevel)
                    {
                        highestLevel = item.Value.ItemTypeId;
                        highestKeycardId = (sbyte)item.Key;
                    }
                }
            }
            if (highestKeycardId == -1)
            {
                if (ev.IsInventoryFull)
                {
                    ev.SendBroadcast("You don't have enough inventory space!", 5);
                }
                else
                {
                    ev.AddItem(ItemType.KeycardScientist);
                    ev.SendBroadcast("You have been given a cool card!", 3);
                }
                return;
            }
            if (highestLevel == ItemType.KeycardO5)
            {
                ev.SendBroadcast("You already have the highest level keycard!", 5);
                return;
            }
            ev.ReferenceHub.inventory.UserInventory.Items.Remove((ushort)highestKeycardId);
            ItemType nextKeycard = (ItemType)(((int)highestLevel) + 1);
            ev.AddItem(nextKeycard);
            ev.SendBroadcast("Your card has been upgraded", 5);

        }
        public void DisableLights(Player ev)
        {
            Map.TurnOffLights();
            Timing.CallDelayed(0.1f, () => Map.TurnOnLights());
            ev.SendBroadcast("You haven't paid the electricity bill!", 5);
           
        }
        public void OnCoinThrow(PlayerFlippedCoinEventArgs ev)
        {
            Player player = ev.Player;
            byte num = (byte)random.Next(1, 15);
            switch (num)
            {
                case 1: SwitchRole(player); break;
                case 2: SetToZombie(player); break;
                case 3: SetToSkeleton(player); break;
                case 4: GrantAnScpWeapon(player); break;
                case 5: ThrowGrenade(player); break;
                case 6: StartWarhead(player); break;
                case 7: Nothing(player); break;
                case 8: ClearInventory(player); break;
                case 9: SetMaxHp(player); break;
                case 10: GiveEffect(player); break;
                case 11: KillPlayer(player); break;
                case 12: SetRandomHp(player); break;
                case 13: HighestTierCard(player); break;
                case 14: DisableLights(player); break;
            }
            Timing.CallDelayed(0.1f, () => ev.Player.RemoveItem(ev.CoinItem));

        }
    }
}
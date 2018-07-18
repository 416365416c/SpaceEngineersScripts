using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        float mwhThreshold = 0.15f;
        string targetGroup = "PowerSave"; // Recommend that the target be a group, so you can turn them back on easily
        bool engageBeacons = true;
        bool checkBatteries = false;
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1; //Every tick, we're lean and that's a tick of power saved
        }

        public void Main()
        {
            float acc = 0.0f;
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyReactor>(blocks);
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyReactor block = blocks[i] as IMyReactor;
                if (block != null)
                {
                    List<IMyInventoryItem> reactorItems = block.GetInventory(0).GetItems();
                    for (int j = 0; j < reactorItems.Count; j++)
                    {
                        acc += (float)reactorItems[j].Amount;
                    }

                }
            }

            if (checkBatteries)
            {
                blocks.Clear();
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(blocks);
                for (int i = 0; i < blocks.Count; i++)
                {
                    IMyBatteryBlock block = blocks[i] as IMyBatteryBlock;
                    if (block != null)
                    {
                        acc += block.CurrentStoredPower;
                    }
                }

            }

            if (acc <= mwhThreshold)
                EnterPowerSave();
        }

        private void EnterPowerSave()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            Echo("Entering power save mode.\n");
            if (engageBeacons)
            {
                Echo("Activating Beacons.\n");
                GridTerminalSystem.GetBlocksOfType<IMyBeacon>(blocks);
                for (int i = 0; i < blocks.Count; i++)
                {
                    IMyBeacon block = blocks[i] as IMyBeacon;
                    if (block != null)
                    {
                        block.Enabled = true;
                    }
                }
                blocks.Clear();
            }

            Echo("Shutting down group: " + targetGroup);
            
            GridTerminalSystem.GetBlockGroupWithName(targetGroup).GetBlocksOfType<IMyFunctionalBlock>(blocks);
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyFunctionalBlock block = blocks[i] as IMyFunctionalBlock;
                if (block == null || block == Me)
                {
                    continue;
                }

                block.Enabled = false;
            }

            Runtime.UpdateFrequency = UpdateFrequency.None; // Save our power too!
        }
    }
}
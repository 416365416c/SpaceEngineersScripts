
        void main() {
            string STONE_STORAGE = "STONE";
            List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> refinery_like = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refinery_like);
            GridTerminalSystem.SearchBlocksOfName(STONE_STORAGE, containers);

            moveStone()
            IMyInventory inv = refinery.GetInventory(1);
            IMyCargoContainer container = findCargo(inv, INGOT_STORAGE);
            transferAllTo(inv, container.GetInventory(0));

            inv = refinery.GetInventory(0);
            container = findCargo(inv, ORE_STORAGE);
            transferAllTo(inv, container.GetInventory(0));
        }

        void cleanOutRefineries()
        {
            for (int i = 0; i < refinery_like.Count; i++)
            {
                IMyRefinery refinery = (IMyRefinery)refinery_like[i];

                // move mats
                IMyInventory inv = refinery.GetInventory(1);
                IMyCargoContainer container = findCargo(inv, STONE_STORAGE);
                transferStoneTo(inv, container.GetInventory(0));

                inv = refinery.GetInventory(0);
                container = findCargo(inv, STONE_STORAGE);
                transferStoneTo(inv, container.GetInventory(0));
            }
        }

        void transferStoneTo(IMyInventory source, IMyInventory dest)
        {
            //This collects both ingot and ore type "Stone"
            List<int> stoneIdxs = new List<int>;
            List<IMyInventoryItem> sourceItems = source.GetItems();
            for (int i = 0; i < sourceItems.Count; i++)
            {
                if (sourceItems[i].Content.SubtypeId == "Stone")
                {
                    stoneIdxs.Add(i);
                }
            }

            for (int i = stoneIdxs.Count - 1; i >= 0; i--)
            {
                source.TransferItemTo(dest, stoneIdxs[i], null, true, null);
            }
        }

        IMyCargoContainer findCargo(IMyInventory sibling, String type)
        {
            IMyCargoContainer selected = null;
            for (int i = 0; i < containers.Count; i++)
            {
                IMyCargoContainer container = (IMyCargoContainer)containers[i];

                if (!container.GetInventory(0).IsFull && sibling.IsConnectedTo(container.GetInventory(0)))
                {
                    if (container.DisplayNameText.Contains(type))
                    {
                        return container;
                    }
                    else
                    {
                        selected = container;
                    }
                }
            }
            return selected;
        }



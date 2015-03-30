List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
string outVar = "OUTPUT";
for (int i=0; i<containers.Count; i++) {
    IMyCargoContainer c = containers[i] as IMyCargoContainer;
    if (c == null)
        continue;
    List<IMyInventoryItem> inv = c.GetInventory(0).GetItems();  
    int stored = 0; 
    outVar += "\n" + c.CustomName + ":";
    for (int j = 0; j < inv.Count; j++) 
        outVar += inv[j].Content.SubtypeId + "," + inv[j].Content.SubtypeId + "\n";    
}

List<IMyTerminalBlock> outputBlocks = new List<IMyTerminalBlock>();  
GridTerminalSystem.SearchBlocksOfName("OUTPUT", outputBlocks);  
for(int i = 0; i < outputBlocks.Count; i++) {   
    IMyTextPanel tp = outputBlocks[i] as IMyTextPanel; 
    if(tp !=null)
        tp.WritePublicText(outVar); 
}   


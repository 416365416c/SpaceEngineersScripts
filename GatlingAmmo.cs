//If bonus, then check rocket and interior turrets too.  
//But note they aren't counted in the global ammo check  
bool bonus = true;  
//Set to true if you want ALL interior lights to change color 
bool lights = true;  
//mags per turret to count system as low  
int amber_threshold = 10;  
//mags in a single turret to count it as low 
int low_threshold = 5;  
      
 
public int turretLoop(List<IMyTerminalBlock> input, int threshold, List<string> low, List<string> depleted)  
{  
    int ret = 0;  
    for(int i = 0; i < input.Count; i++) {   
        IMyLargeTurretBase turret = input[i] as IMyLargeTurretBase;   
        if(turret != null) {   
            List<IMyInventoryItem> inv = turret.GetInventory(0).GetItems();   
            int stored = 0;  
            for (int j = 0; j < inv.Count; j++)  
                stored += (int)inv[j].Amount;  
            if (stored <= 0)  
                depleted.Add(turret.CustomName);  
            else if (stored <= threshold)  
                low.Add(turret.CustomName);  
            ret += stored;  
        }   
    }   
    return ret;  
}  
void Main()   
{   
    List<IMyTerminalBlock> gatlingBlocks = new List<IMyTerminalBlock>();   
    GridTerminalSystem.GetBlocksOfType<IMyLargeGatlingTurret>(gatlingBlocks);   
    List<IMyTerminalBlock> interiorBlocks = new List<IMyTerminalBlock>();    
    GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(interiorBlocks);   
    List<IMyTerminalBlock> rocketBlocks = new List<IMyTerminalBlock>();    
    GridTerminalSystem.GetBlocksOfType<IMyLargeMissileTurret>(interiorBlocks);   
  
    List<String> outTurrets = new List<String>();  
    List<String> lowTurrets = new List<String>();  
  
    //get the current number of blocks found   
    int count = gatlingBlocks.Count;   
    int mags = turretLoop(gatlingBlocks, low_threshold, lowTurrets, outTurrets);  
  
    if (bonus) {  
        turretLoop(interiorBlocks, low_threshold, lowTurrets, outTurrets);  
        turretLoop(rocketBlocks, low_threshold, lowTurrets, outTurrets);  
    }  
  
    String statusColor = "WHITE";  
    VRageMath.Color colorVar = VRageMath.Color.White;  
    if (outTurrets.Count > 0 || count == 0) {  
        statusColor = "RED";  
        colorVar = VRageMath.Color.Red;  
    } else if ((mags / count) < amber_threshold || lowTurrets.Count > 0) {  
        statusColor = "AMBER";  
        colorVar = VRageMath.Color.Yellow;  
    } else {  
        statusColor = "GREEN";  
        colorVar = VRageMath.Color.Green;  
    }  
  
    String output = "AMMO STATUS: " + statusColor;   
    output += "\n" + mags + " magazines across " + count + " gatling turrets.";  
    if (count == 0) { 
        output += "\nWait, 0 turrets?\nWhere are your gatling turrets?"; 
    } else { 
        output += "\nApprox. " + (mags/count) + " magazines per turret.\n";  
        if (statusColor == "GREEN") {  
            output += "All gatling turrets have at least " + low_threshold + " mags.\n";  
        } else {  
            output += "Low Turrets:\n";  
            for (int i=0; i<lowTurrets.Count; i++)  
                output += lowTurrets[i] + "\n";  
            output += "Empty Turrets:\n";  
            for (int i=0; i<outTurrets.Count; i++)  
                output += outTurrets[i] + "\n";  
        }  
    } 
  
    List<IMyTerminalBlock> outputBlocks = new List<IMyTerminalBlock>();   
    GridTerminalSystem.SearchBlocksOfName("AMMO_REPORT", outputBlocks); 
    List<IMyTerminalBlock> lightBlocks = new List<IMyTerminalBlock>();    
    GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(lightBlocks);  
     
    for(int i = 0; i < outputBlocks.Count; i++) {    
        IMyTextPanel tp = outputBlocks[i] as IMyTextPanel;  
        if(tp !=null) {  
            tp.SetValue<Color>("FontColor", colorVar);  
            tp.WritePublicText(output);  
        }  
    }  
     
    if(lights) { 
        for(int i = 0; i < lightBlocks.Count; i++) {     
            IMyInteriorLight l = lightBlocks[i] as IMyInteriorLight;   
            if(l !=null) {  
                    l.SetValue<Color>("Color", colorVar); 
            }  
        } 
    } 
} 

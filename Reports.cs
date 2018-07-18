////General options
//Set to true if you want interior lights (with a matching name) to change color
bool lights = true;

////Ammo options
String ammoNameFilter = "AMMO_REPORT";
float ammoFontSize = 1.8f;
//If allTurrets, then check rocket and interior turrets too.
//But note they aren't counted in the global ammo check
bool allTurrets = false;
//mags per turret to count system as low
int ammo_amber_threshold = 5;
//mags in a single turret to count it as low
int ammo_low_threshold = 1;

////Power options
String powerNameFilter = "POWER_REPORT";
float powerFontSize = 1.8f;
//Amount of uranium in a reactor to consider it "low"
float lowULevel = 1.0f;

////Oxygen options
String oxyNameFilter = "OXYGEN_REPORT";
float oxyFontSize = 1.8f;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;	
}

public void Main()
{
    ammoReport();
	powerReport();
	oxygenReport();
}

public void printReport(String text, VRageMath.Color color, float fontSize, String nameFilter)
{
	List<IMyTerminalBlock> outputBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(nameFilter, outputBlocks);

    for(int i = 0; i < outputBlocks.Count; i++) {
        IMyTextPanel tp = outputBlocks[i] as IMyTextPanel;
        if(tp !=null) {
            tp.SetValue<Color>("FontColor", color);//TODO: This is obsolete?
            tp.SetValue<float>("FontSize", fontSize);
            tp.WritePublicText(text);
        }
    }

    if(lights) {
	    for(int i = 0; i < outputBlocks.Count; i++) {
            IMyInteriorLight l = outputBlocks[i] as IMyInteriorLight;
            if(l !=null) {
                l.SetValue<Color>("Color", color);
            }
        }
    }
}

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

public void ammoReport()
{
    List<IMyTerminalBlock> gatlingBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyLargeGatlingTurret>(gatlingBlocks);

    List<String> outTurrets = new List<String>();
    List<String> lowTurrets = new List<String>();

    //get the current number of blocks found
    int count = gatlingBlocks.Count;
    int mags = turretLoop(gatlingBlocks, ammo_low_threshold, lowTurrets, outTurrets);

    if (allTurrets) {
        List<IMyTerminalBlock> interiorBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(interiorBlocks);
        List<IMyTerminalBlock> rocketBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyLargeMissileTurret>(rocketBlocks);

        turretLoop(interiorBlocks, ammo_low_threshold, lowTurrets, outTurrets);
        turretLoop(rocketBlocks, ammo_low_threshold, lowTurrets, outTurrets);
    }

    String statusColor = "WHITE";
    VRageMath.Color colorVar = VRageMath.Color.White;
    if (outTurrets.Count > 0 || count == 0) {
        statusColor = "RED";
        colorVar = VRageMath.Color.Red;
    } else if ((mags / count) < ammo_amber_threshold || lowTurrets.Count > 0) {
        statusColor = "AMBER";
        colorVar = VRageMath.Color.Yellow;
    } else {
        statusColor = "GREEN";
        colorVar = VRageMath.Color.Green;
    }

    String output = "AMMO STATUS: " + statusColor;
    output += "\n" + mags + " magazines.\n" + count + " gatling turrets.";
    if (count == 0) {
        output += "\nWait, 0 turrets?\nWhere are your\ngatling turrets?";
    } else {
        output += "\nApprox. " + (mags/count) + " mags/turret.\n";
        if (statusColor == "GREEN") {
            output += "All gatling turrets have\n    at least " + ammo_low_threshold + " mags.\n";
        } else {
            output += "Low Turrets:\n";
            for (int i=0; i<lowTurrets.Count; i++)
                output += lowTurrets[i] + "\n";
            output += "Empty Turrets:\n";
            for (int i=0; i<outTurrets.Count; i++)
                output += outTurrets[i] + "\n";
        }
    }

	printReport(output, colorVar, ammoFontSize, ammoNameFilter);
}

public void powerReport()
{
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyReactor>(blocks);
	
	float maxRea = 0.0f;
	float curRea = 0.0f;
    for (int i=0; i < blocks.Count; i++) {
		IMyReactor reactor = blocks[i] as IMyReactor;
        if(reactor != null) {
			maxRea += reactor.MaxOutput;
			curRea += reactor.CurrentOutput;
		}
	}
	blocks.Clear();
	
	float maxSol = 0.0f;
	float curSol = 0.0f;
	GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(blocks);
	for (int i=0; i < blocks.Count; i++) {
		IMySolarPanel sp = blocks[i] as IMySolarPanel;
        if(sp != null) {
			maxSol += sp.MaxOutput;
			curSol += sp.CurrentOutput;
		}
	}
	
	float maxBat = 0.0f; //This is storage, not output, for now
	float curBat = 0.0f;
	GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(blocks);
	for (int i=0; i < blocks.Count; i++) {
		IMyBatteryBlock sp = blocks[i] as IMyBatteryBlock;
        if(sp != null) {
			maxBat += sp.MaxStoredPower;
			curBat += sp.CurrentStoredPower;
		}
	}
	
	String statusColor = "WHITE";
    VRageMath.Color colorVar = VRageMath.Color.White;
    if (curRea/maxRea > 0.99 || curBat/maxBat < 0.1) {
        statusColor = "RED";
        colorVar = VRageMath.Color.Red;
    } else if (curRea/maxRea > 0.1) {
        statusColor = "AMBER";
        colorVar = VRageMath.Color.Yellow;
    } else {
        statusColor = "GREEN";
        colorVar = VRageMath.Color.Green;
    }

    String output = "POWER STATUS: " + statusColor + "\n";
    output += "Solar: " + curSol + "/" + maxSol + "MW\n";
	output += "Nuclear: " + curRea + "/" + maxRea + "MW\n";
	output += "Battery: " + curBat + "/" + maxBat + "MWh\n";
	
	printReport(output, colorVar, powerFontSize, powerNameFilter);
}

public void oxygenReport()
{ 
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(blocks);
	
	float oxyAcc = 0.0f;
	for (int i=0; i < blocks.Count; i++) { 
		IMyGasTank tank = blocks[i] as IMyGasTank;
		if (tank != null) {
			//TODO: Exclude H2 tanks //IMyInventory.GetItems()[i].Content.SubtypeName.ToString().Contains("Stone")
			//Echo((IMyInventoryOwner)tank.GetInventory(0));
			oxyAcc += (float)tank.FilledRatio;
		}
	}
	float oxyPct = oxyAcc / blocks.Count;
	
	blocks.Clear();
	GridTerminalSystem.GetBlocksOfType<IMyAirVent>(blocks);
	List<String> inopVents = new List<String>();
    
	for (int i=0; i < blocks.Count; i++) {
		IMyAirVent av = blocks[i] as IMyAirVent;
		if (av != null && !av.CanPressurize)
			inopVents.Add(av.CustomName);
	}
	
	String statusColor = "WHITE";
    VRageMath.Color colorVar = VRageMath.Color.White;
    if (inopVents.Count > 0 || oxyPct <= 0.001f) {
        statusColor = "RED";
        colorVar = VRageMath.Color.Red;
    } else if (oxyPct <= 0.3f) {
        statusColor = "AMBER";
        colorVar = VRageMath.Color.Yellow;
    } else {
        statusColor = "GREEN";
        colorVar = VRageMath.Color.Green;
    }

    String output = "O2 STATUS: " + statusColor + "\n";
	output += (oxyPct * 100) + "% Full";
    if (inopVents.Count > 0) {
		output += "Inoperative Vents:\n";
        for (int i=0; i<inopVents.Count; i++)
            output += inopVents[i] + "\n";
	}
	
	printReport(output, colorVar, oxyFontSize, oxyNameFilter);
}


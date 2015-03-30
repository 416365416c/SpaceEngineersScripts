//Config Variables 
String rearThrustName = "GDRT"; //Contains, not whole name 
String errorTextName = "ERRORS";//Contains, not whole name 
float overrideValue = 40000.0f; //N of thrust per thruster 
String hangarRotorName ="Rotor - Hangar Door"; //Full Name 
String hatchPistonName = "Piston - Top Hatch"; //Full Name 
 
List<IMyTerminalBlock> thrusters = new List<IMyTerminalBlock>();  
IMyMotorStator hangarRotor;
IMyPistonBase hatchPiston;
IMyTextPanel errorLcd;
 
void Main() 
{ 
    initBlocks(); 
    maybeCloseHangarDoor(); 
    maybeCloseTopHatch(); 
    setThrusters(); 
}

void maybeCloseHangarDoor() 
{ 
    //Thanks to Textor at KeenSWH forums for this way to get the values 
    //http://forums.keenswh.com/post/help-needing-some-help-with-my-code-7225970 
    System.Text.RegularExpressions.Regex query =  
new System.Text.RegularExpressions.Regex("Current angle: ([0-9]{1,3})");  
    System.Text.RegularExpressions.Match m = query.Match(hangarRotor.DetailedInfo);  
  
    if(m.Success) {  
         string s = m.Value.Remove(0,15);  
         float angle = float.Parse(s);  
  
         if(angle <= 275.0f) {  
             ITerminalAction reverseAction = hangarRotor.GetActionWithName("Reverse"); 
             reverseAction.Apply(hangarRotor); 
         }  
    } else {  
        if (errorLcd != null) 
            errorLcd.WritePublicText("Error parsing angle\nfor hangar rotor"); 
    } 
} 
 
void setThrusters() 
{ 
    for(int i=0; i<thrusters.Count; i++) { 
        IMyThrust t = thrusters[i] as IMyThrust; 
        if (t == null) 
            continue; 
 
        t.SetValueFloat("Override", overrideValue); 
    } 
} 
 
void maybeCloseTopHatch() 
{ 
    //Thanks to Textor at KeenSWH forums for this way to get the values 
    //http://forums.keenswh.com/post/guide-programmable-block-detailedinfo-output-7230123?pid=1285630676#post1285630676 
    String openPosition = "Current position: 0.0m";
    if(hatchPiston.DetailedInfo == openPosition) {  
         ITerminalAction reverseAction = hatchPiston.GetActionWithName("Reverse"); 
         reverseAction.Apply(hatchPiston);
    } /*else {   
        if (errorLcd != null)  
            errorLcd.WritePublicText("Piston string was:\n\'"+hatchPiston.DetailedInfo+"\'\n");
    }*/
} 
 
void initBlocks() 
{ 
    thrusters.Clear(); 
    GridTerminalSystem.SearchBlocksOfName(rearThrustName, thrusters); 
    if (errorLcd == null) { 
        List<IMyTerminalBlock> outputBlocks = new List<IMyTerminalBlock>();   
        GridTerminalSystem.SearchBlocksOfName(errorTextName, outputBlocks);   
 
        if (outputBlocks.Count > 0) 
            errorLcd = (IMyTextPanel) outputBlocks[0] as IMyTextPanel; 
    } 
 
    if (hangarRotor == null) 
        hangarRotor = (IMyMotorStator)GridTerminalSystem.GetBlockWithName(hangarRotorName); 
    if (hatchPiston == null) 
        hatchPiston = (IMyPistonBase)GridTerminalSystem.GetBlockWithName(hatchPistonName); 
 
    if (errorLcd != null) { 
            String output = "Errors:\n"; 
            if (hangarRotor == null) 
                output += "Cannot find hangar rotor\n"; 
            if (hatchPiston == null) 
                output += "Cannot find hatch piston\n"; 
            if (thrusters.Count == 0) 
                output += "No thrusters found\n"; 
            errorLcd.WritePublicText(output); 
    } 
} 


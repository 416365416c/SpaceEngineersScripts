//Config Variables  
String rearThrustName = "GDRT"; //Contains, not whole name  
String errorTextName = "ERRORS";//Contains, not whole name  
String drillRotorName ="Advanced Rotor - Drill Array"; //Full Name  
String spinTimerName = "Timer - Stop Mining Spin"; //Full Name  
String nextTimerName = "Timer - Stop Mining 2"; //Full Name   
float overrideValue = 12000.0f; 
float epsilon = 24.0f; //Increase if it spins multiple times before stopping  
float targetAngle = 90.0f;  
  
List<IMyTerminalBlock> thrusters = new List<IMyTerminalBlock>();   
IMyMotorStator drillRotor;  
IMyTextPanel errorLcd;  
IMyTimerBlock spinTimer;
IMyTimerBlock nextTimer;
  
void Main()  
{  
    initBlocks();  
    checkSpin();  
    setThrusters();  
}  
  
void checkSpin()  
{  
    //Thanks to Textor at KeenSWH forums for this way to get the values  
    //http://forums.keenswh.com/post/help-needing-some-help-with-my-code-7225970  
    System.Text.RegularExpressions.Regex query =   
    new System.Text.RegularExpressions.Regex("Current angle: ([0-9]{1,3})");   
    System.Text.RegularExpressions.Match m = query.Match(drillRotor.DetailedInfo);   
   
    if(m.Success) {   
         string s = m.Value.Remove(0,15);   
         float angle = float.Parse(s);   
         if(angle + epsilon >= targetAngle && angle - epsilon <= targetAngle) {   
             //Close enough to target angle. Stop spinning  
             ITerminalAction stopAction = drillRotor.GetActionWithName("OnOff_Off");  
             stopAction.Apply(drillRotor);
             ITerminalAction startAction = nextTimer.GetActionWithName("Start");   
             startAction.Apply(nextTimer);
         } else {  
             //Not close enough to target angle. Keep spinning  
             ITerminalAction startAction = spinTimer.GetActionWithName("Start");  
             startAction.Apply(spinTimer);  
             //This will retrigger the program when the timer expires!  
         }   
    } else {   
        if (errorLcd != null)  
            errorLcd.WritePublicText("Error parsing angle\nfor drill rotor");  
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
  
void initBlocks()  
{  
    thrusters.Clear();  
    GridTerminalSystem.SearchBlocksOfName(rearThrustName, thrusters);  
    if (errorLcd == null) {  
        List<IMyTerminalBlock> outputBlocks = new List<IMyTerminalBlock>();    
        GridTerminalSystem.SearchBlocksOfName(errorTextName, outputBlocks);    
  
        if (outputBlocks.Count > 0) //Yes, just one  
            errorLcd = (IMyTextPanel) outputBlocks[0] as IMyTextPanel;  
    }  
  
    if (spinTimer == null)  
        spinTimer = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName(spinTimerName);  
    if (nextTimer == null)   
        nextTimer = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName(nextTimerName);   
    if (drillRotor == null)  
        drillRotor = (IMyMotorStator)GridTerminalSystem.GetBlockWithName(drillRotorName);  
  
    if (errorLcd != null) {  
            String output = "Errors:\n";  
            if (spinTimer == null)  
                output += "Cannot find spin timer\n";
            if (nextTimer == null)   
                output += "Cannot find next timer\n";   
            if (drillRotor == null)  
                output += "Cannot find drill rotor\n";  
            if (thrusters.Count == 0)  
                output += "No thrusters found\n";  
            errorLcd.WritePublicText(output);  
    }  
}  


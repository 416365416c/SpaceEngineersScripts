void getRocks()
{
}

void main()
{
    //Find a collector and spit rocks/gravel out of it until they're all gone
    string connectorBlockWord = "Connector";
    string timerBlockWord = "Rock Tick";
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>;
    GridTerminalSystem.SearchBlocksOfName(connectorBlockWord, blocks);
    IMyConnector con;
    if ( blocks.Count < 1 || (con = blocks[0] as IMyConnector) == Null)
        return;
    GridTerminalSystem.SearchBlocksOfName(timerBlockWord, blocks);
    IMyTimerBlock tim;
    if ( blocks.Count < 1 || (tim = blocks[0] as IMyTimerBlock) == Null)
        return;

    if (getRocks(con)) {
        if (!con.throwout)
            con.getactionwithname("throwout").apply(con);
        tim.getactionwithname("start").apply(tim);
    } else {
        if (con.throwout)
            con.getactionwithname("throwout").apply(con);
        tim.getactionwithname("stop").apply(tim);
    }
}

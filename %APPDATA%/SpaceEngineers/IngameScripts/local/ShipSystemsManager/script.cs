/*
 *   R e a d m e
 *   -----------
 * 
 * Specify which zones your blocks are located in by adding a "zones=" line to their Custom Data. 
 * Multiple zones are specified by a semi-colon.
 * EG: zones=zone-1;zone-2;zone-3
 * 
 * Specify the function of a block by adding a "functions=" line to their Custom Data. 
 * Multiple functions can be specified with a semi-colon.
 * EG: functions=airlock
 * 
 * Supported Functions:
 * Doors:
 * airlock 
 * - This door will be sealed (closed and disabled) during a decompression event in any zone it is assigned to.
 * - To create an automatic airlock, mark the door in two adjacent zones which both have Air Vents assigned to them.
 * security 
 * - This door will be closed during an intruder alert or battle stations callout.
 * 
 * LCD Displays:
 * doorsign 
 * - Indicates a small Text LCD above a door. It's text will read BATTLE STATIONS, DECOMPRESSION or other similar messages.
 * battle 
 * - Indicates a large LCD which will display a graphic during a BATTLE STATIONS state.
 * warnsign 
 * - Indicates a large LCD which will display an appropriate graphic during any state which provides on.
 * 
 * Sound Blocks:
 * siren 
 * - These sound blocks will play a sound loop appropriate to the current ship state.
 * 
 * Lights
 * warnlight 
 * - These lights will change color and blink depending on the current ship state.
 * 
 */

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means.
    //
    // This method is optional and can be removed if not
    // needed.
}

public void Main(String argument, UpdateType updateSource)
{
    if (String.IsNullOrWhiteSpace(argument))
    {
        // Perform a tick.
        if ((updateSource & UpdateType.Update1) != UpdateType.None)
        {
            // Running in high speed mode is not recommended!
            if (!EnableSingleTickCycle)
            {
                // Throw an exception to prevent further cycles.
                throw new Exception("Running the program at one cycle per tick is not recommended.");
            }
        }

        if ((updateSource & UpdateType.Update100) != UpdateType.None)
        {
            // Script is running in slow mode.
            Output("Script is running in slow mode (once per 6 seconds).");
        }

        Tick();
    }
    else
    {
        // Set or clear argument flags.
        Flags(argument);
    }
}

private void Flags(String argument)
{
    var arguments = argument.Split(' ');
    switch (arguments.FirstOrDefault())
    {
        case "activate":
            var newstate = String.Join(" ", arguments.Skip(1));

            Me.SetConfigFlag("custom-states", newstate);
            return;
        case "deactivate":
            var oldstate = String.Join(" ", arguments.Skip(1));

            Me.ClearConfigFlag("custom-states", oldstate);
            return;
    }
}

private void Tick()
{
    // Only check air vents if pressurization is enabled.
    var pressure = GridTerminalSystem.GetBlocksOfType<IMyAirVent>().FirstOrDefault(v => v.PressurizationEnabled) != default(IMyAirVent);

    foreach (var zone in GridTerminalSystem.GetZones())
    {
        Output("Checking Zone \"" + zone + "\" for new triggers.");

        if (pressure)
        {
            TestAirVents(zone);
        }

        TestSensors(zone);
        TestInteriorWeapons(zone);
        TestBattleStations();

        ApplyBlockStates();
    }
}

private void Output(String message, Boolean append = true)
{
    message = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message;
    Echo(message);

    var lcds = GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(p => p.HasFunction("debug lcd"));

    foreach (var lcd in lcds)
    {
        lcd.WritePublicTitle("ShipSystemsManager Diagnostics");
        lcd.FontSize = 1.5f;
        lcd.Font = "DEBUG";

        if (append)
        {
            var text = lcd.GetPublicText().Split('\n').ToList();
            if (text.Count() > 34)
            {
                text.RemoveAt(0);
            }
            text.Add(message);
            lcd.WritePublicText(String.Join("\n", text));
        }
        else
        {
            lcd.WritePublicText(message);
        }
    }
}

private void ApplyBlockStates()
{
    foreach (var block in GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(b => b.GetConfigs("zones").Any()))
    {
        var states = block.GetConfigs("state");
        var styler = StatePriority.FirstOrDefault(s => states.Contains(s.Key));

        if (styler != default(StateStyler))
        {
            styler.Style(block);
        }
        else
        {
            StyleRestore(block);
        }
    }
}

private void TestAirVents(String zone)
{
    var vents = GridTerminalSystem.GetBlocksOfType<IMyAirVent>(v => v.IsWorking && v.IsInZone(zone) && !v.Depressurize);
    if (vents.Any(v => !v.CanPressurize))
    {
        Output("Depressurization detected in " + vents.Count() + " Air Vents.");

        GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true)
            .SetStates(BlockState.DECOMPRESSION);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .SetStates(BlockState.DECOMPRESSION);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .SetStates(BlockState.DECOMPRESSION);

        GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .SetStates(BlockState.DECOMPRESSION);

        return;

        // TODO: Move to Program.ApplyBlockStates
        var doors = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true);
        if (doors.Any())
        {
            Output("Locking down " + doors.Count() + " doors to zone " + zone + ".");

            doors.ApplyConfig<IMyDoor>(new Dictionary<String, Object>()
            {
                { "Locked", true }
            });
        }
        else
        {
            Output("ALERT: Zone " + zone + " cannot be sealed, no functional doors were found!");
        }

        base.GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .ApplyConfig<IMyTextPanel>(new Dictionary<String, Object>()
        {
            { "FLAGS:state", BlockState.DECOMPRESSION },
            { "PublicText", Configuration.Decompression.ZONE_LABEL },
            { "FontColor", Configuration.Decompression.SIGN_FOREGROUND_COLOR },
            { "BackgroundColor", Configuration.Decompression.SIGN_BACKGROUND_COLOR },
            { "FontSize", 2.9f / Configuration.Decompression.FONTSIZE }
        });

        base.GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .ApplyConfig<IMyTextPanel>(new Dictionary<String, Object>()
        {
            { "FLAGS:state", BlockState.DECOMPRESSION },
            { "Images", Configuration.Decompression.SIGN_IMAGE },
            { "Enabled", true }
        });

        base.GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .ApplyConfig<IMySoundBlock>(new Dictionary<String, Object>
        {
            { "FLAGS:state", BlockState.DECOMPRESSION },
            { "SelectedSound", Configuration.Decompression.ALERT_SOUND },
            { "LoopPeriod", 3600 },
            { "Enabled", true },
            { "Play", true }
        });

        foreach (var group in GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone)).GroupBy(l => l.HasFunction(BlockFunction.LIGHT_WARNING)))
        {
            if (group.Key)
            {
                group.ApplyConfig<IMyLightingBlock>(new Dictionary<String, Object>()
                {
                    { "Color", Configuration.Decompression.LIGHT_COLOR },
                    { "BlinkIntervalSeconds", Configuration.Decompression.LIGHT_BLINK },
                    { "BlinkLength", Configuration.Decompression.LIGHT_DURATION },
                    { "BlinkOffset", Configuration.Decompression.LIGHT_OFFSET },
                    { "Enabled", true },
                });
            }
            else
            {
                group.ApplyConfig<IMyLightingBlock>(new Dictionary<String, Object>()
                {
                    { "Enabled", false }
                });
            }
        }
    }
    else
    {
        GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true)
            .ClearStates(BlockState.DECOMPRESSION);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .ClearStates(BlockState.DECOMPRESSION);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .ClearStates(BlockState.DECOMPRESSION);

        GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .ClearStates(BlockState.DECOMPRESSION);

        return;

        // TODO: Move to Program.ApplyBlockStates
        // Group doors by zoning, test each group once and restore only doors in passing zone groups.
        var doorGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK).GroupBy(d => d.GetZones());

        foreach (var group in doorGroups)
        {
            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }

        var doorSignGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR).GroupBy(d => d.GetZones());
        foreach (var group in doorSignGroups)
        {
            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }

        var signGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING).GroupBy(d => d.GetZones());
        foreach (var group in signGroups)
        {
            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }

        var soundBlockGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN).GroupBy(s => s.GetZones());
        foreach (var group in soundBlockGroups)
        {
            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }

        var lightGroups = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone)).GroupBy(d => d.GetZones());
        foreach (var group in lightGroups)
        {
            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }
    }
}

public static void StyleBattleStations(IMyTerminalBlock block)
{
    var door = block as IMyDoor;
    if (door != default(IMyDoor) && door.HasFunction(BlockFunction.DOOR_SECURITY))
    {
        door.ApplyConfig(new Dictionary<String, Object>()
            {
                { "Closed", true }
            });
    }

    var lcd = block as IMyTextPanel;
    if (lcd != default(IMyTextPanel))
    {
        if (lcd.HasFunction(BlockFunction.SIGN_BATTLE))
        {
            lcd.ApplyConfig(new Dictionary<String, Object>()
            {
                { "PublicText", Configuration.BattleStations.ZONE_LABEL },
                { "FontColor", Configuration.BattleStations.SIGN_FOREGROUND_COLOR },
                { "BackgroundColor", Configuration.BattleStations.SIGN_BACKGROUND_COLOR },
                { "FontSize", Configuration.BattleStations.FONTSIZE }
            });
        }

        if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
        {
            lcd.ApplyConfig(new Dictionary<String, Object>()
            {
                { "Images", Configuration.BattleStations.SIGN_IMAGE },
                { "Enabled", true }
            });
        }
    }

    var soundBlock = block as IMySoundBlock;
    if (soundBlock != default(IMySoundBlock))
    {
        if (soundBlock.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
        {
            soundBlock.ApplyConfig(new Dictionary<String, Object>
                {
                    { "SelectedSound", Configuration.BattleStations.ALERT_SOUND },
                    { "LoopPeriod", 3600 },
                    { "Enabled", true },
                    { "Play", true }
                });
        }
        else
        {
            soundBlock.ApplyConfig(new Dictionary<String, Object>()
            {
                { "Play", false }
            });
        }
    }

    var light = block as IMyLightingBlock;
    if (light != default(IMyLightingBlock))
    {
        if (light.HasFunction(BlockFunction.LIGHT_WARNING))
        {
            light.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "Color", Configuration.BattleStations.LIGHT_COLOR },
                    { "BlinkIntervalSeconds", Configuration.BattleStations.LIGHT_BLINK },
                    { "BlinkLength", Configuration.BattleStations.LIGHT_DURATION },
                    { "BlinkOffset", Configuration.BattleStations.LIGHT_OFFSET },
                    { "Enabled", true },
                });
        }
        else
        {
            light.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "Enabled", false }
                });
        }
    }
}

private void TestBattleStations()
{
    if (Me.HasConfigFlag("custom-states", "battle"))
    {
        GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
            .SetStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
            .SetStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
            .SetStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
            .SetStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.HasFunction(BlockFunction.LIGHT_WARNING))
            .SetStates(BlockState.BATTLESTATIONS);
    }
    else
    {
        GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
            .ClearStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
            .ClearStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
            .ClearStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
            .ClearStates(BlockState.BATTLESTATIONS);
        GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.HasFunction(BlockFunction.LIGHT_WARNING))
            .ClearStates(BlockState.BATTLESTATIONS);
    }
}

public static void StyleDecompression(IMyTerminalBlock block)
{

    var door = block as IMyDoor;
    if (door != default(IMyDoor) && door.HasFunction(BlockFunction.DOOR_AIRLOCK))
    {
        door.ApplyConfig(new Dictionary<String, Object>()
            {
                { "Locked", true }
            });
    }

    var lcd = block as IMyTextPanel;
    if (lcd != default(IMyTextPanel))
    {
        if (lcd.HasFunction(BlockFunction.SIGN_DOOR))
        {
            lcd.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "PublicText", Configuration.Decompression.ZONE_LABEL },
                    { "FontColor", Configuration.Decompression.SIGN_FOREGROUND_COLOR },
                    { "BackgroundColor", Configuration.Decompression.SIGN_BACKGROUND_COLOR },
                    { "FontSize", 2.9f / Configuration.Decompression.FONTSIZE }
                });
        }

        if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
        {
            lcd.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "Images", Configuration.Decompression.SIGN_IMAGE },
                    { "Enabled", true }
                });
        }
    }

    var soundBlock = block as IMySoundBlock;
    if (soundBlock != default(IMySoundBlock))
    {
        if (soundBlock.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
        {
            soundBlock.ApplyConfig(new Dictionary<String, Object>
            {
                { "SelectedSound", Configuration.Decompression.ALERT_SOUND },
                { "LoopPeriod", 3600 },
                { "Enabled", true },
                { "Play", true }
            });
        }
        else
        {
            soundBlock.ApplyConfig(new Dictionary<String, Object>()
            {
                { "Play", false }
            });
        }
    }

    var light = block as IMyLightingBlock;
    if (light != default(IMyLightingBlock))
    {
        if (light.HasFunction(BlockFunction.LIGHT_WARNING))
        {
            light.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "Color", Configuration.Decompression.LIGHT_COLOR },
                    { "BlinkIntervalSeconds", Configuration.Decompression.LIGHT_BLINK },
                    { "BlinkLength", Configuration.Decompression.LIGHT_DURATION },
                    { "BlinkOffset", Configuration.Decompression.LIGHT_OFFSET },
                    { "Enabled", true },
                });
        }
        else
        {
            light.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "Enabled", false }
                });
        }
    }
}

private void TestInteriorWeapons(String zone)
{
    var interiorTurrets = GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInZone(zone));

    if (interiorTurrets.Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
    {
        Output("Turret detected enemy in zone " + zone + "!");

        GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
            .SetStates(BlockState.INTRUDER1);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .SetStates(BlockState.INTRUDER1);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .SetStates(BlockState.INTRUDER1);

        GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .SetStates(BlockState.INTRUDER1);

        return;

        // TODO: Move to Program.ApplyBlockStates
        var doors = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true);
        if (doors.Any())
        {
            Output("Closing " + doors.Count() + " doors to zone " + zone + ".");

            doors.ApplyConfig<IMyDoor>(new Dictionary<String, Object>()
            {
                { "Closed", true }
            });
        }
        else
        {
            Output("ALERT: Zone " + zone + " cannot be sealed, no functional doors were found!");
        }

        base.GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .ApplyConfig<IMyTextPanel>(new Dictionary<String, Object>()
        {
            { "FLAGS:state", BlockState.INTRUDER1 },
            { "PublicText", Configuration.Intruder.ZONE_LABEL },
            { "FontColor", Configuration.Intruder.SIGN_FOREGROUND_COLOR },
            { "BackgroundColor", Configuration.Intruder.SIGN_BACKGROUND_COLOR },
            { "FontSize", 2.9f / Configuration.Intruder.FONTSIZE }
        });

        base.GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .ApplyConfig<IMyTextPanel>(new Dictionary<String, Object>()
        {
            { "FLAGS:state", BlockState.INTRUDER1 },
            { "Images", Configuration.Intruder.SIGN_IMAGE },
            { "Enabled", true }
        });

        base.GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .ApplyConfig<IMySoundBlock>(new Dictionary<String, Object>
        {
            { "FLAGS:state", BlockState.INTRUDER1 },
            { "SelectedSound", Configuration.Intruder.ALERT_SOUND },
            { "LoopPeriod", 3600 },
            { "Enabled", true },
            { "Play", true }
        });

    }
    else
    {
        GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
            .GroupBy(d => d.GetZones())
            .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInAnyZone(g.Key.ToArray()))
            .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER1);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .GroupBy(s => s.GetZones())
            .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInAnyZone(g.Key.ToArray()))
            .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER1);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .GroupBy(d => d.GetZones())
            .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
            .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER1);

        GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .GroupBy(d => d.GetZones())
            .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
            .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER1);

        return;

        // TODO: Move to Program.ApplyBlockStates

        // Group doors by zoning, test each group once and restore only doors in passing zone groups.
        var doorGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK).GroupBy(d => d.GetZones());
        foreach (var group in doorGroups)
        {
            foreach (var block in group)
            {
                block.ClearConfigFlag("state", BlockState.INTRUDER1);
            }

            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }

        var doorSignGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR).GroupBy(d => d.GetZones());
        foreach (var group in doorSignGroups)
        {
            foreach (var block in group)
            {
                block.ClearConfigFlag("state", BlockState.INTRUDER1);
            }

            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }

        var signGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING).GroupBy(d => d.GetZones());
        foreach (var group in signGroups)
        {
            foreach (var block in group)
            {
                block.ClearConfigFlag("state", BlockState.INTRUDER1);
            }

            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }

        var soundBlockGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN).GroupBy(s => s.GetZones());
        foreach (var group in soundBlockGroups)
        {
            foreach (var block in group)
            {
                block.ClearConfigFlag("state", BlockState.INTRUDER1);
            }

            if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
            {
                group.RestoreStates();
            }
        }
    }
}

public static void StyleIntruder(IMyTerminalBlock block)
{
    var door = block as IMyDoor;
    if (door != default(IMyDoor))
    {
        door.ApplyConfig(new Dictionary<String, Object>()
            {
                { "Closed", true }
            });
    }

    var lcd = block as IMyTextPanel;
    if (lcd != default(IMyTextPanel))
    {
        if (lcd.HasFunction(BlockFunction.SIGN_DOOR))
        {
            lcd.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "PublicText", Configuration.Intruder.ZONE_LABEL },
                    { "FontColor", Configuration.Intruder.SIGN_FOREGROUND_COLOR },
                    { "BackgroundColor", Configuration.Intruder.SIGN_BACKGROUND_COLOR },
                    { "FontSize", 2.9f / Configuration.Intruder.FONTSIZE }
                });
        }

        if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
        {
            lcd.ApplyConfig(new Dictionary<String, Object>()
                {
                    { "Images", Configuration.Intruder.SIGN_IMAGE },
                    { "Enabled", true }
                });
        }
    }

    var soundBlock = block as IMySoundBlock;
    if (soundBlock != default(IMySoundBlock))
    {
        if (soundBlock.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
        {
            soundBlock.ApplyConfig(new Dictionary<String, Object>
            {
                { "SelectedSound", Configuration.Intruder.ALERT_SOUND },
                { "LoopPeriod", 3600 },
                { "Enabled", true },
                { "Play", true }
            });
        }
        else
        {
            soundBlock.ApplyConfig(new Dictionary<String, Object>()
            {
                { "Play", false }
            });
        }
    }
}

public static void StyleRestore(IMyTerminalBlock block)
{
    block.RestoreState();
}

private void TestSensors(String zone)
{
    var sensors = GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInZone(zone) && s.DetectEnemy);

    if (sensors.Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
    {
        Output("Sensor detected enemy in zone " + zone + "!");

        GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
            .SetStates(BlockState.INTRUDER2);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .SetStates(BlockState.INTRUDER2);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .SetStates(BlockState.INTRUDER2);

        GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .SetStates(BlockState.INTRUDER2);
    }
    else
    {
        GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
            .GroupBy(d => d.GetZones())
            .Where(g => !GridTerminalSystem
                .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER2);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
            .GroupBy(d => d.GetZones())
            .Where(g => !GridTerminalSystem
                .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER2);

        GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
            .GroupBy(d => d.GetZones())
            .Where(g => !GridTerminalSystem
                .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER2);

        GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
            .GroupBy(d => d.GetZones())
            .Where(g => !GridTerminalSystem
                .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            .SelectMany(g => g)
            .ClearStates(BlockState.INTRUDER2);
    }
}

public static Boolean DebugMode = true;
public static Boolean EnableSingleTickCycle = false;

private const String VERSION = "2018-09-24, 21:45";

static readonly IOrderedEnumerable<StateStyler> StatePriority = new List<StateStyler>
{
    new StateStyler(1, BlockState.DECOMPRESSION, StyleDecompression),
    new StateStyler(2, BlockState.INTRUDER1, StyleIntruder),
    new StateStyler(3, BlockState.INTRUDER2, StyleIntruder),
    new StateStyler(3, BlockState.BATTLESTATIONS, StyleBattleStations)
}.OrderBy(s => s.Priority);

static class BlockFunction
{
    public static readonly String DOOR_AIRLOCK = "airlock";
    public static readonly String DOOR_SECURITY = "security";
    public static readonly String SIGN_DOOR = "doorsign";
    public static readonly String SIGN_BATTLE = "battle";
    public static readonly String SIGN_WARNING = "warnsign";
    public static readonly String SOUNDBLOCK_SIREN = "siren";
    public static readonly String LIGHT_WARNING = "warnlight";
}

static class BlockState
{
    public static readonly String BATTLESTATIONS = "battle";
    public static readonly String DECOMPRESSION = "decompression";
    public static readonly String INTRUDER1 = "intruder1"; // Turrets
    public static readonly String INTRUDER2 = "intruder1"; // Sensors
}

static class Configuration
{
    public static class Decompression
    {
        public static readonly String ZONE_LABEL = "DECOMPRESSION DANGER";
        public static readonly Color SIGN_FOREGROUND_COLOR = new Color(0, 0, 255);
        public static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
        public static readonly String SIGN_IMAGE = "Danger";
        public static readonly String ALERT_SOUND = "Alert 2";
        public static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;

        public static readonly Color LIGHT_COLOR = new Color(0, 0, 255);
        public static readonly Single LIGHT_BLINK = 3;
        public static readonly Single LIGHT_DURATION = 66.6f;
        public static readonly Single LIGHT_OFFSET = 0;
    }

    public static class Intruder
    {
        public static readonly String ZONE_LABEL = "INTRUDER ALERT";
        public static readonly Color SIGN_FOREGROUND_COLOR = new Color(255, 0, 0);
        public static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
        public static readonly String SIGN_IMAGE = "Warning";
        public static readonly String ALERT_SOUND = "Alert 1";
        public static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;

        public static readonly Color LIGHT_COLOR = new Color(255, 0, 0);
        public static readonly Single LIGHT_BLINK = 3;
        public static readonly Single LIGHT_DURATION = 50;
        public static readonly Single LIGHT_OFFSET = 0;
    }

    public static class BattleStations
    {
        public static readonly String ZONE_LABEL = "BATTLE STATIONS";
        public static readonly Color SIGN_FOREGROUND_COLOR = new Color(255, 0, 0);
        public static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
        public static readonly String ALERT_SOUND = "Alert 1";
        public static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;
        public static readonly String SIGN_IMAGE = "Warning";

        public static readonly Color LIGHT_COLOR = new Color(255, 0, 0);
        public static readonly Single LIGHT_BLINK = 3;
        public static readonly Single LIGHT_DURATION = 33.3f;
        public static readonly Single LIGHT_OFFSET = 0;
    }
}

class StateStyler
{
    public readonly Int32 Priority;
    public readonly String Key;
    public readonly Action<IMyTerminalBlock> Style;

    public StateStyler(Int32 priority, String key, Action<IMyTerminalBlock> style)
    {
        Priority = priority;
        Key = key;
        Style = style;
    }
}

}
// This template is intended for extension classes. For most purposes you're going to want a normal
// utility class.
// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
static class Extensions
{
    public static void ApplyConfig<T>(this T block, Dictionary<String, Object> configValues)
        where T : IMyTerminalBlock
    {
        block.SaveState();

        var door = block as IMyDoor;
        var textPanel = block as IMyTextPanel;
        var soundBlock = block as IMySoundBlock;

        foreach (var config in configValues)
        {
            switch (config.Key)
            {
                case "Closed":
                    if (door != default(IMyDoor))
                    {
                        if (Object.Equals(config.Value, true))
                        {
                            if (door.OpenRatio > 0)
                            {
                                door.Enabled = true;
                                door.CloseDoor();
                            }
                        }
                    }
                    break;
                case "Locked":
                    if (door != default(IMyDoor))
                    {
                        if (Object.Equals(config.Value, true))
                        {
                            if (door.OpenRatio > 0)
                            {
                                door.Enabled = true;
                                door.CloseDoor();
                            }
                            else
                            {
                                door.Enabled = false;
                            }
                        }
                        else
                        {
                            door.Enabled = true;
                        }
                    }
                    break;
                case "PublicText":
                    if (textPanel != default(IMyTextPanel))
                    {
                        textPanel.WritePublicText(config.Value.ToString());
                    }
                    break;
                case "PublicTitle":
                    if (textPanel != default(IMyTextPanel))
                    {
                        textPanel.WritePublicTitle(config.Value.ToString());
                    }
                    break;
                case "Images":
                    if (textPanel != default(IMyTextPanel))
                    {
                        textPanel.ClearImagesFromSelection();
                        textPanel.AddImagesToSelection(config.Value.ToString().Split(';').ToList());
                        textPanel.ShowTextureOnScreen();
                    }
                    break;
                case "Play":
                    if (soundBlock != default(IMySoundBlock))
                    {
                        if (Object.Equals(config.Value, true))
                        {
                            soundBlock.Play();
                        }
                        else
                        {
                            soundBlock.Stop();
                        }
                    }
                    break;
                default:
                    if (config.Value is Color)
                    {
                        block.SetValueColor(config.Key, (Color)config.Value);
                    }

                    else if (config.Value is Single)
                    {
                        block.SetValueFloat(config.Key, (Single)config.Value);

                    }

                    else if (config.Value is Boolean)
                    {
                        block.SetValueBool(config.Key, (Boolean)config.Value);
                    }

                    else
                    {
                        block.SetValue(config.Key, config.Value.ToString());
                    }

                    break;
            }
        }
    }

    public static void ApplyConfig<T>(this IEnumerable<T> blocks, Dictionary<String, Object> configValues)
        where T: IMyTerminalBlock
    {
        foreach (var block in blocks)
        {
            block.ApplyConfig(configValues);
        }
    }
}

static class IMyGridTerminalSystemExtensions
{
    public static List<T> GetBlocksOfType<T>(this IMyGridTerminalSystem grid, Func<T, Boolean> collect = null)
        where T : class, IMyTerminalBlock
    {
        var result = new List<T>();
        grid.GetBlocksOfType(result, collect);
        return result;
    }

    public static List<T> GetZoneBlocksByFunction<T>(this IMyGridTerminalSystem grid, String zone, String function, Boolean all = false)
        where T : class, IMyTerminalBlock
    {
        return grid.GetBlocksOfType<T>(p => (p.IsWorking || all) && p.IsInZone(zone) && p.HasFunction(function));
    }

    public static List<String> GetZones(this IMyGridTerminalSystem grid)
    {
        return grid.GetBlocksOfType<IMyTerminalBlock>().SelectMany(b => b.GetZones()).Distinct().ToList();
    }

    public static Boolean AdjacentZonesTest<T>(this IMyGridTerminalSystem grid, Func<T, Boolean> test, params String[] zones)
        where T : class, IMyTerminalBlock
    {
        return grid.GetBlocksOfType<T>(v => v.IsInAnyZone(zones)).All(test);
    }

    public static List<MyDetectedEntityInfo> GetDetectedEntities(this IMySensorBlock sensorBlock, Func<MyDetectedEntityInfo, Boolean> collect = null)
    {
        var result = new List<MyDetectedEntityInfo>();
        sensorBlock.DetectedEntities(result);

        if (collect == null)
        {
            return result;
        }
        else
        {
            return result.Where(collect).ToList();
        }
    }
}

static class IMyTerminalBlockCconfigExtensions
{
    public static Boolean HasConfigFlag(this IMyTerminalBlock block, String key, params String[] values)
    {
        return values.Any(value => block.GetConfigs(key).Contains(value));
    }

    public static void SetConfigFlag(this IMyTerminalBlock block, String key, String value)
    {
        var values = block.GetConfigs(key);

        if (!values.Contains(value))
        {
            values.Add(value);
        }

        block.SetConfigs(key, values);
    }

    public static void ClearConfigFlag(this IMyTerminalBlock block, String key, String value)
    {
        var values = block.GetConfigs(key);

        if (values.Contains(value))
        {
            values.Remove(value);
        }

        block.SetConfigs(key, values);
    }

    public static IEnumerable<IMyTerminalBlock> SetStates(this IEnumerable<IMyTerminalBlock> blocks, params String[] states)
    {
        foreach (var block in blocks)
        {
            var blockStates = block.GetConfigs("state");
            blockStates.AddRange(states);

            block.SetConfigs("state", blockStates.Distinct());
        }

        return blocks;
    }

    public static IEnumerable<IMyTerminalBlock> ClearStates(this IEnumerable<IMyTerminalBlock> blocks, params String[] states)
    {
        foreach (var block in blocks)
        {
            var blockStates = block.GetConfigs("state");
            blockStates.RemoveAll(s => states.Contains(s));

            block.SetConfigs("state", blockStates.Distinct());
        }

        return blocks;
    }

    public static List<String> GetConfigs(this IMyTerminalBlock block, String key, Char denominator = ';')
    {
        return block.GetConfigs<String>(key, denominator);
    }

    public static List<T> GetConfigs<T>(this IMyTerminalBlock block, String key, Char denominator = ';')
    {
        return block.GetConfig(key).Split(denominator).Select(c => (T)Convert.ChangeType(c, typeof(T))).ToList();
    }

    public static T GetConfig<T>(this IMyTerminalBlock block, String key)
    {
        var value = block.GetConfig()[key];
        if (String.IsNullOrWhiteSpace(value))
        {
            return default(T);
        }
        else
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }

    public static String GetConfig(this IMyTerminalBlock block, String key, Boolean multiline = false)
    {
        var value = block.GetConfig()[key];

        if (String.IsNullOrWhiteSpace(value))
        {
            value = "";
        }

        if (multiline)
        {
            value = value.Replace("#N#", "\n");
        }

        return value;
    }

    public static void SetConfig(this IMyTerminalBlock block, String key, Object value, Boolean multiline = false)
    {
        var textvalue = value.ToString();

        if (multiline)
        {
            textvalue = textvalue.Replace("\n", "#N#");
        }

        var lines = block.CustomData.Split('\n').ToList();
        var found = false;

        for (var l = 0; l < lines.Count; l++)
        {
            var parts = lines[l].Split(':');
            if (parts.Length > 1 && parts[0] == key)
            {
                lines[l] = key + ":" + textvalue;
                found = true;
                break;
            }
        }

        if (found == false)
        {
            lines.Add(key + ":" + textvalue);
        }

        block.CustomData = String.Join("\n", lines);
    }

    public static void SetConfigs(this IMyTerminalBlock block, String key, IEnumerable<Object> values, Char denominator = ';')
    {
        var value = String.Join(denominator.ToString(), values.Select(v => v.ToString()));

        block.SetConfig(key, value);
    }

    public static Dictionary<String, String> GetConfig(this IMyTerminalBlock block)
    {
        var config = new Dictionary<String, String>();

        var lines = block.CustomData.Split('\n').Where(l => l.Contains(':'));
        foreach (var line in lines)
        {
            var split = line.Split(':');
            config.Add(split.First(), String.Join(":", split.Skip(1)));
        }

        return config;
    }
}

static class IMyTerminalBlockSelectorExtensions
{
    public static List<String> GetFunctions(this IMyTerminalBlock block)
    {
        return block.GetConfigs("functions");
    }

    public static Boolean HasFunction(this IMyTerminalBlock block, String function)
    {
        return block.GetFunctions().Contains(function);
    }

    public static List<String> GetZones(this IMyTerminalBlock block)
    {
        return block.GetConfigs("zones");
    }

    public static Boolean IsInZone(this IMyTerminalBlock block, String zone)
    {
        return block.GetZones().Contains(zone);
    }

    public static Boolean IsInAnyZone(this IMyTerminalBlock block, params String[] zones)
    {
        return block.GetZones().Any(z => zones.Contains(z));
    }

    public static Boolean IsInAllZones(this IMyTerminalBlock block, params String[] zones)
    {
        return block.GetZones().All(z => zones.Contains(z));
    }
}

static class IMyTerminalBlockState
{
    /// <summary>
        /// Save the current configuration of the block into <see cref="IMyTerminalBlock.CustomData"/>, if no previous save was done.
        /// </summary>
    public static void SaveState(this IMyTerminalBlock block)
    {
        if (block.GetConfig<Boolean>("saved"))
            return;

        var properties = new List<ITerminalProperty>();
        block.GetProperties(properties, p => p.Id != "CustomData");

        foreach (var property in properties)
        {
            switch (property.TypeName)
            {
                case "bool":
                    block.SetConfig(property.Id, property.AsBool().GetValue(block) ? "true" : "false");
                    break;
                case "float":
                    block.SetConfig(property.Id, property.AsFloat().GetValue(block));
                    break;
                case "color":
                    block.SetConfig(property.Id, property.AsColor().GetValue(block).PackedValue);
                    break;
                default:
                    var value = property.As<String>().GetValue(block);
                    block.SetConfig(property.Id, value);
                    break;
            }
        }

        var lcd = block as IMyTextPanel;
        if (lcd != default(IMyTextPanel))
        {
            lcd.SetConfig("PublicText", lcd.GetPublicText(), true);
            lcd.SetConfig("PublicTitle", lcd.GetPublicTitle());

            var images = new List<String>();
            lcd.GetSelectedImages(images);
            lcd.SetConfigs("SelectedImages", images);
            lcd.SetConfig("ShowText", lcd.ShowText);
        }

        //var soundBlock = block as IMySoundBlock;
        //if (soundBlock != default(IMySoundBlock))
        //{
        //    soundBlock.SetConfig("IsPlaying", soundBlock.IsPlaying);
        //}

        block.SetConfig("saved", true);
    }

    /// <summary>
        /// Restore the configuration of the block from <see cref="IMyTerminalBlock.CustomData"/>, if previous
        /// <see cref="SaveState(IMyTerminalBlock)"/> was done.
        /// </summary>
    public static void RestoreState(this IMyTerminalBlock block)
    {
        if (block.GetConfig<Boolean>("saved"))
            return;

        var properties = new List<ITerminalProperty>();
        block.GetProperties(properties, p => p.Id != "CustomData");

        foreach (var property in properties)
        {
            switch (property.TypeName)
            {
                case "bool":
                    block.SetValueBool(property.Id, block.GetConfig<Boolean>(property.Id));
                    break;
                case "float":
                    block.SetValueFloat(property.Id, block.GetConfig<Single>(property.Id));
                    break;
                case "color":
                    block.SetValueColor(property.Id, new Color(block.GetConfig<UInt32>(property.Id)));
                    break;
                default:
                    block.SetValue(property.Id, block.GetConfig(property.Id));
                    break;
            }
        }

        var lcd = block as IMyTextPanel;
        if (lcd != default(IMyTextPanel))
        {
            lcd.WritePublicText(lcd.GetConfig("PublicText"), true);
            lcd.WritePublicTitle(lcd.GetConfig("PublicTitle"));

            var images = lcd.GetConfigs("SelectedImages");

            lcd.ClearImagesFromSelection();
            lcd.AddImagesToSelection(images);

            if (lcd.GetConfig<Boolean>("ShowText"))
            {
                lcd.ShowPublicTextOnScreen();
            }
            else
            {
                lcd.ShowTextureOnScreen();
            }
        }
    }

    /// <summary>
        /// Saves the current configuration of all blocks in the collection to their respective <see cref="IMyTerminalBlock.CustomData"/>,
        /// if no previous save was done on the block.
        /// </summary>
    public static IEnumerable<IMyTerminalBlock> SaveStates(this IEnumerable<IMyTerminalBlock> blocks)
    {
        foreach (var block in blocks)
        {
            block.SaveState();
        }

        return blocks;
    }

    /// <summary>
        /// Restores the configuration of all blocks in the collection from their respective <see cref="IMyTerminalBlock.CustomData"/>,
        /// if previous <see cref="SaveState(IMyTerminalBlock)"/> was done on the block.
        /// </summary>
    public static IEnumerable<IMyTerminalBlock> RestoreStates(this IEnumerable<IMyTerminalBlock> blocks)
    {
        foreach (var block in blocks)
        {
            block.RestoreState();
        }

        return blocks;
    }
# SE-ShipSystemsManager
A Space Engineers script to automate block states, such as doors, lights and sound blocks based on certain conditions (such as decompression or enemies detected).

The script takes a snapshot of the state of all blocks which it has control over before the first time it edits them, allowing it to restore the blocks to default state when all states are cleared.

# Concepts
## Zones
Each block is marked as being in zero or more "zones", these are arbitrary grouped blocks on the Grid and don't specifically have to be in the same room.

Zones are specified in the Custom Data of a block with the following format:
```zones=zone-1;zone-2;zone-3```

## Functions
Each block has one or more functions, which depend on the block type.

### Doors
* `airlock` This door will be **sealed** (closed and disabled) during a decompression event in any zone it is assigned to.
* `security` This door will be **closed** during an intruder alert or battle stations callout.

### LCD Displays
* `doorsign` This specifies the LCD above a door, and will change text to read BATTLE STATIONS, DECOMPRESSION or other similar text messages.
* `battle` This specifies a large LCD which will display a graphic during a BATTLE STATIONS state.
* `warnsign` This specifies a large LCD which will display an appropriate graphic during any state which provides on.

### Sound Blocks
* `siren` These sound blocks will play a sound loop appropriate to the current ship state.

### Lights
* `warnlight` These lights will change color and blink depending on the current ship state.
* **Special Case**: During many ship state triggers, any lights not marked with a `warnlight` state will be disabled.

## States
Each input block (currently Air Vents, Sensors, Interior Turrets and manual input) will trigger an effect on the blocks in it's grouping.

Air Vents will mark a zone as depressurized, Sensors and Turrets will trigger an intruder alert and manual input can be used to trigger or clear battle stations.

Additional custom states can be added later.

## Properties
Depending on the function of a block, one or more properties will be changed depending on state. For example, during a battle stations state, all lights will be turned off, except those explicited flagged with the `warnlight` function.

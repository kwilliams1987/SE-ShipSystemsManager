
# Space Engineers: Ship Systems Manager
A Space Engineers script to automate block states, such as doors, lights and sound blocks based on certain conditions (such as decompression or enemies detected).

The script takes a snapshot of the state of all blocks which it has control over before the first time it edits them, allowing it to restore the blocks to default state when all states are cleared.

The script is completely automated, polling every 10 frames.
*It is possible, but not recommended, to run the script faster or slower.*
*Doing so will either cause excess performance impact, or reduce the speed at which the script can react to emergencies.*
## Special Thanks
Big shout out to [Malware](https://github.com/malware-dev) for his [Space Engineers Visual Studio Developers Kit](https://github.com/malware-dev/MDK-SE).

Without this I'd have never had the patience to go though with such an ambitious project!

# Work in Progress
Note that this script is still very, very work in progress and probably doesn't work at all yet.

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
* `warnsign` This specifies a large LCD which will display an appropriate graphic during any state which provides one.

### Sound Blocks
* `siren` These sound blocks will play a sound loop appropriate to the current ship state.

### Lights
* `warnlight` These lights will change color and blink style depending on the current ship state.
* **Special Case**: During many ship state triggers, any lights not marked with a `warnlight` state will be disabled.

## States
Each input block (currently Air Vents, Sensors, Interior Turrets and manual input) will trigger an effect on the blocks in it's grouping.

Air Vents will mark a zone as depressurized, Sensors and Turrets will trigger an intruder alert and manual input can be used to trigger or clear battle stations.

Additional custom states can be added later.

## Programmable Block Arguments
The Programmable Block can be sent arguments to affect it's behaviour.
Running the script with no arguments is the same as a normal "Tick" cycle.
### activate [state]
This will set a custom state on the block.
* `activate battle` will enable the "Battle Stations" state.
* `activate destruct` will start the "Self Destruct" system.
### deactivate [state]
This will clear a custom state on the block, for example `deactivate battle` will cancel the "Battle Stations" alerts.
### toggle [state]
This will intelligently swap between `activate [state]` and `deactivate [state]`, depending on it's current condition.
### customize preserve
This will write out the default values for all state conditions to the Programmable Block's Custom Data for you to edit, without overwriting any existing values.
### customize overwrite
This will write out the default values for all state conditions, but reset any customization to it's default value.
### customize reset
This will clear all Custom Data from the Programmable Block and revert to using the values stored in code.

## Properties
Depending on the function of a block, one or more properties will be changed depending on state. For example, during a battle stations state, all lights will be turned off, except those explicitly flagged with the `warnlight` function.
### Customization
When values exist in the Programmable Block's Custom Data with the correct key, the script will try to use these values instead, Otherwise default values in code are used.
**Note:** Setting invalid values for these custom properties may cause the script to fail.

## Basic Example
Empty world with Air-tightness and Programmable Blocks enabled.

A simple grid in space with:
* 1x Battery
* 1x Programmable Block
* 1x Oxygen Source
* 2x Airtight, each with:
  * 1x air-vent connected to the oxygen source.
  * 1x door leading into space.
  * 2x Light.

Add a single door between the two airtight rooms.

Set the custom data of each block as follows:
* Air Vent (Room 1):
  * `zones=room-1`
* Door External (Room 1):
  * `zones=room-1`
  * `functions=airlock`
* Light 1 (Room 1):
  * `zones=room-1`
  * `functions=warnlight`
* Light 2 (Room 1):
	* `zones=room-1`
* Air Vent (Room 2):
  * `zones=room-2`
* Door External (Room 2):
  * `zones=room-2`
  * `functions=airlock` 
* Light 1 (Room 2):
  * `zones=room-2`
  * `functions=warnlight`
* Light 2 (Room 2):
	* `zones=room-2`
* Door Between:
  * `zones=room-1;room-2`
  * `functions=airlock`

Load the script into the programmable block and start it.

Upon opening either external door:
* The script will change the warning light in the attached room to an alert state (a blue blink effect) and attempt to close and lock all doors to stop the leak.
* The none-warning light will be disabled.
* Once Oxygen pressure is restored
	* The doors "unlock" (IE are re-enabled)
	* All the lights are restored to the default white, on, solid state.

Open the middle door, and then delete an external door:
* Both rooms will enter alert state as above, with the two remaining doors being sealed.
* Once the "safe" room is re-pressurized:
	*  It's external door will unlock
	*  The door between the two rooms will remain disabled (it's still counted as in a depressurized zone)
*  The depressurized room's lights remain in alert state until the decompression is fixed.

## Debugging
If you're having issues with the script, you can check the Programmable Block debug output.

You can also add a Text Panel to the current Grid with `function=debug lcd` in it's Custom Data for more verbose information.

﻿/*
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
 * = Supported Functions =
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
 * = Custom States = 
 * Custom states are applied directly to the programmable block and have unique, grid-wide effects.
 *
 * Custom states are triggered by running the block with the following syntax:
 * activate [state] - Enables the custom state.
 * deactivate [state] - Disables the custom state.
 * toggle [state] - Switches between enabled and disabled.
 * 
 * Available States:
 * battle:
 * - Sounds battle stations and closes all security doors on the Grid.
 * - All door signs will read "BATTLE STATIONS".
 * destruct:
 * - Sounds battle stations, closes all security doors and sets a 3 minute countdown on all Warheads on the current Grid tagged with the "selfdestruct" function.
 * - All warheads will have their detonation time resynced to the lowest current value each tick.
 * - All door signs will read "SELF DESTRUCT {time remaining}" until the ship destroys itself.
 */
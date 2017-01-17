using UnityEngine;
using System.Collections;

public class EventCode {

	public enum AsyncEventCode
    {
        LEVEL_START, // Logs the Start of the Level -- Format -- [TIMESTAMP, EVENTCODE, EXPID, USRID, LVLID] -- DONE
        LEVEL_FINISH, // Log the End of the Level -- Format -- [TIMESTAMP, EVENTCODE, EXPID, USRID, LVLID, ENDEVENT] -- TO DO
        PLAYER_DIES, // Log when the Player Dies -- Format -- [TIMESTAMP, EVENTCODE, MONSTERKILLERID, ROOMID, TILEID] -- DONE
        PLAYER_NEW_ROOM, // Log when the Player Enters new Room -- Format -- [TIMESTAMP, EVENTCODE, NEWROOMID, OLDROOMID, TILEID] -- DONE
        PLAYER_KEYPRESS, // Log when a key is pressed -- Format -- [TIMESTAMP, EVENTCODE, KEYCODE, ROOMID, TILEID] -- DONE
        PLAYER_KEYUNPRESS, // Log when a key is unpressed -- Format -- [TIMESTAMP, EVENTCODE, KEYCODE, ROOMID, TILEID] -- DONE
        PLAYER_RESETSPAWN, // Log when the player presses the reset button -- Format -- [TIMESTAMP, EVENTCODE] -- DONE
        ENEMY_DAMAGE, // Log when the Player takes Damage -- Format -- [TIMESTAMP, EVENTCODE, MONSTERDAMAGEID, PLAYER_ROOMID, PLAYER_TILEID, MONSTER_ROOMID, MONSTER_TILEID, DAMAGEVALUE, CURRENTPLAYERHEALTH] -- DONE
        ENEMY_CHASE,  // Log when the monster chases the player -- Format -- [TIMESTAMP, EVENTCODE, MONSTERID, PLAYER_ROOMID, PLAYER_TILEID, MONSTER_ROOMID, MONSTER_TILEID] -- DONE
        ENEMY_PATROL, // Log when the monster stops chasing the player -- Format -- [TIMESTAMP, EVENTCODE, MONSTERID, PLAYER_ROOMID, PLAYER_TILEID, MONSTER_ROOMID, MONSTER_TILEID] -- DONE
        ENEMY_SIGHT, // Log when the monsters gains sight of the player -- Format -- [TIMESTAMP, EVENTCODE, MONSTERID, PLAYER_ROOMID, PLAYER_TILEID, MONSTER_ROOMID, MONSTER_TILEID] -- DONE
        ENEMY_LOSESIGHT, // Log when the monsters loses sight of the player -- Format -- [TIMESTAMP, EVENTCODE, MONSTERID, PLAYER_ROOMID, PLAYER_TILEID, MONSTER_ROOMID, MONSTER_TILEID] -- DONE
        ENEMY_PATROLSOUND, // Log when the monsters plays the patrol growl -- Format -- [TIMESTAMP, EVENTCODE, MONSTERID, PLAYER_ROOMID, PLAYER_TILEID, MONSTER_ROOMID, MONSTER_TILEID] -- DONE
        SOUND_TRIGGERED // Log when a 3D Sound is triggered -- Format -- [TIMESTAMP, EVENTCODE, 3DSOUNDID, SOUNDNAME, ROOMID, TILEID] -- DONE
    }

    public enum SyncEventCode
    {
        PLAYER_UPDATE_STATUS, // Once a Global Update Status is sent, the player object reply's with it's update status.
        MONSTER_UPDATE_STATUS, // Once a Global Update Status is sent, the monster objects reply with their update statuses.
        LEVEL_START,
        LEVEL_FINISH
    }

    /// <summary>
    /// Used by the EventDataBase ONLY. Serves as a General Update Code to query ObjectLogger Objects. The EventDataBase
    /// sends a specific code for synchronization queries, allowing for each object to respond accordingly with a SyncEventCode.
    /// </summary>
    public enum GlobalUpdateEventCode
    {
        // This is to allow further extensibility in case we want to send different types of updates to each object, just in case...
        GLOBAL_UPDATE_STATUS // This serves as a general update query for each object logger to return an update on their status
    }
}

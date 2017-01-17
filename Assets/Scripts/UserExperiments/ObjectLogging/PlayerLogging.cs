using UnityEngine;
using System.Collections;
using System;

public class PlayerLogging : MonoBehaviour, ObjectLogger {

    private Vector3 currentPos;
    private int currentTile;
    private int currentRoom;
    private int frames;
    private int currentPlayerHealth;
    private static int LOG_PER_FRAME = 10; // Change the amount of check every X frames 
    private int numOfDeath; // How many times has the player died.

    private EventDataBase globalData;
    private LevelBuilder lvlBuild;
    private PlayerHealth playerHealth;
    private PlayerRespawn playerRespawn;
    private PlayerUI playerUI;

	// Use this for initialization
	void Start () {

        // Load External Classes (Outside of the Player Object)
        globalData = FindObjectOfType(typeof(EventDataBase)) as EventDataBase;
        lvlBuild = FindObjectOfType(typeof(LevelBuilder)) as LevelBuilder;

        // Load the Internal Classes (Contained in the Player Object)
        playerHealth = GetComponent<PlayerHealth>();
        playerRespawn = GetComponent<PlayerRespawn>();
        playerUI = GetComponent<PlayerUI>();

        currentPos = GetComponent<Transform>().position;
        currentTile = -1; // initilization 
        currentRoom = -1;
        frames = 0;
        this.numOfDeath = 0;

        checkCurrentTile();

        // Connect this Logger to the Main Logging Object -- THIS IS REQUIRED FOR ALL LOGGER TYPES
        registerLoggerToEventLogger();

        // Log Start
	}
	
	// Update is called once per frame
	void Update () {

        if(EventDataBase.startLogging)
        {
            currentPos = GetComponent<Transform>().position;
            frames++;
            if (frames % LOG_PER_FRAME == 0)
            { //If the remainder of the current frame divided by 10 is 0 run the function.
                asyncEventChecker();
            }

            // KEY DOWN LOGGER FUNCTIONS
            if (Input.GetKeyDown(KeyCode.W))
            {
                logKeyDown(KeyCode.W);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                logKeyDown(KeyCode.A);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                logKeyDown(KeyCode.S);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                logKeyDown(KeyCode.D);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                logKeyDown(KeyCode.F);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                logKeyDown(KeyCode.LeftShift);
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                logPlayerReset();
            }

            // KEY UP LOGGER FUNCTIONS
            if (Input.GetKeyUp(KeyCode.W))
            {
                logKeyUp(KeyCode.W);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                logKeyUp(KeyCode.A);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                logKeyUp(KeyCode.S);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                logKeyUp(KeyCode.D);
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                logKeyUp(KeyCode.F);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                logKeyUp(KeyCode.LeftShift);
            }
        }
        //if (frames % 60 == 0)
        //{ //If the remainder of the current frame divided by 10 is 0 run the function.
        //    keyPressEvent();
        //}
    }

    // Log when a key is pressed -- Format -- [TIMESTAMP, EVENTCODE, KEYCODE, ROOMID, TILEID]
    void logKeyDown(KeyCode k)
    {
        if(EventDataBase.startLogging)
        {
            string[] str = { k.ToString(), Convert.ToString(currentRoom), Convert.ToString(currentTile) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.PLAYER_KEYPRESS, str));
        }
    }

    // Log when a key is unpressed -- Format -- [TIMESTAMP, EVENTCODE, KEYCODE, ROOMID, TILEID]
    void logKeyUp(KeyCode k)
    {
        if(EventDataBase.startLogging)
        {
            string[] str = { k.ToString(), Convert.ToString(currentRoom), Convert.ToString(currentTile) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.PLAYER_KEYUNPRESS, str));
        }
    }

    // Log when a key is unpressed -- Format -- [TIMESTAMP, EVENTCODE, ROOMID, TILEID]
    void logPlayerReset()
    {
        if (EventDataBase.startLogging)
        {
            string[] str = { };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.PLAYER_RESETSPAWN, str));
        }
    }

    void checkCurrentTile()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(currentPos,Vector3.down,out hit))
        {
            string[] t_name = hit.collider.gameObject.name.Split('_');

            if(t_name[0] == "tile")
            {
                currentTile = Convert.ToInt32(t_name[1]);
                checkCurrentRoom();
            }
        }
        
    }

    // [TIMESTAMP, EVENTCODE, NEWROOMID, OLDROOMID, TILEID]
    void checkCurrentRoom()
    {
        if (EventDataBase.startLogging)
        {
            int newRoom = lvlBuild.getRoomIDByTileID(currentTile);
            if (newRoom != currentRoom)
            {
                int oldRoom = currentRoom;
                currentRoom = newRoom;
                // Send Async Event
                string[] str = { Convert.ToString(currentRoom), Convert.ToString(oldRoom), Convert.ToString(currentTile) };
                asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.PLAYER_NEW_ROOM, str));
            }
        }
    }

    public void asyncLogger(AsyncEvent e)
    {
        globalData.sendAsyncSignal(e);
    }

    public SyncEvent syncLogger(EventCode.GlobalUpdateEventCode code)
    {
        return replyToSyncEvent(code);
    }

    public void registerLoggerToEventLogger()
    {
        globalData.registerLogger(this);
    }

    public void asyncEventChecker()
    {
        checkCurrentTile();
        checkPlayerHealth();
    }

    public void checkPlayerHealth()
    {
        if(currentPlayerHealth != playerHealth.getCurrentHealth())
        {
            // Player took Damage
            currentPlayerHealth = playerHealth.getCurrentHealth();

            // Send Async Event
            string[] str = { Convert.ToString(currentPlayerHealth) };
            //asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.PLAYER_DAMAGE, str));
        }
    }

    public void logPlayerDeath(int ghoulID)
    {
        if (EventDataBase.startLogging)
        {
            numOfDeath++;
            string[] str = { Convert.ToString(ghoulID), Convert.ToString(currentRoom), Convert.ToString(currentTile) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.PLAYER_DIES, str));
        }
    }

    public int getCurrentTile()
    {
        return currentTile;
    }

    public int getCurrentRoom()
    {
        return currentRoom;
    }

    public int getCurrentPlayerHealth()
    {
        return playerHealth.getCurrentHealth();
    }

    /// <summary>
    /// Sends a global Update on the status of the Player once a GLOBAL_UPDATE_STATUS event is received.
    /// Object Style : [TIMESTAMP, PLAYER_UPDATE_STATUS, CURRENT_ROOMID, CURRENT_TILEID, CURRENT_COORD_X, 
    /// CURRENT_COORD_Y, CURRENT_COORD_Z, CURRENT_HEALTH, NUMBER_OF_DEATHS]
    /// </summary>
    /// <returns></returns>
    public SyncEvent globalUpdateEvent()
    {
        string[] str = { Convert.ToString(currentRoom), Convert.ToString(currentTile),
            Convert.ToString(currentPos.x),  Convert.ToString(currentPos.y), Convert.ToString(currentPos.z),
            Convert.ToString(currentPlayerHealth), Convert.ToString(numOfDeath)};
        return new SyncEvent(EventCode.SyncEventCode.PLAYER_UPDATE_STATUS, str);
    }

    /// <summary>
    /// Function that determines what type of reply to send once a sync event is received from the EventDataBase.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public SyncEvent replyToSyncEvent(EventCode.GlobalUpdateEventCode e)
    {
        if(e == EventCode.GlobalUpdateEventCode.GLOBAL_UPDATE_STATUS)
        {
            return globalUpdateEvent();
        }
        else
        {
            return null;
        }
    }

}

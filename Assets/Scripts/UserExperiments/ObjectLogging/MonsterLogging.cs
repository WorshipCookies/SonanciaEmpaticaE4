using UnityEngine;
using System.Collections;
using System;

public class MonsterLogging : MonoBehaviour, ObjectLogger
{
    private Vector3 currentPos;
    private int currentTile;
    private int currentRoom;
    private int frames;
    private int currentPlayerHealth;
    private static int LOG_PER_FRAME = 20; // Change the amount of check every X frames
    private int numOfKills;


    private bool playerInSight;

    private EventDataBase globalData;
    private PlayerLogging playerLog;
    private LevelBuilder lvlBuild;

    private AstarAI enemyAI;
    private AILineOfSight enemyLineOfSight;

    // Use this for initialization
    void Start()
    {
        // Load External Classes (Outside of the Monster Object)
        globalData = FindObjectOfType(typeof(EventDataBase)) as EventDataBase;
        lvlBuild = FindObjectOfType(typeof(LevelBuilder)) as LevelBuilder;
        playerLog = FindObjectOfType(typeof(PlayerLogging)) as PlayerLogging;

        // Load Internal Classes (Inside the Monster Object)
        enemyAI = GetComponent<AstarAI>();
        enemyLineOfSight = GetComponent<AILineOfSight>();


        currentPos = GetComponent<Transform>().position;
        currentTile = -1; // initilization 
        currentRoom = -1;
        frames = 0;
        this.numOfKills = 0;

        this.playerInSight = enemyLineOfSight.playerInSight;

        checkCurrentTile();

        registerLoggerToEventLogger();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventDataBase.startLogging)
        {
            currentPos = GetComponent<Transform>().position;
            frames++;
            if (frames % LOG_PER_FRAME == 0)
            { //If the remainder of the current frame divided by 10 is 0 run the function.
                asyncEventChecker();
            }
        }
    }

    public void asyncLogger(AsyncEvent e)
    {
        globalData.sendAsyncSignal(e);
    }

    public void registerLoggerToEventLogger()
    {
        globalData.registerLogger(this);
    }

    public SyncEvent syncLogger(EventCode.GlobalUpdateEventCode code)
    {
        return replyToSyncEvent(code);
    }

    void checkCurrentTile()
    {
        RaycastHit hit;

        if (Physics.Raycast(currentPos, Vector3.down, out hit))
        {
            string[] t_name = hit.collider.gameObject.name.Split('_');

            if (t_name[0] == "tile")
            {
                currentTile = Convert.ToInt32(t_name[1]);
                checkCurrentRoom();
            }
        }

    }

    void checkCurrentRoom()
    {
        int newRoom = lvlBuild.getRoomIDByTileID(currentTile);
        if (newRoom != currentRoom)
        {
            int oldRoom = currentRoom;
            currentRoom = newRoom;
            // Send Async Event
            //string[] str = { Convert.ToString(currentRoom), Convert.ToString(oldRoom), Convert.ToString(currentTile) };
            //asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.PLAYER_NEW_ROOM, str));
        }
    }

    public void asyncEventChecker()
    {
        checkCurrentTile();
        checkEnemySight();
    }

    // [TIMESTAMP, EVENTCODE, MONSTERID, PLAYER_ROOMID, PLAYER_TILEID, MONSTER_ROOMID, MONSTER_TILEID]
    public void checkEnemySight()
    {
        if (EventDataBase.startLogging)
        {
            if (enemyLineOfSight.playerInSight != playerInSight)
            {
                playerInSight = enemyLineOfSight.playerInSight;
                // LOG IT

                string[] str = { Convert.ToString(enemyAI.getGhoulID()), Convert.ToString(playerLog.getCurrentRoom()),
                Convert.ToString(playerLog.getCurrentTile()), Convert.ToString(currentRoom),
                Convert.ToString(currentTile) };

                if (playerInSight)
                    asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.ENEMY_SIGHT, str));
                else
                    asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.ENEMY_LOSESIGHT, str));
            }
        }
    }

    public void logIsPatrolling()
    {
        if(EventDataBase.startLogging)
        {
            string[] str = { Convert.ToString(enemyAI.getGhoulID()), Convert.ToString(playerLog.getCurrentRoom()),
                Convert.ToString(playerLog.getCurrentTile()), Convert.ToString(currentRoom),
                Convert.ToString(currentTile) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.ENEMY_PATROL, str));
        }
    }

    public void logIsChasing()
    {
        if (EventDataBase.startLogging)
        {
            string[] str = { Convert.ToString(enemyAI.getGhoulID()), Convert.ToString(playerLog.getCurrentRoom()),
                Convert.ToString(playerLog.getCurrentTile()), Convert.ToString(currentRoom),
                Convert.ToString(currentTile) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.ENEMY_CHASE, str));
        }
    }

    public void logPlayerDamage(int value)
    {
        if (EventDataBase.startLogging)
        {
            // Keep track of how many times this monster kills the player
            if (playerLog.getCurrentPlayerHealth() == 0)
            {
                numOfKills++;
            }
            string[] str = { Convert.ToString(enemyAI.getGhoulID()), Convert.ToString(playerLog.getCurrentRoom()),
                Convert.ToString(playerLog.getCurrentTile()), Convert.ToString(currentRoom),
                Convert.ToString(currentTile), Convert.ToString(value), Convert.ToString(playerLog.getCurrentPlayerHealth()) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.ENEMY_DAMAGE, str));
        }
    }

    public void logPatrolSound()
    {
        if (EventDataBase.startLogging)
        {
            string[] str = { Convert.ToString(enemyAI.getGhoulID()), Convert.ToString(playerLog.getCurrentRoom()),
                Convert.ToString(playerLog.getCurrentTile()), Convert.ToString(currentRoom),
                Convert.ToString(currentTile) };
            asyncLogger(new AsyncEvent(EventCode.AsyncEventCode.ENEMY_PATROLSOUND, str));
        }
    }

    /// <summary>
    /// Sends a global Update on the status of the Monster once a GLOBAL_UPDATE_STATUS event is received.
    /// Object Style : [TIMESTAMP, MONSTER_UPDATE_STATUS, MONSTERID, CURRENT_ROOMID, CURRENT_TILEID,  
    /// CURRENT_COORD_X, CURRENT_COORD_Y, CURRENT_COORD_Z, IS_PATROLLING, IS_CHASING, NUMBER_OF_KILLS]
    /// </summary>
    /// <returns></returns>
    public SyncEvent globalUpdateEvent()
    {
        string[] str = { Convert.ToString(enemyAI.getGhoulID()), Convert.ToString(currentRoom),
            Convert.ToString(currentTile), Convert.ToString(currentPos.x), Convert.ToString(currentPos.y),
            Convert.ToString(currentPos.z), Convert.ToString(enemyAI.isPatrolling()),
            Convert.ToString(enemyAI.isChasing()), Convert.ToString(numOfKills)};
        return new SyncEvent(EventCode.SyncEventCode.MONSTER_UPDATE_STATUS, str);
    }

    /// <summary>
    /// Function that determines what type of reply to send once a sync event is received from the EventDataBase.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public SyncEvent replyToSyncEvent(EventCode.GlobalUpdateEventCode e)
    {
        if (e == EventCode.GlobalUpdateEventCode.GLOBAL_UPDATE_STATUS)
        {
            return globalUpdateEvent();
        }
        else
        {
            return null;
        }
    }
}

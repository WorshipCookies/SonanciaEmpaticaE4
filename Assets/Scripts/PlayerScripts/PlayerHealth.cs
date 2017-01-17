using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    private int playerHealth;
    private bool isDead;

    private PlayerRespawn spawn;

    private static int MAX_HEALTH = 3;
    private static int DEFAULT_DMG = 1;

    private PlayerLogging log;

    private PlayerUI playerUI;

    void Awake()
    {
        playerHealth = MAX_HEALTH;
        isDead = false;
        spawn = GetComponent<PlayerRespawn>();
        playerUI = GetComponent<PlayerUI>();
    }

    void Start()
    {
        log = GetComponent<PlayerLogging>();
    }

    void Update()
    {
        if (isDead)
        {
            Debug.Log("Player Has Died!");
            spawn.respawnPlayerStart();
            refillHealth();
        }
    }

    public void takeDamage(int dmg, int ghoulID)
    {
        playerUI.playerHit();
        playerHealth -= dmg;

        if(playerHealth <= 0)
        {
            isDead = true;
            log.logPlayerDeath(ghoulID);
        }

        Debug.Log("Current Health == " + playerHealth + " , By Monster " + ghoulID);
    }

    public void takeDamage()
    {
        playerUI.playerHit();
        playerHealth -= DEFAULT_DMG;
        if (playerHealth <= 0)
        {
            isDead = true;
        }

        Debug.Log("Current Health == " + playerHealth);
    }

    public void regainHealth(int health)
    {
        int newHealth = playerHealth + health;
        playerHealth = Mathf.Min(MAX_HEALTH, newHealth);
    }
	
    public void regainHealth()
    {
        int newHealth = playerHealth + DEFAULT_DMG;
        playerHealth = Mathf.Min(MAX_HEALTH, newHealth);
    }

    public bool playerIsDead()
    {
        return isDead;
    }

    public void refillHealth()
    {
        playerHealth = MAX_HEALTH;
        isDead = false;
    }

    public int getCurrentHealth()
    {
        return playerHealth;
    }
}

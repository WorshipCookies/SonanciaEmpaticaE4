using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    private bool playerAtObjective;
    private Text endText;
    private Text beginText;

    private Image hurtScreen;
    private CanvasGroup hurtCanvas;

    private string objectiveText = "(Press Ctrl + P) To Finish the Level ...";

    private bool gameStarted;

    // Use this for initialization
    void Start () {

        playerAtObjective = false;
        Text[] textObjects = this.GetComponentInChildren<Canvas>().GetComponentsInChildren<Text>();
        endText = textObjects[0];
        endText.enabled = false;

        gameStarted = true;

        beginText = textObjects[1];
        beginText.enabled = true;

        hurtScreen = this.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>();
        hurtCanvas = this.GetComponentInChildren<Canvas>().GetComponentInChildren<CanvasGroup>(); ;
        hurtCanvas.alpha = 0f;

        hurtScreen.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (playerAtObjective && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Player at the Objective and Pressed P!");
            LevelBuilder.playerReachedEnd = true;
            //Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.P) && gameStarted)
        {
            beginText.enabled = false;
            gameStarted = false;
        }
	}

    public void setPlayerObjective(bool isAtObjective)
    {
        playerAtObjective = isAtObjective;
        endText.enabled = playerAtObjective;
    }

    public void playerHit()
    {
        flashWhenHit();
    }

    private void flashWhenHit()
    {
        StartCoroutine(Fade(0.2f, 0f, 0.5f));
    }

    private IEnumerator Fade(float start, float end, float length)
    {
        hurtCanvas.alpha = start;
        if (hurtCanvas.alpha == start)
        {
            for (float i = 0f; i < 1f; i += Time.deltaTime * (1 / length))
            { //for the length of time
                hurtCanvas.alpha = Mathf.Lerp(start, end, i); //lerp the value of the transparency from the start value to the end value in equal increments
                yield return new WaitForSeconds(.1f);
                hurtCanvas.alpha = end; // ensure the fade is completely finished (because lerp doesn't always end on an exact value)
            }
        }
    }
}

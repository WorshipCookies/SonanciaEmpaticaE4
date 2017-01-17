using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

    public float minFlickerSpeed = 0.1f;
    public float maxFlickerSpeed = 5.0f;

    bool canCallFunction = true;
    
    float counter = 0f;
    float currentMaxTime;

    Light light;

	// Use this for initialization
	void Start () {
        light = this.GetComponent<Light>();
        currentMaxTime = Random.Range(minFlickerSpeed, maxFlickerSpeed);
	}
	
	// Update is called once per frame
	void Update () {
        counter += Time.deltaTime;
        if (counter >= currentMaxTime)
        {
            flickLight();
            currentMaxTime = Random.Range(minFlickerSpeed, maxFlickerSpeed);
            counter = 0;
        }
	}


    void flickLight()
    {
        light.enabled = !light.enabled; 
    }
}

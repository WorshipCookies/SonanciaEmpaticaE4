using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIGraph : MonoBehaviour {
    public Texture2D pBlankMember;  // the blank "paper", assigned in Inspector
    public Texture2D pMember;  // the "paper" to draw on, assigned in Inspector
    public Texture2D tMember;  // the symbol to draw, assigned in Inspector
 
    float pXMin; // x axis min value
    float pXMax; // x axis max value
    float pYMin; // y axis min value
    float pYMax; // y axis max value
    float pXOffset;  // x axis origin offset
    float pYOffset;  // y axis origin offset
    List<float> pPlotXRange = new List<float>();  // min/max
    List<float> pPlotYRange = new List<float>();  // min/max
    float pPlotXScale;  // size of the bitmap
    float pPlotYScale;  // size of the bitmap
    float pNewXScale;  // adjusted scale
    float pNewYScale;  // adjusted scale
    string pDot;  // for graphs with more than one symbol
    int pNumDots;  // number of symbols in graph
    int pCurrentDot;  // always draws dots in order
    float halfWidth;  // half size of symbol
    float halfHeight;  // half size of symbol
 
    Rect dRect;
    static float gHCenter;
    Color[] dotColors;
    int xx;
    int yy;


	// Use this for initialization
	void Start () {
        InitGraph();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
 
//----------------------------------------------
 
    void InitGraph () {
   
        pXMin = 0.0f;
        pXMax = 0.575f;
        pYMin = 0.0f;
        pYMax = 0.575f;
        pXOffset = 0.0f;
        pYOffset = 0.0f;
 
        SetXRange(pXMin, pXMax);
        SetYRange(pYMin, pYMax);
 
        pPlotXScale = pMember.width - pXOffset;
        pPlotYScale = pMember.height - pYOffset;
        SetupScale();
 
        pCurrentDot = 1;
        dotColors = pBlankMember.GetPixels(0);
        pMember.SetPixels(dotColors, 0);
        pMember.Apply();
        GetComponent<Renderer>().material.mainTexture = pMember;
    }
 
    //----------------------------------------------
 
    void DrawGraph (List<Vector2> theData, string theDot, int numDots) {
 
        dotColors = pBlankMember.GetPixels(0);
        pMember.SetPixels(dotColors, 0);
   
        pDot = theDot;
        pNumDots = numDots;
        pCurrentDot = 1;
        SetupScale();
        for (var i=0; i<theData.Count; i++) {
            PlotPoint(theData[i].x, theData[i].y);
        }
        pMember.Apply();   
        GetComponent<Renderer>().material.mainTexture = pMember;
    }
 
//----------------------------------------------
 
    void PlotPoint (float xx, float yy) {
     
        yy = pPlotYScale - Mathf.Round((yy * pNewYScale) - (pPlotYRange[0] * pNewYScale));
 
        xx = Mathf.Round((xx * pNewXScale) - (pPlotXRange[0] * pNewXScale)) + pXOffset;
        gHCenter = xx;
       
        halfWidth = Mathf.Round(tMember.width / 2.0f) ;
        halfHeight = Mathf.Round(tMember.height / 2.0f);
        dRect = new Rect(xx - halfWidth, yy - halfHeight, tMember.width, tMember.height);
   
        dotColors = tMember.GetPixels(0);
        pMember.SetPixels((int)(xx - halfWidth), (int)(yy - halfHeight), tMember.width, tMember.height, dotColors);
   
        pCurrentDot++;
        if (pCurrentDot > pNumDots) {
            pCurrentDot = 1;
        }  
    }
 
//----------------------------------------------
 
    void SetXRange (float tMin, float tMax) {
        pPlotXRange.Clear();
        pPlotXRange.Add(tMin);
        pPlotXRange.Add(tMax);  
    }
 
//----------------------------------------------
 
    void SetYRange (float tMin, float tMax) {  
        pPlotYRange.Clear();
        pPlotYRange.Add(tMin);
        pPlotYRange.Add(tMax);  
    }
 
    // must call before plotting first point if anything has changed since InitGraph
 
    void SetupScale () {
        pNewXScale = pPlotXScale/(pPlotXRange[1] - pPlotXRange[0]);
        pNewYScale = pPlotYScale/(pPlotYRange[1] - pPlotYRange[0]);
    }
}

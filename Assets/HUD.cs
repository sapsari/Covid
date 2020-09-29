using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public UnityEngine.UI.Text TextHealthy;
    public UnityEngine.UI.Text TextInfected;

    public int CountHealthy;
    public int CountInfected;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TextHealthy.text =  $"Healthy:  {CountHealthy}";
        TextInfected.text = $"Infected: {CountInfected}";
    }
}

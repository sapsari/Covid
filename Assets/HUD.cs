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
        // fix camera here
        var area = SpawnerSystem.Area;

        var y = Camera.main.transform.position.y;
        Camera.main.transform.position = new Vector3(area.x / 2, y, area.y / 2);
        Camera.main.orthographicSize = Mathf.Max(area.x, area.y) / 2 * 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        //TextHealthy.text =  $"Healthy:  {CountHealthy}";
        //TextInfected.text = $"Infected: {CountInfected}";

        TextHealthy.text = CountHealthy.ToString();
        TextInfected.text = CountInfected.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text TextHealthy;
    public Text TextInfected;
    public Text TextFPS;

    public Canvas CanvasGame;
    public Canvas CanvasBegin;
    public Canvas CanvasEnd;


    DrawLine drawLine;

    // Start is called before the first frame update
    void Start()
    {
        // fix camera here
        var area = SpawnerSystem.Area;

        var y = Camera.main.transform.position.y;
        Camera.main.transform.position = new Vector3(area.x / 2, y, area.y / 2);
        Camera.main.orthographicSize = Mathf.Max(area.x, area.y) / 2 * 1.2f;

        drawLine = GameObject.FindObjectOfType<DrawLine>();

        gameStartTime = Time.time;
    }

    int previousHealthy;
    float previousHealthyTime;

    float gameStartTime;

    // Update is called once per frame
    void Update()
    {
        //TextHealthy.text =  $"Healthy:  {CountHealthy}";
        //TextInfected.text = $"Infected: {CountInfected}";

        var lifetime = World.DefaultGameObjectInjectionWorld.GetExistingSystem<LifeTimeSystem>();
        var healthy = lifetime.HealthyCount;
        var infected = lifetime.InfectedCount;

        if (Time.time - gameStartTime > Constants.TickDelayTime * 2 &&
            healthy == previousHealthy)
        {
            var dt = Time.time - previousHealthyTime;
            if (dt > Constants.GameEndingSeconds)
                ResetSim();
        }
        else
        {
            previousHealthyTime = Time.time;
            previousHealthy = healthy;
        }


        TextHealthy.text = healthy.ToString();
        TextInfected.text = infected.ToString();

        TextFPS.text = ((int)Mathf.Floor(1 / Time.deltaTime)).ToString();
    }

    bool isEnding;

    public void ResetSim()
    {
        isEnding = !isEnding;

        var spawner = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SpawnerSystem>();

        if (isEnding)
        {
            drawLine.EndSim();
            spawner.EndSim();
        }
        else
        {
            drawLine.StartSim();
            spawner.StartSim();

            gameStartTime = Time.time;
        }
    }

    public void Restart()
    {

    }

    public void ToggleMask()
    {

    }
}

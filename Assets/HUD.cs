using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


public enum GameState { Splash, Intro, Game, Score }

public class HUD : MonoBehaviour
{
    public Canvas CanvasGame;
    public Canvas CanvasIntro;
    public Canvas CanvasEnd;

    public Text TextHealthy;
    public Text TextInfected;
    public Text TextFPS;
    public Text TextScore;

    public Image ImageMaskOn;
    public Image ImageMaskOff;

    DrawLine drawLine;

    GameState state;

    bool wearingMask;


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
        
        CanvasEnd.enabled = false;
        CanvasGame.enabled = false;
        CanvasIntro.enabled = true;
        state = GameState.Intro;

        ToggleMask();
    }

    int previousHealthy;
    float previousHealthyTime;

    float gameStartTime;

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Intro)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Restart();
            }
        }

        if (state == GameState.Game)
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
                    EndSim();
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

    public void EndSim()
    {
        TextScore.text = TextHealthy.text;

        CanvasIntro.enabled = false;
        CanvasGame.enabled = false;
        CanvasEnd.enabled = true;

        state = GameState.Score;
    }

    public void Restart()
    {
        var spawner = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SpawnerSystem>();

        drawLine.EndSim();
        spawner.EndSim();

        spawner.SetIsWearingMask(wearingMask);
        
        drawLine.StartSim();
        spawner.StartSim();



        gameStartTime = Time.time;

        CanvasGame.enabled = true;
        CanvasEnd.enabled = false;
        CanvasIntro.enabled = false;

        state = GameState.Game;
    }

    public void ToggleMask()
    {
        wearingMask = !wearingMask;

        ImageMaskOff.enabled = !wearingMask;
        ImageMaskOn.enabled = wearingMask;
    }
}

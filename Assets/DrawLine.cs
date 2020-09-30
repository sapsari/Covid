using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public GameObject LinePrefab;

    LineRenderer lineRenderer;
    //public List<Vector3> fingerPositions;
    Vector3 lastPos;


    Camera cam;
    Plane plane;

    public List<LineRenderer> Lines;

    internal NativeMultiHashMap<int, float4> hashmap;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        plane = GetPlane();

        hashmap = new NativeMultiHashMap<int, float4>(4, Allocator.Persistent);
    }

    static Plane GetPlane()
    {
        var p = GameObject.Find("Plane");
        var filter = p.GetComponent<MeshFilter>();
        Vector3 normal = Vector3.up;

        if (filter && filter.mesh.normals.Length > 0)
            normal = filter.transform.TransformDirection(filter.mesh.normals[0]);

        var plane = new Plane(normal, p.transform.position);
        return plane;
    }

    Vector3 GetPosition222()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.point;
        else
            return Vector3.negativeInfinity;
    }

    Vector3 GetPosition()
    {
        //Create a ray from the Mouse click position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        //Initialise the enter variable
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);

            //Move your cube GameObject to the point where you clicked
            //m_Cube.transform.position = hitPoint;
            return hitPoint;
        }
        else
            return Vector3.positiveInfinity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }

        if (Input.GetMouseButton(0))
        {
            //var newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            var newPos = GetPosition();

            //Debug.Log("pos:" + newPos);

            //if (Vector3.Distance(newPos, fingerPositions[fingerPositions.Count - 1]) > 1.05f)
            if (Vector3.Distance(newPos, lastPos) > 1.05f)
            {
                UpdateLine(newPos);
            }
        }
        else
        {
            if (lineRenderer != null)
            {
                FinishLine();
                lineRenderer = null;
            }
        }
    }

    void FinishLine()
    {

        // skip first point on purpose
        for (int i = 1; i < lineRenderer.positionCount - 1; i++)
        {
            var p1 = lineRenderer.GetPosition(i);
            var p2 = lineRenderer.GetPosition(i + 1);

            var mid = (p1 + p2) * .5f;

            var hash = LifeTimeSystem.GetPositionHash(mid);
            var value = new float4(p1.x, p1.z, p2.x, p2.z);

            hashmap.Add(hash, value);

            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, 0, 1), value);
            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, 0, -1), value);
            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, 1, 0), value);
            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, 1, 1), value);
            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, 1, -1), value);
            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, -1, 0), value);
            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, -1, 1), value);
            hashmap.Add(LifeTimeSystem.GetPositionHash(mid, -1, -1), value);
        }
    }

    void CreateLine()
    {
        var currentLine = Instantiate(LinePrefab);

        lineRenderer = currentLine.GetComponent<LineRenderer>();
        //lineRenderer.set

        var pos = GetPosition();
        lineRenderer.SetPosition(0, pos);
        lineRenderer.SetPosition(1, pos);
        lastPos = pos;

        /*
        fingerPositions.Clear();
        //fingerPositions.Add(cam.ScreenToWorldPoint(Input.mousePosition));
        //fingerPositions.Add(cam.ScreenToWorldPoint(Input.mousePosition));
        fingerPositions.Add(GetPosition());
        fingerPositions.Add(GetPosition());

        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);*/

        Lines.Add(lineRenderer);
    }

    void UpdateLine(Vector3 newPos)
    {
        //fingerPositions.Add(newPos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
        lastPos = newPos;

    }
}

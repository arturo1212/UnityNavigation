using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Triangulation : MonoBehaviour {

    List<GameObject> cubes = new List<GameObject>();

    // Use this for initialization
    void Start () {
        // List<Vector3> centers = CenterCalculator();
        // DrawCubes(centers, Color.blue);
    }

    void ClearField()
    {
        // Realizar una limpieza de los centros calculados.
        foreach(GameObject cube in cubes)
        {
            Destroy(cube);
        }
        cubes.Clear();
    }

    List<Vector3> CentroidPerTriangle(List<Triangle> triangles)
    {
        List<Vector3> centers = new List<Vector3>();
        foreach(Triangle t in triangles)
        {
            centers.Add(t.GetCentroid());
        }
        return centers;
    }

    List<Vector3> MidPointPerTriangle(List<Triangle> triangles)
    {
        List<Vector3> centers = new List<Vector3>();
        foreach (Triangle t in triangles)
        {
            List<Vector3> mids = t.GetMidPoints();
            foreach(Vector3 m in mids)
            {
                centers.Add(m);
            }
        }
        return centers;
    }

    void DrawCircles(List<Vector3> centers, float radius, Color color)
    {
        foreach(Vector3 center in centers)
        {
            Vector3 prevPos = center + new Vector3(radius, 0, 0);
            for (int i = 0; i < 30; i++)
            {
                float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
                Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Debug.DrawLine(prevPos, newPos, color);
                prevPos = newPos;
            }
        }

    }

    void DrawCubes(List<Vector3> positions, Color color)
    {
        foreach(Vector3 p in positions)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<Renderer>().material.color = color;
            cube.transform.position = p;
            cube.transform.localScale = new Vector3(0.1f, 1, 0.1f);
            //Destroy(cube);
            cubes.Add(cube);
        }
    }

    // Obtener todos los triangulos del NavMesh de Unity
    List<Triangle> GetTriangles(Vector3[] vertices, int[] polygons)
    {
        int aux = 0;
        List<Triangle> triangles = new List<Triangle>();
        foreach (int index in polygons)
        {
            if(aux%3 == 0)
            {
                triangles.Add(new Triangle());
            }
            triangles.Last<Triangle>().corners.Add(vertices[index]);
            aux++;
        }
        foreach(Triangle t in triangles)
        {
            t.Update();
        }
        return triangles;
    }

    // Update is called once per frame
    Vector3[] vertices;
    List<Vector3> centers;
    void Update () {
        //ClearField();

        // Calcular Triangulos utilizando el navmesh de Unity.
        var navMesh = NavMesh.CalculateTriangulation();
        int[] polygons = navMesh.indices;
        vertices = navMesh.vertices;
        List<Triangle> triangles = GetTriangles(vertices, polygons);
        print("#Triangulos: " + triangles.Count);

        // Calcular nodos a utilizar en el grafo.
        //centers = CentroidPerTriangle(triangles);
        centers = MidPointPerTriangle(triangles);
        DrawCircles(centers, .05f, Color.blue);

        // (DEBUG TESTING)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Vector3 clickedPos = Input.mousePosition;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                float min_dist = 1000;
                Triangle best_T = null;
                foreach (Triangle t in triangles)
                {
                    float dist = t.PointInside(hit.point);
                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        best_T = t;
                    }
                }
                DrawCubes(best_T.corners, Color.red);
            }
        }
        // DrawCubes(new List<Vector3>(vertices), Color.red);
    }
}

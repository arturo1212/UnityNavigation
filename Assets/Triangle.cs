using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle {
    public List<Vector3> corners;
    public List<Side> Sides;
    public List<Triangle> Neighbors;
    public Vector3 MassCenter;

    public Triangle()
    {
        corners = new List<Vector3>();
    }

    // Permite actualizar la informacion del poligono al agregar vertices y crear lados
    public void Update()
    {
        GetSides();

        // Calculo del centro de masa
        MassCenter = new Vector3(0f, 0, 0f);
        foreach( Vector3 corner in corners)
        {
            MassCenter += corner;
        }
        MassCenter = MassCenter / corners.Count;
    }

    // Permite obtener el centroide del poligono
    public Vector3 GetCentroid()
    {
        Vector3 result = new Vector3(0f, 0f, 0f);
        foreach(Vector3 c in corners)
        {
            result = result + c;
        }
        result = result / corners.Count;
        return result;
    }

    // Permite obtener los lados del poligono.
    public List<Side> GetSides()
    {
        List<Side> result = new List<Side>();
        for (int i = 1; i < corners.Count; i++)
        {
            Side side = new Side() { start = corners[i - 1], end = corners[i] };

            // Verificacion de cercania con obstaculos. (NO AGREGAR AL GRAFO).
            side.mid = new Vector3(side.mid.x, 0.7f, side.mid.z);
            Collider[] coll = Physics.OverlapSphere(side.mid, 0.1f);
            if (coll.Length < 1)
            {
                result.Add(side);
                Debug.DrawLine(side.start, side.end, Color.green);
            }
        }

        // Agregar lado faltante para cerrar el poligono.
        Side s = new Side() { start = corners[0], end = corners[corners.Count - 1] };
        s.mid = new Vector3(s.mid.x, 0.7f, s.mid.z);
        Collider[] colls = Physics.OverlapSphere(s.mid, 0.1f);
        if (colls.Length < 1)
        {
            result.Add(s);
            Debug.DrawLine(s.start, s.end, Color.green);
        }

        /* Dibujar lados (DEBUG) */
        for(int i = 0; i<result.Count; i++)
        {
            for(int j = i + 1; j<result.Count; j++)
            {
                Debug.DrawLine(result[i].mid, result[j].mid, Color.red);
            }
        }

        Sides = result;
        return result;
    }


    public List<Vector3> GetMidPoints()
    {
        List<Vector3> result = new List<Vector3>();
        foreach(Side side in Sides)
        {
            result.Add(side.mid);
        }
        return result;
    }

    public float PointInside(Vector3 p)
    {
        float DesiredSum = 0;
        foreach(Vector3 corner in corners)
        {
            Vector3 aux = new Vector3(corner.x, 0, corner.z);
            DesiredSum += Vector3.Distance(MassCenter, aux);
        }

        float ActualSum = 0;
        foreach (Vector3 corner in corners)
        {
            Vector3 aux = new Vector3(corner.x, 0, corner.z);
            ActualSum += Vector3.Distance(p, corner);
        }
        var result = Mathf.Abs(DesiredSum - ActualSum);
        return result;

    }

    public bool PointInTriangle(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
        var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

        if ((s < 0) != (t < 0))
            return false;

        var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;

        return A < 0 ?
                (s <= 0 && s + t >= A) :
                (s >= 0 && s + t <= A);
    }
}

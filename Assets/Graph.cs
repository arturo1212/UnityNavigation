using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {

    // Dado un lado, obtener los dados adyacentes.
    public Dictionary<Side, List<Side>> SideToTriangleCalculator(List<Triangle> Triangles)
    {
        Dictionary<Side, List<Side>> result = new Dictionary<Side, List<Side>>();
        foreach (Triangle t in Triangles)
        {
            var Sides = t.GetSides();
            foreach (Side side in Sides)
            {
                // Si el lado no ha sido visitado, agregar la clave al diccionario
                if (!result.ContainsKey(side))
                {
                    result = new Dictionary<Side, List<Side>>();
                }

                // Agregar todos los lados del triangulo excepto a si mismo
                result[side].AddRange(Sides);
            }
        }
        return result;
    }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		//SideToTriangleCalculator(triangles);
        // Triangle -> Sides
        // Side -> List<Side> (Usar diccionario) 
	}
}

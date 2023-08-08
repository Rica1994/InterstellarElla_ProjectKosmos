
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using PositionUnits = Cinemachine.CinemachinePathBase.PositionUnits;

// reference -> https://mokapants.hatenablog.com/entry/2021/06/09/213702

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GenerateMeshSmoothPath : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath smoothPath;
    [SerializeField] private float width;
    [SerializeField] private Material meshMaterial;

    private const PositionUnits Units = PositionUnits.PathUnits;

    private void OnValidate()
    {
        Generate();
    }



    private void Generate()
    {
        var mesh = new Mesh();

        // vertices are added as -> (waypoint 0 (1/20), left vertex; right vertex; waypoint 0 (2/20), left vertex; right vertex;...
        mesh.vertices = CalcAllVertices(); 

        mesh.triangles = CalcTriangles(mesh.vertices.Length); 
        mesh.uv = CalcUvsKoen(mesh.vertices); // UV
        mesh.RecalculateNormals();


        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;


        var renderer = GetComponent<Renderer>();
        renderer.material = meshMaterial;
    }




    private Vector3[] CalcAllVertices()
    {
        var allVertices = new List<Vector3>();
        //Debug.Log("calcing vertices on " + this.gameObject.name);
        for (var part = 0; part < smoothPath.m_Waypoints.Length; part++)
        {
            //Debug.Log("-------- waypoint " + part + "--------");
            allVertices = allVertices.Concat(CalcVerticesPart(part)).ToList();
        }

        return allVertices.ToArray();
    }



    private IEnumerable<Vector3> CalcVerticesPart(int part)
    {
        var vertices = new List<Vector3>();

        //Debug.Log(smoothPath.DistanceCacheSampleStepsPerSegment + " <-----");

        for (var i = 0; i < smoothPath.DistanceCacheSampleStepsPerSegment; i++)
        {
            var pos = part + (float)i / smoothPath.DistanceCacheSampleStepsPerSegment;
            var point = smoothPath.EvaluatePositionAtUnit(pos, Units);
            var localPoint = transform.InverseTransformPoint(point);
            var rot = smoothPath.EvaluateOrientationAtUnit(pos, Units);
            var r = (rot * Vector3.right) * width / 2;

            vertices.Add(localPoint - r);
            vertices.Add(localPoint + r);

            //var worldPos = transform.TransformPoint(localPoint - r);
            //var worldPos2 = transform.TransformPoint(localPoint + r);

            //Debug.Log(worldPos + " at Step " + i);
            //Debug.Log(worldPos2 + " at Step " + i);
        }

        return vertices;
    }




    private int[] CalcTriangles(int verticesLength)
    {
        var triangles = new List<int>();

        for (var pointNum = 0; pointNum < verticesLength - 2; pointNum += 2)
        {
            triangles.Add(pointNum);
            triangles.Add(pointNum + 2);
            triangles.Add(pointNum + 1);
            triangles.Add(pointNum + 1);
            triangles.Add(pointNum + 2);
            triangles.Add(pointNum + 3);
        }

        return triangles.ToArray();
    }



    private Vector2[] CalcUvsKoen(Vector3[] vertices)
    {
        // vertices are added as -> (waypoint 0 (1/20), left vertex; right vertex; waypoint 0 (2/20), left vertex; right vertex;...
        var vertexUVArray = new Vector2[vertices.Length];

        // put i=0 on Vector2(0,0)
        // put i=1 on Vector2(1,0)
        // put i=2 on Vector2(0,1)
        // put i=3 on Vector2(1,1)

        // make sure we start at 0 after the first increment
        float checkI = -1f;

        for (var i = 0; i < vertexUVArray.Length; i++)
        {
            checkI += 1;

            // reset checkI once 4 vertices have been checked
            if (checkI > 3)
            {
                checkI = 0f;
            }


            if (checkI == 0)
            {
                vertexUVArray[i] = new Vector2(0, 0);
                //Debug.Log("positioned 0,0 with vertext " + i);
            }
            else if (checkI == 1)
            {
                vertexUVArray[i] = new Vector2(1, 0);
                //Debug.Log("positioned 1,0 with vertext " + i);
            }
            else if (checkI == 2)
            {
                vertexUVArray[i] = new Vector2(0, 1);
                //Debug.Log("positioned 0,1 with vertext " + i);
            }
            else 
            {
                vertexUVArray[i] = new Vector2(1, 1);
                //Debug.Log("positioned 1,1 with vertext " + i);
            }
        }

        return vertexUVArray;
    }

    private Vector2[] CalcUvs(Vector3[] vertices)
    {
        // vertices are added as -> (waypoint 0 (1/20), left vertex; right vertex; waypoint 0 (2/20), left vertex; right vertex;...
        var vertexUVArray = new Vector2[vertices.Length];

        for (var i = 0; i < vertexUVArray.Length; i++)
        {
            vertexUVArray[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        return vertexUVArray;
    }
}

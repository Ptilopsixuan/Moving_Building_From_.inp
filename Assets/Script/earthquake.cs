using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class earthquake : MonoBehaviour
{
    //public Material preMaterial;

    private List<MyClass.Building> buildings;
    //private static List<List<Vector3>> displacement;
    private DataTable displacement;
    private static int n = 0;
    private static bool move = true;
    private static Color[] colors;
    //private static Vector2[] vec = new Vector2[]
    //{Vector2.one, Vector2.one, Vector2.one, Vector2.one, Vector2.one, Vector2.one};

    private void Start()
    {

    }
    private void OnEnable()
    {
        n = 0;
        buildings = model.buildings;
        colors = new Color[3];
        colors[0] = Color.blue;
        colors[1] = Color.green;
        colors[2] = Color.red;

        //Debug.Log("buildings' count: " + buildings.Count);

        StartCoroutine(repeatEverySecond(0.05f));//every 2 seconds repeat this function
    }
    IEnumerator repeatEverySecond(float interval)
    {
        while (move)//Loop the time steps
        {
            //Debug.Log("LoopOut");
            foreach (MyClass.Building building in buildings)//Loop the buildings
            {
                //Debug.Log("LoopMid");
                Vector3[] positions = building.pos;//maintain the original position
                //int countNode = building.count;
                displacement = building.displacement;

                //for (int node = 0; node < countNode; node++)//Loop the nodes in one building
                //{
                //    //Debug.Log("LoopIn");
                //    positions[node].x += Convert.ToSingle(displacement.Rows[node][n]) / 1000;
                //    //    newPos[node] = originalPos[node] + displacement[n][node];
                //}

                GameObject[] conns = GameObject.FindGameObjectsWithTag("line");
                // Gradient gradient; // Define your gradient object
                Gradient gradient = new Gradient();

                // Define color stops
                GradientColorKey[] colorKeys = new GradientColorKey[3];
                colorKeys[0].color = Color.green;
                colorKeys[0].time = 0.0f;
                colorKeys[1].color = Color.yellow;
                colorKeys[1].time = 0.01f;
                colorKeys[2].color = Color.red;
                colorKeys[2].time = 0.02f;

                // Define alpha keys (optional)
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0].alpha = 1.0f;
                alphaKeys[0].time = 0.0f;
                alphaKeys[1].alpha = 0.0f;
                alphaKeys[1].time = 1.0f; // Fade out at the end

                gradient.SetKeys(colorKeys, alphaKeys);

                for (int i = 0; i < conns.Length ; i++)//literate the connections
                {
                    LineRenderer line = conns[i].GetComponent<LineRenderer>();
                    int node1Index = (int)building.conn[i][0];
                    int node2Index = (int)building.conn[i][1];
                    float groundmove = Convert.ToSingle(displacement.Rows[0][2 * n]) / 100;
                    float move1 = Convert.ToSingle(displacement.Rows[node1Index][2 * n]) / 100;
                    float move2 = Convert.ToSingle(displacement.Rows[node2Index][2 * n]) / 100;
                    Vector3 pos1 = positions[node1Index];
                    Vector3 pos2 = positions[node2Index];
                    pos1.x += move1 - groundmove;
                    pos2.x += move2 - groundmove;
                    line.SetPositions(new Vector3[2] { pos1, pos2 });

                    float maxDisplacement = 500 / 100;
                    float displacementRatio = Math.Abs(((move1 + move2) / 2) - groundmove) / maxDisplacement;
                    if (i == 100) {
                        print(displacementRatio);
                    }
                    Color lineColor = gradient.Evaluate(displacementRatio);
                    line.GetComponent<LineRenderer>().material.color = lineColor;

                    //if ((move1 + move2) / 2 > 1.5)
                    //{ conns[i].GetComponent<LineRenderer>().material.color = colors[1]; }
                    //if ((move1 + move2) / 2 > 3)
                    //{ conns[i].GetComponent<LineRenderer>().material.color = colors[2]; }
                    //else { conns[i].GetComponent<LineRenderer>().material.color = colors[(n / 1000) % 3]; }
                }
                
                GameObject[] shears = GameObject.FindGameObjectsWithTag("shear4");
                for (int i = 0; i < shears.Length; i++)//literate the shears
                {
                    MeshRenderer Mesh = shears[i].GetComponent<MeshRenderer>();
                    int node1Index = (int)building.s4r[i][0];
                    int node2Index = (int)building.s4r[i][1];
                    int node3Index = (int)building.s4r[i][2];
                    int node4Index = (int)building.s4r[i][3];
                    float groundmove = Convert.ToSingle(displacement.Rows[0][2 * n]) / 100;
                    float move1 = Convert.ToSingle(displacement.Rows[node1Index][2 * n]) / 100;
                    float move2 = Convert.ToSingle(displacement.Rows[node2Index][2 * n]) / 100;
                    float move3 = Convert.ToSingle(displacement.Rows[node3Index][2 * n]) / 100;
                    float move4 = Convert.ToSingle(displacement.Rows[node4Index][2 * n]) / 100;
                    Vector3 pos1 = positions[node1Index];
                    Vector3 pos2 = positions[node2Index];
                    Vector3 pos3 = positions[node3Index];
                    Vector3 pos4 = positions[node4Index];
                    pos1.x += move1 - groundmove;
                    pos2.x += move2 - groundmove;
                    pos3.x += move3 - groundmove;
                    pos4.x += move4 - groundmove;
                    shears[i].GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { pos1, pos2, pos3, pos4 };

                    float maxDisplacement = 500 / 100;
                    float displacementRatio = Math.Abs(((move1 + move2 + move3 + move4) /4) - groundmove) / maxDisplacement;
                    if (i == 100)
                    {
                        print(displacementRatio);
                    }
                    Color lineColor = gradient.Evaluate(displacementRatio);
                    Mesh.material.color = lineColor;
                    //line.GetComponent<LineRenderer>().material.color = lineColor;
                    
                    //if ((move1 + move2) / 2 > 1.5)
                    //{ conns[i].GetComponent<LineRenderer>().material.color = colors[1]; }
                    //if ((move1 + move2) / 2 > 3)
                    //{ conns[i].GetComponent<LineRenderer>().material.color = colors[2]; }
                    //else { conns[i].GetComponent<LineRenderer>().material.color = colors[(n / 1000) % 3]; }
                }
            }

            n++;
            if (n >= displacement.Columns.Count/2) { move = false; }

            yield return new WaitForSeconds(interval);
        }
    }
}
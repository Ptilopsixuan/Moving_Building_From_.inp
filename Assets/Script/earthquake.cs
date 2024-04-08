using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class earthquake : MonoBehaviour
{
    private List<MyClass.Building> buildings;
    private DataTable displacement;

    private static int currentStep = 0;
    private static bool isMoving;

    private float UpdateInterval = 0.05f;

    //private GameObject curve;
    //private LineRenderer displacementCurve;

    public void StartEarthquakeSimulation()
    {
        currentStep = 0;
        isMoving = true;
        Gradient gradient = ColorBar();
        buildings = model.buildings;
        //Debug.Log("buildings' count: " + buildings.Count);
        StartCoroutine(repeatEverySecond(UpdateInterval, gradient));
        //every set seconds repeat this function
    }

    private IEnumerator repeatEverySecond(float interval, Gradient gradient)
    {
        while (isMoving)//Loop the time steps
        {
            //Debug.Log("LoopOut");
            foreach (MyClass.Building building in buildings)//Loop the buildings
            {
                //Debug.Log("LoopMid");
                Vector3[] positions = building.pos;//maintain the original position
                displacement = building.displacement;

                // Assuming building is a GameObject, you can access its Transform
                //Transform buildingTransform = building.original.transform; 
                // Find GameObjects with the tag "line" that are children of the building GameObject
                //GameObject[] conns = buildingTransform.fin

                Transform[] childObjects = building.original.transform.GetComponentsInChildren<Transform>();
                List<GameObject> conns = new List<GameObject>();
                List<GameObject> shear3s = new List<GameObject>();
                List<GameObject> shear4s = new List<GameObject>();
                foreach (Transform childObject in childObjects)
                {
                    if (childObject.CompareTag("line")) { conns.Add(childObject.gameObject); }
                    else if (childObject.CompareTag("shear3")) { shear3s.Add(childObject.gameObject); }
                    else if (childObject.CompareTag("shear4")) { shear4s.Add(childObject.gameObject); }
                }

                //GameObject[] conns = GameObject.FindGameObjectsWithTag("line");
                for (int i = 0; i < building.conn.Length; i++)//literate the connections
                {
                    LineRenderer line = conns[i].GetComponent<LineRenderer>();
                    int node1Index = (int)building.conn[i][0];
                    int node2Index = (int)building.conn[i][1];
                    //in following 3 lines, multiply by 2 to skip a column,
                    //devided by 100 to amplify displacements 100 times because of coordinates are devided by 1e4
                    float groundmove = Convert.ToSingle(displacement.Rows[0][2 * currentStep]) / 100;
                    float move1 = Convert.ToSingle(displacement.Rows[node1Index][2 * currentStep]) / 100;
                    float move2 = Convert.ToSingle(displacement.Rows[node2Index][2 * currentStep]) / 100;
                    Vector3 pos1 = positions[node1Index];
                    Vector3 pos2 = positions[node2Index];
                    pos1.x += move1 - groundmove;
                    pos2.x += move2 - groundmove;
                    line.SetPositions(new Vector3[2] { pos1, pos2 });

                    float maxDisplacement = 500 / 100;//500 is nearly maxima in this displacement data
                    float displacementRatio = Math.Abs(((move1 + move2) / 2) - groundmove) / maxDisplacement;

                    Color lineColor = gradient.Evaluate(displacementRatio);//
                    line.GetComponent<LineRenderer>().material.color = lineColor;
                }
                //same as the line
                //GameObject[] shear3s = GameObject.FindGameObjectsWithTag("shear3");
                for (int i = 0; i < building.s3r.Length; i++)//literate the shears
                {
                    MeshRenderer Mesh = shear3s[i].GetComponent<MeshRenderer>();
                    int node1Index = (int)building.s3r[i][0];
                    int node2Index = (int)building.s3r[i][1];
                    int node3Index = (int)building.s3r[i][2];
                    float groundmove = Convert.ToSingle(displacement.Rows[0][2 * currentStep]) / 100;
                    float move1 = Convert.ToSingle(displacement.Rows[node1Index][2 * currentStep]) / 100;
                    float move2 = Convert.ToSingle(displacement.Rows[node2Index][2 * currentStep]) / 100;
                    float move3 = Convert.ToSingle(displacement.Rows[node3Index][2 * currentStep]) / 100;
                    Vector3 pos1 = positions[node1Index];
                    Vector3 pos2 = positions[node2Index];
                    Vector3 pos3 = positions[node3Index];
                    pos1.x += move1 - groundmove;
                    pos2.x += move2 - groundmove;
                    pos3.x += move3 - groundmove;
                    shear3s[i].GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { pos1, pos2, pos3 };

                    float maxDisplacement = 500 / 100;
                    float displacementRatio = Math.Abs(((move1 + move2 + move3) / 3) - groundmove) / maxDisplacement;

                    Color meshColor = gradient.Evaluate(displacementRatio);
                    Mesh.material.color = meshColor;
                }
                //GameObject[] shear4s = GameObject.FindGameObjectsWithTag("shear4");
                for (int i = 0; i < building.s4r.Length; i++)//literate the shears
                {
                    MeshRenderer Mesh = shear4s[i].GetComponent<MeshRenderer>();
                    int node1Index = (int)building.s4r[i][0];
                    int node2Index = (int)building.s4r[i][1];
                    int node3Index = (int)building.s4r[i][2];
                    int node4Index = (int)building.s4r[i][3];
                    float groundmove = Convert.ToSingle(displacement.Rows[0][2 * currentStep]) / 100;
                    float move1 = Convert.ToSingle(displacement.Rows[node1Index][2 * currentStep]) / 100;
                    float move2 = Convert.ToSingle(displacement.Rows[node2Index][2 * currentStep]) / 100;
                    float move3 = Convert.ToSingle(displacement.Rows[node3Index][2 * currentStep]) / 100;
                    float move4 = Convert.ToSingle(displacement.Rows[node4Index][2 * currentStep]) / 100;
                    Vector3 pos1 = positions[node1Index];
                    Vector3 pos2 = positions[node2Index];
                    Vector3 pos3 = positions[node3Index];
                    Vector3 pos4 = positions[node4Index];
                    pos1.x += move1 - groundmove;
                    pos2.x += move2 - groundmove;
                    pos3.x += move3 - groundmove;
                    pos4.x += move4 - groundmove;
                    shear4s[i].GetComponent<MeshFilter>().mesh.vertices = new Vector3[] { pos1, pos2, pos3, pos4 };

                    float maxDisplacement = 500 / 100;
                    float displacementRatio = Math.Abs(((move1 + move2 + move3 + move4) / 4) - groundmove) / maxDisplacement;

                    Color meshColor = gradient.Evaluate(displacementRatio);
                    Mesh.material.color = meshColor;
                }

                //UpdateDisplacementCurve(2000);//2000 is the index of one node, can be changed
            }

            currentStep++;
            if (currentStep >= displacement.Columns.Count / 2) //200)//
            { isMoving = false; }

            yield return new WaitForSeconds(interval);
        }

        //recover to the original
        if (!isMoving)
        {
            GameObject[] conns = GameObject.FindGameObjectsWithTag("line");
            for (int i = 0; i < conns.Length; i++)//literate the connections
            {
                LineRenderer line = conns[i].GetComponent<LineRenderer>();
                line.GetComponent<LineRenderer>().material.color = Color.gray;
            }
            GameObject[] shear3s = GameObject.FindGameObjectsWithTag("shear3");
            for (int i = 0; i < shear3s.Length; i++)//literate the shears
            {
                shear3s[i].GetComponent<MeshRenderer>().material.color = Color.gray;
            }
            GameObject[] shear4s = GameObject.FindGameObjectsWithTag("shear4");
            for (int i = 0; i < shear4s.Length; i++)//literate the shears
            {
                shear4s[i].GetComponent<MeshRenderer>().material.color = Color.gray;
            }
            Debug.Log("Already back to original style.");
        }
    }

    private Gradient ColorBar()
    {
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
        return gradient;
    }

    //private void UpdateDisplacementCurve(int nodeIndex)
    //{
    //    //// Assuming you have the displacement data for the current step
    //    //float maxDisplacement = Convert.ToSingle(displacement.Rows[nodeIndex][2 * currentStep]) / 100; 
    //    //// Calculate max displacement here
    //    //float time = currentStep * UpdateInterval; // Assuming each step represents a time interval

    //    //Vector3 point = new Vector3(time, maxDisplacement * 5, 0f);

    //    //displacementCurve.transform.SetParent(curvePanel.transform, false);

    //    //displacementCurve.positionCount = currentStep + 1;
    //    //displacementCurve.SetPosition(currentStep, point);
    //    //displacementCurve = curve.AddComponent<LineRenderer>();
    //}
}

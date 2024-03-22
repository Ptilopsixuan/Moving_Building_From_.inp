using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Material preMaterial;
    private static int n = 0;

    private static List<List<Vector3>> displacement = new List<List<Vector3>>();
    private static List<Vector3> vec0 = new List<Vector3>();
    private static List<Vector3> vec1 = new List<Vector3>();
    private static List<Vector3> vec2 = new List<Vector3>();

    private void Awake()
    {
        vec0.Add(new Vector3(0, 1, 2));
        vec0.Add(new Vector3(0, -1, -2));
        vec1.Add(new Vector3(3, 4, 5));
        vec1.Add(new Vector3(-3, -4, -5));
        vec2.Add(new Vector3(6, 7, 8));
        vec2.Add(new Vector3(-6, -7, -8));

        displacement.Add(vec0);
        displacement.Add(vec1);
        displacement.Add(vec2);
    }

    private void Start()
    {
        StartCoroutine(repeatEverySecond(5f));
    }
    IEnumerator repeatEverySecond(float interval)
    {
        while (true)
        {
            if (n >= 3) { n = 0; }
            GameObject[] gos = GameObject.FindGameObjectsWithTag("test");
            Tremble(gos, displacement, n);
            //for (int i = 0; i < gos.Length; i++)
            //{
            //    gos[i].transform.position = displacement[n][i];
            //    Debug.Log(gos[i].name);
            //}
            n += 1;
            yield return new WaitForSeconds(interval);
        }
    }

    private void Tremble(GameObject[] gos, List<List<Vector3>> displacement, int time_step)
    {

        int count = gos.Length;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = gos[i].transform.position;
            pos.x += displacement[time_step][i][0];
            pos.z += displacement[time_step][i][1];
            gos[i].transform.position = pos;
            print(i);
            print(pos);
        }
    }

}

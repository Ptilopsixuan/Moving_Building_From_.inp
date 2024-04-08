using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using TreeEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

public class MyFunc : MonoBehaviour
{
    public static DataTable OpenCSV(string filePath)
    {
        DataTable dt = new DataTable();
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine = null;
                string[] tableHead = null;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (IsFirst == true)
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (aryLine != null && aryLine.Length > 0)
                {
                    //这行没懂
                    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                }
                sr.Close();
                fs.Close();
                return dt;
            }
        }
    }
    private static GameObject createConn(Vector3[] vec, Material material, string name, string tag)
    {
        GameObject go = new GameObject(name);
        go.tag = tag;
        LineRenderer line = go.AddComponent<LineRenderer>();

        line.material = material;
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;
        line.positionCount = 2;
        line.SetPositions(vec);

        return go;
    }
    private static GameObject createS3R(Vector3[] vec, Material material, string name, string tag)
    {
        GameObject go = new GameObject(name);
        go.tag = tag;
        Vector3[] vertices = vec;
        int[] tris = new int[3] { 0, 2, 1 };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();

        meshRenderer.material = material;
        meshFilter.mesh = mesh;

        return go;
    }
    private static GameObject createS4R(Vector3[] vec, Material material, string name, string tag)
    {
        GameObject go = new GameObject(name);
        go.tag = tag;
        Vector3[] vertices = vec;
        int[] tris = new int[6] { 0, 2, 1, 3, 2, 0 };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();

        meshRenderer.material = material;
        meshFilter.mesh = mesh;

        return go;
    }
    public static void DrawBuilding(MyClass.Building building, Material preMaterial)
    {
        //draw the points of .inp
        //for (int i = 0; i < building.pos.Length; i++)//iterate each Node
        //{
        //    GameObject point = new GameObject(building.name + i);
        //    point.transform.position = building.pos[i];
        //    point.name = building.name + i;//name each copied Node
        //    point.transform.parent = building.original.transform;//set points' parent object in order to control
        //    point.tag = "point";
        //}
        for (int i = 0; i < building.conn.Length; i++)
        //foreach (Vector2 conn in building.connect) //iterate each connection
        {
            int node1Index = (int)building.conn[i][0];
            int node2Index = (int)building.conn[i][1];
            Vector3 pos1 = building.pos[node1Index];
            Vector3 pos2 = building.pos[node2Index];
            Vector3[] vec = new Vector3[2] { pos1, pos2 };
            GameObject connection = createConn(vec, preMaterial, "Con" + i, "line");
            connection.transform.parent = building.original.transform;
        }
        for (int i = 0; i < building.s3r.Length; i++)//iterate each s3r
        {
            int node1Index = (int)building.s3r[i][0];
            int node2Index = (int)building.s3r[i][1];
            int node3Index = (int)building.s3r[i][2];
            Vector3 pos1 = building.pos[node1Index];
            Vector3 pos2 = building.pos[node2Index];
            Vector3 pos3 = building.pos[node3Index];
            Vector3[] vec = new Vector3[3] { pos1, pos2, pos3 };
            GameObject shear3 = createS3R(vec, preMaterial, "S3R" + i, "shear3");
            shear3.transform.parent = building.original.transform;
        }
        for (int i = 0; i < building.s4r.Length; i++)//iterate each s4r
        {
            int node1Index = (int)building.s4r[i][0];
            int node2Index = (int)building.s4r[i][1];
            int node3Index = (int)building.s4r[i][2];
            int node4Index = (int)building.s4r[i][3];
            Vector3 pos1 = building.pos[node1Index];
            Vector3 pos2 = building.pos[node2Index];
            Vector3 pos3 = building.pos[node3Index];
            Vector3 pos4 = building.pos[node4Index];
            Vector3[] vec = new Vector3[4] { pos1, pos2, pos3, pos4 };
            GameObject shear4 = createS4R(vec, preMaterial, "S4R" + i, "shear4");
            shear4.transform.parent = building.original.transform;
        }
    }

    //public static void ChangeColors(MyClass.Building building) 

    public static void DrawCurve()
    {

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

public class MyClass : MonoBehaviour
{
    public class Building
    {
        public string name;
        public GameObject original;
        public Vector3[] pos;
        public int count;
        public Vector2[] conn;
        public Vector3[] s3r;
        public Vector4[] s4r;

        //public List<List<Vector3>> displacement;//inside layer of List is Nodes' list, outside layer is time steps' list.
        public DataTable displacement;

        public void readInp(string filePath)
        {
            //judge different data
            bool nodeBegin = false;
            bool connBegin = false;
            bool s4rBegin = false;
            bool s3rBegin = false;
            //store different data
            DataTable nodeDt = new DataTable();
            for (int i = 0; i < 4; i++) { nodeDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable connDt = new DataTable();
            for (int i = 0; i < 2; i++) { connDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable s4rDt = new DataTable();
            for (int i = 0; i < 4; i++) { s4rDt.Columns.Add(new DataColumn(i.ToString())); }
            DataTable s3rDt = new DataTable();
            for (int i = 0; i < 3; i++) { s3rDt.Columns.Add(new DataColumn(i.ToString())); }
            //read .inp
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string strLine = "";//record words of every line
                    string[] aryLine = null;//trans data
                    int n = 0;//Debug
                    Regex IsStop = new Regex("\\*.*");//to stop
                    Regex IsConn = new Regex(".*=B31.*");//to find connection
                    Regex IsS4R = new Regex(".*=S4R");//to find 4 points' wall
                    Regex IsS3R = new Regex(".*=S3R");//to find 3 points' wall
                    while ((strLine = sr.ReadLine()) != null)//collect corresponding data
                    {
                        if (IsStop.IsMatch(strLine))
                        { nodeBegin = false; connBegin = false; s4rBegin = false; s3rBegin = false; }
                        if (strLine == "*Node") { nodeBegin = true; n++; continue; }//print("n1" + n); 
                        if (IsConn.IsMatch(strLine)) { connBegin = true; n++; continue; }//print("n2" + n);
                        if (IsS3R.IsMatch(strLine)) { s3rBegin = true; n++; continue; }//print("n3" + n);
                        if (IsS4R.IsMatch(strLine)) { s4rBegin = true; n++; continue; }//print("n4" + n); 
                        if (nodeBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(nodeDt, aryLine, 3);
                        }
                        if (connBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(connDt, aryLine, 2);
                        }
                        if (s3rBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(s3rDt, aryLine, 3);
                        }
                        if (s4rBegin)
                        {
                            aryLine = strLine.Split(',');
                            str2dt(s4rDt, aryLine, 4);
                        }
                        n++;
                    }
                    sr.Close();
                    fs.Close();
                }
            }

            (pos, count) = dt2pos(nodeDt);
            //pos = dt2vec(nodeDt, 2).Select(v => new Vector3(v.x / 10000, v.z / 10000, v.y / 10000)).ToArray();//y is height in Unity, while z is height in .inp
            conn = dt2vec(connDt, 2).Select(v => new Vector2(v.x, v.y)).ToArray();
            s3r = dt2vec(s3rDt, 3).Select(v => new Vector3(v.x, v.y, v.z)).ToArray();
            s4r = dt2vec(s4rDt, 4);
        }

        private static void str2dt(DataTable dt, string[] aryLine, int n)
        {
            DataRow dr = dt.NewRow();
            for (int j = 0; j < n; j++)
            {
                dr[j] = aryLine[j + 1];
            }
            dt.Rows.Add(dr);
        }

        private static (Vector3[], int) dt2pos(DataTable dt)
        {
            int n = dt.Rows.Count;
            Vector3[] vec = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                float x = Convert.ToSingle(dt.Rows[i][0]) / 10000;
                float z = Convert.ToSingle(dt.Rows[i][1]) / 10000;
                float y = Convert.ToSingle(dt.Rows[i][2]) / 10000;
                vec[i] = new Vector3(x, y, z);
            }
            return (vec, n);
        }

        private static Vector4[] dt2vec(DataTable dt, int n)
        {
            int count = dt.Rows.Count;
            Vector4[] vec = new Vector4[count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    vec[i][j] = Convert.ToSingle(dt.Rows[i][j]) - 1;
                }
            }
            return vec;
        }
    }
}

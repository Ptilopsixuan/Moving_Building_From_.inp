using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class model : MonoBehaviour
{
    public Material preMaterial;
    public static List<MyClass.Building> buildings = new List<MyClass.Building>();

    public void Import()
    //void Awake()
    {
        float startTime = Time.realtimeSinceStartup;//count time cost

        string dataPath = Application.streamingAssetsPath;

        //read model's points in .inp
        string modelPath = dataPath + "/model";//folder including models' information
        DirectoryInfo modelDir = new DirectoryInfo(modelPath);
        FileInfo[] modelFiles = modelDir.GetFiles();//get all files' name

        for (int i = 0; i < modelFiles.Length; i++)//iterate the file
        {
            if (modelFiles[i].Name.EndsWith("inp"))//("meta")){ continue; }
            {
                MyClass.Building building = new MyClass.Building();//create a building class
                building.name = modelFiles[i].Name;//set buidling's name

                GameObject original = new GameObject(modelFiles[i].Name);//create a parent object represent each building
                building.original = original;//create a parent object so as to contain all this buildings' GameObjects

                string modelFile = (modelPath + "/" + modelFiles[i].Name).ToString();//load path
                building.readInp(modelFile);//read .inp file
                buildings.Add(building);//to transmit the information

                Debug.Log(i + ": " + modelFiles[i].Name);
                //Debug.Log("nodesCount" + building.pos.Length);

                MyFunc.DrawBuilding(building, preMaterial);//create the buildings
            }
        }
        float readInpTime = Time.realtimeSinceStartup;

        //read displacements
        string displacementPath = dataPath + "/displacement";//folder including displacements' information
        DirectoryInfo displacementDir = new DirectoryInfo(displacementPath);
        FileInfo[] displacementFiles = displacementDir.GetFiles();//get all files' name
        int n = 0;
        for (int i = 0; i < displacementFiles.Length; i++)//iterate the file
        {
            if (displacementFiles[i].Name.EndsWith("meta"))
            {
                n++;
            }
            if (displacementFiles[i].Name.EndsWith("csv"))//("meta")){ continue; }
            {
                string displacementFile = (displacementPath + "/" + displacementFiles[i].Name).ToString();//load path
                DataTable dt = MyFunc.OpenCSV(displacementFile);
                buildings[i-n].displacement = dt;//if the displacement files' sequence is the same as the model files'

                Debug.Log(i + ": " + displacementFiles[i].Name);
                //Debug.Log(dt.Rows.Count);
                //Debug.Log(dt.Columns.Count);

            }
        }

        float endTime = Time.realtimeSinceStartup;

        Debug.Log("readInpTime: " + (readInpTime - startTime));
        Debug.Log("readCsvTime: " + (endTime - readInpTime));
    }
}

using System.IO;
using UnityEngine;

public class ShadowLayoutImporter : MonoBehaviour
{
    void Start()
    {
        string fileToImport = Path.Combine(Application.dataPath, "Sample Stage\\stg0100_cmn.dat"); // change this to be the layout you wish to import
        HeroesPowerPlant.LayoutEditor.LayoutEditorSystem.SetupLayoutEditorSystem(); // change this function to use Heroes's ini if you want Heroes objects

            var list = HeroesPowerPlant.LayoutEditor.LayoutEditorFunctions.GetShadowLayout(fileToImport);
        //for each
        for (int i = 0; i < list.Count; i++) { 
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = list[i].GetName;
            cube.transform.localScale = new Vector3(10, 10, 10);
            cube.transform.position = new Vector3(-list[i].Position.x, list[i].Position.y, list[i].Position.z);
        } 
    }
}

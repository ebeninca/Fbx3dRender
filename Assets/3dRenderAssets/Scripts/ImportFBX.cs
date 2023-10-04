using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autodesk.Fbx;
using UnityEditor;



public class ImportFBX : MonoBehaviour
{
    protected void ImportScene(string fileName)
    {
        using (FbxManager fbxManager = FbxManager.Create())
        {
            // configure IO settings.
            fbxManager.SetIOSettings(FbxIOSettings.Create(fbxManager, Globals.IOSROOT));

            // Import the scene to make sure file is valid
            using (FbxImporter importer = FbxImporter.Create(fbxManager, "myImporter"))
            {

                // Initialize the importer.
                bool status = importer.Initialize(fileName, -1, fbxManager.GetIOSettings());

                // Create a new scene so it can be populated by the imported file.
                FbxScene scene = FbxScene.Create(fbxManager, "myScene");

                // Import the contents of the file into the scene.
                importer.Import(scene);
            }
        }
    }

    protected void convertFbxToObj(string[] args)
    {
        if (args.Length != 2)
        {
            Debug.Log("Usage: ConvertFBXtoOBJ <input.fbx> <output.obj>");
            return;
        }

        string inputFilePath = args[0];
        string outputFilePath = args[1];

        // Initialize the FBX SDK
        FbxManager manager = FbxManager.Create();
        FbxIOSettings ioSettings = FbxIOSettings.Create(manager, Globals.IOSROOT);
        manager.SetIOSettings(ioSettings);

        // Create an importer
        FbxImporter importer = FbxImporter.Create(manager, "");

        if (!importer.Initialize(inputFilePath, -1, ioSettings))
        {
            Debug.Log("Failed to initialize FBX importer.");
            return;
        }

        // Create an FBX scene
        FbxScene scene = FbxScene.Create(manager, "Scene");

        // Import the FBX file into the scene
        importer.Import(scene);

        // Destroy the importer as it is no longer needed
        importer.Destroy();

        // Create an OBJ exporter
        FbxExporter objExporter = FbxExporter.Create(manager, "");

        if (!objExporter.Initialize(outputFilePath, -1, ioSettings))
        {
            Debug.Log("Failed to initialize OBJ exporter.");
            return;
        }

        // Export the scene to OBJ format
        objExporter.Export(scene);

        // Destroy the exporter as it is no longer needed
        objExporter.Destroy();

        // Destroy the FBX manager
        manager.Destroy();

        Debug.Log($"Conversion completed. OBJ file saved to {outputFilePath}");
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

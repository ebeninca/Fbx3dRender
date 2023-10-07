using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System.IO;
using Autodesk.Fbx;

public class InitApp : MonoBehaviour
{
    GameObject obj;
    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPosDelta = Vector3.zero;

    void OnDestroy()
    {

    }

    void Start()
    {
        Debug.Log("STARTTTTTT...");

        string[] args = System.Environment.GetCommandLineArgs();

        Debug.Log("cli args size >> " + args.Length);

        for (int i = 0; i < args.Length; i++)
        {
            Debug.Log(string.Format("arg {0} >>> {1}", i, args[i]));
        }

        string pathMesh = GetArg("-mesh", args);
        string pathCoeff = GetArg("-coeff", args);
        string pathNormal = GetArg("-normal", args);
        string pathColor = GetArg("-color", args);
        string pathEmboss = GetArg("-emboss", args);

        if (!File.Exists(pathMesh))
            return;

        string outputOBJPath = pathMesh.Remove(pathMesh.Length - 3) + "obj";
        ConvertFBXtoOBJ(pathMesh, outputOBJPath);

        Stream meshStream = new MemoryStream(File.ReadAllBytes(outputOBJPath));
        Debug.Log("pathMesh >> " + outputOBJPath);
        Debug.Log("readAllBytes >> " + File.ReadAllBytes(outputOBJPath));

        obj = new OBJLoader().Load(meshStream);
        obj.transform.localScale = new Vector3(10, 10, 10);
        obj.transform.localPosition = new Vector3(0.97f, 0.72f, -6.5f);
        GameObject.Find("ball_mat").transform.localScale = new Vector3(-1, 1, 1);

        Debug.Log("Texture Found...");

        foreach (Renderer m in GameObject.FindObjectsOfType<Renderer>())
        {
            Debug.Log(string.Format("Renderer {0}, {1} >> ", m, m.name));
        }

        Material[] materialsArray;

        if (File.Exists(pathEmboss))
        {
            Debug.Log("Emboss >> " + pathEmboss);

            materialsArray = new Material[1];
            materialsArray[0] = NewMaterial(pathEmboss);

            GameObject.Find("ball_mat").transform.GetComponent<Renderer>().materials = materialsArray;
            return;
        }
        else if (File.Exists(pathColor))
        {
            Debug.Log("Color >> " + pathColor);

            materialsArray = new Material[3];
            materialsArray[0] = NewMaterial(pathColor);
            materialsArray[1] = NewMaterial(pathCoeff);
            materialsArray[2] = NewMaterial(pathNormal);

            GameObject.Find("ball_mat").transform.GetComponent<Renderer>().materials = materialsArray;
            return;
        }

        Debug.Log("Texture NOT Found !!!");
    }

    private Material NewMaterial(string pathColor)
    {
        var bytes = File.ReadAllBytes(pathColor);
        var texColor = new Texture2D(1, 1);
        texColor.LoadImage(bytes);
        Material mat = new(Shader.Find("Standard"));
        mat.mainTexture = texColor;
        return mat;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mPosDelta = Input.mousePosition - mPrevPos;

            if (Vector3.Dot(obj.transform.up, Vector3.up) >= 0)
            {
                obj.transform.Rotate(transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
            }
            else
            {
                obj.transform.Rotate(transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
            }

            obj.transform.Rotate(Camera.main.transform.right, Vector3.Dot(mPosDelta, Camera.main.transform.up), Space.World);
        }
        mPrevPos = Input.mousePosition;
    }

    // Helper function for getting the command line arguments
    private static string GetArg(string name, string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                Debug.Log("Found in cli args >> " + args[i]);
                Debug.Log("Found in cli args >> " + args[i + 1]);
                if (args[i + 1] != null)
                    return args[i + 1];
            }
        }

        Debug.Log("Not found in cli parameters, trying default values for >> " + name);
        if (name == "-mesh")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_101\\ball_101_mesh.fbx";
        if (name == "-coeff")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_101\\ball_101_coeff.png";
        if (name == "-normal")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_101\\ball_101_normal.png";
        if (name == "-color")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_101\\ball_101_color.png";

        return null;
    }


    private void ConvertFBXtoOBJ(string inputFilePath, string outputFilePath)
    {
        if (inputFilePath == null || outputFilePath == null)
        {
            Debug.Log("Usage: ConvertFBXtoOBJ <input.fbx> <output.obj>");
            return;
        }

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


}

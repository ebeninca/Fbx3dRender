using Autodesk.Fbx;
using Dummiesman;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class InitApp : MonoBehaviour, IPointerClickHandler
{
    GameObject mainObj;
    GameObject childObj;
    TCPListenerServer tcpServer;

    //Rotation controls
    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPosDelta = Vector3.zero;

    //FOV controls
    private readonly float zoomSpeed = 5.0f;
    private readonly float minFOV = 5.0f;
    private readonly float maxFOV = 60.0f;
    private float scrollInput = 0f;
    private float newFOV;

    //Zoom controls
    public readonly float moveSpeed = 20.0f;
    private Vector3 dragOrigin;

    void Start()
    {
        Debug.Log("STARTTTTTT...");
        string[] args = System.Environment.GetCommandLineArgs();

        Debug.Log("cli args size >> " + args.Length);

        string runServer = GetArg("-runServer", args);
        if (runServer == null) runServer = "8888";
        tcpServer = new TCPListenerServer(Int16.Parse(runServer));
        tcpServer.Start();

        BuildGameObject(args);
    }

    private void BuildGameObject(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            Debug.Log(string.Format("arg {0} >>> {1}", i, args[i]));
        }

        string pathMesh = GetArg("-mesh", args);
        string pathCoeff = GetArg("-coeff", args);
        string pathNormal = GetArg("-normal", args);
        string pathColor = GetArg("-color", args);
        string pathEmboss = GetArg("-emboss", args);

        pathMesh = pathMesh?.Replace("\"", "");
        pathCoeff = pathCoeff?.Replace("\"", "");
        pathNormal = pathNormal?.Replace("\"", "");
        pathColor = pathColor?.Replace("\"", "");
        pathEmboss = pathEmboss?.Replace("\"", "");

        Debug.Log("BuildGameObject PathMesh >> " + pathMesh);

        if (!File.Exists(pathMesh))
        {
            Debug.Log("No File for mesh path found, cancelling.");
            return;
        }

        string outputOBJPath = pathMesh.Remove(pathMesh.Length - 3) + "obj";
        ConvertFBXtoOBJ(pathMesh, outputOBJPath);

        Stream meshStream = new MemoryStream(File.ReadAllBytes(outputOBJPath));
        Debug.Log("pathMesh >> " + outputOBJPath);
        Debug.Log("readAllBytes >> " + File.ReadAllBytes(outputOBJPath));

        mainObj = new OBJLoader().Load(meshStream);
        mainObj.transform.localScale = new Vector3(10, 10, 10);
        mainObj.transform.localPosition = new Vector3(0.97f, 0.72f, -6.5f);

        childObj = mainObj.transform.GetChild(0).gameObject;
        childObj.transform.localScale = new Vector3(-1, 1, 1);
        AddColliderAndEventTrigger(childObj);

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

            childObj.transform.GetComponent<Renderer>().materials = materialsArray;
            return;
        }
        else if (File.Exists(pathColor))
        {
            Debug.Log("Color >> " + pathColor);

            materialsArray = new Material[3];
            materialsArray[0] = NewMaterial(pathColor);
            materialsArray[1] = NewMaterial(pathCoeff);
            materialsArray[2] = NewMaterial(pathNormal);

            childObj.transform.GetComponent<Renderer>().materials = materialsArray;
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

    private void AddColliderAndEventTrigger(GameObject targetObj)
    {
        targetObj.AddComponent<CapsuleCollider>();

        // Create an EventTrigger component for the new GameObject.
        EventTrigger eventTrigger = targetObj.AddComponent<EventTrigger>();

        // Create a new EventTrigger.Entry for the pointer click event.
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };

        // Add a callback to call the OnPointerClick method.
        entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });

        // Add the entry to the EventTrigger's triggers list.
        eventTrigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mPosDelta = Input.mousePosition - mPrevPos;

            if (Vector3.Dot(mainObj.transform.up, Vector3.up) >= 0)
            {
                mainObj.transform.Rotate(transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
            }
            else
            {
                mainObj.transform.Rotate(transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
            }

            mainObj.transform.Rotate(Camera.main.transform.right, Vector3.Dot(mPosDelta, Camera.main.transform.up), Space.World);
        }

        mPrevPos = Input.mousePosition;

        if ((scrollInput = Input.GetAxis("Mouse ScrollWheel")) != 0)
        {
            newFOV = Camera.main.fieldOfView - scrollInput * zoomSpeed;
            newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);
            Camera.main.fieldOfView = newFOV;
        }

        // Check for right mouse button down to start dragging.
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
        }

        // If the right mouse button is held down, move the camera based on mouse movement.
        if (Input.GetMouseButton(1))
        {
            Vector3 dragDelta = Input.mousePosition - dragOrigin;
            float moveAmount = dragDelta.y * moveSpeed * Time.deltaTime;

            // Move the camera forward or backward based on mouse movement.
            Camera.main.transform.Translate(Vector3.forward * moveAmount);

            dragOrigin = Input.mousePosition; // Update the drag origin for the next frame.
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            //reset camera
            mainObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Camera.main.fieldOfView = 20f;
            Camera.main.transform.position = new Vector3(0.98f, 0.67f, 0.05f);
        }
    }

    public void Reload(string[] args)
    {
        Debug.Log("//TODO RELOADING");

        if (mainObj != null)
        {
            Destroy(childObj);
            Destroy(mainObj);
        }

        BuildGameObject(args);
    }

    void OnDestroy()
    {
        Debug.Log("ON DESTROY...");
        tcpServer?.Destroy();
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

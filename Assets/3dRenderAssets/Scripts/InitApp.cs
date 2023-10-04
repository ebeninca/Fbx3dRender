using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System.IO;

public class InitApp : MonoBehaviour
{
    public Vector2 turn;
    public GameObject obj;
    public bool rotateObject = true;

    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPosDelta = Vector3.zero;

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
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_mesh.obj";
        if (name == "-coeff")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_coeff.png";
        if (name == "-normal")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_normal.png";
        if (name == "-color")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_color.png";

        return null;
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

        if (!File.Exists(pathMesh))
            return;

        Stream meshStream = new MemoryStream(File.ReadAllBytes(pathMesh));
        Debug.Log("pathMesh >> " + pathMesh);
        Debug.Log("readAllBytes >> " + File.ReadAllBytes(pathMesh));

        obj = new OBJLoader().Load(meshStream);
        obj.transform.localScale = new Vector3(10, 10, 10);
        obj.transform.localPosition = new Vector3(0.97f, 0.72f, -6.5f);
        GameObject.Find("ball_mat").transform.localScale = new Vector3(-1, 1, 1);

        if (File.Exists(pathColor))
        {
            Debug.Log("Texture Found...");

            foreach (Renderer m in GameObject.FindObjectsOfType<Renderer>())
            {
                Debug.Log(string.Format("Renderer {0}, {1} >> ", m, m.name));
            }

            Debug.Log("Color >> " + pathColor);

            var bytes = File.ReadAllBytes(pathColor);
            var texColor = new Texture2D(1, 1);
            texColor.LoadImage(bytes);
            GameObject.Find("ball_mat").transform.GetComponent<Renderer>().materials[0].mainTexture = texColor;

            bytes = File.ReadAllBytes(pathCoeff);
            var texCoeff = new Texture2D(1, 1);
            texCoeff.LoadImage(bytes);
            //GameObject.Find("ball_mat").transform.GetComponent<Renderer>().materials[1].mainTexture = texCoeff;

            bytes = File.ReadAllBytes(pathNormal);
            var texNormal = new Texture2D(1, 1);
            texNormal.LoadImage(bytes);
            //GameObject.Find("ball_mat").transform.GetComponent<Renderer>().materials[1].SetTexture("_MainText_3", texNormal);

            return;
        }

        Debug.Log("Texture NOT Found !!!");
    }

    // Start is called before the first frame update
    void Awake()
    {
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

}

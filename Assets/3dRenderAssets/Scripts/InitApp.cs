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
    //readonly float rotSpeed = 200;

    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPosDelta = Vector3.zero;

    // Helper function for getting the command line arguments
    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();

        if (name == "mesh")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_mesh.obj";
        if (name == "coeff")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_coeff.png";
        if (name == "normal")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_normal.png";
        if (name == "color")
            return "F:\\EAGames\\FIFA 17_editing\\Content\\Character\\ball\\ball_48\\ball_48_color.png";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    void Start()
    {
        Debug.Log("STARTTTTTT...");

        string pathMesh = GetArg("mesh");
        string pathCoeff = GetArg("coeff");
        string pathNormal = GetArg("normal");
        string pathColor = GetArg("color");

        if (!File.Exists(pathMesh))
            return;

        Stream meshStream = new MemoryStream(File.ReadAllBytes(pathMesh));
        Debug.Log("pathMesh >> " + pathMesh);
        Debug.Log("readAllBytes >> " + File.ReadAllBytes(pathMesh));
        Debug.Log("meshStream >> " + meshStream);

        obj = new OBJLoader().Load(meshStream);
        obj.transform.localScale = new Vector3(10, 10, 10);
        obj.transform.localPosition = new Vector3(0.97f, 0.72f, -6.5f);
        GameObject.Find("ball_mat").transform.localScale = new Vector3(-1, 1, 1);

        if (File.Exists(pathColor))
        {
            Debug.Log("Texture Found...");

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
        Debug.Log("AWWAAAKKKEEE...");
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

    void OnMouseDrag()
    {

        Debug.Log("aaa");
        //float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        //float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        //obj.transform.Rotate(Vector3.up, -rotX);
        //obj.transform.Rotate(Vector3.right, rotY);
    }

}

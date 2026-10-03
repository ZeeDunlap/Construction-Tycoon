using UnityEngine;
using UnityEngine.InputSystem;

public class PipeController : MonoBehaviour
{
    private const int UP = 0;
    private const int RIGHT = 1;
    private const int DOWN = 2;
    private const int LEFT = 3;

    // an array that holds which direction water can flow through
    private bool[] outlets = new bool[4];

    // attributes that are used to check if a path from start to end has been craeted
    public bool start;
    public bool end;
    public int[] coordinates = new int[2];
    public bool isTraversed = false;
    public PipeController parent;
    public PipeController startNode;
    public BoardManager boardManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (name.Equals("Pipe Straight(Clone)"))
        {
            outlets[UP] = true;
            outlets[DOWN] = true;
        }
        else if (name.Equals("Pipe Cross(Clone)"))
        {
            outlets[LEFT] = true;
            outlets[RIGHT] = true;
            outlets[UP] = true;
            outlets[DOWN] = true;
        }
        else if (name.Equals("Pipe T(Clone)"))
        {
            outlets[LEFT] = true;
            outlets[RIGHT] = true;
            outlets[DOWN] = true;
        }
        else if (name.Equals("Pipe Elbow(Clone)"))
        {
            outlets[LEFT] = true;
            outlets[DOWN] = true;
        }
        else if (name.Equals("Pipe End(Clone)"))
        {
            return;
        }
        else
        {
            Debug.Log("Invalid pipe name");
        }

        // rotate the pipe a random number of times to shuffle the board
        for (int i = 0; i < Random.Range(0, 4); i++)
        {
            RotatePipe();
        }

        boardManager = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // If ray hit this target, rotate it
                if (hit.transform == transform)
                {
                    RotatePipe();
                    if (boardManager.TraverseTree(startNode))
                    {
                        Debug.Log("Path Complete");
                    }
                }
            }
        }
    }

    private void RotatePipe()
    {
        transform.Rotate(Vector3.back, 90f);
        // change the directions that water can flow to match the rotation
        bool tmp = outlets[0];
        outlets[0] = outlets[3];
        outlets[3] = outlets[2];
        outlets[2] = outlets[1];
        outlets[1] = tmp;
    }

    public void setOutlets(bool[] outlets)
    {
        this.outlets = outlets;
    }
    public bool[] GetOutlets()
    {
        return outlets;
    }
}

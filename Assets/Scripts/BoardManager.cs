using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    private const int UP = 0;
    private const int RIGHT = 1;
    private const int DOWN = 2;
    private const int LEFT = 3;

    private static int boardSize = 5;
    private GameObject[,] board = new GameObject[boardSize, boardSize];
    private List<PipeController> frontier = new List<PipeController>();
    private GameObject endPipe;

    public GameObject completeButton;
    public List<GameObject> pipes;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // put the start at a random location on the left
        int randRow = Random.Range(0, boardSize);
        GameObject startPipe =  Instantiate(pipes[pipes.Count - 1], new Vector3(-1 * boardSize - 1, (randRow - boardSize / 2) * 2, 0),
            Quaternion.Euler(0, 0, 90));
        startPipe.GetComponent<PipeController>().start = true;
        startPipe.GetComponent<PipeController>().coordinates[0] = randRow;
        startPipe.GetComponent<PipeController>().coordinates[1] = -1;
        startPipe.GetComponent<PipeController>().setOutlets(new bool[] { false, true, false, false });

        // put the end at a random location on the right
        randRow = Random.Range(0, boardSize);
        endPipe = Instantiate(pipes[pipes.Count - 1], new Vector3(boardSize + 1, (randRow - boardSize / 2) * 2, 0),
            Quaternion.Euler(0, 0, -90));
        endPipe.GetComponent<PipeController>().end = true;
        endPipe.GetComponent<PipeController>().coordinates[0] = randRow;
        endPipe.GetComponent<PipeController>().coordinates[1] = boardSize;
        endPipe.GetComponent<PipeController>().setOutlets(new bool[] {false,false,false,true});

        // fill the board with random pipes
        GameObject pipe;
        for (int row = 0; row < boardSize; row++)
        {
            for (int column = 0; column < boardSize; column++)
            {
                Vector3 coordinates = new Vector3((column - boardSize / 2) * 2, (row - boardSize / 2) * 2, 0);
                pipe = Instantiate(pipes[Random.Range(0, pipes.Count - 1)], coordinates, Quaternion.Euler(0, 0, 0));
                board[row, column] = pipe;
                pipe.GetComponent<PipeController>().startNode = startPipe.GetComponent<PipeController>();
                pipe.GetComponent<PipeController>().coordinates[0] = row;
                pipe.GetComponent<PipeController>().coordinates[1] = column;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    // method to find the adjacent pipes that can be expanded
    private void ExploreFrontier(PipeController pipe)
    {
        int row = pipe.coordinates[0];
        int column = pipe.coordinates[1];


        // if this pipe has an outlet on the right, check the pipe to the right
        if (pipe.GetOutlets()[RIGHT] && column + 1 < boardSize)
        {
            PipeController rightPipe = board[row, column + 1].GetComponent<PipeController>();
            if (!rightPipe.isTraversed && rightPipe.GetOutlets()[LEFT])
            {
                //pipes are inserted into the front of the frontier to create a first in last out list
                rightPipe.parent = pipe;
                frontier.Insert(0, rightPipe);
            }
        }

        // if this pipe has an outlet on the bottom, check the pipe to the bottom
        if (pipe.GetOutlets()[DOWN] && row - 1 > 0)
        {
            PipeController bottomPipe = board[row - 1, column].GetComponent<PipeController>();
                if (!bottomPipe.isTraversed && bottomPipe.GetOutlets()[UP])
                {
                //pipes are inserted into the front of the frontier to create a first in last out list
                bottomPipe.parent = pipe;
                frontier.Insert(0, bottomPipe);
                }
        }

        // if this pipe has an outlet on the left, check the pipe to the left
        if (pipe.GetOutlets()[LEFT] && column - 1 > 0)
        {
            PipeController leftPipe = board[row, column - 1].GetComponent<PipeController>();
                if (!leftPipe.isTraversed && leftPipe.GetOutlets()[RIGHT])
            {
                //pipes are inserted into the front of the frontier to create a first in last out list
                leftPipe.parent = pipe;
                frontier.Insert(0, leftPipe);
                }
        }

        // if this pipe has an outlet on the top, check the pipe to the top
        if (pipe.GetOutlets()[UP] && row + 1 < boardSize)
        {
            PipeController topPipe = board[row + 1, column].GetComponent<PipeController>();
                if (!topPipe.isTraversed && topPipe.GetOutlets()[DOWN])
            {
                //pipes are inserted into the front of the frontier to create a first in last out list
                topPipe.parent = pipe;
                frontier.Insert(0, topPipe);
                }
        }

        return;
    }

    // method to traverse the tree
    public bool TraverseTree(PipeController pipe)
    {
        // mark the pipe as traversed
        pipe.isTraversed = true;

        // take all instances of this pipe out of the frontier
        while (frontier.Contains(pipe))
        {
            frontier.Remove(pipe);
        }

        // find the adjacent, expandable pipes
        ExploreFrontier(pipe);

        // check if this pipe is next to the goal
        if (pipe.GetOutlets()[RIGHT] && pipe.coordinates[1] == boardSize - 1 && 
            pipe.coordinates[0] == endPipe.GetComponent<PipeController>().coordinates[0])
        {
            completeButton.SetActive(true);
            return true;
        }
        else if (frontier.Count > 0)
        {
            // explore the next pipe in the frontier
            if(TraverseTree(frontier[0])) {
                return true;
            }
            else
            {
                pipe.isTraversed = false;
                return false;
            }
        }
        else
        {
            pipe.isTraversed = false;
            return false;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

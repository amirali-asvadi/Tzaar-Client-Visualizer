using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] row_0;
    [SerializeField] GameObject[] row_1;
    [SerializeField] GameObject[] row_2;
    [SerializeField] GameObject[] row_3;
    [SerializeField] GameObject[] row_4;
    [SerializeField] GameObject[] row_5;
    [SerializeField] GameObject[] row_6;
    [SerializeField] GameObject[] row_7;
    [SerializeField] GameObject[] row_8;
    [SerializeField] string gameJson;
    
    [SerializeField] public GameObject blackTotts;
    [SerializeField] public GameObject blackTzarras;
    [SerializeField] public GameObject blackTzaars;
    [SerializeField] public GameObject whiteTotts;
    [SerializeField] public GameObject whiteTzarras;
    [SerializeField] public GameObject whiteTzaars;

    [SerializeField] TextMeshProUGUI log_label;


    private Game game;
    private List<Cell> cells;
    private int currentAction;

    private float heightOffset = 0.02f;
    private bool applyOutlineEffect = false;

    void Start()
    {
        cells = new List<Cell>();
        currentAction = 0;
        GameObject[][] allBeads = { row_0, row_1, row_2, row_3, row_4, row_5, row_6, row_7, row_8 };
        for (int i = 0; i < allBeads.Length; i++)
        {
            for (int j = 0; j < allBeads[i].Length; j++)
            {
                if (allBeads[i][j] != null)
                {
                    cells.Add(new Cell(allBeads[i][j], i, j, this));
                }
            }
        }
        if (File.Exists("./Game.log"))
        {
            gameJson = System.IO.File.ReadAllText("./Game.log");
            game = JsonUtility.FromJson<Game>(gameJson);
            applyOutlineToGameObjects();
            setLog("Game.log was read successfully!");
        }
        else
        {
            setLog("Game.log was not found in current directory!");
        }
    }


    void Update()
    {
        if (game != null && currentAction < game.actions.Length)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                removeOutlineFromGameObjects();
                applyAction(game.actions[currentAction]);
                currentAction++;
                if (currentAction != game.actions.Length)
                {
                    applyOutlineToGameObjects();   
                    if (currentAction == 1)
                    { 
                        setLog("");
                    }
                }
                else
                {
                    setLog(game.winner.Substring(0,1).ToUpper() + game.winner.Substring(1,game.winner.Length - 1).ToLower() + " won the game!" );
                }
            }
        }
        if (game != null && currentAction > 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentAction != game.actions.Length)
                {
                    removeOutlineFromGameObjects();   
                }
                else
                {
                    setLog("");
                }
                currentAction--;
                reverseAction(game.actions[currentAction]);
                applyOutlineToGameObjects();
            }
        }
    }
    
    public void reloadClicked(){
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }
    
    private void reverseAction(Action action)
    {
        Cell start = getCellByPoint(action.start);
        Cell target = getCellByPoint(action.target);
        start.reverseLastData(heightOffset);
        target.reverseLastData(heightOffset);
    }

    private void removeOutlineFromGameObjects()
    {
        applyOutlineEffect = false;
        Action action = game.actions[currentAction];
        Cell start = getCellByPoint(action.start);
        Cell target = getCellByPoint(action.target);
        start.removeOutline();
        target.removeOutline();
    }

    private void applyOutlineToGameObjects()
    {
        applyOutlineEffect = true;
        Action action = game.actions[currentAction];
        Cell start = getCellByPoint(action.start);
        Cell target = getCellByPoint(action.target);
        start.applyOutline(Color.green, 5);
        target.applyOutline(Color.red, 5);
    }


    private void applyAction(Action action)
    {
        Cell start = getCellByPoint(action.start);
        Cell target = getCellByPoint(action.target);
        start.saveCurrentData();
        target.saveCurrentData();
        if (action.type.Equals("Attack", StringComparison.OrdinalIgnoreCase))
        {
            foreach (GameObject bead in target.getBeads())
            {
                Destroy(bead);
            }

            float currentY = 0.018f;
            foreach (GameObject bead in start.getBeads())
            {
                bead.transform.position = new Vector3(target.position.x, currentY, target.position.z);
                currentY += heightOffset;
            }

            target.attackCell(start);
            start.makeCellEmpty();
        }
        else if (action.type.Equals("Reinforce", StringComparison.OrdinalIgnoreCase))
        {
            float currentY = (target.getHeight() + 1) * heightOffset;
            foreach (GameObject bead in start.getBeads())
            {
                bead.transform.position = new Vector3(target.position.x, currentY, target.position.z);
                currentY += heightOffset;
            }

            target.reinforceCell(start);
            start.makeCellEmpty();
        }
    }

    private Cell getCellByPoint(Point point)
    {
        foreach (Cell cell in cells)
        {
            if (cell.getRow() == point.row && cell.getCol() == point.col)
            {
                return cell;
            }
        }
        return null;
    }

    private void setLog(String message)
    {
        log_label.text = message;
    }

}


[Serializable]
public class Game
{
    public Action[] actions;
    public String winner;
}

[Serializable]
public class Action
{
    public string type;
    public Point start;
    public Point target;
}

[Serializable]
public class Point
{
    public int row;
    public int col;
}


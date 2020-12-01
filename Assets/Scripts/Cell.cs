using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellData
{
    public BeadType type;
    public Player player;
    public int height;
}


public class Cell
{
    private List<GameObject> beads;
    private List<CellData> oldDatas;
    private BeadType type;
    private Player player;
    private int height;
    private int row;
    private int col;
    public Vector3 position;
    private GameManager gameManager;

    public Cell(GameObject bead, int row, int col, GameManager gameManager)
    {
        this.gameManager = gameManager;
        oldDatas = new List<CellData>();
        beads = new List<GameObject>();
        beads.Add(bead);
        this.row = row;
        this.col = col;
        this.height = 1;
        this.position = new Vector3(bead.transform.position.x,bead.transform.position.y,bead.transform.position.z);
        setBeadTypeByName(bead);
        setPlayerByName(bead);
    }

    public void saveCurrentData()
    {
        CellData data = new CellData();
        data.height = this.height;
        data.type = this.type;
        data.player = this.player;
        oldDatas.Add(data);
    }
    
    public void reverseLastData(float heightOffset)
    {
        CellData data = oldDatas[oldDatas.Count - 1];
        oldDatas.Remove(data);
        this.height = data.height;
        this.type = data.type;
        this.player = data.player;
        if (this.getBeads() != null)
        {
            foreach (GameObject bead in this.getBeads())
            {
                Object.Destroy(bead);
            }
        }
        this.beads = new List<GameObject>();
        GameObject beadPrefab = null;
        if (this.player == Player.Black)
        {
            if (this.type == BeadType.Totts)
            {
                beadPrefab = gameManager.blackTotts;
            }else if (this.type == BeadType.Tzarras)
            {
                beadPrefab = gameManager.blackTzarras;
            }else if (this.type == BeadType.Tzaars)
            {
                beadPrefab = gameManager.blackTzaars;
            }
        } else {
            if (this.type == BeadType.Totts)
            {
                beadPrefab = gameManager.whiteTotts;
            }else if (this.type == BeadType.Tzarras)
            {
                beadPrefab = gameManager.whiteTzarras;
            }else if (this.type == BeadType.Tzaars)
            {
                beadPrefab = gameManager.whiteTzaars;
            }
        }
        float currentY = 0.018f;
        for (int i = 0; i < this.height; i++)
        {
            GameObject instant = Object.Instantiate(beadPrefab,new Vector3(this.position.x,currentY,this.position.z), beadPrefab.transform.rotation);
            this.beads.Add(instant);
            currentY += heightOffset;
        }
    }

    public void applyOutline(Color color, float width)
    {
        foreach (GameObject bead in this.getBeads())
        {
            Outline outline = bead.GetComponent<Outline>();
            if (outline == null)
            { 
                outline = bead.AddComponent<Outline>();
            }
            else
            {
                bead.GetComponent<Outline>().enabled = true;
            }
            outline.OutlineColor = color;
            outline.OutlineWidth = width;
        }
    }

    public void removeOutline()
    {
        foreach (GameObject bead in this.getBeads())
        {
            bead.GetComponent<Outline>().enabled = false;
        }
    }

    public void reinforceCell(Cell addCell)
    {
        foreach(GameObject bead in addCell.getBeads())
        {
            beads.Add(bead);
        }
        height += addCell.getHeight();
        type = addCell.getBeadType();
    }

    public void attackCell(Cell attackedCell)
    {
        beads = attackedCell.getBeads();
        height = attackedCell.getHeight();
        player = attackedCell.getPlayer();
        type = attackedCell.getBeadType();
    }

    public void makeCellEmpty()
    {
        beads = null;
        height = 0;
    }

    public List<GameObject> getBeads()
    {
        return beads;
    }

    public int getHeight()
    {
        return height;
    }

    public int getRow()
    {
        return row;
    }

    public int getCol()
    {
        return col;
    }

    public Player getPlayer()
    {
        return player;
    }

    public BeadType getBeadType()
    {
        return type;
    }

    public void setBeadTypeByName(GameObject bead)
    {
        if (bead.gameObject.name.Contains("Tzaars"))
            type = BeadType.Tzaars;
        else if (bead.gameObject.name.Contains("Tzarras"))
            type = BeadType.Tzarras;
        else
            type = BeadType.Totts;
    }

    public void setPlayerByName(GameObject bead)
    {
        if (bead.gameObject.name.Contains("Black"))
            player = Player.Black;
        else
            player = Player.White;
    }

}

public enum Player
{
    Black, White
}

public enum BeadType
{
    Tzaars, Tzarras, Totts
}
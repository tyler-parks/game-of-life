using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static int SCREEN_WIDTH = 64;       //1024px
    private static int SCREEN_HEIGHT = 48;      //768 px

    private float nextActionTime = 0.0f;
    public float period = 0.05f;

    public Sprite black;

    private bool startGame = false;

    Tile[,] grid = new Tile[SCREEN_WIDTH, SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start()
    {
        PlaceTiles();
    }

    // Update is called once per frame
    void Update()
    {

        print("Hello!");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            startGame = !startGame;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    grid[x, y].setState(false);
                }
            }
        }

        if (startGame)
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                CheckNeighbors();
                setNextGeneration();               
            }
        }
        else
        {
            if(Input.GetMouseButton(0))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);

                if ((int)pos.x > 0 && (int)pos.x < SCREEN_WIDTH && (int)pos.y > 0 && (int)pos.y < SCREEN_HEIGHT)
                {
                    grid[(int)pos.x, (int)pos.y].setState(true);
                }
            }
            else if (Input.GetMouseButton(1))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);

                if ((int)pos.x > 0 && (int)pos.x < SCREEN_WIDTH && (int)pos.y > 0 && (int)pos.y < SCREEN_HEIGHT)
                {
                    grid[(int)pos.x, (int)pos.y].setState(false);
                }
            }

            nextActionTime = Time.time;                   
        }
    }

    void PlaceTiles()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                Tile tile = Instantiate(Resources.Load("Prefabs/blackTile", typeof(Tile)), new Vector2(x, y), Quaternion.identity) as Tile;
                grid[x, y] = tile;
                grid[x, y].setState(GetRandomAliveTile());
            }
        }
    }

    bool GetRandomAliveTile()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand > 100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckNeighbors()
    {
        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {

                if (grid[x, y].getNumNeighbors() == 0)
                {
                    //Corner Tiles
                    if ((x == 0 && y == 0) || (x == SCREEN_WIDTH - 1 && y == 0) || (x == 0 && y == SCREEN_HEIGHT - 1) || (x == SCREEN_WIDTH - 1 && y == SCREEN_HEIGHT - 1))
                    {
                        FindNumNeighbors(x, y, 0);
                    }

                    //Edge Tiles
                    else if (x == 0 || x == SCREEN_WIDTH - 1 || y == 0 || y == SCREEN_HEIGHT - 1)
                    {
                        FindNumNeighbors(x, y, 1);
                    }

                    //Inner Tiles
                    else
                    {
                        FindNumNeighbors(x, y, 2);
                    }
                }
            }
        }
    }

    void FindNumNeighbors(int x, int y, int type)
    {
        //Is a corner
        if (type == 0)
        {
            //Top-Left Corner
            if (x == 0 && y == 0)
            {
                CornerTile(x, y, 0);
            }
            //Top-Right Corner
            else if (x == SCREEN_WIDTH - 1 && y == 0)
            {
                CornerTile(x, y, 1);
            }
            //Bottom-Left Corner
            else if (x == 0 && y == SCREEN_HEIGHT - 1)
            {
                CornerTile(x, y, 2);
            }
            //Bottom-Right Corner
            else if (x == SCREEN_WIDTH - 1 && y == SCREEN_HEIGHT - 1)
            {
                CornerTile(x, y, 3);
            }
        }
        else if (type == 1)
        {
            //Left-most column
            if (x == 0)
            {
                EdgeTile(x, y, 0);
            }
            //Right-most column
            else if (x == SCREEN_WIDTH - 1)
            {
                EdgeTile(x, y, 2);
            }
            //Top row
            else if (y == 0)
            {
                EdgeTile(x, y, 1);
            }
            //Bottom row
            else if (y == SCREEN_HEIGHT - 1)
            {
                EdgeTile(x, y, 3);
            }
        }
        else if (type == 2)
        {
            InnerTile(x, y);
        }
    }

    void CornerTile(int x, int y, int location)
    {
        //Top left
        if (location == 0)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleRight(x, y));     //[ ][ ][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomRight(x, y));     //[ ][ ][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomMiddle(x, y));    //[ ][x][x]
        }
        //Top right
        else if (location == 1)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleLeft(x, y));      //[ ][ ][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomLeft(x, y));      //[x][ ][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomMiddle(x, y));    //[x][x][ ]
        }
        //Bottom left
        else if (location == 2)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopMiddle(x, y));       //[ ][x][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopRight(x, y));        //[ ][ ][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleRight(x, y));     //[ ][ ][ ]
        }
        //Bottom right
        else if (location == 3)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopLeft(x, y));         //[x][x][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopMiddle(x, y));       //[x][ ][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleLeft(x, y));      //[ ][ ][ ]
        }
    }

    void EdgeTile(int x, int y, int location)
    {
        //Left Edge
        if (location == 0)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleRight(x, y));     //[ ][x][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomRight(x, y));     //[ ][ ][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomMiddle(x, y));    //[ ][x][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopMiddle(x, y));
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopRight(x, y));
        }
        //Top Edge
        else if (location == 1)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleLeft(x, y));      //[ ][ ][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomLeft(x, y));      //[x][ ][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomMiddle(x, y));    //[x][x][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomRight(x, y));
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleRight(x, y));
        }
        //Right Edge
        else if (location == 2)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopLeft(x, y));         //[x][x][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopMiddle(x, y));       //[x][ ][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleLeft(x, y));      //[x][x][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomLeft(x, y));
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomMiddle(x, y));

        }
        //Bottom Edge
        else if (location == 3)
        {
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopMiddle(x, y));       //[x][x][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopRight(x, y));        //[x][ ][x]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleRight(x, y));     //[ ][ ][ ]
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopLeft(x, y));
            grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleLeft(x, y));
        }
    }

    void InnerTile(int x, int y)
    {
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopLeft(x, y));          
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopMiddle(x, y));       
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkTopRight(x, y));        
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleLeft(x, y));
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkMiddleRight(x, y));
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomLeft(x, y));     
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomMiddle(x, y));
        grid[x, y].setNumNeighbors(grid[x, y].getNumNeighbors() + checkBottomRight(x, y));
    }

    int checkTopLeft(int x, int y)
    {
        if (grid[x - 1, y - 1].getState())
        {
            return 1;
        }
        return 0;
    }

    int checkTopMiddle(int x, int y)
    {
        if (grid[x, y - 1].getState())
        {
            return 1;
        }
        return 0;
    }

    int checkTopRight(int x, int y)
    {
        if (grid[x + 1, y - 1].getState())
        {
            return 1;
        }
        return 0;
    }

    int checkMiddleLeft(int x, int y)
    {
        if (grid[x - 1, y].getState())
        {
            return 1;
        }
        return 0;
    }

    int checkMiddleRight(int x, int y)
    {
        if (grid[x + 1, y].getState())
        {
            return 1;
        }
        return 0;
    }

    int checkBottomLeft(int x, int y)
    {
        if (grid[x - 1, y + 1].getState())
        {
            return 1;
        }
        return 0;
    }

    int checkBottomMiddle(int x, int y)
    {
        if (grid[x, y + 1].getState())
        {
            return 1;
        }
        return 0;
    }

    int checkBottomRight(int x, int y)
    {
        if (grid[x + 1, y + 1].getState())
        {
            return 1;
        }
        return 0;
    }

    void setNextGeneration()
    {
        int neighbors = 0;

        for (int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for (int x = 0; x < SCREEN_WIDTH; x++)
            {
                neighbors = grid[x, y].getNumNeighbors();

                //tile is alive
                if (grid[x, y].getState())
                {
                    //if less than 2 neighbors
                    if (neighbors < 2)
                    {
                        //dies
                        grid[x, y].setState(false);
                    }
                    //if more than 3 neighbors
                    else if (neighbors > 3)
                    {
                        //dies
                        grid[x, y].setState(false);
                    }
                }
                //tile is dead
                else
                {
                    //if exactly 3 neighbors
                    if (neighbors == 3)
                    {
                        //revive
                        grid[x, y].setState(true);
                    }
                }

                grid[x, y].setNumNeighbors(0);
            }
        }
    }
}



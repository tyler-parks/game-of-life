using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public bool isAlive = false;
    public int numNeighbors = 0;

    public Sprite black;
    public Sprite white; 

    public void setState(bool b)
    {
        isAlive = b;

        if (isAlive)
        {
            //GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().sprite = black;
        }
        else
        {
            //GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = white;
        }
    }

    public bool getState()
    {
        return isAlive;
    }

    public void setNumNeighbors(int n)
    {
        numNeighbors = n;
    }

    public int getNumNeighbors()
    {
        return numNeighbors;
    }

}

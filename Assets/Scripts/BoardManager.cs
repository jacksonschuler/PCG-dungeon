using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public int rows;
    public int columns;

    public int minRoomSize;
    public int maxRoomSize;
    public GameObject[] floorTiles;
    public GameObject tile;

    private GameObject[,] boardPosFloor;

    public class Subdungeon
    {

        public Subdungeon left; //reference to the left subdungeon
        public Subdungeon right; //reference to the right subdungeon
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);

        

        public Subdungeon(Rect mrect)
        {
            rect = mrect;

        }

        //is the current node a leaf?
        public bool isLeaf()
        {
            return left == null && right == null;
        }

        public bool split(int minRoomSize, int maxRoomsize)
        {
            if (!isLeaf())
            {
                return false;
            }

            bool splitT;

            if (rect.width / rect.height >= 1.25)
            {
                splitT = false;
            }
            else if (rect.height / rect.width >= 1.25)
            {
                splitT = true;
            }
            else
            {
                splitT = Random.Range(0.0f, 1.0f) > 0.5;
            }

            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                
                return false;
            }

            if (splitT)
            {
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                left = new Subdungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new Subdungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));
                left = new Subdungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new Subdungeon(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;
        }

        public void CreateRooms()
        {
            if (left != null)
            {
                left.CreateRooms();
            }
            if (right != null)
            {
                right.CreateRooms();
            }

            if (isLeaf())
            {

                int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2); 
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                

            }
        }

    }

    public void CreateBSP(Subdungeon subdungeon)
    {
        if (subdungeon.isLeaf())
        {
            if (subdungeon.rect.width > maxRoomSize || subdungeon.rect.height > maxRoomSize || Random.Range(0.0f, 1.0f) > 0.25)
            {
                if (subdungeon.split(minRoomSize, maxRoomSize))
                {
                    CreateBSP(subdungeon.left);
                    CreateBSP(subdungeon.right);
                }
            }
        }
    }

    public void DrawRooms(Subdungeon subdungeon)
    {
        if (subdungeon == null)
        {
            return;
        }
        if (subdungeon.isLeaf())
        {
 
            for (int i = (int)subdungeon.room.x; i < subdungeon.room.xMax; i++)
            {
                
                for (int j = (int)subdungeon.room.y; j < subdungeon.room.yMax; j++)
                {
                 
                    int randIndex = Random.Range(0, floorTiles.Length); //pick a random tile from the array
                    Vector3 pos = new Vector3(i, j, 0f); //the position where the tile will be placed

                    GameObject newInstance = Instantiate(floorTiles[randIndex], pos, Quaternion.identity) as GameObject;
                    newInstance.transform.SetParent(transform);

                    boardPosFloor[i, j] = newInstance;
                    
                }
            }
        } else
        {
            DrawRooms(subdungeon.left);
            DrawRooms(subdungeon.right);
        }
    }

    private void Start()
    {
        Subdungeon rootSubDungeon = new Subdungeon(new Rect(0, 0, rows, columns));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRooms();
        boardPosFloor = new GameObject[rows, columns];
        DrawRooms(rootSubDungeon);
    }

}

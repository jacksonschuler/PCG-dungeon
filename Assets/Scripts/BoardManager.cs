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
    public List<Rect> corridors = new List<Rect>();

    private GameObject[,] boardPosFloor;

    public class Subdungeon
    {

        public Subdungeon left; //reference to the left subdungeon
        public Subdungeon right; //reference to the right subdungeon
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);
        public List<Rect> corridors = new List<Rect>();
        public List<Rect> rooms = new List<Rect>();
   

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

        public void CreateCorridor(Subdungeon left, Subdungeon right)
        {
            Rect rroom = right.GetRoom();
            Rect lroom = left.GetRoom();

            Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
            Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

            // always be sure that left point is on the left to simplify the code
            if (lpoint.x > rpoint.x)
            {
                Vector2 temp = lpoint;
                lpoint = rpoint;
                rpoint = temp;
            }

            int w = (int)(lpoint.x - rpoint.x);
            int h = (int)(lpoint.y - rpoint.y);

            

            // if the points are not aligned horizontally
            if (w != 0)
            {
                // choose at random to go horizontal then vertical or the opposite
                if (Random.Range(0, 1) > 2)
                {
                    // add a corridor to the right
                    corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs(w) + 1, 1));

                    // if left point is below right point go up
                    // otherwise go down
                    if (h < 0)
                    {
                        corridors.Add(new Rect(rpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(rpoint.x, lpoint.y, 1, -Mathf.Abs(h)));
                    }
                }
                else
                {
                    // go up or down
                    if (h < 0)
                    {
                        corridors.Add(new Rect(lpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(lpoint.x, rpoint.y, 1, Mathf.Abs(h)));
                    }

                    // then go right
                    corridors.Add(new Rect(lpoint.x, rpoint.y, Mathf.Abs(w) + 1, 1));
                }
            }
            else
            {
                // if the points are aligned horizontally
                // go up or down depending on the positions
                if (h < 0)
                {
                    corridors.Add(new Rect((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(h)));
                }
            }


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
            if (right != null && left != null)
            {
                CreateCorridor(left, right);
            }


            if (isLeaf())
            {

                int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2); 
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                rooms.Add(room);
                
       
  
            }
        }

        public Rect GetRoom()
        {
            if (isLeaf())
            {
                return room;
            }
            if (left != null)
            {
                Rect leftRoom = left.GetRoom();
                if (leftRoom.x != -1)
                {
                    return leftRoom;
                }
            }
            if (right != null)
            {
                Rect rightRoom = right.GetRoom();
                if (rightRoom.x != -1)
                {
                    return rightRoom;
                }
            }
            return new Rect(-1, -1, 0, 0);
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

    public void DrawCorridors(Subdungeon subdungeon)
    {
        if (subdungeon == null)
        {
            return;
        }

        DrawCorridors(subdungeon.left);
        DrawCorridors(subdungeon.right);

        foreach (Rect corridor in subdungeon.corridors)
        {
            for (int i=(int)corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int)corridor.y; j < corridor.yMax; j++)
                {
                    Vector3 pos = new Vector3(i, j, 0f); //this made it work I dont know 8 and 4
                    Vector3 pos2 = new Vector3(i+1, j+1, 0f);
                    int rand = (int)Random.Range(0, floorTiles.Length);
                    GameObject newInstance = Instantiate(floorTiles[rand], pos, Quaternion.identity) as GameObject;
                    newInstance.transform.SetParent(transform);
                    //for making hallways 2 tile wide
                    GameObject newInstance2 = Instantiate(floorTiles[rand], pos2, Quaternion.identity) as GameObject;
                    newInstance2.transform.SetParent(transform);
                }
            }
        }
    }

 

    private void Start()
    {
        Subdungeon rootSubDungeon = new Subdungeon(new Rect(0, 0, rows, columns));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRooms();
        boardPosFloor = new GameObject[rows, columns];
        
        DrawRooms(rootSubDungeon);
        DrawCorridors(rootSubDungeon);
    }

}
 
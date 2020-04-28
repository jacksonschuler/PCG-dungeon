using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public int rows; //the number of rows
    public int columns; //the number of columns

    public int minRoomSize; //the max size of a room
    public int maxRoomSize; //the min size of a room

    public GameObject[] floorTiles; //array of tiles for the floors

    public GameObject baseTile; //the base tile

    public GameObject Bottom; //bottom wall

    public GameObject Top; //top wall

    public GameObject Left; //left facing wall

    public GameObject Right; //right facing wall

    public GameObject CornerBotRight; //bottom right corner

    public GameObject CornerBotLeft; //bottom left corner

    public GameObject CornerTopRight; //top right corner

    public GameObject CornerTopLeft; //top left corner

    public List<Rect> corridors = new List<Rect>(); //list of rectangles for the corridors

    private GameObject[,] boardPosFloor; //array representation of rooms

    public GameObject player; //the player

    public Camera minimap; //the camera depicting the minimap

    public GameObject Darkness; //the darkness sprite

    public class Subdungeon
    {

        public Subdungeon left; //reference to the left subdungeon
        public Subdungeon right; //reference to the right subdungeon
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);
        public List<Rect> corridors = new List<Rect>();
        
   

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
            //picks a random position in each room and connect those points
            Rect rroom = right.GetRoom();
            Rect lroom = left.GetRoom();

            Vector2 leftPt = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
            Vector2 rightPt = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

            if (leftPt.x > rightPt.x)
            {
                Vector2 temp = leftPt;
                leftPt = rightPt;
                rightPt = temp;
            }

            int w = (int)(leftPt.x - rightPt.x); //do those random points have the same width?
            int h = (int)(leftPt.y - rightPt.y); //do those random points have the same height?

            

            if (w != 0)
            {

                if (Random.Range(0, 1) > 2)
                {
                    corridors.Add(new Rect(leftPt.x, leftPt.y, Mathf.Abs(w) + 1, 1));

                    if (h < 0)
                    {
                        corridors.Add(new Rect(rightPt.x, leftPt.y, 1, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(rightPt.x, leftPt.y, 1, -Mathf.Abs(h)));
                    }
                }
                else
                {
                    if (h < 0)
                    {
                        corridors.Add(new Rect(leftPt.x, leftPt.y, 1, Mathf.Abs(h)));
                    }
                    else
                    {
                        corridors.Add(new Rect(leftPt.x, rightPt.y, 1, Mathf.Abs(h)));
                    }

                    corridors.Add(new Rect(leftPt.x, rightPt.y, Mathf.Abs(w) + 1, 1));
                }
            }
            else
            {
                if (h < 0)
                {
                    corridors.Add(new Rect((int)leftPt.x, (int)leftPt.y, 1, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect((int)rightPt.x, (int)rightPt.y, 1, Mathf.Abs(h)));
                }
            }


        }

        public void CreateRooms()
        {

            //Get to a leaf node
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

            //if we are on a leaf, create a new room
            if (isLeaf())
            {

                int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2); 
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                
                
       
  
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
            return new Rect(-1, -1, 0, 0); // a null rectangle
        }





        
    }

    public void CreateBSP(Subdungeon subdungeon)
    //Create a BinarySpacePartition from a given subdungeon
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
        //Instantiates the tiles on the tiles into the game based on the construction of the rooms from CreateRooms()
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

    public void DrawBase()
    //Draws the dark coloured tile below the dungeon to fill in the space between rooms and corridors
    {
        for (int i = -rows; i < rows*2; i++)
        {
            for (int j = -columns; j < columns*2; j++)
            {

                Vector3 pos = new Vector3(i, j, 0f); //the position where the tile will be placed
                GameObject newInstance = Instantiate(baseTile, pos, Quaternion.identity) as GameObject;
                newInstance.transform.SetParent(transform);

            }
        }
    }

    public void DrawWalls()
    //Draws the walls around the rooms and the corridors that are procedurally generated. 
    //Checks to ensure that the correc tiles are being placed in correct position
    {
        for (int i = 0; i < boardPosFloor.GetLength(0); i++)
        {
            for (int j = 0; j < boardPosFloor.GetLength(1); j++)
            {
                if (boardPosFloor[i, j] != null) //if we are on a gametile
                {

                    if (boardPosFloor[i, j - 1] == null && j != columns && j != 0) //tile below (has correct sprite)
                    {
                        if (boardPosFloor[i-1,j-1] != null && i != rows && i != 0)
                        {
                            //check if we are on a corner
                            Vector3 pos1 = new Vector3(i, j - 1, 0f);
                            GameObject newInstance1 = Instantiate(CornerTopLeft, pos1, Quaternion.identity) as GameObject;
                            newInstance1.transform.SetParent(transform);

                        } if (boardPosFloor[i + 1, j -1] != null && i != rows && i != 0) {
                            Vector3 pos1 = new Vector3(i, j - 1, 0f); 
                            GameObject newInstance1 = Instantiate(CornerTopRight, pos1, Quaternion.identity) as GameObject;
                            newInstance1.transform.SetParent(transform);

                        }

                        else
                        {
                            Vector3 pos = new Vector3(i, j - 1, 0f); 
                            GameObject newInstance = Instantiate(Bottom, pos, Quaternion.identity) as GameObject;
                            newInstance.transform.SetParent(transform);

                        }
                    }

                    if (boardPosFloor[i,j+1] == null && j != columns && j != 0) //tile above
                    {

                        if (boardPosFloor[i - 1, j + 1] != null && i != rows && i != 0)
                        {
                            Vector3 pos1 = new Vector3(i, j + 1, 0f); 
                            GameObject newInstance1 = Instantiate(CornerBotRight, pos1, Quaternion.identity) as GameObject;
                            newInstance1.transform.SetParent(transform);

                        }
                        if (boardPosFloor[i + 1, j + 1] != null && i != rows && i != 0)
                        {
                            Vector3 pos1 = new Vector3(i, j + 1, 0f); 
                            GameObject newInstance1 = Instantiate(CornerBotLeft, pos1, Quaternion.identity) as GameObject;
                            newInstance1.transform.SetParent(transform);
                        }
                        else
                        {
                            Vector3 pos = new Vector3(i, j + 1, 0f); 
                            GameObject newInstance = Instantiate(Top, pos, Quaternion.identity) as GameObject;
                            newInstance.transform.SetParent(transform);
                        }
                    }

                    if (boardPosFloor[i-1,j] == null && i != rows && i != 0) //tile to the left
                    {
                        Vector3 pos = new Vector3(i - 1, j, 0f); 
                        GameObject newInstance = Instantiate(Left, pos, Quaternion.identity) as GameObject;
                        newInstance.transform.SetParent(transform);


                    }

                    if (boardPosFloor[i + 1, j] == null && i != rows && i != 0) //tile to the right
                    {
                        
                        Vector3 pos = new Vector3(i + 1, j, 0f); 
                        GameObject newInstance = Instantiate(Right, pos, Quaternion.identity) as GameObject;
                        newInstance.transform.SetParent(transform);

                    }
                }
            }
        }
    }


    public void DrawCorridors(Subdungeon subdungeon)
    {
        //Draws the corridors onto the map based on the construction given from the CreateCorridors()
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

                    if (boardPosFloor[i,j] == null)
                    {
                        Vector3 pos = new Vector3(i, j, 0f); //this made it work I dont know 8 and 4

                        int rand = (int)Random.Range(0, floorTiles.Length);
                        GameObject newInstance = Instantiate(floorTiles[rand], pos, Quaternion.identity) as GameObject;
                        newInstance.transform.SetParent(transform);
                        boardPosFloor[i, j] = newInstance;
                    }

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
        DrawWalls();
        DrawBase();


        Vector3 playerPos = new Vector3(rootSubDungeon.GetRoom().xMax / 2, rootSubDungeon.GetRoom().yMax / 2, 0f); //put the player in the middle of the first room
        player.transform.position = playerPos; //set player position

        Vector3 center = new Vector3(rows/2,columns/2,-1f); //have the minimap render out a top down view of the center of the map
        minimap.transform.position = center;
        minimap.orthographicSize = rows / 2f; //keep size large enough that it can see the entire dungeon output
        playerPos.z = -9f;
        Darkness.transform.position = playerPos;
    }

    private void Update()
    {
        Vector3 pos = player.transform.position;
        pos.z = -9f; //the darkness must be above the main camera but below the minimap camera 
        Darkness.transform.position = pos; //the darkness follows the player, moves when the player does.
    }

}
 
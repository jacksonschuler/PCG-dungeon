using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public int rows;
    public int columns;

    public int minRoomSize;
    public int maxRoomSize;


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


    private void Start()
    {
        Subdungeon rootSubDungeon = new Subdungeon(new Rect(0, 0, rows, columns));
        CreateBSP(rootSubDungeon);
    }

}

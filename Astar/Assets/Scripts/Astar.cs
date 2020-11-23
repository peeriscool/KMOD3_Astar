using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 
    float width;
    float height;
    Cell[,] Grid;
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        /*
         * convert cells to Node 
         * calculate positions from start to end position
         * use node to save calculated values which must include G and H score 
         * 
         * first conver cells to nodes
         * give H score
         * go through A*
         * calculate G
         * calculate F
         * return path
         * */
        List<Node> OpenSet = new List<Node>(); //has to be filled
        OpenSet = CreateOpenList(grid, startPos, endPos);
        HashSet<Node> ClosedSet = new HashSet<Node>(); //final path
        width = grid.GetLength(0);
        height = grid.GetLength(1);
        Grid = grid;

        
        Node StartNode = new Node(startPos, null, 0, 0);
        Node EndNode = new Node(endPos, null, 0, 0); //change 0,0 to G and H 

        foreach (Node item in OpenSet) //set start node and end node
        {
            if(item.position == startPos)
            {
                StartNode = item;
                StartNode.GScore = 0;
            }
            if (item.position == endPos)
            {
                EndNode = item;
                EndNode.HScore = 0;
            }
        }
        ClosedSet.Add(StartNode);
        
        while (OpenSet.Count > 0) //loop through all nodes in the open set
        {
            Node current = OpenSet[0];
            for (int i = 1; i < OpenSet.Count; i++) 
            {
                if(OpenSet[i].FScore < current.FScore || (OpenSet[i].FScore == current.FScore && OpenSet[i].HScore < current.HScore)) //find the lowest F cost using Current and index
                {
                    current = OpenSet[i]; //lowest F cost found
                }
            }
            OpenSet.Remove(current);
            ClosedSet.Add(current); 

            if (current.position == endPos) //found end node
            {
                return RetracePath(StartNode, EndNode);
            }

            
              var neighbours = OpenSet.Where(n => (n.position.x == current.position.x - 1 && n.position.y == current.position.y && !grid[current.position.x, current.position.y].HasWall(Wall.RIGHT)) ||                    //credit to micheal smith
                                                                (n.position.x == current.position.x + 1 && n.position.y == current.position.y && !grid[current.position.x, current.position.y].HasWall(Wall.LEFT)) ||
                                                                (n.position.x == current.position.x && n.position.y == current.position.y - 1 && !grid[current.position.x, current.position.y].HasWall(Wall.UP)) ||
                                                                (n.position.x == current.position.x && n.position.y == current.position.y + 1 && !grid[current.position.x, current.position.y].HasWall(Wall.DOWN)));

            foreach (Node node in neighbours)                                                                                                                                                                               //credit to micheal smith
            {
                float tempGScore = current.GScore + GetDistance(current, node);
                if (tempGScore < node.GScore)
                {//the new path is shorter, update the GScore and the parent (for pathing)
                    node.GScore = tempGScore;
                   // node.HScore = GetDistance(node, EndNode);
                    node.parent = current;

                    if(!OpenSet.Contains(node))
                    {
                        OpenSet.Add(node);
                    }
                }
            }
                     
        }
        return null;
    }
    int GetDistance(Node A, Node B)
    {
        int disX = Mathf.Abs(A.position.x - B.position.x);
        int disY = Mathf.Abs(A.position.y - B.position.y);

        if (disX > disY)
        {
            return 14 * disY + 10 * (disX - disY);
        }
        else return 14 * disX + 10 * (disY - disX);
    }


    List<Vector2Int> RetracePath(Node _startNode, Node _EndNode) //retraces path using parenting nodes
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = _EndNode;

        while(currentNode != _startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        //add start poss
        path.Reverse();
        return path;
    }
    private List<Node> CreateOpenList(Cell[,] _Grid, Vector2Int _StartPos, Vector2Int _EndPos)
    {
        List<Node> nodes = new List<Node>();
        float H = 0;


        for (int i = 0; i < _Grid.GetLength(0); i++) //loop through cell grid and make nodes
        {
            for (int j = 0; j < _Grid.GetLength(1); j++)
            {
                H = (int) (_Grid[i,j].gridPosition.x - _EndPos.x) + (_Grid[i, j].gridPosition.y - _EndPos.y);
                nodes.Add(new Node(_Grid[i, j].gridPosition, null, int.MaxValue, (int)H));
            }
        }
        return nodes;
    }

   
    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}


//List<Node> neighbours = new List<Node>();

//    for (int x = -1; x < 2; x++) //check 3x3 grid from current node
//    {
//        for (int y = -1; y < 2; y++)
//        {
//            if (x == 0 && y == 0) continue; //self

//            int cellX = current.position.x + x;
//            int cellY = current.position.y + y;

//            if(cellX >= 0 && cellX < width && cellY >= 0 && cellY < height) //if its a valid neigbour
//             //if cellx/y is bigger or equal to 0
//             //&& cellx/y smaller then width/height of the grid
//            {
//                Node aNeighbour = new Node(new Vector2Int(cellX, cellY), null, 0, 0); //fake neigbour 
//                neighbours.Add(aNeighbour);
//                //list of all surrounding nodes

//            }
//        }

//    }


//private List<Node> GetNeighbours(Node node)
//{
//    List<Node> result = new List<Node>();
//    for (int x = -1; x < 2; x++) //search 3x3 nodes
//    {
//        for (int y = -1; y < 2; y++)
//        {
//            if(x == 0 && y == 0)continue; //itself

//            int cellX = node.position.x + x;
//            int cellY = node.position.y + y;
//            if (cellX >= 0 && cellX < width && cellY >= 0 && cellY < height)
//            {
//                Node canditateCell = ;
//                result.Add(canditateCell);
//            }

//        }
//    }
//    return result;
//}

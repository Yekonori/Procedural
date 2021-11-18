using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

enum NodeRelativePos
{
    left,
    right,
    up,
    down
}

public class DungeonGraph : MonoBehaviour
{
    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();

    int sizeX = 11;
    int sizeY = 9;
    int maxIter = 10;
    int nbIter = 0;

    private int nbNodes = 8;
    Dictionary<Vector2Int, Node> allPos = new Dictionary<Vector2Int, Node>();

    private List<List<Node>> allNodes = new List<List<Node>>();
    private List<Connexion> allConnexions = new List<Connexion>();

    // Start is called before the first frame update
    void Start()
    {
        Restart();
    }

    void Restart()
    {
        ++nbIter;
        allNodes.Clear();
        allPos.Clear();
        allConnexions.Clear();

        try
        {
            //1st principal path
            nbNodes = Random.Range(6, 11);
            allNodes.Add(CreatePath(nbNodes, Vector2Int.zero, 0, true));

            //1st secondary path
            int randNode = Random.Range(0, nbNodes);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[0][randNode].position, 1, false));

            //2nd principal path
            nbNodes = Random.Range(6, 11);
            allNodes.Add(CreatePath(nbNodes, new Vector2Int(50, 50), 2, true));

            //2nd secondary path
            randNode = Random.Range(0, nbNodes);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[2][randNode].position, 3, false));

            //3rd principal path
            nbNodes = Random.Range(6, 11);
            allNodes.Add(CreatePath(nbNodes, new Vector2Int(100, 100), 4, true));

            //3rd secondary path
            randNode = Random.Range(0, nbNodes);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[4][randNode].position, 5, false));

            //3rd secondary path bis
            randNode = Random.Range(0, allNodes[4].Count);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[4][randNode].position, 6, false));

            foreach (var bla in allConnexions)
            {
                NodeRelativePos relativePos = GetRelativePos(bla.nodes[0], bla.nodes[1]);
                DOORSTATE state = bla.hasLock ? DOORSTATE.CLOSED : DOORSTATE.OPEN;
                switch (relativePos)
                {
                    case NodeRelativePos.left:
                        bla.nodes[0].doorRightOpen = state;
                        bla.nodes[1].doorLeftOpen = state;
                        break;
                    case NodeRelativePos.right:
                        bla.nodes[0].doorLeftOpen = state;
                        bla.nodes[1].doorRightOpen = state;
                        break;
                    case NodeRelativePos.up:
                        bla.nodes[0].doorDownOpen = state;
                        bla.nodes[1].doorUpOpen = state;
                        break;
                    case NodeRelativePos.down:
                        bla.nodes[0].doorUpOpen = state;
                        bla.nodes[1].doorDownOpen = state;
                        break;
                }
            }

            foreach (var bla in allPos)
            {
                List<GameObject> listRooms = GetRooms(bla.Value);
                if (listRooms.Count == 0)
                {
                    string errorMessage = $"No room with difficulty = {bla.Value.difficulty}";
                    errorMessage += bla.Value.doorDownOpen != DOORSTATE.WALL ? " and down door available" : "";
                    errorMessage += bla.Value.doorLeftOpen != DOORSTATE.WALL ? " and left door available" : "";
                    errorMessage += bla.Value.doorRightOpen != DOORSTATE.WALL ? " and right door available" : "";
                    errorMessage += bla.Value.doorUpOpen != DOORSTATE.WALL ? " and up door available" : "";
                    
                    Debug.LogError(errorMessage);
                    throw new System.Exception();
                }

                int rndRoom = Random.Range(0, listRooms.Count);
                GameObject go = Instantiate(listRooms[rndRoom]);

                go.transform.position = new Vector3(bla.Key.x * sizeX, bla.Key.y * sizeY, 0);
                Room room = go.GetComponent<Room>();
                room.SetDoor(room.NorthDoor, bla.Value.doorUpOpen);
                room.SetDoor(room.SouthDoor, bla.Value.doorDownOpen);
                room.SetDoor(room.WestDoor, bla.Value.doorLeftOpen);
                room.SetDoor(room.EastDoor, bla.Value.doorRightOpen);
                room.isStartRoom = (bla.Value.nodeType == Node.NodeType.start);
            }
        }
        catch
        {
            if (nbIter < maxIter)
            {
                Restart();
            }
            else Debug.LogError("Path unable to be created");
        }
    }

    List<Node> CreatePath(int nbNodes, Vector2Int pos, int blockNumber, bool isPrincipal)
    {
        List<Node> nodes = new List<Node>();

        for (int i = 0; i < nbNodes; ++i)
        {
            Node node = new Node();
            Connexion connexion = new Connexion();

            node.nodeType = Node.NodeType.standard;

            if (i == 0)
            {
                if (isPrincipal)
                {
                    node.position = pos;

                    if (blockNumber == 0) node.nodeType = Node.NodeType.start;

                    allPos.Add(pos, node);
                    connexion.nodes[0] = node;
                    allConnexions.Add(connexion);
                    nodes.Add(node);
                    pos = NextPosition(pos);
                }
                else
                {
                    connexion.nodes[0] = allPos[pos];
                    allConnexions.Add(connexion);
                    nodes.Add(allPos[pos]);
                    pos = NextPosition(pos);
                }
            }
            else if (i == nbNodes - 1)
            {
                node.position = pos;
                if (isPrincipal && blockNumber == 4)
                {
                    allConnexions.Last().hasLock = true;
                    node.nodeType = Node.NodeType.end;
                }
                else
                {
                    node.nodeType = Node.NodeType.teleport;
                }
                allPos.Add(pos, node);
                allConnexions.Last().nodes[1] = node;

                nodes.Add(node);
            }
            else
            {
                node.position = pos;
                allPos.Add(pos, node);

                allConnexions.Last().nodes[1] = node;
                connexion.nodes[0] = node;

                allConnexions.Add(connexion);
                nodes.Add(node);
                pos = NextPosition(pos);
            }
        }
        return nodes;
    }

    Vector2Int NextPosition(Vector2Int currentPos)
    {
        if (!CheckPath(currentPos))
        {
            throw new System.Exception();
        }

        Vector2Int newPos = currentPos;
        do
        {
            int nextIDOrientation = Random.Range(0, 4);
            newPos = currentPos;
            switch (nextIDOrientation)
            {
                case 0:
                    newPos.x++;
                    break;
                case 1:
                    newPos.x--;
                    break;
                case 2:
                    newPos.y++;
                    break;
                case 3:
                    newPos.y--;
                    break;
            }
        } while (allPos.ContainsKey(newPos));
        return newPos;
    }

    bool CheckPath(Vector2Int currentPos)
    {
        return !allPos.ContainsKey(new Vector2Int(currentPos.x + 1, currentPos.y)) ||
            !allPos.ContainsKey(new Vector2Int(currentPos.x - 1, currentPos.y)) ||
            !allPos.ContainsKey(new Vector2Int(currentPos.x, currentPos.y + 1)) ||
            !allPos.ContainsKey(new Vector2Int(currentPos.x, currentPos.y - 1));
    }

    NodeRelativePos GetRelativePos(Node node1, Node node2)
    {
        NodeRelativePos relPos = NodeRelativePos.down;

        if (node1.position.x == node2.position.x)
        {
            if (node1.position.y > node2.position.y)
            {
                relPos = NodeRelativePos.up;
            }
            else
            {
                relPos = NodeRelativePos.down;
            }
        }
        else
        {
            if (node1.position.x < node2.position.x)
            {
                relPos = NodeRelativePos.left;
            }
            else
            {
                relPos = NodeRelativePos.right;
            }
        }
        return relPos;
    }

    List<GameObject> GetRooms(Node node)
    {
        List<GameObject> rooms = new List<GameObject>();
        foreach(GameObject obj in roomPrefabs)
        {
            RoomConfig roomConfig = obj.GetComponent<RoomConfig>();
            if ((roomConfig.isStartRoom != (node.nodeType == Node.NodeType.start)) ||
                (roomConfig.isEndRoom != (node.nodeType == Node.NodeType.end)) ||
                (roomConfig.HasTeleporter != (node.nodeType == Node.NodeType.teleport)) ||
                (!roomConfig.eastDoorAvailable && node.doorRightOpen != DOORSTATE.WALL) ||
                (!roomConfig.westDoorAvailable && node.doorLeftOpen != DOORSTATE.WALL) ||
                (!roomConfig.southDoorAvailable && node.doorDownOpen != DOORSTATE.WALL) ||
                (!roomConfig.northDoorAvailable && node.doorUpOpen != DOORSTATE.WALL) ||
                (roomConfig.difficulty != node.difficulty))
                continue;

            rooms.Add(obj);
        }
        return rooms;
    }
}

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

    List<Node>[] nodeTP = new List<Node>[3];

    // Start is called before the first frame update
    void Start()
    {
        nodeTP[0] = new List<Node>();
        nodeTP[1] = new List<Node>();
        nodeTP[2] = new List<Node>();
        Restart();
    }

    void Restart()
    {
        ++nbIter;
        allNodes.Clear();
        allPos.Clear();
        allConnexions.Clear();
        nodeTP[0].Clear();
        nodeTP[1].Clear();
        nodeTP[2].Clear();

        try
        {
            //1st principal path
            nbNodes = Random.Range(6, 11);
            allNodes.Add(CreatePath(nbNodes, Vector2Int.zero, 0, true));

            //1st secondary path
            int randNode = Random.Range(0, nbNodes);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[0][randNode].position, 0, false));

            //2nd principal path
            nbNodes = Random.Range(6, 11);
            allNodes.Add(CreatePath(nbNodes, new Vector2Int(20, 0), 1, true));

            //2nd secondary path
            randNode = Random.Range(0, nbNodes);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[2][randNode].position, 1, false));

            //3rd principal path
            nbNodes = Random.Range(6, 11);
            allNodes.Add(CreatePath(nbNodes, new Vector2Int(40, 0), 2, true));

            //3rd secondary path
            randNode = Random.Range(0, nbNodes -1);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[4][randNode].position, 2, false));

            //3rd secondary path bis
            randNode = Random.Range(0, allNodes[4].Count -1);
            nbNodes = Random.Range(4, 7);
            allNodes.Add(CreatePath(nbNodes, allNodes[4][randNode].position, 2, false));

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

            // define which portal is associated with which
            AssignNodePortal();

            //Key creation
            KeyCreation();

            //Secret room
            Node secreNode = new Node();
            secreNode.nodeType = Node.NodeType.secret;
            Vector2Int secretPos = new Vector2Int(60, 0);
            secreNode.position = secretPos;
            //secreNode.associatedNode = allNodes[4].Last();
            allPos.Add(secretPos, secreNode);

            Dictionary<Node, Teleport> teleportOfNode = new Dictionary<Node, Teleport>();
            Dictionary<Teleport, Node> associatedNodeOfTeleport = new Dictionary<Teleport, Node>();

            Teleport endTeleport = null;

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

                if (bla.Value.nodeType == Node.NodeType.teleport)
                {
                    Teleport port = room.GetComponentInChildren<Teleport>();
                    teleportOfNode.Add(bla.Value, port);
                    associatedNodeOfTeleport.Add(port, bla.Value.associatedNode);
                }
                else if (bla.Value.nodeType == Node.NodeType.end)
                {
                    endTeleport = room.GetComponentInChildren<Teleport>();
                    endTeleport.canEnter = false;
                    TeleportManager.Get.endRoom = endTeleport;
                }
                else if (bla.Value.nodeType == Node.NodeType.secret)
                {
                    Teleport port = room.GetComponentInChildren<Teleport>();
                    TeleportManager.Get.secretRoom = port;
                }
            }

            AssignRoomPortal(teleportOfNode, associatedNodeOfTeleport);
        }
        catch (System.Exception e)
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
            node.blockNumber = blockNumber;
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
                if (isPrincipal && blockNumber == 2)
                {
                    allConnexions.Last().hasLock = true;
                    node.nodeType = Node.NodeType.end;
                }
                else
                {
                    node.nodeType = Node.NodeType.teleport;
                    nodeTP[blockNumber].Add(node);
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
                (roomConfig.HasKey != node.hasKey) ||
                (roomConfig.isSecretRoom != (node.nodeType == Node.NodeType.secret)) ||
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

    void AssignNodePortal()
    {
        Dictionary<int, portalObj> portals = new Dictionary<int, portalObj>()
        {
            {1, new portalObj {} },
            {2, new portalObj {} },
            {3, new portalObj {} },
            {4, new portalObj {} }
        };

        // Assignation des numeros aux Nodes teleport existants
        int nbBlock = -1;
        foreach (List<Node> listN in nodeTP)
        {
            ++nbBlock;

            int tpNb = Random.Range(1, 5);
            while (portals[tpNb].count == 2)
            {
                tpNb = Random.Range(1, 5);
            }
            listN[0].tpNumber = tpNb;
            ++portals[tpNb].count;
            portals[tpNb].blocks.Add(nbBlock);

            int[] availablePortNumber = GetAvailablePortNumber(tpNb);
            int random = Random.Range(0, 2);
            tpNb = availablePortNumber[random];
            if (portals[tpNb].count == 2)
            {
                tpNb = availablePortNumber[1 - random];
            }
            listN[1].tpNumber = tpNb;
            ++portals[tpNb].count;
            portals[tpNb].blocks.Add(nbBlock);
        }

        // recup num tp pas utilisés 2 fois avec le block ou il existe deja
        Dictionary<int, portalObj> tpNotUsed = new Dictionary<int, portalObj>();
        foreach (var kvp in portals)
        {
            if (kvp.Value.count < 2)
            {
                tpNotUsed.Add(kvp.Key, kvp.Value);
            }
        }

        // modif 2 Nodes into portals;
        foreach (var kvp in tpNotUsed)
        {
            //choix du block
            int random = Random.Range(0, 2);
            int[] blocks = new int[2];
            if (kvp.Value.blocks[0] == 0)
            {
                blocks[0] = 1;
                blocks[1] = 2;
            }
            else if (kvp.Value.blocks[0] == 1)
            {
                blocks[0] = 0;
                blocks[1] = 2;
            }
            else
            {
                blocks[0] = 0;
                blocks[1] = 1;
            }
            int block = blocks[random];

            //list des nodes possibles à modifier en tp
            List<Node> nodesToModify = allPos.Values.Where((node) => node.nodeType == Node.NodeType.standard && node.blockNumber == block).ToList();

            // modifier 1 aléatoire
            random = Random.Range(0, nodesToModify.Count);
            nodesToModify[random].nodeType = Node.NodeType.teleport;
            nodesToModify[random].tpNumber = kvp.Key;
        }

        List<Node> tpNodes = allPos.Values.Where((node) => node.nodeType == Node.NodeType.teleport).ToList();

        foreach (Node node in tpNodes)
        {
            node.associatedNode = tpNodes.First((n) => n.tpNumber == node.tpNumber && n != node);
        }
    }

    int[] GetAvailablePortNumber(int previousPortNb)
    {
        int[] availablePortNb = new int[2];
        if (previousPortNb == 1)
        {
            availablePortNb[0] = 4;
            availablePortNb[1] = 2;
        }
        else if (previousPortNb == 2)
        {
            availablePortNb[0] = 1;
            availablePortNb[1] = 3;
        }
        else if (previousPortNb == 3)
        {
            availablePortNb[0] = 2;
            availablePortNb[1] = 4;
        }
        else
        {
            availablePortNb[0] = 3;
            availablePortNb[1] = 1;
        }
        return availablePortNb;
    }

    class portalObj
    {
        public int count = 0;
        public List<int> blocks = new List<int>();
    }
    
    void AssignRoomPortal(Dictionary<Node, Teleport> teleportOfNode, Dictionary<Teleport, Node> associatedNodeOfTeleport)
    {
        foreach (var kvp in associatedNodeOfTeleport)
        {
            kvp.Key.SetValues(teleportOfNode[kvp.Value], kvp.Value.tpNumber);
        }
    }

    void KeyCreation()
    {
        List<Node> nodes = allPos.Values.Where((node) => node.nodeType == Node.NodeType.standard).ToList();
        int rand = Random.Range(0, nodes.Count);
        nodes[rand].hasKey = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGraph : MonoBehaviour
{
    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] GameObject test;
    [SerializeField] GameObject test2;

    int sizeX = 11;
    int sizeY = 9;

    private int nbNodes = 8;
    Dictionary<Vector2Int, Node> allPos = new Dictionary<Vector2Int, Node>();

    private List<List<Node>> allNodes = new List<List<Node>>();
    private List<Connexion> allConnexions = new List<Connexion>();

    // Start is called before the first frame update
    void Start()
    {
        Restart();
    }

    List<Node> CreatePath(int nbNodes, Vector2Int pos)
    {
        List<Node> nodes = new List<Node>();

        for (int i = 0; i < nbNodes; ++i)
        {
            Node node = new Node();
            Connexion connexion = new Connexion();

            if (i == 0)
            {
                if (!allPos.ContainsKey(pos))
                {
                    node.position = pos;
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

    void Restart()
    {
        allNodes.Clear();
        allPos.Clear();
        allConnexions.Clear();

        try
        {
            //1st principal path
            nbNodes = Random.Range(4, 10);
            allNodes.Add(CreatePath(nbNodes, Vector2Int.zero));

            //1st secondary path
            int randNode = Random.Range(0, nbNodes);
            nbNodes = Random.Range(2, 6);
            allNodes.Add(CreatePath(nbNodes, allNodes[0][randNode].position));

            //2nd principal path
            nbNodes = Random.Range(4, 10);
            allNodes.Add(CreatePath(nbNodes, allNodes[0].Last().position));

            //2nd secondary path
            randNode = Random.Range(0, nbNodes);
            nbNodes = Random.Range(2, 6);
            allNodes.Add(CreatePath(nbNodes, allNodes[2][randNode].position));

            //3rd principal path
            nbNodes = Random.Range(4, 10);
            allNodes.Add(CreatePath(nbNodes, allNodes[2].Last().position));

            foreach (var bla in allPos)
            {
                GameObject r = Instantiate(test);
                r.transform.position = new Vector3(bla.Key.x, bla.Key.y, 0);
            }
            foreach (var bla in allConnexions)
            {
                GameObject r = Instantiate(test2);
                Vector2 pos = Vector2.Lerp(bla.nodes[0].position, bla.nodes[1].position, 0.5f);
                r.transform.position = new Vector3(pos.x, pos.y, 0);

                if (bla.nodes[0].position.x == bla.nodes[1].position.x)
                {
                    r.transform.Rotate(Vector3.forward, 90f);
                }
            }
        }
        catch
        {
            Restart();
        }
    }

    void InstanciateRoom(Vector2Int pos)
    {
        int num = Random.Range(0, roomPrefabs.Count);
        GameObject go = Instantiate(roomPrefabs[num], new Vector3(pos.x * sizeX, pos.y * sizeY, 0), Quaternion.identity);
        Room room = go.GetComponent<Room>();
        room.isStartRoom = (pos == Vector2Int.zero);
    }
}

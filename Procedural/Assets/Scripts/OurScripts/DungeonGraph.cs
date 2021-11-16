using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph : MonoBehaviour
{
    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();

    int sizeX = 11;
    int sizeY = 9;

    private int nbNodes = 8;
    Vector2Int currentPos = Vector2Int.zero;
    List<Vector2Int> allPos = new List<Vector2Int>();
    Connexion oldConnexion = null;

    // Start is called before the first frame update
    void Start()
    {
        nbNodes = Random.Range(4, 20);

        for(int i = 0; i < nbNodes; ++i)
        {
            Node node = new Node();
            Connexion connexion = new Connexion();
            
            if (i == 0)
            {
                allPos.Add(currentPos);
                node.connexions.Add(connexion);
                connexion.nodes[0] = node;
                InstanciateRoom(currentPos);
                NextPosition();
                oldConnexion = connexion;
            }
            else if (i == nbNodes - 1)
            {
                node.position = currentPos;
                allPos.Add(currentPos);
                oldConnexion.nodes[1] = node;
                InstanciateRoom(currentPos);
            }
            else
            {
                node.position = currentPos;
                allPos.Add(currentPos);

                oldConnexion.nodes[1] = node;

                node.connexions.Add(connexion);
                connexion.nodes[0] = node;
                InstanciateRoom(currentPos);
                NextPosition();
                oldConnexion = connexion;
            }
        }

        foreach(var bla in allPos)
        {
            Debug.Log(bla);
        }
    }

    void NextPosition()
    {
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
        } while (allPos.Contains(newPos));
        currentPos = newPos;
    }

    void InstanciateRoom(Vector2Int pos)
    {
        int num = Random.Range(0, roomPrefabs.Count);
        GameObject go = Instantiate(roomPrefabs[num], new Vector3(pos.x * sizeX, pos.y * sizeY, 0), Quaternion.identity);
        Room room = go.GetComponent<Room>();
        room.isStartRoom = (pos == Vector2Int.zero);
    }
}

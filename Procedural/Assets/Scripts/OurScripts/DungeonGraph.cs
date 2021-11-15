using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph : MonoBehaviour
{
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

                NextPosition();
                oldConnexion = connexion;
            }
            else if (i == nbNodes - 1)
            {
                node.position = currentPos;
                allPos.Add(currentPos);
                oldConnexion.nodes[1] = node;
            }
            else
            {
                node.position = currentPos;
                allPos.Add(currentPos);

                oldConnexion.nodes[1] = node;

                node.connexions.Add(connexion);
                connexion.nodes[0] = node;

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
            int nextIDOrientation = Random.Range(1, 5);
            Utils.ORIENTATION orientation = (Utils.ORIENTATION)nextIDOrientation;
            newPos = currentPos;
            switch (orientation)
            {
                case Utils.ORIENTATION.EAST:
                    newPos.x++;
                    break;
                case Utils.ORIENTATION.WEST:
                    newPos.x--;
                    break;
                case Utils.ORIENTATION.NORTH:
                    newPos.y++;
                    break;
                case Utils.ORIENTATION.SOUTH:
                    newPos.y--;
                    break;
            }
        } while (allPos.Contains(newPos));
        currentPos = newPos;
    }
}

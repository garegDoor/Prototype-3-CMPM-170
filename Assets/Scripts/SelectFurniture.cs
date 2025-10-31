using UnityEditor.SearchService;
using UnityEngine;

public class SelectFurniture : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Material selectMat;
    [SerializeField] private GameObject selected;

    private Material originalMat;
    private RaycastHit hit;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selected = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(r, out hit))
            {
                Debug.Log("Player selected object:" + hit.transform.name);

                if (hit.transform.name == "Connection" && selected == null) // Player has nothing selected and is selecting a connection
                {
                    Debug.Log("Player has selected a connection!");
                    //selected = hit.transform.gameObject;
                    Select(hit.transform.gameObject);
                }
                else if (hit.transform.name == "Connection" && selected != null && hit.transform.gameObject != selected) // Player already has a connection selected and wants to combine it with the new connection
                {
                    ConnectFurniture(hit.transform.gameObject);
                    //selected = null;
                    Deselect();
                }
                else // Player is deselecting connection
                {
                    //selected = null;
                    Deselect();
                }
            }
            else
            {
                //selected = null;
                Deselect();
            }
        }
    }

    private void ConnectFurniture(GameObject con2)
    {
        GameObject movingPiece = selected.transform.parent.gameObject;
        Vector3 offset = new Vector3(movingPiece.transform.position.x - selected.transform.position.x, movingPiece.transform.position.y - selected.transform.position.y, movingPiece.transform.position.z - selected.transform.position.z);
        movingPiece.transform.position = con2.transform.position + offset;
    }

    private void Select(GameObject ob)
    {
        originalMat = ob.GetComponent<Renderer>().material;
        selected = ob;
        selected.GetComponent<Renderer>().material = selectMat;
    }
    private void Deselect()
    {
        if (selected == null) return;

        selected.GetComponent<Renderer>().material = originalMat;
        selected = null;
    }
}

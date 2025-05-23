using UnityEngine;

public class MapAttributeEvent3 : MonoBehaviour
{
    private Node _previousNodeHauteur1;
    private Node _previousNodeHauteur2;
    private Node _previousNodeHauteur3;
    private Node _actualNodeHauteur1;
    private Node _actualNodeHauteur2;
    private Node _actualNodeHauteur3;

    public void SetupEventNode()
    {
        foreach(Node node in MapMaker2.Instance.AllNodeGood)
        {
            //if(MapBuildingTools.Instance.ReturnListOfNodeFromNodePosition())
        }
    }
}

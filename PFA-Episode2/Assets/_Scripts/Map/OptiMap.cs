using UnityEngine;

public class OptiMap : MonoBehaviour
{
    public static OptiMap Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ActiveNode()
    {
        foreach (Node node in MapMaker2.Instance.DicoNode.Values)
        {
            if (node.Position > PlayerMap.Instance.PositionMap + 3)
            {
                node.gameObject.SetActive(false);
                foreach (GameObject obj in node.PathBetweenNode) { obj.SetActive(false); }
            }
        }
    }
}

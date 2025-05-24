using System.Collections.Generic;
using UnityEngine;
using static NodeTypes;

public class MapAttributeEvent3 : MonoBehaviour
{
    private NodesEventTypes _previousNodeSetup;
    private int NumberBeforeHeal;
    private int NumberHeal;

    [Header ("Probability Section")]
    [SerializeField] private int _probaCuisine = 17;
    [SerializeField] private int _probaIngredient = 17;
    [SerializeField] private int _probaCombat = 50;
    [SerializeField] private int _probaHeal = 16;

    [Header("Rules Section")]
    [SerializeField] private int NumberOfConsecutiveHealMax;

    public static MapAttributeEvent3 Instance;

    private void Awake() { Instance = this; }

    public void SetupEventNode() //L'objectif est de parcourir chaque node et d'attribu� leur r�le selon leur position
    {
        for (int index = 4; index < MapMaker2.Instance.MapRange - 1; index++)
        {
            List<Node> _listOfNodeAtPosition = MapBuildingTools.Instance.ReturnListOfNodeFromNodePosition(index);
            foreach (Node node in _listOfNodeAtPosition) { AttributeEventNode(node); }
        }
        foreach(Node node in MapMaker2.Instance.AllNodeGood) { node.SetupSprite(); }
    }

    private void AttributeEventNode(Node SamePositionNode)
    {
        int Total = _probaCuisine + _probaCombat + _probaIngredient + _probaHeal;
        int result = Random.Range(1, Total + 1);

        if (result <= _probaCuisine && _previousNodeSetup != NodesEventTypes.Cuisine && SamePositionNode.Creator.EventName != NodesEventTypes.Cuisine)
        {
            SamePositionNode.EventName = NodesEventTypes.Cuisine;
            _previousNodeSetup = NodesEventTypes.Cuisine;
        }
        else if (result <= _probaCombat + _probaCuisine && _previousNodeSetup != NodesEventTypes.Combat && SamePositionNode.Creator.EventName != NodesEventTypes.Combat)
        {
            SamePositionNode.EventName = NodesEventTypes.Combat;
            _previousNodeSetup = NodesEventTypes.Combat;
        }
        else if (result <= _probaIngredient + _probaCombat + _probaCuisine && _previousNodeSetup != NodesEventTypes.Ingredient && SamePositionNode.Creator.EventName != NodesEventTypes.Ingredient)
        {
            SamePositionNode.EventName = NodesEventTypes.Ingredient;
            _previousNodeSetup = NodesEventTypes.Ingredient;
        }
        else if (_previousNodeSetup != NodesEventTypes.Heal && SamePositionNode.Creator.EventName != NodesEventTypes.Heal)
        {
            if (NumberBeforeHeal < NumberOfConsecutiveHealMax)
            {
                SamePositionNode.EventName = NodesEventTypes.Combat;
                _previousNodeSetup = NodesEventTypes.Combat;
                NumberBeforeHeal++;
                print("Combat");
            }
            else if (NumberHeal <= 2)
            {
                SamePositionNode.EventName = NodesEventTypes.Heal;
                _previousNodeSetup = NodesEventTypes.Heal;
                NumberHeal++;
            }
            else { AttributeEventNode(SamePositionNode); }
        }
        else { AttributeEventNode(SamePositionNode); }  //Je relance car j'ai un tirage pas possible
    }
}

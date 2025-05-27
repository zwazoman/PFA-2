using System.Collections.Generic;
using UnityEngine;

public class SetupSpellInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _targetInventory = new();
    [SerializeField] private List<Transform> _equippedInventory = new();
    [SerializeField] private GameObject _prefabItem;
    private int _index = 0;
    private GetInfoInVariant _refInfoVariant;
    public SpellData SpellChoose;
    [HideInInspector] public GameObject ConnardDeMes2;

    private void Start()
    {
        SetupInventory();
    }

    public void SetupInventory()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Spells.Count; i++)
        {
            SpellChoose = GameManager.Instance.playerInventory.Spells[i];

            GameObject go = Instantiate(_prefabItem, _targetInventory[i]); //Création 
            _refInfoVariant = go.GetComponent<GetInfoInVariant>();
            _refInfoVariant.IndexInPlayerSpell = i;
            _refInfoVariant.SpellIcon.sprite = SpellChoose.Sprite; //Sprite
            _refInfoVariant.SpellIconDisable.sprite = SpellChoose.Sprite;

            _refInfoVariant.SpellName.text = SpellChoose.Name; //Nom
            for (int index = 0; index < SpellChoose.Effects.Count; index++) //Effets
            {
                SpellEffect spellEffect = SpellChoose.Effects[index];
                _refInfoVariant.Effect.text = Serializer.GetSpellEffectString(spellEffect);
            }

            if (_refInfoVariant.SpellZoneEffect.sprite != null) //Sécurité
            { _refInfoVariant.SpellZoneEffect.sprite = SpellChoose.AreaOfEffect.sprite; } //Area

            foreach (int spellDataIndex in GameManager.Instance.playerInventory.playerEquipedSpellIndex) //pour chaque spell qu'on construit on vérifie si il est equipe
            {
                if(i == spellDataIndex)
                {
                    ConnardDeMes2 = go; //L'objet entier
                    ConnardDeMes2.GetComponent<DraggableSpellContainer>()._faudraRemove = true;
                    ConnardDeMes2.GetComponent<DraggableSpellContainer>().Target = _targetInventory[i];
                    Transform parent = go.transform.parent; //SpellSlot
                    GameObject enfant = ConnardDeMes2.transform.GetChild(1).gameObject; //L'image disable

                    ConnardDeMes2.transform.SetParent(_equippedInventory[_index].gameObject.transform);
                    ConnardDeMes2.transform.localPosition = Vector3.zero;
                    enfant.SetActive(true);
                    enfant.transform.SetParent(parent);
                    enfant.transform.localPosition = Vector3.zero;
                    ConnardDeMes2.transform.GetComponent<DraggableSpellContainer>().originalParent = enfant.transform.parent;
                    _index++;
                }
            }
        }
    }

    public void PourNestror()
    {
        GameManager.Instance.playerInventory.playerEquipedSpell.Clear();
        foreach (int spellDataIndex in GameManager.Instance.playerInventory.playerEquipedSpellIndex)
        {
            GameManager.Instance.playerInventory.playerEquipedSpell.Add(GameManager.Instance.playerInventory.Spells[spellDataIndex]);
        }
    }
}

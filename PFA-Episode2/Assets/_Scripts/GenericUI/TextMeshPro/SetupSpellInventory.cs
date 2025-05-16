using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupSpellInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _targetInventory = new();
    [SerializeField] private List<Transform> _equippedInventory = new();
    [SerializeField] private GameObject _prefabItem;
    private int _index = 0;
    private GetInfoInVariant _refInfoVariant;
    public SpellData SpellChoose;
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
            _refInfoVariant.ActualSpell = SpellChoose;
            _refInfoVariant.SpellIcon.sprite = SpellChoose.Sprite; //Sprite
            _refInfoVariant.SpellIconDisable.sprite = SpellChoose.Sprite;

            _refInfoVariant.SpellName.text = SpellChoose.Name; //Nom
            for (int index = 0; index < SpellChoose.Effects.Count; index++) //Effets
            {
                SpellEffect spellEffect = SpellChoose.Effects[index];
                _refInfoVariant.Effect.text = spellEffect.ToString();
            }

            if (_refInfoVariant.SpellZoneEffect.sprite != null) //Sécurité
            { _refInfoVariant.SpellZoneEffect.sprite = SpellChoose.AreaOfEffect.sprite; } //Area

            foreach(SpellData spellData in GameManager.Instance.playerEquipedSpell) //pour chaque spell qu'on construit on vérifie si il est equipe
            {
                if (SpellChoose == spellData) //Il est équipé
                {
                    go.transform.GetChild(0).gameObject.SetActive(true);
                    gameObject.transform.SetParent(_equippedInventory[_index].gameObject.transform);
                    gameObject.transform.localPosition = Vector3.zero;
                    _index++;
                }
            }
        }
    }

    //public void SaveSpellEquiped() //Button
    //{
    //    for (int i = 0; i < GameManager.Instance.playerEquipedSpell.Count; i++)
    //    {
    //        GameManager.Instance.playerEquipedSpell[i] = gameObject.transform.parent.GetComponent<SetupSpellInventory>().SpellChoose;
    //    }
    //    print("c'est save... ?");
    //}
}

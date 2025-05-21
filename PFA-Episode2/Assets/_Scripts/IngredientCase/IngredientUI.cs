using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
    public TextMeshProUGUI title; //Titre
    public Image imageLogoRef; //Sprite

    public Image rarityFrame; //Cadre

    public Image famillyPanelColorLight; //Panel 1
    public List<Image> famillyPanelColorDark = new(); //Panel3

    public TextMeshProUGUI effectDescription;

    public GameObject SpriteZone;
    public GameObject Porte;
    public TextMeshProUGUI PorteTxt;

    public TextMeshProUGUI familly;
}

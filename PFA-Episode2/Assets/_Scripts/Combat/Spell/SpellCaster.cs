using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    public async void PreviewSpell()
    {
        //attache le spell au doigt du joueur. tire un raycast dans la direction du doigt. si il est sur une case, preview la zone du sort et ses dégâts. si le joueur relache dans le vide/sur une case non accessible : reset, sinon, await CastSpell(spell)
    }

    public async UniTask CastSpell()
    {
        while(true /* tant que le joueur a pas relaché le spell*/)
        {
            await UniTask.Yield();
        }
    }
}

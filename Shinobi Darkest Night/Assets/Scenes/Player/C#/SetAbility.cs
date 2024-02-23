using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAbility : MonoBehaviour
{
    public AddAbility Itaiken;
    public PS_Start_Itaiken ps_itaiken = new PS_Start_Itaiken();


    public AddAbility Shokyaku;
    public PS_Start_Shokyaku ps_shokyaku = new PS_Start_Shokyaku();

    public AddAbility Desumiru;
    public PS_Start_Desumiru ps_desumiru = new PS_Start_Desumiru(); 

    public AddAbility Heal;
    PS_Start_Heal ps_Heal = new PS_Start_Heal();

    public List<Ability> Abilities = new List<Ability>();

    private PlayerStateMachine player;
    private int curentKey;

    void Awake()
    {
        player = GetComponent<PlayerStateMachine>();
        curentKey = 1;
        if (Itaiken.add)
        {
            Ability ab = new Ability();
            ab.state = ps_itaiken;
            ab.key = Itaiken.key;
            Abilities.Add(ab);
        }
        if (Shokyaku.add)
        {
            Ability ab = new Ability();
            ab.state = ps_shokyaku;
            ab.key = Shokyaku.key;
            Abilities.Add(ab);
        }
        if (Desumiru.add)
        {
            Ability ab = new Ability();
            ab.state = ps_desumiru;
            ab.key = Desumiru.key;
            Abilities.Add(ab);
        }
        if (Heal.add)
        {
            Ability ab = new Ability();
            ab.state = ps_Heal;
            ab.key = Heal.key;
            Abilities.Add(ab);
        }

        while (curentKey <= 4)
        {
            AddThis(curentKey);
            curentKey++;
        }
    }

    private void AddThis(int keyNum)
    {
        foreach (Ability ab in Abilities)
        {
            if (ab.key == curentKey)
            {
                player.ability.Add(ab.state);
            }
        }
    }

}

[System.Serializable]
public class AddAbility
{
    public bool add;
    public int key;
}

[System.Serializable]
public class Ability
{ 
    public PlayerState state;
    public int key;
}
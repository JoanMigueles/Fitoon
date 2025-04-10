using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Obsolete]
public class GameOver : MonoBehaviour
{
    //Debe de ejecutarse este script cuando el jugador acaba la partida por completo, ganando (1 puesto) o perdiendo

    //Primer puesto: Medalla de oro
    //Puesto 20% o superior: Medalla de plata
    //Puesto 50% o superior: Medalla de bronce 
    //Puesto menor al 50%: Sin medalla (loser points) -> Perder en la primera carrera


    [SerializeField] int goldPoints, silverPoints, bronzePoints, loserPoints;
    [SerializeField] int goldMoney, silverMoney, bronzeMoney;
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] TextMeshProUGUI moneyText;
    private void Awake()
    {
    }
    void Start()
    {
		SaveData.ReadFromJson();
        CalculateReward();
		SaveData.SaveToJson();
    }

    private void CalculateReward()
    {
        
    }

    public void PlayAgain()
    {
        
    }
}

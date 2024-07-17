﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateParameters : MonoBehaviour
{
    SaveData save;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI diamondsText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI pointsLeftText;
    public TextMeshProUGUI levelText;
    [SerializeField] Slider xpSlider;

    [Header("Level progression")]
    public int currentLevel;
    int xpForNextLevel;
    public float baseXP = 100f;
    public float factor = 1.5f;

    private void Start()
    {
        save = GetComponent<SaveData>();
        UpdateMoneyText();
        UpdatePoints();
    }

    public void UpdateMoneyText()
    {
        coinsText.text = save.player.normalCoins.ToString();
        diamondsText.text = save.player.diamondsCoins.ToString();
        
    }

    //Progresion de puntos
    //Se utiliza una progresion geométrica de manera que sea un aumento exponencial de dificultad. De esta manera ganar nivel al principio es sencillo para motivar a jugar, pero
    //subir de nivel en un momento más avanzado es más difícil. Implica jugar muchas partidas o pagar para conseguir monedas.
    //Formula: PuntosNecesarios(nivel)=base*factor elevado a (nivel−1)
    private void Update()
    {
        //A modo de testing, para subir puntos
        if (Input.GetKeyUp(KeyCode.P))
        {
            save.player.points += 50;
            UpdatePoints();
        }
        else if (Input.GetKeyUp(KeyCode.O))
        {
            save.player.points -= 50;
            UpdatePoints();
        }
    }
    public void UpdatePoints()
    {
        currentLevel = CalculateLevelForXP(save.player.points);
        levelText.text = currentLevel.ToString();

        int totalXPNeededForNextLevel = CalculateTotalXPNeededForNextLevel(currentLevel);
        int pointsNeededForNextLevel = totalXPNeededForNextLevel - save.player.points;

        pointsText.text = $"{save.player.points}/{totalXPNeededForNextLevel} xp";
        pointsLeftText.text = $"{pointsNeededForNextLevel} left";

        xpSlider.maxValue = totalXPNeededForNextLevel;
        xpSlider.value = save.player.points;
    }

    //Ejemplo con base 100 y factor 1.5: Para llegar al nivel 1 hacen falta 0+100 puntos. Nivel 2: 100+150 puntos. Nivel 3: 100+150+225 puntos...
    private int CalculateXPForNextLevel(int level)
    {
        return Mathf.CeilToInt(baseXP * Mathf.Pow(factor, level - 1));
    }

    public int CalculateLevelForXP(int xp)
    {
        int level = 1;
        int xpForNextLevel = Mathf.CeilToInt(baseXP);

        while (xp >= xpForNextLevel)
        {
            xp -= xpForNextLevel;
            level++;
            xpForNextLevel = CalculateXPForNextLevel(level);
        }

        return level;
    }

    private int CalculateTotalXPNeededForNextLevel(int currentLevel)
    {
        int totalXP = 0;

        for (int level = 1; level <= currentLevel; level++)
        {
            totalXP += CalculateXPForNextLevel(level);
        }

        return totalXP;
    }
}

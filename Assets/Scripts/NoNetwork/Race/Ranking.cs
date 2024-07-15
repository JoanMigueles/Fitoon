using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    [SerializeField] GameObject rankingPosPrefab;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject content;
    RectTransform playerEntry;

    private void Start()
    {
        //Clean ranking
          for (int i = 0; i < content.transform.childCount; i++)
          {
              Destroy(content.transform.GetChild(i).gameObject);
          }

          UpdateRanking();

        Canvas.ForceUpdateCanvases();
        //ScrollViewFocusFunctions.FocusOnItem(scrollRect, playerEntry);
        StartCoroutine(ScrollViewFocusFunctions.FocusOnItemCoroutine(scrollRect, playerEntry, 0.5f));
    }
    void UpdateRanking()
    {
        for (int i = 0; i < RaceManager.Instance.raceBots.Count; i++)
        {
            GameObject rankingEntry = Instantiate(rankingPosPrefab, content.transform);
            //posicion
            rankingEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"#{i+1}";
            //nombre
            rankingEntry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = RaceManager.Instance.raceBots[i].botID;

            if (RaceManager.Instance.raceBots[i].isPlayer)
            {
                playerEntry = rankingEntry.GetComponent<RectTransform>();
                rankingEntry.GetComponent<Image>().color = Color.green;
            }
        }
    }

}

using System.Buffers.Text;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Menu Window Dark Background")]
    [SerializeField] private GameObject darkenBackground;

    [Header("Private Match Passcode")]
    [SerializeField] private TMP_InputField passcodeField;

    [Header("Player Resources And Stats")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI medalText;
    [SerializeField] private Slider medalSlider;
    [SerializeField] private TextMeshProUGUI leagueText;
    [SerializeField] private Image leagueImage;
    [SerializeField] private TextMeshProUGUI streakText;
    public float baseXP = 100f;
    public int rankMedalInterval = 500;

    [Header("Player Data")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private GameObject banner;
    [SerializeField] private Image profileIconImage;


    private void Start()
    {
        SaveData.ReadFromJson();
        UpdateAllUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete)) {
            Debug.Log("Erased");
            SaveData.ResetPlayerData();
            SaveData.SaveToJson();
            UpdateAllUI();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            
            SaveData.player.normalCoins += 20;
            SaveData.SaveToJson();
            UpdateCoins();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)) {

            SaveData.player.expPoints += 50;
            SaveData.SaveToJson();
            UpdateExp();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {

            SaveData.player.medals += 50;
            SaveData.SaveToJson();
            UpdateMedals();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {

            SaveData.player.medals -= 50;
            SaveData.SaveToJson();
            UpdateMedals();
        }

    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------
    // Menu and scene management
    // --------------------------------------------------------------------------------------------------------------------------------------------------

    public void OpenMenu(GameObject menu)
    {
        if (menu != null) {
            menu.SetActive(true);
            if (darkenBackground != null) darkenBackground.SetActive(true);
        }
    }
    public void CloseMenu(GameObject menu)
    {
        if (menu != null) {
            menu.SetActive(false);
            if (darkenBackground != null) darkenBackground.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------
    // Data display
    // --------------------------------------------------------------------------------------------------------------------------------------------------

    public void SaveUsername(string value)
    {
        SaveData.player.username = value;
        SaveData.SaveToJson();
        UpdateProfile();
    }

    public void UpdateAllUI()
    {
        UpdateExp();
        UpdateCoins();
        UpdateMedals();
        UpdateProfile();
        UpdateStreak();
    }

    public void UpdateCoins()
    {
        if (coinsText != null) coinsText.text = SaveData.player.normalCoins.ToString();
    }

    public void UpdateExp()
    {
        int currentLevel = CalculateLevelFromXP(SaveData.player.expPoints);
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
        }
        if (expText != null) {
            int totalXPNeededForNextLevel = CalculateTotalXPNeededForNextLevel(currentLevel);
            int pointsNeededForNextLevel = totalXPNeededForNextLevel - SaveData.player.expPoints;

            expText.text = $"{SaveData.player.expPoints}/{totalXPNeededForNextLevel}";
        }

        
    }

    public void UpdateMedals()
    {

        if (medalText != null) medalText.text = SaveData.player.medals.ToString();
        if (medalSlider != null) {
            medalSlider.minValue = SaveData.player.medals / rankMedalInterval * rankMedalInterval;
            medalSlider.maxValue = ((SaveData.player.medals / rankMedalInterval) + 1) * rankMedalInterval;
            medalSlider.value = SaveData.player.medals;
        }
    }

    public void UpdateStreak()
    {
        if (streakText != null) streakText.text = SaveData.player.streak.ToString();
    }

    public void UpdateProfile()
    {
        
        if (playerNameText != null) playerNameText.text = SaveData.player.username;
        if (playerNameField != null) playerNameField.text = SaveData.player.username;

    }
    // --------------------------------------------------------------------------------------------------------------------------------------------------
    // Progresion de puntos
    // --------------------------------------------------------------------------------------------------------------------------------------------------
    //Se utiliza una progresion geométrica de manera que sea un aumento exponencial de dificultad. De esta manera ganar nivel al principio es sencillo para motivar a jugar, pero
    //subir de nivel en un momento más avanzado es más difícil. Implica jugar muchas partidas o pagar para conseguir monedas.
    //Formula: PuntosNecesarios(nivel)=base*factor elevado a (nivel−1)


    //Ejemplo con base 100 y factor 1.5: Para llegar al nivel 1 hacen falta 0+100 puntos. Nivel 2: 100+150 puntos. Nivel 3: 100+150+225 puntos...
    private int CalculateXPForNextLevel(int level)
    {
        return Mathf.CeilToInt(baseXP * Mathf.Pow(1.5f, level - 1));
    }

    public int CalculateLevelFromXP(int exp)
    {
        int level = 1;
        int xpForNextLevel = Mathf.CeilToInt(baseXP);

        while (exp >= xpForNextLevel) {
            exp -= xpForNextLevel;
            level++;
            xpForNextLevel = CalculateXPForNextLevel(level);
        }

        return level;
    }

    private int CalculateTotalXPNeededForNextLevel(int currentLevel)
    {
        int totalXP = 0;

        for (int level = 1; level <= currentLevel; level++) {
            totalXP += CalculateXPForNextLevel(level);
        }

        return totalXP;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------
    // Lobby
    // --------------------------------------------------------------------------------------------------------------------------------------------------
    public void ExitApp()
    {
        Application.Quit();
    }

    public void SetPasscode(string value)
    {
        DiscoveryHandler.Passcode = value;
    }

    public void JoinLobby(bool privateLobby)
    {
        if (!privateLobby) DiscoveryHandler.Passcode = null;
        Debug.Log("Passcode: " + DiscoveryHandler.Passcode);
        SceneManager.LoadScene("LobbyScene");
    }
}

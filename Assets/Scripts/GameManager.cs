using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour {

    [SerializeField] Text bestScoreText;
    [SerializeField] InputField nameField;

    public string playerName = "";
    public string bestName;
    public int bestScore;

    public static GameManager Instance;

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGameInfo();
    }

       private void OnDisable() {
        if (Instance == this) {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        UpdateBestScoreText(); // Update best score text on startup
    }

    public void SetBestScore(int score) {
        if(score > bestScore) {
            bestScore = score;
            bestName = playerName;
            SaveGameInfo();
            if (bestScoreText != null) {
            UpdateBestScoreText();
        }
        }
        Debug.Log("Score: " + score + "  Player: " + playerName);
    }

    public void StartNew() {
        LoadGameInfo(); // Load data when starting
        if(nameField.text != "") {
            playerName = nameField.text;
            SceneManager.LoadScene(1); // Load Main Game Scene
        } else {
            Debug.LogWarning("Please enter a name!");
        }
    }

    public void Exit() {
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #else
                Application.Quit();
        #endif
            }

    [System.Serializable]
    class SaveData {
        public string name;
        public int bestScore;
    }

    public void SaveGameInfo() {
        SaveData data = new SaveData {
            name = bestName,
            bestScore = bestScore
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log($"Saved Best Name: {bestName}, Best Score: {bestScore}");
    }

    public void LoadGameInfo() {
    string path = Application.persistentDataPath + "/savefile.json";

    if (File.Exists(path)) {
        string json = File.ReadAllText(path);   

        SaveData data = JsonUtility.FromJson<SaveData>(json);   


        // Check if data is null
        if (data != null) {
            bestName = data.name;
            bestScore = data.bestScore;
            Debug.Log($"Loaded Best Name: {bestName}, Best Score: {bestScore}");
        } else {
            Debug.LogError("Failed to load save data.");
        }
    } else {
        Debug.LogWarning("Save file not found!");
    }

    // Handle null values as a fallback
    if (string.IsNullOrEmpty(bestName)) {
        bestName = "Default Player"; // Set a default name
    }
    }

    public void SavePlayerPrefs() {
        PlayerPrefs.SetString("BestPlayerName", bestName);
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() {
        SavePlayerPrefs();
        SaveGameInfo();
    }

    public void UpdateBestScoreText() {
        if(bestScoreText != null) {
            bestScoreText.text = $"{bestName} : {bestScore}";
        } else {
            Debug.LogWarning("Best Score Text is not assigned!");
        }
    }
}

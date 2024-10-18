using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour {

    [SerializeField] Text bestScoreText; // Assign this in the Inspector
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

    private void Start() {
        UpdateBestScoreText(); // Update best score text on startup
    }

    public void SetBestScore(int score) {
        if(score > bestScore) {
            bestScore = score;
            bestName = playerName;
            SaveGameInfo();
            UpdateBestScoreText(); // Update text when a new best score is set
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

        if(File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestName = data.name;
            bestScore = data.bestScore;
            Debug.Log($"Loaded Best Name: {bestName}, Best Score: {bestScore}");
        } else {
            Debug.LogWarning("Save file not found!");
        }

        // Load best name from PlayerPrefs as a fallback
        bestName = PlayerPrefs.GetString("BestPlayerName", bestName);
    }

    public void SavePlayerPrefs() {
        PlayerPrefs.SetString("BestPlayerName", bestName);
        PlayerPrefs.SetInt("BestScore", bestScore);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() {
        SavePlayerPrefs(); // Save PlayerPrefs when the application quits
        SaveGameInfo(); // Save game info to file when the application quits
    }

    public void UpdateBestScoreText() {
        if(bestScoreText != null) {
            bestScoreText.text = $"{bestName} : {bestScore}";
        } else {
            Debug.LogWarning("Best Score Text is not assigned!");
        }
    }
}

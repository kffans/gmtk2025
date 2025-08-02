using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Słownik poziomów: nazwa sceny -> indeks
    public Dictionary<string, int> Levels = new Dictionary<string, int>()
    {
        {"SceneStartLevel", 0},
        {"SceneLevelFire", 1},
        {"SceneLevelIce", 2},
        {"SceneLevelInvisibility", 3}
    };

    // Graf skierowany: klucz to "z", wartość to "do"
    public Dictionary<int, int> DirectedGraph = new Dictionary<int, int>();

    private int currentLevelIndex;

    public enum PortalType { NEXT, BEFORE }
    public GameObject gameOverUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentLevelIndex = 0;

        if (SceneManager.GetActiveScene().name == "SceneStartLevel")
        {
            AudioManager.instance?.PlayMusic("game");
        }
    }

    public void OnPortalEntered(PortalType portalType)
    {
        switch (portalType)
        {
            case PortalType.NEXT:
                MoveToNextLevel();
                break;
            case PortalType.BEFORE:
                MoveBack();
                break;
        }
    }

    public void StumbledOnEnemy()
    {
        Debug.Log("Zderzono się z przeciwnikiem");
        Debug.Log($"{gameOverUI}");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f; // Zatrzymaj grę
        }
        else
        {
            Debug.Log("dupa");
         }
    }

    public void StartOver()
    {
        Time.timeScale = 1f;
        DirectedGraph.Clear();
        currentLevelIndex = 0;
        
        // Przeładuj scenę całkowicie od nowa
        SceneManager.LoadScene("SceneStartLevel", LoadSceneMode.Single);
        
        // Usuń starą instancję przed załadowaniem nowej sceny
        Destroy(gameObject);
    }

    private void MoveToNextLevel()
    {
        // Sprawdzamy, czy z aktualnego poziomu mamy już połączenie wychodzące
        if (DirectedGraph.TryGetValue(currentLevelIndex, out int nextIndex))
        {
            // Jeśli tak, to ładujemy połączony poziom
            Debug.Log($"Existing connection found: {currentLevelIndex} -> {nextIndex}");
            LoadLevel(nextIndex);
        }
        else
        {
            // Jeśli nie, losujemy nowy poziom (który nie jest obecnym i nie ma jeszcze połączenia)
            nextIndex = GetRandomUnconnectedLevel();
            if (nextIndex != -1)
            {
                Debug.Log($"No existing connections. Creating new: {currentLevelIndex} -> {nextIndex}");

                // Dodajemy połączenie w grafie skierowanym
                DirectedGraph.Add(currentLevelIndex, nextIndex);

                LoadLevel(nextIndex);
            }
            else
            {
                SceneManager.LoadScene("SceneLevelBoss");
            }
        }
    }

    private void MoveBack()
    {
        // Szukamy poziomu który ma połączenie do naszego aktualnego poziomu
        var previousLevel = DirectedGraph.FirstOrDefault(x => x.Value == currentLevelIndex);
        
        if (previousLevel.Key != 0 || DirectedGraph.ContainsKey(0)) // Sprawdzamy czy znaleźliśmy prawidłowe połączenie
        {
            Debug.Log($"Moving back from {currentLevelIndex} to {previousLevel.Key}");
            LoadLevel(previousLevel.Key);
        }
        else
        {
            Debug.Log($"Cannot move back - no incoming connections to level {currentLevelIndex}");
        }
    }

    private void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        string sceneName = Levels.FirstOrDefault(x => x.Value == levelIndex).Key;
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }

    private int GetRandomUnconnectedLevel()
    {
        List<int>  possibleLevels = Levels.Values
            .Where(value => !DirectedGraph.ContainsKey(value) && value != currentLevelIndex)
            .ToList();
        
        // Jeśli wszystkie poziomy są już połączone, losujemy z wszystkich oprócz obecnego
        if (possibleLevels.Count == 0)
        {
            return -1;
        }
        
        return possibleLevels[Random.Range(0, possibleLevels.Count)];
    }
}
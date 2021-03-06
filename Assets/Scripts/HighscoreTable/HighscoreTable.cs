using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HighscoreTable : MonoBehaviour
{

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;
    private int count = 0;



    private void Awake()    
        { 
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");       
        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable" + SceneManager.GetActiveScene().name + WordManager.letters);
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        

        if (highscores == null)
        {

            AddHighscoreEntry(0, " ");

            jsonString = PlayerPrefs.GetString("highscoreTable" + SceneManager.GetActiveScene().name + WordManager.letters);
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            if (count < 10)
            {
                CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
            }
            count++;
        }
    }

   

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
      
     
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + " ."; break;

            case 1: rankString = "1 ."; break;
            case 2: rankString = "2 ."; break;
            case 3: rankString = "3 ."; break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;

        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Set background visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        // Highlight First
      /*  if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().fontStyle = FontStyle.Normal;
            entryTransform.Find("scoreText").GetComponent<Text>().fontStyle = FontStyle.Normal;
            entryTransform.Find("nameText").GetComponent<Text>().fontStyle = FontStyle.Normal;
        }*/
        if (highscoreEntry.name == UserName.name &&highscoreEntry.name != "OYUNCU")
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.blue;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.blue;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.blue;
        }

        // Set tropy
        switch (rank)
        {
            default:
                entryTransform.Find("trophy").gameObject.SetActive(false);
                break;
            case 1:
                entryTransform.Find("trophy").GetComponent<Image>().color = Color.yellow;
                break;
            case 2:
                entryTransform.Find("trophy").GetComponent<Image>().color = Color.gray;
                break;
            case 3:
                entryTransform.Find("trophy").GetComponent<Image>().color = Color.white;
                break;

        }
       
        transformList.Add(entryTransform);
    }

    public void AddHighscoreEntry(int score, string name)
    {


        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };

        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable" + SceneManager.GetActiveScene().name + WordManager.letters);
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // There's no stored table, initialize
            highscores = new Highscores()
            {
                highscoreEntryList = new List<HighscoreEntry>()
            };
     }

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable" + SceneManager.GetActiveScene().name + WordManager.letters, json);
        PlayerPrefs.Save();
        

    }


    public void Reset()
    {
       
        PlayerPrefs.DeleteKey("highscoreTable" + SceneManager.GetActiveScene().name + WordManager.letters);
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    /*
     * Represents a single High score entry
     * */
    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }

}

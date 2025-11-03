using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class SaveManager : MonoBehaviour
{
    public bool ResetProgress;
    private void Awake()
    {
        if (G.SaveManager == null)
        {
            G.SaveManager = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        if (ResetProgress)
            Reset();

    }


    public void SaveCoins(int amount)
    {
        PlayerPrefs.SetInt("Coins", amount);
    }

    public int LoadCoins() 
    {
        return PlayerPrefs.GetInt("Coins", -1);
    }

    public void SaveInsight(int amount)
    {
        PlayerPrefs.SetInt("Insight", amount);
    }

    public int LoadInsight()
    {
        return PlayerPrefs.GetInt("Insight", -1);
    }

    public void SaveRecipeStatuses(List<int> statuses)
    {
        PlayerPrefs.SetString("Recipes", string.Join(";", statuses.Select(x => x.ToString()).ToArray()));
    }

    public List<int> LoadRecipeStatuses()
    {
        string loaded = PlayerPrefs.GetString("Recipes", "");
        if (loaded.Length == 0)
            return new List<int>();
        return Array.ConvertAll(loaded.Split(';'), int.Parse).ToList();
    }

    public void SaveMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public float LoadMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    public void SaveSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }

    public float LoadSoundVolume()
    {
        return PlayerPrefs.GetFloat("SoundVolume", 0.5f);
    }



    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }

}

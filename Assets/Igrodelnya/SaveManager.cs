using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
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

    }


    public void SaveCoins(int amount)
    {
        PlayerPrefs.SetInt("Coins", amount);
    }

    public int LoadCoins() 
    {
        return PlayerPrefs.GetInt("Coins", 0);
    }

    
    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }

}

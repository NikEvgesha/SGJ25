using UnityEngine;

public class GameUI : MonoBehaviour
{
    

    public void OpenRecipeBook()
    {
        G.Game.RecipeBook.ToggleOpen();
    }
}

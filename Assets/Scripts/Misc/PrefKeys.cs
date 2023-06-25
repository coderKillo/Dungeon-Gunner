
public class PrefKeys
{
    public static string highscore = "highscore";

    public static string CardKey(CardSO card)
    {
        return "card:" + card.title;
    }
}

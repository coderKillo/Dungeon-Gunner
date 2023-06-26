
public class PrefKeys
{
    public static string highscore = "highscore";
    public static string soundVolume = "soundVolume";
    public static string musicVolume = "musicVolume";
    public static string tutorialOn = "tutorialOn";

    public static string CardKey(CardSO card)
    {
        return "card:" + card.title;
    }
}

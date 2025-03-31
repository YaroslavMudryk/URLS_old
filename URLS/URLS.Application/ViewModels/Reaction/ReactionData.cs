namespace URLS.Application.ViewModels.Reaction;

public class ReactionData
{
    public static int MaxReactionId = 9;

    public static string Like = "👍"; //1
    public static string Dislike = "👎"; //2
    public static string Heart = "❤️"; //3
    public static string Congratulations = "🥳"; //4
    public static string Laughter = "😂"; //5
    public static string Shit = "💩"; //6
    public static string Swearing = "🤬"; //7
    public static string Cry = "😭"; //8
    public static string Wow = "🤩"; //9

    public static Dictionary<int, string> GetAllReactions()
    {
        var dictionary = new Dictionary<int, string>();
        dictionary.Add(1,Like);
        dictionary.Add(2,Dislike);
        dictionary.Add(3,Heart);
        dictionary.Add(4,Congratulations);
        dictionary.Add(5,Laughter);
        dictionary.Add(6,Shit);
        dictionary.Add(7,Swearing);
        dictionary.Add(8,Cry);
        dictionary.Add(9,Wow);
        return dictionary;
    }
}

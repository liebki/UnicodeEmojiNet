namespace UnicodeEmojiNet;

public class EmojiInfo
{
    public EmojiInfo(string value, string name, bool isRecentlyAdded)
    {
        Value = value;
        Name = name;
        IsRecentlyAdded = isRecentlyAdded;
    }

    public string Value { get; set; }
    public string Name { get; set; }
    public bool IsRecentlyAdded { get; set; }


    public override string ToString()
    {
        return $"Name: {Name} | Value: {Value} | IsNew: {IsRecentlyAdded}";
    }
}
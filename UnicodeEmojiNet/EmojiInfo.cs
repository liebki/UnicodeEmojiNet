namespace UnicodeEmojiNet;

public class EmojiInfo
{
    public EmojiInfo(string value, string name, bool isRecentlyAdded, string mainCategory, string subCategory)
    {
        Value = value;
        Name = name;
        IsRecentlyAdded = isRecentlyAdded;
        MainCategory = mainCategory;
        SubCategory = subCategory;
    }

    public string Value { get; set; }
    public string Name { get; set; }
    public bool IsRecentlyAdded { get; set; }
    public string MainCategory { get; set; }
    public string SubCategory { get; set; }
}
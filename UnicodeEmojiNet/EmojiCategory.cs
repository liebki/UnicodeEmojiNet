namespace UnicodeEmojiNet;

public class EmojiCategory
{
    public EmojiCategory(string categoryName, string[] subCategories)
    {
        CategoryName = categoryName;
        SubCategories = subCategories;
    }

    public string CategoryName { get; set; }

    public string[] SubCategories { get; set; }
}
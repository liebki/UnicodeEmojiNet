# UnicodeEmojiNet

## Introduction

UnicodeEmojiNet allows to simply retrieve all Unicode emojis with their name and categories in C# either get them as a
JSON string or as custom objects in a List.

## Features ⭐

- **Retrieve Emojis:** Retrieve emojis from the Unicode website.
    - As JSON string or as custom objects in a list
- **Retrieve emoji categories:** Get all available categories (main and sub)
    - As custom objects in a list

## Usage 🔧

### Constructor

Initialize the EmojiManager using the constructor:

```csharp
EmojiManager emojiManager = new EmojiManager(string operatingFolder, bool skipAdditional);
```

#### What is skipAdditional?

If **skipAdditional**  is true, only the main-list will be queried and
processed (https://unicode.org/emoji/charts/full-emoji-list.html) the other list(s) won't be used because they "only"
contain variations like all skin tones, the main list only contains the yellow tone.

### Code

Here are all available things you can do right now:

#### Check if the html files are present in the operatingFolder:

This method checks if the **needed** html files are present so it returns true or false.

```csharp
bool AreHtmlFilesPresent = emojiManager.AreSourcesPresent();
```

#### Download html files and process those:

To download the needed emoji files and process them, you need to call this before you do anything else (or ensure the
files are available).
If this was called once and the files are present, it doesn't have to be run again.

```csharp
emojiManager.DownloadAndProcessFiles();
```

#### Get all emoji categories as list:

Get all "MainCategories" (like "Animals" etc.) and their "SubCategories" (like "marine" etc.) which are inside the main
ones.

```csharp
List<EmojiCategory> AllCategories = emojiManager.GetAllCategories();
```

#### Give the JSON back as string:

Get the JSON as string object.

```csharp
string EmojiListAsJson = emojiManager.GetEmojiListAsJsonString();
```

#### Give the json back as List<EmojiInfo>:

Get the emojis as List<EmojiInfo>.

```csharp
List<EmojiInfo> EmojiList = emojiManager.GetEmojiList();
```

#### Example code:

Here is a example snippet to use the code.

```csharp
static async Task Main(string[] args)
{
    EmojiManager emojiManager = new("/Users/YourUser/Desktop/", false);

    if (emojiManager.AreSourcesPresent())
    {
        await emojiManager.DownloadAndProcessFiles();

        string x = await emojiManager.GetEmojiListAsJsonString();
        await File.WriteAllTextAsync(Path.Combine("/Users/YourUser/Desktop/", "unicode-emojis.json"), x, Encoding.UTF8);
    }
}
```

### How to skip/speed-up source download:

- You can download them yourself and put them in a folder
- You can download them, pack them in a binary and copy them in a certain folder

.. you get it..

## Types 🔖

- **EmojiInfo:** The emoji itself
    - Id (a running number given by unicode) - int
    - Value (the emoji character) - string
    - Name (the name) - string
    - IsRecentlyAdded (is it a newly added emoji?) - bool
    - MainCategory (like animals) - string
    - SubCategory (like marine-animals) - string


- **EmojiCategory:** A category in which emojis are sorted in
    - CategoryName (this is the name of the main-category) - string
    - SubCategories (the names of all sub-categories under this main-category) - string[]

## FAQ

Q: How do you expect me to use this when it needs to download the html files?

A: I will think of something to make it easier, but for now you could download them yourself and them deliver them with
your file and copy them to a "operatingFolder".

## To-Do

- Implement some caching and/or a faster way to download everything

## License 📜

UnicodeEmojiNet is licensed under the GNU General Public License v3.0.

You can read the full license details of the GNU General Public License
v3.0 [here](https://choosealicense.com/licenses/gpl-3.0/).

## Icon

The icon was created by uxwing.com, found here: https://uxwing.com/smiling-line-icon/

## Disclaimer and Acceptance ⚠️

By using this library, you acknowledge that you have read and understood the full disclaimer in the DISCLAIMER.md file
and accept its terms. Additionally, you agree to abide by the GNU General Public License v3.0 under which
UnicodeEmojiNet is licensed, regardless of whether you have read the license text.

Please be aware that the author of the project and the project itself are not endorsed by Unicode and do not reflect the
views or opinions of Unicode or any individuals officially involved with the project. The author of this library is not
responsible for any incorrect or inappropriate usage. Please ensure that you use this library in accordance with its
intended purpose and guidelines.

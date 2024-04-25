# UnicodeEmojiNet


## Introduction

UnicodeEmojiNet allows to simply retrieve all Unicode emojis with their name in C# either get them as a JSON file, json string or as custom objects in a List.


## Features ⭐

- **Retrieve Emojis:** Retrieve emojis from the Unicode website.
- **Generate JSON file:** Generate a JSON file containing emojis with their values, names, and recent addition status.
- **Get the JSON as string:** Get the JSON file as string object and what you want
- **Get a List<EmojiInfo>:** Get a list identical to the JSON file, which contains all emojis as objects


## Usage 🔧

### Constructor

Initialize the EmojiManager using the constructor:

```csharp
EmojiManager emojiManager = new EmojiManager(string operatingFolder, bool skipAdditional);
```

#### What is skipAdditional?
If **skipAdditional**  is true, only the main-list will be queried and processed (https://unicode.org/emoji/charts/full-emoji-list.html) the other list(s) won't be used because they "only" contain variations like all skin tones, the main list only contains the yellow tone.


### Code

Here are all available things you can do right now:

#### Check if the html files are present in the operatingFolder:

This method checks if the **needed** html files are present so it returns true or false.

```csharp
bool AreHtmlFilesPresent = emojiManager.AreSourcesPresent();
```

#### Download html files and create JSON:

Call the method `DownloadEmojiFilesAndCreateJson()` to download emoji files and generate the JSON:
The JSON is called "unicode-emojis.json" and will also be created in the "operatingFolder"-directory.

```csharp
emojiManager.DownloadEmojiFilesAndCreateJson();
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
EmojiManager man = new EmojiManager(@"/Users/YourName/Desktop/", false);

if (man.AreSourcesPresent())
{
    Console.WriteLine("Sources are present, operating:");
    
    List<EmojiInfo> EmojiListe = await man.GetEmojiList();
    string ListJson = await man.GetEmojiListAsJsonString();
    
    // ...
}
```


### Information on usage:
Calling any of these methods needs the html files (because they contain the emojis) so if those files are NOT in the "operatingFolder" calling any of those methods will result in an error, you can also just AreSourcesPresent() to check before!

### Skipping in-code download:

To skip the download in-code you could download the complete source of the sites in the code and put them in the folder you will specify as "operatingFolder" then they won't be downloaded but I can't and won't deliver the files with this code!

## Types 🔖

- **EmojiInfo:** Represents information about an emoji including
  - Value (the emoji character) - string
  - Name (the emoji name) - string
  - IsRecentlyAdded (is it a newly added emoji?) - bool
  - Argument/Category (**This is WIP**)


## To-Do

- Implement some caching and/or a faster way to download everything
- Implement categories and/or arguments to put the emojis inside to filter them (important).


## License 📜

UnicodeEmojiNet is licensed under the GNU General Public License v3.0.

You can read the full license details of the GNU General Public License v3.0 [here](https://choosealicense.com/licenses/gpl-3.0/).


## Icon

The icon was created by uxwing.com, found here: https://uxwing.com/smiling-line-icon/


## Disclaimer and Acceptance ⚠️

By using this library, you acknowledge that you have read and understood the full disclaimer in the DISCLAIMER.md file and accept its terms. Additionally, you agree to abide by the GNU General Public License v3.0 under which UnicodeEmojiNet is licensed, regardless of whether you have read the license text.

Please be aware that the author of the project and the project itself are not endorsed by Unicode and do not reflect the views or opinions of Unicode or any individuals officially involved with the project. The author of this library is not responsible for any incorrect or inappropriate usage. Please ensure that you use this library in accordance with its intended purpose and guidelines.

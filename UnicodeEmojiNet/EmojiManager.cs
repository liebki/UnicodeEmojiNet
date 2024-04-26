using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace UnicodeEmojiNet
{
    public class EmojiManager(string operatingFolder, bool skipAdditional)
    {
        // For now those two urls contain all emojis
        private readonly List<(string, string)> _urls =
        [
            ("https://unicode.org/emoji/charts/full-emoji-list.html", "unicode-full-list.html"),
            ("https://unicode.org/emoji/charts/full-emoji-modifiers.html", "unicode-modifier-list.html")
        ];

        private List<EmojiInfo> _allInfosList = [];
        private List<EmojiCategory> _categoryList = [];
        private string _emojiListAsJson = string.Empty;

        /// <summary>
        /// To check if the needed source files are present.
        /// </summary>
        /// <returns>True if present / False if not</returns>
        public bool AreSourcesPresent()
        {
            int urlsExistCount = 0;
            if (skipAdditional)
            {
                _urls.RemoveAt(1);
            }
            
            foreach ((string urlstring, string name) data in _urls)
            {
                string htmlFilePath = Path.Combine(operatingFolder, data.name);
                if (File.Exists(htmlFilePath))
                {
                    urlsExistCount++;
                }
            }

            return _urls.Count == urlsExistCount;
        }

        private bool CheckVariables()
        {
            return _allInfosList.Count > 0 && _emojiListAsJson.Length > 0 && _categoryList.Count > 0;
        }
        
        /// <summary>
        /// Returns a list with all EmojiCategory objects
        /// </summary>
        /// <returns>List with all EmojiCategory objects.</returns>
        public async Task<List<EmojiCategory>> GetAllCategories()
        {
            if(CheckVariables())
                return _categoryList;
            
            await DownloadAndProcessFiles();
            return _categoryList;
        }

        /// <summary>
        /// Returns a list with all EmojiInfo objects
        /// </summary>
        /// <returns>List with all EmojiInfo objects.</returns>
        public async Task<List<EmojiInfo>> GetEmojiList()
        {
            if(CheckVariables())
                return _allInfosList;
            
            await DownloadAndProcessFiles();
            return _allInfosList;
        }
        
        /// <summary>
        /// Returns a JSON string with all EmojiInfo objects
        /// </summary>
        /// <returns>JSON string with all EmojiInfo objects.</returns>
        public async Task<string> GetEmojiListAsJsonString()
        {
            if(CheckVariables())
                return _emojiListAsJson;

            await DownloadAndProcessFiles();
            return _emojiListAsJson;
        }
        
        /// <summary>
        /// Iterate trough all source files and process them.
        /// </summary>
        public async Task DownloadAndProcessFiles()
        {
            if (skipAdditional)
            {
                _urls.RemoveAt(1);
            }
            
            foreach ((string urlstring, string name) data in _urls)
            {
                string HtmlFilePath = Path.Combine(operatingFolder, data.name);
                await DownloadHtmlFiles(data.urlstring, HtmlFilePath);
                
                ProcessHtmlFiles(HtmlFilePath);
            }

            DeDuplicateCategories();
        }

        private void DeDuplicateCategories()
        {
            Dictionary<string, HashSet<string>> mergedCategories = new Dictionary<string, HashSet<string>>();

            foreach (EmojiCategory category in _categoryList)
            {
                if (!mergedCategories.ContainsKey(category.CategoryName))
                {
                    mergedCategories[category.CategoryName] = new HashSet<string>();
                }

                foreach (string subCategory in category.SubCategories)
                {
                    mergedCategories[category.CategoryName].Add(subCategory);
                }
            }

            List<EmojiCategory> result = [];
            foreach (KeyValuePair<string, HashSet<string>> kvp in mergedCategories)
            {
                result.Add(new EmojiCategory(kvp.Key, kvp.Value.ToArray()));
            }

            _categoryList = result;
        }

        /// <summary>
        /// Download source files and stream them to a file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        private async Task DownloadHtmlFiles(string url, string filePath)
        {
            if (!File.Exists(filePath))
            {
                using HttpClient client = new HttpClient();

                // We use a high timeout, because the page is huge and the speed varies between internet connections
                client.Timeout = TimeSpan.FromMinutes(30);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

                await using FileStream fs = File.OpenWrite(filePath);
                using HttpResponseMessage response =
                    await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                await using Stream stream = await response.Content.ReadAsStreamAsync();
                await stream.CopyToAsync(fs);
            }
        }

    /// <summary>
    /// Process a source file, generate objects and categories here.
    /// </summary>
    /// <param name="htmlFilePath">File to process</param>
    private void ProcessHtmlFiles(string htmlFilePath)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.Load(htmlFilePath);

        HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//table//tr");
        if (rows == null)
            return;

        string lastMainCategory = "";
        string lastCategory = "";

        EmojiCategory emojiCategory = null;
        List<string> tempSubCategoryList = [];

        HtmlNode LastIteratedNode = rows.Last();
        foreach (HtmlNode row in rows)
        {
            if (row.InnerHtml.Contains("class='bighead'"))
            {
                if (tempSubCategoryList.Count > 0)
                {
                    emojiCategory.SubCategories = tempSubCategoryList.ToArray();
                    _categoryList.Add(emojiCategory);
                    
                    tempSubCategoryList.Clear();
                }
                
                lastMainCategory = HttpUtility.HtmlDecode(row.InnerText);
                
                if (tempSubCategoryList.Count == 0)
                {
                    emojiCategory = new EmojiCategory(lastMainCategory, []);
                }
            }
            if (row.InnerHtml.Contains("class='mediumhead'"))
            {
                lastCategory = HttpUtility.HtmlDecode(row.InnerText);
                tempSubCategoryList.Add(lastCategory);
            }
            
            HtmlNodeCollection cells = row.SelectNodes(".//td");
            if (cells != null && cells.Count >= 2)
            {
                string emoji = cells[2].InnerText.Trim();
                string nameRaw = cells[cells.Count - 1].InnerText.Trim();
                
                string name = HttpUtility.HtmlDecode(nameRaw);
                bool IsRecentlyAdded = name.Contains("\u229b");
                
                _allInfosList.Add(new EmojiInfo(emoji, name.Replace("\u229b ", ""), IsRecentlyAdded, lastMainCategory, lastCategory));
            }

            if (LastIteratedNode.Equals(row))
            {
                emojiCategory.SubCategories = tempSubCategoryList.ToArray();
                _categoryList.Add(emojiCategory);
            }
            
        }
    }
    }
}
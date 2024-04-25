using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private string _emojiListAsJson = string.Empty;

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

        public async Task<List<EmojiInfo>> GetEmojiList()
        {
            if(_allInfosList.Count > 0 || _emojiListAsJson.Length > 0)
                return _allInfosList;
            
            await DownloadEmojiFilesAndCreateJson();
            return _allInfosList;
        }
        
        public async Task<string> GetEmojiListAsJsonString()
        {
            if(_allInfosList.Count > 0 || _emojiListAsJson.Length > 0)
                return _emojiListAsJson;

            await DownloadEmojiFilesAndCreateJson();
            return _emojiListAsJson;
        }
        
        public async Task DownloadEmojiFilesAndCreateJson()
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

            _emojiListAsJson = JsonConvert.SerializeObject(_allInfosList, Formatting.Indented);
            await File.WriteAllTextAsync(Path.Combine(operatingFolder, "unicode-emojis.json"), _emojiListAsJson, Encoding.UTF8);
        }

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

        private void ProcessHtmlFiles(string htmlFilePath)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(htmlFilePath);

            var rows = doc.DocumentNode.SelectNodes("//table//tr");
            if (rows == null)
                return;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells != null && cells.Count >= 2)
                {
                    string emoji = cells[2].InnerText.Trim();
                    string name = cells[cells.Count - 1].InnerText.Trim();

                    bool IsRecentlyAdded = name.Contains("\u229b");
                    _allInfosList.Add(new EmojiInfo(emoji, name.Replace("\u229b ", ""), IsRecentlyAdded));
                }
            }
        }
    }
}
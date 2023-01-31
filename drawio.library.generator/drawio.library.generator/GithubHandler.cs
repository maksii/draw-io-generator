using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace drawio.library.generator
{
    public class GithubHandler
    {
        public List<Image> GetFiles(string username, string repository, string targetDirectory)
        {
            var fileList = new List<Image>();
            string baseUrl = $"https://api.github.com/repos/{username}/{repository}/contents/{targetDirectory}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "GitHubImageList");
                var response = client.GetAsync(baseUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var json = JArray.Parse(content);
                    foreach (var item in json)
                    {
                        if (item["type"].Value<string>() == "file")
                        {
                            
                            var file = new Image
                            {
                                DownloadUrl = item["download_url"].Value<string>(),
                                Name = item["name"].Value<string>()
                            };
                            if (IsImage(file.Name))
                            {
                                file.Name = FormatFileName(file.Name);
                                fileList.Add(file);
                            }
                        }
                        else if (item["type"].Value<string>() == "dir")
                        {
                            var subDirectory = item["path"].Value<string>();
                            fileList.AddRange(GetFiles(username, repository, subDirectory));
                        }
                    }
                }
            }

            return fileList;
        }

        public void CreateXmlDocument(List<Image> files)
        {
            var json = JsonConvert.SerializeObject(
                files.Select(file => new
                {
                    data = file.DownloadUrl,
                    w = 96,
                    h = 96,
                    title = file.Name.Split('.').First(),
                    aspect = "fixed"
                })
            );

            File.WriteAllText("output.xml", $"<mxlibrary>{json}</mxlibrary>");
        }

        static string FormatFileName(string name)
        {
            // Remove file extension
            int extensionIndex = name.LastIndexOf('.');
            if (extensionIndex > 0)
            {
                name = name.Substring(0, extensionIndex);
            }

            // Replace dash with space
            name = name.Replace("-", " ");

            // Replace underscore with space
            name = name.Replace("_", " ");


            // Replace scalable with empty
            name = name.Replace("scalable", "");

            // Add spaces for camel case wording
            name = Regex.Replace(name, "(?<=[a-z])([A-Z])", " $1");

            return name;
        }

        static bool IsImage(string name)
        {
            var extension = name.Split('.').Last();
            return extension == "svg" || extension == "png";
        }
    }

    public class Image
    {
        public string DownloadUrl { get; set; }
        public string Name { get; set; }
    }
}

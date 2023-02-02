using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace drawio.library.generator;

public class GithubHandler
{
    private readonly string? _token;

    public GithubHandler(string? token, string parent)
    {
        _token = token;
    }

    public List<Image> GetFiles(string username, string repository, string targetDirectory)
    {
        var fileLists = new List<Image>();
        var baseUrl = $"https://api.github.com/repos/{username}/{repository}/contents/{targetDirectory}";

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Add("User-Agent", "GitHubImageList");
        if (!string.IsNullOrEmpty(_token)) client.DefaultRequestHeaders.Add("Authorization", $"token {_token}");
        Console.WriteLine($"Getting contents for '{targetDirectory}'");
        Thread.Sleep(1000);
        var response = client.GetAsync(baseUrl).Result;
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Github Request Failed {response.IsSuccessStatusCode}");
            return fileLists;
        }

        var content = response.Content.ReadAsStringAsync().Result;
        var json = JArray.Parse(content);
        foreach (var item in json)
        {
            var type = item["type"].Value<string>();
            var path = item["path"].Value<string>();
            var pathIndex = path.LastIndexOf('/');
            var fullpath = path.Substring(0, pathIndex);

            if (type == "file")
            {
                var file = new Image
                {
                    DownloadUrl = item["download_url"].Value<string>(),
                    Name = item["name"].Value<string>(),
                    Path = path,
                    FullPath = fullpath
                };
                if (!IsImage(file.Name)) continue;

                file.FriendlyName = FormatFileName(file.Name);
                fileLists.Add(file);
            }
            else if (type == "dir")
            {
                var subDirectory = path;
                Console.WriteLine($"Getting files for subdirectory: {subDirectory}");
                var subFiles = GetFiles(username, repository, subDirectory);
                fileLists.AddRange(subFiles);
            }
        }

        return fileLists;
    }

    public void CreateXmlDocument(List<Image> fileLists)
    {
        //Some packages may have multiple files with the same name, but different extensions. We only want to keep the svg files.
        var duplicateRemoval = fileLists.GroupBy(x => x.Path.Split('.')[0])
            .Select(g => g.FirstOrDefault(i => i.Path.EndsWith(".svg")) ?? g.First());

        var groups = duplicateRemoval.GroupBy(x => x.FullPath);

        foreach (var group in groups)
        {
            var json = JsonConvert.SerializeObject(
                group.Select(file => new
                {
                    data = file.DownloadUrl,
                    w = 96,
                    h = 96,
                    title = file.FriendlyName,
                    aspect = "fixed"
                })
            );
            var groupFileName = group.Key.Split('/').Last();
            var fileName = $"{groupFileName}.xml";
            Console.WriteLine($"Writing to '{fileName}'");
            var groupFolderPath = group.First().FullPath;
            if (!Directory.Exists(groupFolderPath))
            {
                Directory.CreateDirectory(groupFolderPath);
            }
            
            File.WriteAllText($"{groupFolderPath}/{fileName}", $"<mxlibrary>{json}</mxlibrary>");
        }
    }

    private static string FormatFileName(string name)
    {
        // Remove file extension
        var extensionIndex = name.LastIndexOf('.');
        if (extensionIndex > 0) name = name.Substring(0, extensionIndex);

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

    private static bool IsImage(string name)
    {
        var extension = name.Split('.').Last();
        return extension == "svg" || extension == "png";
    }
}
using drawio.library.generator;

Console.WriteLine(
    "Enter URL in format https://github.com/<username>/<repository name>/tree/master/<path to target directory>");
var inputUrl = Console.ReadLine();

Console.WriteLine("Please Provide your access token for github or just hit enter");
var token = Console.ReadLine();

if (inputUrl != null)
{
    var parts = inputUrl.Split('/');
    var username = parts[3];
    var repository = parts[4];
    var targetDirectory = string.Join("/", parts.Skip(7));

    var folderToCreate = targetDirectory;
    var child = targetDirectory.Split('/');
    if (child.Length > 1 && !string.IsNullOrEmpty(child.Last())) folderToCreate = child.Last();

    if (Directory.Exists(folderToCreate)) Directory.Delete(folderToCreate, true);
    Directory.CreateDirectory(folderToCreate);

    var githubHandler = new GithubHandler(token, folderToCreate);
    var fileList = githubHandler.GetFiles(username, repository, targetDirectory);
    githubHandler.CreateXmlDocument(fileList);

    Console.WriteLine("Finished");
}
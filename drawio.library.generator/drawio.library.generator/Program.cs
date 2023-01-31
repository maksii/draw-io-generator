using drawio.library.generator;

Console.WriteLine("Enter URL in format https://github.com/<username>/<repository name>/tree/master/<path to target directory>");
string? inputUrl = Console.ReadLine();

if (inputUrl != null)
{
    string[] parts = inputUrl.Split('/');
    string username = parts[3];
    string repository = parts[4];
    string targetDirectory = string.Join("/", parts.Skip(7));

    var githubHandler = new GithubHandler();
    var fileList = githubHandler.GetFiles(username, repository, targetDirectory);
    githubHandler.CreateXmlDocument(fileList);
}
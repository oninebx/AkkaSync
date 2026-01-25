using AkkaSync.Publish;

var config = PublishConfigReader.Load();

var root = Directory.GetCurrentDirectory();
var releaseRoot = Path.Combine(root, "..", "..", "..", "releases", config.ReleaseVersion);
Directory.CreateDirectory(releaseRoot);

var templateRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
CopyDirectory(templateRoot, releaseRoot);

var envFilePath = Path.Combine(releaseRoot, ".env");
var envContent = $"AKKASYNC_VERSION={config.ReleaseVersion}";
File.WriteAllText(envFilePath, envContent);

Console.WriteLine($".env file generated at {envFilePath}");

var exampleRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, config.ExamplesRoot, config.Demo));

CopyDirectory(Path.Combine(exampleRoot, "configs"), Path.Combine(releaseRoot, "pipelines"));
CopyDirectory(Path.Combine(exampleRoot, "input"), Path.Combine(releaseRoot, "data", "input"));
CopyDirectory(Path.Combine(exampleRoot, "output"), Path.Combine(releaseRoot, "data", "output"));


static void CopyDirectory(string source, string target)
{
  if (!Directory.Exists(target))
  {
    Directory.CreateDirectory(target);
  }

  foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
  {
    Directory.CreateDirectory(dir.Replace(source, target));
  }

  foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
  {
    File.Copy(file, file.Replace(source, target), overwrite: true);
  }
    
}


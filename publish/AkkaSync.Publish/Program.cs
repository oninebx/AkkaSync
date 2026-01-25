using AkkaSync.Publish;

var config = PublishHelper.LoadConfig();

var root = Directory.GetCurrentDirectory();
var releaseRoot = Path.Combine(AppContext.BaseDirectory, "../../../..", "releases", config.ReleaseVersion);
Directory.CreateDirectory(releaseRoot);

var templateRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
PublishHelper.CopyDirectory(templateRoot, releaseRoot);

var envFilePath = Path.Combine(releaseRoot, ".env");
var envContent = $"AKKASYNC_VERSION={config.ReleaseVersion}";
File.WriteAllText(envFilePath, envContent);

Console.WriteLine($".env file generated at {envFilePath}");

var exampleRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, config.ExamplesRoot, config.Demo));

PublishHelper.CopyDirectory(Path.Combine(exampleRoot, "configs"), Path.Combine(releaseRoot, "pipelines"));
PublishHelper.CopyDirectory(Path.Combine(exampleRoot, "input"), Path.Combine(releaseRoot, "data", "input"));
PublishHelper.CopyDirectory(Path.Combine(exampleRoot, "output"), Path.Combine(releaseRoot, "data", "output"));





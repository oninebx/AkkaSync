using AkkaSync.Publish;

var config = PublishHelper.LoadConfig();

var releaseRoot = Path.Combine(AppContext.BaseDirectory, "../../../..", "releases", config.ReleaseVersion);
Directory.CreateDirectory(releaseRoot);

var templateRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
PublishHelper.CopyDirectory(templateRoot, releaseRoot);

var envFilePath = Path.Combine(releaseRoot, ".env");
var envContent = $"AKKASYNC_VERSION={config.AkkaSyncVersion}";
File.WriteAllText(envFilePath, envContent);

Console.WriteLine($".env file generated at {envFilePath}");

var demoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, config.ExamplesRoot, config.Demo));

PublishHelper.CopyDirectory(Path.Combine(demoRoot, "configs"), Path.Combine(releaseRoot, "pipelines"));
PublishHelper.CopyDirectory(Path.Combine(demoRoot, "input"), Path.Combine(releaseRoot, "data", config.Demo, "input"));
PublishHelper.CopyDirectory(Path.Combine(demoRoot, "output"), Path.Combine(releaseRoot, "data", config.Demo, "output"));

var pluginRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, config.ExamplesRoot, config.PluginFolder));
PublishHelper.CopyDirectory(Path.Combine(pluginRoot, "present"), Path.Combine(releaseRoot, "plugins"));
PublishHelper.CopyDirectory(Path.Combine(pluginRoot, "legacy"), Path.Combine(releaseRoot, "plugins-swap"));





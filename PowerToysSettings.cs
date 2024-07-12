using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.PowerToys;

public class PowerToysSettings
{
    public PowerToysSettings()
    {
        _watcher = new FileSystemWatcher
        {
            Path = SettingsFolderPath,
            Filter = SettingsFileName,
            NotifyFilter = NotifyFilters.LastWrite,
        };
        _watcher.Changed += (_, _) => _ = RefreshSettings();
        _watcher.EnableRaisingEvents = true;
    }
    
    private Dictionary<string, bool> _settings;
    private FileSystemWatcher _watcher;

    private SemaphoreSlim _sem = new (1,1);
    
    private string SettingsFileName { get; } = "settings.json";
    private string SettingsFilePath => Path.Combine(SettingsFolderPath, SettingsFileName);
    private string SettingsFolderPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        @"Microsoft\PowerToys");
    
    public async Task RefreshSettings()
    {
        if (_sem.CurrentCount < 1)
            return;
        await _sem.WaitAsync();
        var retries = 2;
        while (retries > 0)
        {
            try
            {
                await using var file = File.OpenRead(SettingsFilePath);
                var jdoc = await JsonDocument.ParseAsync(file);
                _settings = jdoc.RootElement.GetProperty("enabled").Deserialize<Dictionary<string, bool>>();
                retries = 0;
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                retries -= 1;
            }
        }

        _sem.Release();
    }
    
    public bool IsEnabled(string settingName)
    {
        return _settings?.GetValueOrDefault(settingName) ?? false;
    }
}

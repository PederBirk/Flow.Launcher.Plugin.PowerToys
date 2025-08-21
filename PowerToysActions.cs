using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.PowerToys;

public interface IAction
{
    string Title { get; }
    IEnumerable<string> Keywords { get; }
    string Icon { get; }
    void Execute();
    IEnumerable<IAction> GetContextMenuActions();
}

public class PowerToysUtilityAction : IAction
{
    public IEnumerable<string> Keywords { get; init; } = [];
    public required string EventKey { get; init; }
    public required string Title { get; init; }
    
    public string Icon { get; init; } = "";
    public virtual void Execute()
    {
        using var eventHandle = new EventWaitHandle(false, EventResetMode.AutoReset, EventKey);
        eventHandle.Set();
    }

    public virtual IEnumerable<IAction> GetContextMenuActions() => [];
}

public class PowerToysUtilityActionWithSettings : PowerToysUtilityAction
{
    public required string SettingsLinkName { get; init; }
    public string SettingsEnabledNameOverride { get; init; } = "";

    public string SettingsEnabledName =>
        string.IsNullOrEmpty(SettingsEnabledNameOverride) ? SettingsLinkName : SettingsEnabledNameOverride;
    
    public override IEnumerable<IAction> GetContextMenuActions()
    {
        return base.GetContextMenuActions().Concat([
            new OpenPowerToysSettingsAction
                { Title = "Open Settings for this Utility", Icon = Icon, SettingsLinkName = SettingsLinkName }
        ]);
    }
}

public class OpenPowerToysSettingsAction : IAction
{
    public required string SettingsLinkName { get; init; }
    public required string Title { get; init; }
    public string Icon { get; init; } = "";
    public IEnumerable<IAction> GetContextMenuActions() => [];

    public IEnumerable<string> Keywords { get; init; } = [];
    public void Execute()
    {
        var relativePath = "PowerToys\\PowerToys.exe";
        var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), relativePath);
        if (!File.Exists(appPath))
        {
            appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), relativePath);
        }

        if (File.Exists(appPath))
        {
            Process.Start(new ProcessStartInfo(appPath) { Arguments = $"--open-settings={SettingsLinkName}" });
        }
    }
}

public class DelayedPowerToysUtilityAction : PowerToysUtilityActionWithSettings
{
    public override void Execute()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(0.8));
            base.Execute();
        });
    }
}

public class PowerToysUtilityActionWithAsAdmin : PowerToysUtilityActionWithSettings
{
    public required string RunAsAdminEventKey { get; init; }

    public override IEnumerable<IAction> GetContextMenuActions()
    {
        return base.GetContextMenuActions().Concat([
            new PowerToysUtilityAction { EventKey = RunAsAdminEventKey, Title = "Run as Administrator", Icon = Icon }
        ]);
    }
}
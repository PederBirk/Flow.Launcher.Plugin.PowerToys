using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.PowerToys;

public static class Events
{
    public const string ShowColorPickerSharedEvent = "Local\\ShowColorPickerEvent-8c46be2a-3e05-4186-b56b-4ae986ef2525";
    public const string FZEToggleEvent = "Local\\FancyZones-ToggleEditorEvent-1e174338-06a3-472b-874d-073b21c62f14";
    public const string ShowHostsSharedEvent = "Local\\Hosts-ShowHostsEvent-5a0c0aae-5ff5-40f5-95c2-20e37ed671f0";
    public const string MeasureToolTriggerEvent = "Local\\MeasureToolEvent-3d46745f-09b3-4671-a577-236be7abd199";
    public const string ShortcutGuideTriggerEvent = "Local\\ShortcutGuide-TriggerEvent-d4275ad3-2531-4d19-9252-c0becbd9b496";
    public const string RegistryPreviewTriggerEvent = "Local\\RegistryPreviewEvent-4C559468-F75A-4E7F-BC4F-9C9688316687";
    public const string CropAndLockThumbnailEvent = "Local\\PowerToysCropAndLockThumbnailEvent-1637be50-da72-46b2-9220-b32b206b2434";
    public const string CropAndLockReparentEvent = "Local\\PowerToysCropAndLockReparentEvent-6060860a-76a1-44e8-8d0e-6355785e9c36";
    public const string ShowEnvironmentVariablesSharedEvent = "Local\\PowerToysEnvironmentVariables-ShowEnvironmentVariablesEvent-1021f616-e951-4d64-b231-a8f972159978";
    public const string AlwaysOnTopPinEvent = "Local\\AlwaysOnTopPinEvent-892e0aa2-cfa8-4cc4-b196-ddeb32314ce8";
    public const string ShowPowerOcrEvent = "Local\\PowerOCREvent-dc864e06-e1af-4ecc-9078-f98bee745e3a";
    public const string ShowAdvancedPasteEvent = "Local\\ShowAdvancedPasteEvent-9a46be2a-3e05-4186-b56b-4ae986ef2526";
}

public static class Icons
{
    public const string BasePath =
        "https://cdn.jsdelivr.net/gh/microsoft/PowerToys/src/settings-ui/Settings.UI/Assets/Settings/Icons/";
    public const string AdvancedPaste = BasePath + "AdvancedPaste.png";
    public const string ColorPicker = BasePath + "ColorPicker.png";
    public const string Hosts = BasePath + "Hosts.png";
    public const string FancyZones = BasePath + "FancyZones.png";
    public const string AlwaysOnTop = BasePath + "AlwaysOnTop.png";
    public const string ScreenRuler = BasePath + "ScreenRuler.png";
    public const string ShortcutGuide = BasePath + "ShortcutGuide.png";
    public const string TextExtractor = BasePath + "TextExtractor.png";
    public const string PowerToys = BasePath + "PowerToys.png";
    public const string EnvironmentVariables = BasePath + "EnvironmentVariables.png";
    public const string CropAndLock = BasePath + "CropAndLock.png";
    public const string RegistryPreview = BasePath + "RegistryPreview.png";

}



public class PowerToysLauncher
{
    public static PowerToysAction[] Actions =
    [
        new()
        {
            EventKey = Events.MeasureToolTriggerEvent,
            Keywords = ["measure", "tool", "screen", "ruler"],
            Title = "Screen Ruler",
            Icon = Icons.ScreenRuler
        },
        new()
        {
            EventKey = Events.ShortcutGuideTriggerEvent,
            Keywords = ["shortcut", "guide"],
            Title = "Shortcut Guide",
            Icon = Icons.ShortcutGuide
        },
        new()
        {
            EventKey = Events.ShowColorPickerSharedEvent,
            Keywords = ["color", "colour", "picker"],
            Title = "Show Color Picker",
            Icon = Icons.ColorPicker
        },
        new()
        {
            EventKey = Events.AlwaysOnTopPinEvent,
            Keywords = ["pin", "always", "top"],
            Title = "Pin Always On Top",
            WaitBeforeExecute = true,
            Icon = Icons.AlwaysOnTop
        },
        new()
        {
            EventKey = Events.ShowPowerOcrEvent,
            Keywords = ["ocr", "text", "extract"],
            Title = "Extract Text",
            Icon = Icons.TextExtractor
        },
        new()
        {
            EventKey = Events.FZEToggleEvent,
            Keywords = ["fancy", "zone"],
            Title = "Fancy Zones Editor",
            Icon = Icons.FancyZones
        },
        new()
        {
            EventKey = Events.ShowHostsSharedEvent,
            Keywords = ["hosts"],
            Title = "Hosts Editor",
            Icon = Icons.Hosts
        },
        new()
        {
            EventKey = Events.RegistryPreviewTriggerEvent,
            Keywords = ["registry", "preview"],
            Title = "Registry Preview",
            Icon = Icons.RegistryPreview
        },
        new()
        {
            EventKey = Events.CropAndLockReparentEvent,
            Keywords = ["crop", "lock", "reparent"],
            Title = "Crop And Lock - Reparent",
            WaitBeforeExecute = true,
            Icon = Icons.CropAndLock
        },
        new()
        {
            EventKey = Events.CropAndLockThumbnailEvent,
            Keywords = ["crop", "lock", "thumbnail"],
            Title = "Crop And Lock - Thumbnail",
            WaitBeforeExecute = true,
            Icon = Icons.CropAndLock
        },
        new()
        {
            EventKey = Events.ShowAdvancedPasteEvent,
            Keywords = ["advanced", "paste"],
            Title = "Advanced Paste",
            Icon = Icons.AdvancedPaste
        },
        new()
        {
            EventKey = Events.ShowEnvironmentVariablesSharedEvent,
            Keywords = ["environment", "variable"],
            Title = "Environment Variables",
            Icon = Icons.EnvironmentVariables
        }
    ];
}

public class PowerToysAction
{
    public required IEnumerable<string> Keywords { get; init; }
    public required string EventKey { get; init; }
    public required string Title { get; init; }
    public string Icon { get; init; } = "";

    public bool WaitBeforeExecute { get; init; } = false;
    
    public void Execute()
    {
        if (WaitBeforeExecute)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                ExecuteEventHandle();
            });
            return;
        }

        ExecuteEventHandle();
    }

    private void ExecuteEventHandle()
    {
        using var eventHandle = new EventWaitHandle(false, EventResetMode.AutoReset, EventKey);
        eventHandle.Set();
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.PowerToys;

public static class Events
{
    public const string ShowColorPickerSharedEvent = "Local\\ShowColorPickerEvent-8c46be2a-3e05-4186-b56b-4ae986ef2525";
    public const string FZEToggleEvent = "Local\\FancyZones-ToggleEditorEvent-1e174338-06a3-472b-874d-073b21c62f14";
    public const string ShowHostsSharedEvent = "Local\\Hosts-ShowHostsEvent-5a0c0aae-5ff5-40f5-95c2-20e37ed671f0";
    public const string ShowHostsAdminEvent = "Local\\Hosts-ShowHostsAdminEvent-60ff44e2-efd3-43bf-928a-f4d269f98bec";
    public const string MeasureToolTriggerEvent = "Local\\MeasureToolEvent-3d46745f-09b3-4671-a577-236be7abd199";
    public const string ShortcutGuideTriggerEvent = "Local\\ShortcutGuide-TriggerEvent-d4275ad3-2531-4d19-9252-c0becbd9b496";
    public const string RegistryPreviewTriggerEvent = "Local\\RegistryPreviewEvent-4C559468-F75A-4E7F-BC4F-9C9688316687";
    public const string CropAndLockThumbnailEvent = "Local\\PowerToysCropAndLockThumbnailEvent-1637be50-da72-46b2-9220-b32b206b2434";
    public const string CropAndLockReparentEvent = "Local\\PowerToysCropAndLockReparentEvent-6060860a-76a1-44e8-8d0e-6355785e9c36";
    public const string ShowEnvironmentVariablesSharedEvent = "Local\\PowerToysEnvironmentVariables-ShowEnvironmentVariablesEvent-1021f616-e951-4d64-b231-a8f972159978";
    public const string ShowEnvironmentVarablesAdminEvent = "Local\\PowerToysEnvironmentVariables-EnvironmentVariablesAdminEvent-8c95d2ad-047c-49a2-9e8b-b4656326cfb2";
    public const string AlwaysOnTopPinEvent = "Local\\AlwaysOnTopPinEvent-892e0aa2-cfa8-4cc4-b196-ddeb32314ce8";
    public const string ShowPowerOcrEvent = "Local\\PowerOCREvent-dc864e06-e1af-4ecc-9078-f98bee745e3a";
    public const string LaunchWorkspacesEditorEvent = "Local\\Workspaces-LaunchEditorEvent-a55ff427-cf62-4994-a2cd-9f72139296bf";
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
    public const string Workspaces = BasePath + "Workspaces.png";
}
public class PowerToysLauncher
{
    public IEnumerable<IAction> EnabledActions => Actions.Where(ActionEnabled);
    private PowerToysSettings _settings = new();
    public async Task ApplySettings()
    {
        await _settings.RefreshSettings();
    }

    public bool IsPowerToysRunning()
    {
        var processes = Process.GetProcessesByName("PowerToys");
        return processes.Length != 0;
    }

    private bool ActionEnabled(IAction action)
    {
        return action switch
        {
            PowerToysUtilityActionWithSettings s => _settings.IsEnabled(s.SettingsEnabledName),
            _ => true
        };
    }
    
    private static IAction[] Actions =
    [
        new PowerToysUtilityActionWithSettings
        {
            EventKey = Events.MeasureToolTriggerEvent,
            Keywords = ["measure", "tool", "screen", "ruler"],
            TitleKey = "screen_ruler",
            Icon = Icons.ScreenRuler,
            SettingsLinkName = "MeasureTool",
            SettingsEnabledNameOverride = "Measure Tool"
        },
        new PowerToysUtilityActionWithSettings
        {
            EventKey = Events.ShortcutGuideTriggerEvent,
            Keywords = ["shortcut", "guide"],
            TitleKey = "shortcut_guide",
            Icon = Icons.ShortcutGuide,
            SettingsLinkName = "ShortcutGuide",
            SettingsEnabledNameOverride = "Shortcut Guide"
        },
        new PowerToysUtilityActionWithSettings
        {
            EventKey = Events.ShowColorPickerSharedEvent,
            Keywords = ["color", "colour", "picker"],
            TitleKey = "color_picker",
            Icon = Icons.ColorPicker,
            SettingsLinkName = "ColorPicker"
        },
        new DelayedPowerToysUtilityAction
        {
            EventKey = Events.AlwaysOnTopPinEvent,
            Keywords = ["pin", "always", "top"],
            TitleKey = "always_on_top",
            Icon = Icons.AlwaysOnTop,
            SettingsLinkName = "AlwaysOnTop"
        },
        new PowerToysUtilityActionWithSettings
        {
            EventKey = Events.ShowPowerOcrEvent,
            Keywords = ["ocr", "text", "extract"],
            TitleKey = "text_extractor",
            Icon = Icons.TextExtractor,
            SettingsLinkName = "PowerOcr",
            SettingsEnabledNameOverride = "TextExtractor"
        },
        new PowerToysUtilityActionWithSettings
        {
            EventKey = Events.FZEToggleEvent,
            Keywords = ["fancy", "zones"],
            TitleKey = "fancy_zones",
            Icon = Icons.FancyZones,
            SettingsLinkName = "FancyZones"
        },
        new PowerToysUtilityActionWithAsAdmin
        {
            EventKey = Events.ShowHostsSharedEvent,
            Keywords = ["hosts"],
            TitleKey = "hosts_editor",
            Icon = Icons.Hosts,
            SettingsLinkName = "Hosts",
            RunAsAdminEventKey = Events.ShowHostsAdminEvent
        },
        new PowerToysUtilityActionWithSettings
        {
            EventKey = Events.RegistryPreviewTriggerEvent,
            Keywords = ["registry", "preview"],
            TitleKey = "registry_preview",
            Icon = Icons.RegistryPreview,
            SettingsLinkName = "RegistryPreview"
        },
        new PowerToysUtilityActionWithSettings
        {
            EventKey = Events.LaunchWorkspacesEditorEvent,
            Keywords = ["workspaces"],
            TitleKey = "workspaces",
            Icon = Icons.Workspaces,
            SettingsLinkName = "Workspaces"
        },
        new DelayedPowerToysUtilityAction
        {
            EventKey = Events.CropAndLockReparentEvent,
            Keywords = ["crop", "lock", "reparent"],
            TitleKey = "crop_and_lock_reparent",
            Icon = Icons.CropAndLock,
            SettingsLinkName = "CropAndLock"
        },
        new DelayedPowerToysUtilityAction
        {
            EventKey = Events.CropAndLockThumbnailEvent,
            Keywords = ["crop", "lock", "thumbnail"],
            TitleKey = "crop_and_lock_thumbnail",
            Icon = Icons.CropAndLock,
            SettingsLinkName = "CropAndLock"
        },
        new PowerToysUtilityActionWithAsAdmin
        {
            EventKey = Events.ShowEnvironmentVariablesSharedEvent,
            Keywords = ["environment", "variable"],
            TitleKey = "environment_variables",
            Icon = Icons.EnvironmentVariables,
            SettingsLinkName = "EnvironmentVariables",
            RunAsAdminEventKey = Events.ShowEnvironmentVarablesAdminEvent
        },
        new OpenPowerToysSettingsAction
        {
            Icon = Icons.PowerToys,
            Keywords = ["settings"],
            SettingsLinkName = "PowerToys",
            TitleKey = "settings"
        }
    ];
}


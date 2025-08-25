using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.PowerToys;

public class PowerToys : IAsyncPlugin, IContextMenu, IAsyncReloadable, IPluginI18n
{
    internal static PluginInitContext _context { get; private set; }
    private PowerToysLauncher _launcher;
    public async Task InitAsync(PluginInitContext context)
    {
        _context = context;
        _launcher = new PowerToysLauncher();
        await _launcher.ApplySettings();
    }
    
    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        if (!_launcher.IsPowerToysRunning())
        {
            return [new Result{Title = _context.API.GetTranslation("powertoys_not_running"), SubTitle = _context.API.GetTranslation("powertoys_not_running_subtitle") }];
        }
        if(string.IsNullOrWhiteSpace(query.Search))
        {
            return _launcher.EnabledActions.Select(MapActionToResult).ToList();
        }
        // Split search string, segment by spaces (ignore empty string)
        var searchTerms = query.Search.ToLower().Split([" "], System.StringSplitOptions.RemoveEmptyEntries);
        var filteredResults = _launcher.EnabledActions.Where(x => searchTerms.All(term => x.Keywords.Any(keyword => keyword.Contains(term))));
        if(filteredResults.Any())
        {
            return filteredResults.Select(MapActionToResult).ToList();
        }
        return new List<Result>();
    }

    private Result MapActionToResult(IAction action)
    {
        return new Result
        {
            Action =  _ =>  { action.Execute(); return true; },
            Title = GetTranslation(action.TitleKey),
            SubTitle = action.Keywords.Any() ? GetTranslation("keywords") + ": " + string.Join(" ", action.Keywords) : string.Empty,
            IcoPath = action.Icon,
            ContextData = action
        };
    }

    public List<Result> LoadContextMenus(Result selectedResult)
    {
        var action = (IAction)selectedResult.ContextData;
        return action.GetContextMenuActions().Select(MapActionToResult).ToList();
    }

    public async Task ReloadDataAsync()
    {
        await _launcher.ApplySettings();
    }

    public string GetTranslatedPluginTitle()
    {
        return GetTranslation("plugin_name");
    }

    public string GetTranslatedPluginDescription()
    {
        return GetTranslation("plugin_description");
    }

    public string GetTranslation(string key)
    {
        return _context.API.GetTranslation(key);
    }
}
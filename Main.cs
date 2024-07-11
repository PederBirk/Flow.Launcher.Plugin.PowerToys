using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.PowerToys;

public class PowerToys : IPlugin, IContextMenu, IReloadable
{
    private PluginInitContext _context;
    private PowerToysLauncher _launcher;
    public void Init(PluginInitContext context)
    {
        _context = context;
        _launcher = new PowerToysLauncher();
        _launcher.ApplySettings();
    }
    
    public List<Result> Query(Query query)
    {
        if(string.IsNullOrWhiteSpace(query.Search))
        {
            return _launcher.EnabledActions.Select(MapActionToResult).ToList();
        }
        var filteredResults = _launcher.EnabledActions.Where(x => x.Keywords.Any(y => y.Contains(query.Search.ToLower())));
        if(filteredResults.Any())
        {
            return filteredResults.Select(MapActionToResult).ToList();
        }
        return new List<Result>();
    }

    private static Result MapActionToResult(PowerToysAction action)
    {
        return new Result
        {
            Action =  _ =>  { action.Execute(); return true; },
            Title = action.Title,
            IcoPath = action.Icon,
            ContextData = action
        };
    }

    public List<Result> LoadContextMenus(Result selectedResult)
    {
        var action = (PowerToysAction)selectedResult.ContextData;
        return
        [
            new()
            {
                Action = _ => { action.OpenSettings(); return true; },
                IcoPath = action.Icon,
                Title = "Open Settings for this utility"
            }
        ];
    }

    public void ReloadData()
    {
        _launcher.ApplySettings();
    }
}
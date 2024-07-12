using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.PowerToys;

public class PowerToys : IAsyncPlugin, IContextMenu, IAsyncReloadable
{
    private PluginInitContext _context;
    private PowerToysLauncher _launcher;
    public async Task InitAsync(PluginInitContext context)
    {
        _context = context;
        _launcher = new PowerToysLauncher();
        await _launcher.ApplySettings();
    }
    
    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
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

    private static Result MapActionToResult(IAction action)
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
        var action = (IAction)selectedResult.ContextData;
        return action.GetContextMenuActions().Select(MapActionToResult).ToList();
    }

    public async Task ReloadDataAsync()
    {
        await _launcher.ApplySettings();
    }
}
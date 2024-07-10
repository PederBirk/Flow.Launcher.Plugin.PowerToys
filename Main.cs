using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.PowerToys;

public class PowerToys : IPlugin
{
    private PluginInitContext _context;

    public void Init(PluginInitContext context)
    {
        _context = context;
    }

    public List<Result> Query(Query query)
    {
        if(string.IsNullOrWhiteSpace(query.Search))
        {
            return PowerToysLauncher.Actions.Select(MapActionToResult).ToList();
        }
        var filteredResults = PowerToysLauncher.Actions.Where(x => x.Keywords.Any(y => y.Contains(query.Search.ToLower())));
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
            IcoPath = action.Icon
        };
    }
}
using ESIDotNET.DependencyInjection;

namespace ESIDotNET
{
    public class ESIConfiguration
    {
        public required INavigationService NavigationService { get; init; }
    }
}

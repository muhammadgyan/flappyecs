

using Svelto.ECS;
using Svelto.ECS.Hybrid;

public interface IScoreHUD
{
    public int Score
    {
        get;
        set;
    }
}

public struct HUDView : IEntityViewComponent
{
    public IScoreHUD ScoreHUD;
    public EGID                   ID { get; set; }
}

public class HUDEntityDescriptor : IEntityDescriptor
{
    static readonly IComponentBuilder[] _componentsToBuild =
    {
        new ComponentBuilder<HUDView>(),
        new ComponentBuilder<GameStateHUDView>()
    };
        
    public IComponentBuilder[] componentsToBuild  => _componentsToBuild;

}
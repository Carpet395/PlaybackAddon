global using Celeste.Mod.playback.Modules;
using Monocle;
using Microsoft.Xna.Framework;


namespace Celeste.Mod.BellTemple;

public class playbackModule : EverestModule {
    // Only one alive module instance can exist at any given time.
    public static playbackModule Instance;

    public playbackModule()
    {
        Instance = this;
    }

    public override void Load()
    {
        ShaderUtils.Load();
    }

    public override void Unload()
    {
    }
}
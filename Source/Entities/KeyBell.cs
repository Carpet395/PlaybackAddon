using Celeste;
using Celeste.Mod.audiohelper.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;


namespace PlayBackAddon.Entities;

[CustomEntity("playback/Keybell", "carpet/Keybell")]
[Tracked(true)]
public class KeyBell : Bell
{

    public Switch Switch;
    public Color ActiveColour;

    public bool TriggerOnTouch = true;

    public KeyBell(EntityData data, Vector2 offset, EntityID id)
        : base(data, offset, id)
    {
        ActiveColour = data.HexColor("activeColour", Color.Magenta);
        effect = data.Attr("Effect", "");
        Add(Switch = new Switch(groundReset: false));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (Switch.Activated)
        {
            t = 1;
            sprite.Color = ActiveColour;
        }
    }

    public override void Ring(float xSpeed, float ySpeed)
    {
        if (TriggerOnTouch)
        {
            TurnOn();
        }
        base.Ring(xSpeed, ySpeed);
    }

    public virtual void TurnOn()
    {
        if (!Switch.Activated)
        {
            if (Switch.Activate())
            {
                SoundEmitter.Play("event:/game/general/touchswitch_last_oneshot");
                Add(new SoundSource("event:/game/general/touchswitch_last_cutoff"));
            }
            Add(new Coroutine(Colorfade(Colour, ActiveColour, 2f)));
        }
    }

    public float t = 0;
    
    public IEnumerator Colorfade(Color colora, Color colorb, float duration)
    {
        t = 0;
        while (t != 1)
        {
            t = Calc.Approach(t, 1, Engine.DeltaTime / duration);
            sprite.Color = Color.Lerp(colora, colorb, t);
            yield return null;
        }
    }

    public string effect;
    public Effect Effect {
        get
        {
            if (effect != null && effect != "" && effect != string.Empty)
            {
                return ShaderUtils.TryGetEffect(effect);
            }
            else
            {
                return null;
            }
        }
    }

    public override void Render()
    {
        if (Effect != null)
        {
            Effect.AddParameters(this);
            GameplayRenderer.End();
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, Effect, SceneAs<Level>().Camera.Matrix);
            base.Render();
            Draw.SpriteBatch.End();
            GameplayRenderer.Begin();
        }
        else
        {
            base.Render();
        }
    }
}

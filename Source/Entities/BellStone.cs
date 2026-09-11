using Celeste;
using Celeste.Mod.audiohelper.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;
using System.Collections.Generic;

namespace PlayBackAddon.Entities;

[CustomEntity("playback/Bellstone", "carpet/Bellstone")]
public class BellStone : Entity
{
    private OrderKeyBell leader = null;
    public OrderKeyBell Leader
    {
        get
        {
            if (leader != null) return leader;
            foreach (OrderKeyBell bell in Scene.Tracker.GetEntities<OrderKeyBell>())
            {
                if (bell.PlaceInQueue[0] == 0)
                    return leader = bell;
            }
            return null;
        }
    }

    public Image sprite;
    public Image overlay_sprite;

    public List<float> Speeds;

    public float VolumeBoost;

    public MultiParameterSoundSource sfx;

    public SineWave sine;
    public SineWave overlay_sine;

    public Color Colour;
    public Color ActiveColour;

    int ColorIndex = 0;

    public TalkComponent Talker;

    public float NoteDelay = 0.075f;

    public readonly string[] stone_sprite = { "objects/bellstone/stone", "objects/bellstone/stone_alt", "objects/bellstone/stone_alt_2" };
    public readonly string[] overlay_stone_sprite = { "objects/bellstone/stone_glow", "objects/bellstone/stone_glow_alt", "objects/bellstone/stone_glow_alt_2" };

    public BellStone(EntityData data, Vector2 offset, EntityID id)
        : base(data.Position + offset)
    {
        Speeds = new List<float>();
        foreach (var i in data.Attr("speed").Split(','))
        {
            if (float.TryParse(i, out float y))
            {
                Speeds.Add(y);
            }
        }
        VolumeBoost = data.Float("VolumeBoost");
        NoteDelay = data.Float("NoteDelay", 0.075f);
        Colour = data.HexColor("colour");
        ActiveColour = data.HexColor("activeColour", Color.Magenta);
        VolumeBoost = Calc.Clamp(VolumeBoost, -1, 3);
        Add(sine = new SineWave(0.5f));
        Add(overlay_sine = new SineWave(2f));
        string spriteDir = data.Attr("sprite", "objects/bellstone/stone");
        switch (spriteDir) {
            case "normal":
            Add(sprite = new Image(GFX.Game[stone_sprite[0]]));
                break;
            case "tablet":
                Add(sprite = new Image(GFX.Game[stone_sprite[1]]));
                break;
            case "spiky":
                Add(sprite = new Image(GFX.Game[stone_sprite[2]]));
                break;
            default:
                Add(sprite = new Image(GFX.Game[spriteDir]));
                break;
        }
        spriteDir = data.Attr("overlaySprite", "objects/bellstone/stone_glow");
        switch (spriteDir)
        {
            case "normal":
                Add(overlay_sprite = new Image(GFX.Game[overlay_stone_sprite[0]]));
                break;
            case "tablet":
                Add(overlay_sprite = new Image(GFX.Game[overlay_stone_sprite[1]]));
                break;
            case "spiky":
                Add(overlay_sprite = new Image(GFX.Game[overlay_stone_sprite[2]]));
                break;
            default:
                Add(overlay_sprite = new Image(GFX.Game[spriteDir]));
                break;
        }
        sprite.RenderPosition -= new Vector2(8, 0);
        overlay_sprite.RenderPosition -= new Vector2(8, 0);
        overlay_sprite.Color = Colour;
        Add(sfx = new MultiParameterSoundSource());
        sfx.Position.Y = 8f;
        base.Collider = new Circle(12f);
        base.Collider.Position.Y = 8f;
        Vector2 drawAt = new Vector2(data.Width / 2, 0f);
        Add(Talker = new TalkComponent(new Rectangle(-18, -8, 36, 36), drawAt, Interact));
        Talker.Enabled = true;
        base.Depth = 2000;
        ColorOverride = false;
        playing = false;
    }
    float t = 1;
    bool complete = false;
    bool tingDelay = false;

    public OrderKeyBell ActiveBell;

    public override void Render()
    {
        if (ActiveBell != null && ActiveBell.Effect != null)
        {
            ActiveBell.Effect.AddParameters(ActiveBell);
            if (complete)
            {
                ActiveBell.Effect.Parameters["CompleteFade"]?.SetValue(playing ? 0 : ActiveBell.Effect.Parameters["CompleteFade"]?.GetValueSingle() == 1 ? t : ActiveBell.Effect.Parameters["CompleteFade"].GetValueSingle());
            }
            sprite.Render();
            GameplayRenderer.End();
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, ActiveBell.Effect, SceneAs<Level>().Camera.Matrix);
            overlay_sprite.Render();
            Draw.SpriteBatch.End();
            GameplayRenderer.Begin();
        }
        else
        {
            base.Render();
        }
    }


    public override void Update()
    {
        if (!complete)
        {
            complete = Switch.Check(Scene);
            if (complete)
            {
                Add(new Coroutine(Fade()));
            }
        }
        if (!playing && !ColorOverride)
        {
            t = Calc.Approach(t, 1, Engine.DeltaTime);
            if (Leader != null && Leader.Code != null && Leader.Code.Count > 0)
            {
                ActiveBell = Leader.Code[Leader.Code.Count - 1];
                if (ActiveBell != null && ActiveBell.PlaceInQueue.Contains(ColorIndex))
                    overlay_sprite.Color = new Color(Color.Lerp(ActiveBell.Colour, !complete ? Colour : ActiveColour, t), 0.75f + (sine.Value + 1) / 4f);
            }
        }
        else if (!ColorOverride)
        {
            t = 0;
            if (tingDelay)
            {
                overlay_sprite.Color = new Color(0, 0, 0, 0);
            }
            else if (Leader.Code != null)
            {
                OrderKeyBell bell = Leader.Code[ColorIndex];
                if (bell != null && bell.PlaceInQueue.Contains(ColorIndex))
                    overlay_sprite.Color = new Color(bell.Colour, 0.75f + (overlay_sine.Value + 1) / 4f);
            }
        }
        Talker.Enabled = !playing;
        base.Update();
    }

    public void Interact(Player player)
    {
        ColorIndex = 0;
        Add(new Coroutine(Play()));
    }

    bool ColorOverride = false;
    bool playing = false;

    public IEnumerator Play()
    {
        playing = true;
        ColorOverride = false;
        t = 0;
        ColorIndex = 0;

        foreach (OrderKeyBell bell in Leader.Code)
        {
            t = 0;
            tingDelay = false;
            sfx.Play(Leader.Code[ColorIndex].Sound, "pitch", Leader.Code[ColorIndex].Pitch, "speed", 0.25f + 0.25f * VolumeBoost);
            int TimeIndex = Calc.Clamp(ColorIndex, 0, (int)Calc.Max(Speeds.Count - 1, 0));
            yield return Speeds[TimeIndex] <= 0f ? 0.75f : Speeds[TimeIndex];
            ColorIndex++;
            tingDelay = true;
            if (ColorIndex != Leader.Code.Count)
            yield return 0.075f;
        }
        ColorIndex = Leader.Code.Count - 1;
        ColorOverride = true;
        t = 0;
        while (t != 1)
        {
            t = Calc.Approach(t, 1, Engine.DeltaTime);
            overlay_sprite.Color = Color.Lerp(Leader.Code[Leader.Code.Count - 1].Colour, complete ? ActiveColour : Colour, t);
            yield return null;
        }
        ColorOverride = false;
        playing = false;
    }
    public IEnumerator Fade()
    {
        ColorOverride = true;
        t = 0;
        while (t != 1)
        {
            t = Calc.Approach(t, 1, Engine.DeltaTime);
            overlay_sprite.Color = Color.Lerp(Colour, complete ? ActiveColour : Colour, t);
            yield return null;
        }
        ColorOverride = false;
    }
}
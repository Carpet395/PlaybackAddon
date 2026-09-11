using Celeste;
using Celeste.Mod;
using Celeste.Mod.audiohelper.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;


namespace PlayBackAddon.Entities;


[CustomEntity("playback/piano")]
[Tracked(true)]
public class Piano : Entity
{

    public enum PianoModes
    {
        FourWay,
        FourWayShift,
        EightWay,
        EightWayShift
    }

    public PianoModes Mode;

    private Dictionary<int, Bell> Bells;

    public Image sprite;

    public Image SpriteArrows;
    public Image SpriteArrowsHollow;

    public Image SpriteArrowsBase;
    public Image SpriteArrowsBaseHollow;

    public Image spriteArrowsBase
    {
        get
        {
            return (LastShift ? SpriteArrowsBaseHollow : SpriteArrowsBase);
        }
    }

    public Image SpriteArrowsLeft;
    public Image spriteArrowsLeftHollow;

    public bool Shift
    {
        get
        {
            return (Mode == PianoModes.FourWayShift || Mode == PianoModes.EightWayShift) && Input.Grab.Check;
        }
    }

    public Image spriteArrowsLeft
    {
        get
        {
            return (LastShift ? spriteArrowsLeftHollow : SpriteArrowsLeft);
        }
    }

    public Image spriteArrows
    {
        get
        {
            return (LastShift ? SpriteArrowsHollow : SpriteArrows);
        }
    }


    public TalkComponent Talker;

    public OptionsUI UI;

    public Dictionary<float, int> Pitches; 
    public Dictionary<float, int> PitchesShift; 

    Vector2 drawAt;
    Vector2 ArrowDrawAt;

    Color color;

    float ExitTimer;

    public bool NoMiddle = false;

    public Piano(EntityData data, Vector2 offset, EntityID id)
        : base(data.Position + offset)
    {
        Pitches = new Dictionary<float, int>();
        PitchesShift = new Dictionary<float, int>();
        if (Enum.TryParse<PianoModes>(data.Attr("Mode", "FourWay"), out PianoModes Mode))
        {
            this.Mode = Mode;
            switch (Mode)
            {
                case PianoModes.FourWay:
                    Pitches.Add(180, data.Int("Left"));
                    Pitches.Add(0, data.Int("Right"));
                    Pitches.Add(-90, data.Int("Up"));
                    Pitches.Add(90, data.Int("Down"));
                    Pitches.Add(-1, data.Int("Middle"));
                    break;
                case PianoModes.EightWay:
                    NoMiddle = data.Bool("noMiddle");
                    Pitches.Add(180, data.Int("Left"));
                    Pitches.Add(-135, data.Int("UpLeft"));
                    Pitches.Add(135, data.Int("DownLeft"));
                    Pitches.Add(0, data.Int("Right"));
                    Pitches.Add(45, data.Int("DownRight"));
                    Pitches.Add(-90, data.Int("Up"));
                    Pitches.Add(90, data.Int("Down"));
                    Pitches.Add(-1, data.Int("Middle"));
                    break;
                case PianoModes.FourWayShift:
                    Pitches.Add(180, data.Int("Left"));
                    Pitches.Add(0, data.Int("Right"));
                    Pitches.Add(-90, data.Int("Up"));
                    Pitches.Add(90, data.Int("Down"));
                    Pitches.Add(-1, data.Int("Middle"));

                    PitchesShift.Add(180, data.Int("LeftShift"));
                    PitchesShift.Add(0, data.Int("RightShift"));
                    PitchesShift.Add(-90, data.Int("UpShift"));
                    PitchesShift.Add(90, data.Int("DownShift"));
                    PitchesShift.Add(-1, data.Int("MiddleShift"));
                    break;
                case PianoModes.EightWayShift:
                    NoMiddle = data.Bool("noMiddle");
                    Pitches.Add(180, data.Int("Left"));
                    Pitches.Add(-135, data.Int("UpLeft"));
                    Pitches.Add(135, data.Int("DownLeft"));
                    Pitches.Add(0, data.Int("Right"));
                    Pitches.Add(45, data.Int("DownRight"));
                    Pitches.Add(-90, data.Int("Up"));
                    Pitches.Add(90, data.Int("Down"));
                    Pitches.Add(NoMiddle ? -45 : -1, data.Int("Middle"));

                    PitchesShift.Add(180, data.Int("LeftShift"));
                    PitchesShift.Add(-135, data.Int("UpLeftShift"));
                    PitchesShift.Add(135, data.Int("DownLeftShift"));
                    PitchesShift.Add(0, data.Int("RightShift"));
                    PitchesShift.Add(45, data.Int("DownRightShift"));
                    PitchesShift.Add(-90, data.Int("UpShift"));
                    PitchesShift.Add(90, data.Int("DownShift"));
                    PitchesShift.Add(NoMiddle ? -45 : -1, data.Int("MiddleShift"));
                    break;
            }
        }



        Add(sprite = new Image(GFX.Game["objects/bellpiano/piano"]));
        //Pitch = data.Int("pitch");
        drawAt = new Vector2(sprite.Width / 2, 2);
        Add(Talker = new TalkComponent(new Rectangle(-14, -8, 50, 36), drawAt, Interact));
        Talker.Enabled = true;
        Add(SpriteArrows = new Image(GFX.Game["objects/bellpiano/arrows_empty"]));
        Add(SpriteArrowsHollow = new Image(GFX.Game["objects/bellpiano/arrows_empty_hollow"]));

        string baseTex = Mode == PianoModes.FourWay || Mode == PianoModes.FourWayShift ? "objects/bellpiano/arrows_base" : "objects/bellpiano/arrows_base_diagonals";
        if (NoMiddle) baseTex += "_nomiddle";
        Add(SpriteArrowsBase = new Image(GFX.Game[baseTex]));
        Add(SpriteArrowsBaseHollow = new Image(GFX.Game[baseTex + "_hollow"]));
        
        
        Add(SpriteArrowsLeft = new Image(GFX.Game["objects/bellpiano/arrows_left"]));
        Add(spriteArrowsLeftHollow = new Image(GFX.Game["objects/bellpiano/arrows_left_hollow"]));
        ArrowDrawAt = new Vector2(SpriteArrowsBase.Width / 2, SpriteArrowsBase.Height / 2);
        SpriteArrows.CenterOrigin();
        SpriteArrowsHollow.CenterOrigin();
        SpriteArrowsBase.CenterOrigin();
        SpriteArrowsBaseHollow.CenterOrigin();
        SpriteArrowsLeft.CenterOrigin();
        spriteArrowsLeftHollow.CenterOrigin();
        SpriteArrowsHollow.RenderPosition = Position + ArrowDrawAt;
        SpriteArrowsHollow.RenderPosition = Position + ArrowDrawAt;
        SpriteArrowsBase.RenderPosition = Position + ArrowDrawAt;
        SpriteArrowsBaseHollow.RenderPosition = Position + ArrowDrawAt;
        SpriteArrowsLeft.RenderPosition = Position + ArrowDrawAt;
        spriteArrowsLeftHollow.RenderPosition = Position + ArrowDrawAt;
        SpriteArrowsLeft.Visible = false;
        spriteArrowsLeftHollow.Visible = false;
        SpriteArrowsBase.Visible = false;
        SpriteArrowsBaseHollow.Visible = false;
        SpriteArrows.Visible = false;
        SpriteArrowsHollow.Visible = false;
        Add(new BeforeRenderHook(PreRender));
        drawAt = new Vector2(0, -18);
    }

    private void Interact(Player player)
    {
        Add(new Coroutine(PlayRoutine(player)));
    }

    bool Playing = false;
    bool PianoUI = false;

    Vector2 LastAim = Vector2.Zero;
    bool LastShift = false;


    public IEnumerator PlayRoutine(Player player)
    {
        if (UI == null)
        {
            Scene.Add(UI = new(this));
            UI.enabled = false;
        }
        Playing = true;
        PianoUI = true;
        Talker.Enabled = false;
        fade = 0;
        player.StateMachine.State = 11;
        player.StateMachine.Locked = true;

        Bells = new Dictionary<int, Bell>();
        foreach (Bell bell in Scene.Entities.FindAll<Bell>())
        {
            if (!Bells.ContainsKey(bell.Pitch))
            {
                Bells.Add(bell.Pitch, bell);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "PlaybackAddon", "Duplicate bell pitches! choosing the one with the lower ID.");
            }
        }

        Coroutine walk;
        Add(walk = new Coroutine(player.DummyWalkTo(Position.X + sprite.Width / 2 + 4)));
        while (fade < 1f)
        {
            fade += Engine.DeltaTime * 4;
            yield return null;
        }
        fade = 1;

        while (!walk.Finished) yield return null;

        player.Facing = Facings.Left;
        UI.enabled = true;
        while (ExitTimer < ExitTime)
        {
            Vector2 Aim = Mode == PianoModes.FourWay || Mode == PianoModes.FourWayShift ? Input.Aim.Value.FourWayNormal() : Input.Aim.Value.EightWayNormal();

            if (!NoMiddle && Aim == Vector2.Zero)
            {
                RingBell(-1);
            }
            else if (Aim != Vector2.Zero)
            {
                if (!NoMiddle && Aim.Angle().ToDeg() == -45f) Aim = Aim.Rotate(Calc.ToRad(-45));
                RingBell(Aim.Angle().ToDeg());
            }
            yield return null;
            LastShift = (Mode == PianoModes.FourWayShift || Mode == PianoModes.EightWayShift) && Input.Grab.Check;
            LastAim = Aim;
        }
        UI.enabled = false;
        sprite.Color = Color.White;
        Input.Dash.ConsumePress();
        player.StateMachine.Locked = false;
        player.StateMachine.State = 0;
        Playing = false;
        fade = 1f;
        while (fade > 0f)
        {
            fade -= Engine.DeltaTime * 4;
            yield return null;
        }
        PianoUI = false;
        fade = 0f;
        spriteArrowsLeft.Visible = false;
        spriteArrowsBase.Visible = false;
        spriteArrows.Visible = false;

        yield return 0.25f;
        Talker.Enabled = true;
    }

    private bool jumpBuffer = false;
    public float fade = 1f;

    public override void Update()
    {
        jumpBuffer = false;
        if (Playing && Input.Dash.Check)
        {
            ExitTimer += Engine.DeltaTime;
        }
        else
        {
            ExitTimer = 0f;
        }
        if (Input.Jump.Pressed & Playing)
        {
            jumpBuffer = true;
            Input.Jump.ConsumePress();
        }
        base.Update();
    }

    public readonly Color ShiftBlue = new Color(3, 17, 43);
    public readonly Color Blue = new Color(94, 139, 230);

    private void RingBell(float Aim)
    {
        if (!((Mode == PianoModes.FourWayShift || Mode == PianoModes.EightWayShift) && Input.Grab.Check ? PitchesShift : Pitches).ContainsKey(Aim))
        {
            Logger.Log(LogLevel.Warn, "PlaybackAddon", $"Tried Hovering A non found bell, Direction {Aim}"); 
            Scene.Add(new DirectionalSnapshot(TrailManager.Add(Position + ArrowDrawAt, Aim != -1 ? spriteArrowsLeft : spriteArrows, null, Vector2.One, Shift ? ShiftBlue : Blue, 0, 0.5f), Aim != -1 ? Calc.AngleToVector(Aim.ToRad(), 1) : Vector2.Zero, Position + ArrowDrawAt));
            return;
        }
        if (Bells.TryGetValue(((Mode == PianoModes.FourWayShift || Mode == PianoModes.EightWayShift) && Input.Grab.Check ? PitchesShift : Pitches)[Aim], out Bell? bell))
        {
           // sprite.Color = bell.Colour;
            if (jumpBuffer)
            {

                bell?.Ring(30, 0);
                Scene.Add(new DirectionalSnapshot(TrailManager.Add(Position + ArrowDrawAt, Aim != -1 ? spriteArrowsLeft : spriteArrows, null, Vector2.One, Shift ? ShiftBlue : Blue, 0, 0.5f), Aim != -1 ? Calc.AngleToVector(Aim.ToRad(), 1) : Vector2.Zero, Position + ArrowDrawAt));
            }
        }
        else
        {
            Logger.Log(LogLevel.Warn, "PlaybackAddon", "Piano tried to accessing a non existant bell, could be a pitch mismatch or an oversight?");
        }
    }

    public VirtualRenderTarget target;

    public void PreRender()
    {
        if (target == null)
        {
            target = VirtualContent.CreateRenderTarget("piano-options", 24, 24);
        }

        spriteArrows.RenderPosition = ArrowDrawAt;
        spriteArrowsBase.RenderPosition = ArrowDrawAt;
        spriteArrowsLeft.RenderPosition = ArrowDrawAt;

        Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone);
        if (PianoUI)
        {
            spriteArrowsBase.Visible = true;
            spriteArrowsBase.Render();
            spriteArrowsBase.Visible = false;
            if (!NoMiddle && LastAim.Angle().ToDeg() == -45f) LastAim = LastAim.Rotate(Calc.ToRad(-45));
            if (LastAim.Length() > 0)
            {
                spriteArrowsLeft.Visible = true;
                spriteArrowsLeft.Rotation = LastAim.Rotate(Calc.ToRad(180)).Angle();
                spriteArrowsLeft.Render();
                spriteArrowsLeft.Visible = false;
            }
            else if (!NoMiddle)
            {
                spriteArrows.Visible = true;
                spriteArrows.Render();
                spriteArrows.Visible = false;
            }
        }

        spriteArrows.RenderPosition = Position + drawAt + ArrowDrawAt;
        spriteArrowsBase.RenderPosition = Position + drawAt + ArrowDrawAt;
        spriteArrowsLeft.RenderPosition = Position + drawAt + ArrowDrawAt;
        Draw.SpriteBatch.End();
    }

    public override void Render()
    {
        if (target != null)
            Draw.SpriteBatch.Draw(target, Position + drawAt, Color.White * fade);
        sprite.DrawSimpleOutline();
        sprite.Render();
        base.Render();
    }

    public class DirectionalSnapshot : Entity
    {

        public TrailManager.Snapshot Base;
        public Vector2 Dir;
        private Vector2 position;

        public DirectionalSnapshot(TrailManager.Snapshot snapshot, Vector2 Direction, Vector2 Position)
        {
            Base = snapshot;
            Dir = Direction;
            position = Position;
        }



        public override void Update()
        {
            if (Base.Percent >= 1f)
            {
                RemoveSelf();
            }
            Base.Position = position + Dir * Ease.CubeOut(Base.Percent) * 12.5f;

            base.Update();
        }

    }

    float ExitTime = 0.5f;

    public class OptionsUI : Entity
    {
        public bool enabled = true;
        public Piano Piano;

        public OptionsUI(Piano Piano)
        {
            this.Piano = Piano;
            base.Tag = Tags.HUD;
        }

        float ShiftFade = 0;
        float JumpFade = 0;

        public override void Update()
        {
            ShiftFade = Calc.Approach(ShiftFade, Piano.Shift ? 1 : 0, Engine.DeltaTime * 5);
            JumpFade = Calc.Approach(JumpFade, Piano.jumpBuffer ? 1 : 0, Piano.jumpBuffer ? 1 : Engine.DeltaTime * 10f);
            base.Update();
        }

        public override void Render()
        {
            if (enabled)
            {
                string diag = Dialog.Get("OPTIONS_BUTTON_HOLD");
                if (Piano.Mode == PianoModes.FourWayShift || Piano.Mode == PianoModes.EightWayShift)
                {
                    Input.GuiButton(Input.Grab, default).DrawJustified(new Vector2(50, 940), Vector2.Zero, Color.Lerp(Color.White, Piano.Blue, ShiftFade), 0.5f);
                    string diag_shift = Dialog.Get("PLAYBACK_SHIFT");
                    ActiveFont.Draw($"[{diag}]: {diag_shift}", new Vector2(95, 943), Vector2.Zero, Vector2.One / 2, Color.Lerp(Color.White, Piano.Blue, ShiftFade));
                }
                Input.GuiButton(Input.Jump, default).DrawJustified(new Vector2(50, 980), Vector2.Zero, Color.Lerp(Color.White, Color.Yellow, JumpFade), 0.5f);
                Input.GuiButton(Input.Dash, default).DrawJustified(new Vector2(50, 1020), Vector2.Zero, Color.Lerp(Color.White, Color.Red, Piano.ExitTimer / Piano.ExitTime), 0.5f);
                string diag2 = Dialog.Get("MENU_EXIT");
                string diag3 = Dialog.Get("PLAYBACK_PLAY");
                ActiveFont.Draw($": {diag3}", new Vector2(95, 983), Vector2.Zero, Vector2.One / 2, Color.Lerp(Color.White, Color.Yellow, JumpFade));
                ActiveFont.Draw($"[{diag}]: {diag2}", new Vector2(95, 1023), Vector2.Zero, Vector2.One / 2, Color.Lerp(Color.White, Color.Red, Piano.ExitTimer / Piano.ExitTime));
            }
        }

    }
}



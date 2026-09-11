using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace PlayBackAddon.Entities;

[CustomEntity("playback/OrderKeybell", "carpet/OrderKeybell")]
[Tracked(true)]
public class OrderKeyBell : KeyBell
{
    public List<int> PlaceInQueue = new List<int>();

    public OrderKeyBell Leader
    {
        get
        {
            if (PlaceInQueue[0] == 0) return this;
            foreach (OrderKeyBell bell in Scene.Tracker.GetEntities<OrderKeyBell>())
            {
                if (bell != this && bell.PlaceInQueue[0] == 0)
                    return bell;
            }
            return null;
        }
    }

    public List<OrderKeyBell> Code { get; private set; } = null;
    public List<OrderKeyBell> order = new();

    public bool FadeOnTouch = true;

    public OrderKeyBell(EntityData data, Vector2 offset, EntityID id)
        : base(data, offset, id)
    {
        base.TriggerOnTouch = false;
        foreach (var i in data.Attr("order").Split(','))
        {
            if (int.TryParse(i, out int y))
            {
                PlaceInQueue.Add(y);
            }
        }
        FadeOnTouch = data.Bool("FadeBackOnTouch", true);
    }

    public override void Update()
    {
        if (PlaceInQueue[0] == 0 && Code == null)
        {
            Code = Scene.Tracker.GetEntities<OrderKeyBell>()
                .OfType<OrderKeyBell>()
                .SelectMany(bell => bell.PlaceInQueue.Except(new int[] { -1 }).Select(order => new { Bell = bell, Order = order }))
                .OrderBy(x => x.Order)
                .Select(x => x.Bell)
                .ToList();
            Logger.Log(LogLevel.Debug, "MtCarpet/LeaderBell", "Loaded as leader! code length is: " + Code.Count);
        }
        base.Update();
    }

    public override void SceneEnd(Scene scene)
    {
        RemoveSelf();
        base.SceneEnd(scene);
    }

    public override void Ring(float xSpeed, float ySpeed)
    {
        Collider MainCollider = base.Collider;
        base.Collider = new Circle(16f);
        base.Collider.Position.Y = 0f;
        if (CollideFirst<BadelineOldsite>() != null)
        {
            base.Collider = MainCollider;
            Logger.Log(LogLevel.Debug, "MtCarpet/BellChased", "Bell touched by chaser!");
            return;
        }
        base.Collider = MainCollider;
        Leader.CheckBells(this);
        base.Ring(xSpeed, ySpeed);
    }

    public void TouchFade()
    {
        if (FadeOnTouch)
        {
            Add(new Coroutine(Colorfade(Colour, ActiveColour, 1f)));
        }
    }

    public void CheckBells(OrderKeyBell callBell)
    {
        if (Leader != this) return;

        if (Switch.Activated)
        {
            foreach (OrderKeyBell bell in Scene.Tracker.GetEntities<OrderKeyBell>())
            {
                bell.TouchFade();
            }
        }
        else
        {
            order.Add(callBell);
            if (order.Count > Code.Count)
            {
                order.RemoveAt(0);
            }

            if (order.Count == Code.Count)
            {
                bool flag = true;
                for (int i = 0; i < Code.Count; i++)
                {
                    if (!order[i].Equals(Code[i]))
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    foreach (OrderKeyBell bell in Scene.Tracker.GetEntities<OrderKeyBell>())
                    {
                        bell.TurnOn();
                    }
                }
            }
        }
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        if (Leader == this)
        {
            base.Collider.Render(camera, Color.Gold);
        }
    }
}

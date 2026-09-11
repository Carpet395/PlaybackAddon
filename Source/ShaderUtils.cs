using Celeste.Mod.audiohelper.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using PlayBackAddon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.playback.Modules;

public static class ShaderUtils
{
    public static Dictionary<string, Effect> effectDictionary = new();

    public static void Load()
    {
        Everest.Content.OnUpdate += Content_OnUpdate;
    }


    public static void Content_OnUpdate(ModAsset from, ModAsset to)
    {
        if (to.Format is "cso" && effectDictionary.ContainsKey(to.PathVirtual.TrimEnd(".cso".ToCharArray())))
        {
            effectDictionary[to.PathVirtual.TrimEnd(".cso".ToCharArray())] = new Effect(Engine.Graphics.GraphicsDevice, to.Data);
        }
    }


    public static Effect? TryGetEffect(string id)
    {
        if (effectDictionary.ContainsKey(id))
        {
            return effectDictionary[id];
        }
        else if (Everest.Content.TryGet(id + ".cso", out var metadata, includeDirs: true))
        {
            try
            {
                Effect effect = new Effect(Engine.Graphics.GraphicsDevice, metadata.Data);
                effectDictionary.Add(id, effect);
                return effect;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "MtCarpet/Shader", "Couldn't load the shader " + id);
                Logger.Log(LogLevel.Error, "MtCarpet/Shader", "Exception: \n" + ex.ToString());
            }
        }
        Logger.Log(LogLevel.Warn, "MtCarpet/Shader", "Couldn't find shader " + id);
        return null;
    }

    public static Effect AddParameters(this Effect Effect)
    {
        if (Engine.Scene is Level)
        {
            Level level = Engine.Scene as Level;
            EffectParameterCollection parameters = Effect.Parameters;
            parameters["DeltaTime"]?.SetValue(Engine.DeltaTime);
            parameters["Time"]?.SetValue(Engine.Scene.TimeActive);
            parameters["Dimensions"]?.SetValue(new Vector2(320, 180));
            if (level != null)
            {
                parameters["CamPos"]?.SetValue(level.Camera.Position);
            }
            Viewport viewport = Engine.Graphics.GraphicsDevice.Viewport;
            Matrix matrix = Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, 0f, 1f);
            Matrix matrix2 = Matrix.Identity;
            parameters["TransformMatrix"]?.SetValue(matrix2 * matrix);
            parameters["ViewMatrix"]?.SetValue(level.Camera.Matrix);
        }
        return Effect;
    }

    public static Effect AddParameters(this Effect Effect, Bell bell)
    {
        if (Engine.Scene is Level)
        {
            Level level = Engine.Scene as Level;
            EffectParameterCollection parameters = Effect.Parameters;
            parameters["DeltaTime"]?.SetValue(Engine.DeltaTime);
            parameters["Time"]?.SetValue(Engine.Scene.TimeActive);
            parameters["Dimensions"]?.SetValue(new Vector2(bell.sprite.Width, bell.sprite.Height));
            if (level != null)
            {
                parameters["CamPos"]?.SetValue(level.Camera.Position);
            }
            Viewport viewport = Engine.Graphics.GraphicsDevice.Viewport;
            Matrix matrix = Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, 0f, 1f);
            Matrix matrix2 = Matrix.Identity;
            parameters["TransformMatrix"]?.SetValue(matrix2 * matrix);
            parameters["ViewMatrix"]?.SetValue(level.Camera.Matrix);
            parameters["Colour"]?.SetValue(bell.Colour.ToVector3());
            parameters["Pitch"]?.SetValue(bell.Pitch);
            parameters["Angle"]?.SetValue(bell.Angle);
        }
        return Effect;
    }
    public static Effect AddParameters(this Effect Effect, KeyBell bell)
    {
        if (Engine.Scene is Level)
        {
            Level level = Engine.Scene as Level;
            EffectParameterCollection parameters = Effect.Parameters;
            parameters["DeltaTime"]?.SetValue(Engine.DeltaTime);
            parameters["Time"]?.SetValue(Engine.Scene.TimeActive);
            parameters["Dimensions"]?.SetValue(new Vector2(bell.sprite.Width, bell.sprite.Height));
            if (level != null)
            {
                parameters["CamPos"]?.SetValue(level.Camera.Position);
            }
            Viewport viewport = Engine.Graphics.GraphicsDevice.Viewport;
            Matrix matrix = Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, 0f, 1f);
            Matrix matrix2 = Matrix.Identity;
            parameters["TransformMatrix"]?.SetValue(matrix2 * matrix);
            parameters["ViewMatrix"]?.SetValue(level.Camera.Matrix);
            parameters["Complete"]?.SetValue(bell.Switch.Activated);
            parameters["CompleteFade"]?.SetValue(bell.t);
            parameters["Colour"]?.SetValue(bell.Colour.ToVector3());
            parameters["CompleteColour"]?.SetValue(bell.Colour.ToVector3());
            parameters["Pitch"]?.SetValue(bell.Pitch);
            parameters["Angle"]?.SetValue(bell.Angle);
        }
        return Effect;
    }
}
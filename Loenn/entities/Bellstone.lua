local drawableSprite = require("structs.drawable_sprite")
local Bell = {}

local sprite_options = { "objects/bellstone/stone", "objects/bellstone/stone_alt", "objects/bellstone/stone_alt_2"}
local overlay_sprite_options = { "objects/bellstone/stone_glow", "objects/bellstone/stone_glow_alt", "objects/bellstone/stone_glow_alt_2"}

local option_names = { "normal", "tablet", "spiky" }

Bell.name = "playback/Bellstone"
Bell.depth = -8500
Bell.fieldInformation = {
    speed = {
        fieldType = "list",
        elementSeparator = ",",
        elementDefault = "0.75",
        elementOptions = {
        }
    },
    colour = {
        fieldType = "color"
    },
    activeColour = {
        fieldType = "color"
    },
    sprite = {
        options = option_names,
        editable = true
    },
    overlaySprite = {
        options = option_names,
        editable = true
    }
}
Bell.placements = {
    name = "Bellstone",
    data = {
        sprite = "normal",
        overlaySprite = "normal",
        speed = "0.75",
        NoteDelay = "0.075",
        colour = "fc9803",
        activeColour = "de047c",
        VolumeBoost = 0
    }
}
function Bell.sprite(room, entity)
    local sprites = {}
    local sprite
    if entity.sprite == "normal" then
        sprite = drawableSprite.fromTexture(sprite_options[1], entity)
    elseif entity.sprite == "tablet" then
        sprite = drawableSprite.fromTexture(sprite_options[2], entity)
    elseif entity.sprite == "spiky" then
        sprite = drawableSprite.fromTexture(sprite_options[3], entity)
    else
        sprite = drawableSprite.fromTexture(entity.sprite, entity)
    end
    local sprite2
    if entity.overlaySprite == "normal" then
        sprite2 = drawableSprite.fromTexture(overlay_sprite_options[1], entity)
    elseif entity.overlaySprite == "tablet" then
        sprite2 = drawableSprite.fromTexture(overlay_sprite_options[2], entity)
    elseif entity.overlaySprite == "spiky" then
        sprite2 = drawableSprite.fromTexture(overlay_sprite_options[3], entity)
    else
        sprite2 =  drawableSprite.fromTexture(entity.overlaySprite, entity)
    end
    if not sprite then  
        sprite = drawableSprite.fromTexture("objects/bellstone/stone", entity)
    end
    if not sprite2 then
        sprite2 = drawableSprite.fromTexture("objects/bellstone/stone_glow", entity)    
    end
    sprite:setJustification(0.5, 0)
    sprite2:setJustification(0.5, 0)
    sprite2:setColor(entity.colour)
    table.insert(sprites, sprite)
    table.insert(sprites, sprite2)
    return sprites
end

return Bell

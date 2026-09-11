local drawableSprite = require("structs.drawable_sprite")
local Piano = {}
local noteNames = {
    {"E6", 24},
    {"D#/Eb6", 23},
    {"D6", 22},
    {"C#/Db6", 21},
    {"C6", 20},
    {"B5", 19},
    {"A#/Bb5", 18},
    {"A5", 17},
    {"G#/Ab5", 16},
    {"G5", 15},
    {"F#/Gb5", 14},
    {"F5", 13},
    {"E5", 12},
    {"D#/Eb5", 11},
    {"D5", 10},
    {"C#/Db5", 9},
    {"C5", 8},
    {"B4", 7},
    {"A#/Bb4", 6},
    {"A4", 5},
    {"G#/Ab4", 4},
    {"G4", 3},
    {"F#/Gb4", 2},
    {"F4", 1},
    {"E4",  0},
}
local sounds ={
    "event:/vert_audiohelper/bell",
    "event:/vert_audiohelper/chime"
}
local textures = {
    "objects/bellpiano/piano",
    "objects/bellpiano/piano_stone"
}

Piano.name = "playback/piano"
Piano.depth = 100
Piano.fieldInformation = {
    texture = {
        fieldType = "string",
        options = textures,
        editable = true
    },
    Left = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    Right = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    Up = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    Down = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    UpLeft = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    DownLeft = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    DownRight = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    Middle = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    LeftShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    RightShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    UpShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    DownShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    UpLeftShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    DownLeftShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    DownRightShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    },
    MiddleShift = {
        fieldType = "integer",
        options = noteNames,
        editable = false,
    }
}
function Piano.ignoredFields(entity)
    local ignored = {   
        "Mode",
    }

    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end

    return ignored
end

Piano.fieldOrder = {
    "x", "y",
    "MiddleShift", "RightShift",
    "DownRightShift", "DownShift",
    "DownLeftShift", "LeftShift",
    "UpLeftShift", "UpShift",
    
    "Middle","Right",
    "DownRight", "Down",
    "DownLeft","Left",
    "UpLeft", "Up",
    "noMiddle", "texture"
}

Piano.placements = {
    {
        name = "Piano_Four",
        data = {
            Mode = "FourWay",
            Middle = 12,
            Up = 24,
            Down = 17,
            Left = 20,
            Right = 13,
            texture = "objects/bellpiano/piano"
        }
    },
    {
        name = "Piano_Eight",
        data = {
            Mode = "EightWay",
            Middle = 12,
            Up = 24,
            Down = 17,
            Left = 20,
            UpLeft = 22,
            DownLeft = 19,
            Right = 13,
            DownRight = 15,
            noMiddle = false,
            texture = "objects/bellpiano/piano"
        }
    },
        {
        name = "Piano_FourShift",
        data = {
            Mode = "FourWayShift",
            MiddleShift = 0,
            UpShift = 12,
            DownShift = 5,
            LeftShift = 8,
            RightShift = 1,

            Middle = 12,
            Up = 24,
            Down = 17,
            Left = 20,
            Right = 13,
            texture = "objects/bellpiano/piano"
        }
    },
    {
        name = "Piano_EightShift",
        data = {
            Mode = "EightWayShift",
            MiddleShift = 0,
            UpShift = 12,
            DownShift = 5,
            LeftShift = 8,
            UpLeftShift = 10,
            DownLeftShift = 7,
            RightShift = 1,
            DownRightShift = 3,

            Middle = 12,
            Up = 24,
            Down = 17,
            Left = 20,
            UpLeft = 22,
            DownLeft = 19,
            Right = 13,
            DownRight = 15,
            noMiddle = false,
            texture = "objects/bellpiano/piano"
        }
    }
}

function Piano.sprite(room, entity)
    local sprite = drawableSprite.fromTexture(entity.texture, entity)
    sprite:setJustification(-0.00, -0.00)
    return sprite
end

return Piano
export interface ApiProperty {
  type: ApiClass | string
  isArray: boolean
}

function $Property(type: ApiClass | string, isArray: boolean = false): ApiProperty {
  return { type, isArray };
}

export interface ApiParameter {
  name: string
  types: (ApiClass | string)[]
  isOptional: boolean
}

function $Parameter(name: string, types: ApiClass | string | (ApiClass | string)[], isOptional: boolean = false): ApiParameter {
  return { name, types: Array.isArray(types) ? types : [types], isOptional };
}

export interface ApiMethod {
  parameters: ApiParameter[]
  return?: ApiParameter
  isOptional: boolean
}

function $Method(parameters: ApiParameter[], _return: ApiParameter | undefined = undefined, isOptional: boolean = false): ApiMethod {
  return { parameters, return: _return, isOptional };
}

export function isMethod(obj: any): obj is ApiMethod {
  if (typeof obj !== 'object' || obj === null) {
    return false;
  }

  if (!Array.isArray(obj.parameters)) {
    return false;
  }

  return true;
}

type ApiClassMembers = { [key: string]: ApiMethod | ApiProperty | ApiProperty[] };

export interface ApiClass {
  name: string
  isArray: boolean
  members?: ApiClassMembers
  generics?: (ApiClass | string)[]
  example?: string
}

function $Class(name: string, setMembers?: (self: ApiClass) => ApiClassMembers, generics?: (ApiClass | string)[], isArray: boolean = false, example?: string): ApiClass {
  const obj: ApiClass = { name, isArray, members: setMembers ? {} : undefined, generics, example };
  if (setMembers !== undefined) {
    obj.members = setMembers(obj);
  }
  return obj;
}

export interface ApiModule {
  name: string
  type: ApiClass
  subcategory?: ApiModule[]
}

const base = {
  any: $Class("any"),
  nil: $Class("nil"),
  boolean: $Class("boolean"),
  string: $Class("string"),
  stringList: $Class("string", undefined, undefined, true),
  number: $Class("number"),
  integer: $Class("integer"),
  function: $Class("function")
};

const Bytes = $Class("Bytes");
const EmptyTable = $Class("EmptyTable");

const SkiaSharp_SKPaintStyle = $Class("SkiaSharp.SKPaintStyle");
const SkiaSharp_SKTextAlign = $Class("SkiaSharp.SKTextAlign");
const SkiaSharp_SKStrokeCap = $Class("SkiaSharp.SKStrokeCap");
const SkiaSharp_SKStrokeJoin = $Class("SkiaSharp.SKStrokeJoin");
const SkiaSharp_SKBlendMode = $Class("SkiaSharp.SKBlendMode");
const SkiaSharp_SKPathDirection = $Class("SkiaSharp.SKPathDirection");
const SkiaSharp_SKPathFillType = $Class("SkiaSharp.SKPathFillType");
const SkiaSharp_SKPathEffectTrimMode = $Class("SkiaSharp.SKPathEffectTrimMode");
const SkiaSharp_SKShaderTileMode = $Class("SkiaSharp.SKShaderTileMode");
const SkiaSharp_SKColorType = $Class("SkiaSharp.SKColorType");
const SkiaSharp_SKPathEffect = $Class("SkiaSharp.SKPathEffect");
const SkiaSharp_SKShader = $Class("SkiaSharp.SKShader");
const SkiaSharp_SKPaint = $Class("SkiaSharp.SKPaint");
const SkiaSharp_SKPath = $Class("SkiaSharp.SKPath");
const SkiaSharp_SKFont = $Class("SkiaSharp.SKFont");
const SkiaSharp_SKBitmap = $Class("SkiaSharp.SKBitmap");
const SkiaSharp_SKRect = $Class("SkiaSharp.SKRect");
const SkiaSharp_SKImageInfo = $Class("SkiaSharp.SKImageInfo");
const SkiaSharp_SKPoint = $Class("SkiaSharp.SKPoint");
const SkiaSharp_SKMatrix = $Class("SkiaSharp.SKMatrix");


const ResultMatchHandlers = (_return: (ApiClass | string)[] = []) => $Class("Rotoris.Result.MatchHandlers",
  (self) => ({
    "Ok": $Method(
      [
        $Parameter("value", _return)
      ],
      $Parameter("return", base.any)
    ),
    "Err": $Method(
      [
        $Parameter("error", base.string)
      ],
      $Parameter("return", base.any)
    )
  }),
  _return
);

const Result = (_return: (ApiClass | string)[] = []) => $Class("Rotoris.Result",
  (self) => ({
    "is_ok": $Property(base.boolean),
    "is_err": $Property(base.boolean),
    "ok": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("return", [..._return, base.nil])
    ),
    "err": $Method(
      [
        $Parameter("self", [self])
      ],
      $Parameter("return", [base.string, base.nil])
    ),
    "unwrap": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("return", _return)
    ),
    "match": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("handlers", ResultMatchHandlers(_return))
    )
  }),
  _return
);

const Async = (_return: (ApiClass | string)[] = []) => $Class("Rotoris.Async",
  (self) => ({
    "wait": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("return", Result(_return))
    ),
    "match": $Method(
      [
        $Parameter("self", self),
        $Parameter("luaResultHandlers", ResultMatchHandlers(_return)),
        $Parameter("timeoutMilliseconds", base.number, true)
      ],
      $Parameter("return", base.any)
    )
  }),
  _return
);

const env = $Class("Rotoris.Environment",
  (self) => ({
    "UiSize": $Property(base.number),
    "ModuleName": $Property(base.string)
  })
);

const processAudioSession = $Class("Rotoris.LuaAudio.ProcessAudioSession", (self) => ({
  "SessionId": $Property(base.string),
  "ProcessName": $Property(base.string)
}));

const processAudioSessionList: ApiClass = { ...processAudioSession, isArray: true };

const microphoneSession = $Class("Rotoris.LuaAudio.MicrophoneSession", (self) => ({
  "SessionId": $Property(base.string),
  "FriendlyName": $Property(base.string)
}));

const microphoneSessionList: ApiClass = { ...microphoneSession, isArray: true };

const audio = $Class("Rotoris.LuaAudio",
  (self) => ({
    "get_sessions": $Method(
      [
        $Parameter("self", self),
        $Parameter("emptyTable", EmptyTable)
      ],
      $Parameter("return", Async([processAudioSessionList]))
    ),
    "increase_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil]),
        $Parameter("volumeChange", base.number)
      ],
      $Parameter("return", Async([base.number, base.nil]))
    ),
    "set_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil]),
        $Parameter("volume", base.number)
      ],
      $Parameter("return", Async([base.boolean]))
    ),
    "decrease_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil]),
        $Parameter("volumeChange", base.number)
      ],
      $Parameter("return", Async([base.number, base.nil]))
    ),
    "get_current_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", base.string, true)
      ],
      $Parameter("return", Async([base.number]))
    ),
    "toggle_mute": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", base.string, true)
      ],
      $Parameter("return", Async([base.boolean]))
    ),
    "is_muted": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", base.string, true)
      ],
      $Parameter("return", Async([base.boolean]))
    ),
    "get_mic_sessions": $Method(
      [
        $Parameter("self", self),
        $Parameter("emptyTable", EmptyTable)
      ],
      $Parameter("return", Async([microphoneSessionList]))
    ),
    "increase_mic_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil]),
        $Parameter("volumeChange", base.number)
      ],
      $Parameter("return", Async([base.boolean]))
    ),
    "set_mic_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil]),
        $Parameter("volume", base.number)
      ],
      $Parameter("return", Async([base.boolean]))
    ),
    "decrease_mic_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil]),
        $Parameter("volumeChange", base.number)
      ],
      $Parameter("return", Async([base.boolean]))
    ),
    "get_mic_volume": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil])
      ],
      $Parameter("return", Async([base.number]))
    ),
    "toggle_mic_mute": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil])
      ],
      $Parameter("return", Async([base.boolean]))
    ),
    "is_mic_muted": $Method(
      [
        $Parameter("self", self),
        $Parameter("id", [base.string, base.nil])
      ],
      $Parameter("return", Async([base.boolean]))
    )
  })
);

const cache = $Class("Rotoris.LuaCache", (self) => ({
  "set": $Method(
    [
      $Parameter("self", self),
      $Parameter("key", base.string),
      $Parameter("value", base.any)
    ]
  ),
  "get": $Method(
    [
      $Parameter("self", self),
      $Parameter("key", base.string)
    ],
    $Parameter("return", base.any)),
  "remove": $Method(
    [
      $Parameter("self", self),
      $Parameter("key", base.string)
    ],
    $Parameter("return", base.any)),
  "exists": $Method(
    [
      $Parameter("self", self),
      $Parameter("key", base.string)
    ],
    $Parameter("return", base.boolean)),
  "clear": $Method(
    [
      $Parameter("self", self)]),
  "exclusive": $Method(
    [
      $Parameter("self", self),
      $Parameter("callback", base.function)])
}));

const canvasContextArgs = $Class("Rotoris.LuaCanvas.CanvasContext.Args", (self) => ({
  "CanvasWidth": $Property(base.number),
  "CanvasHeight": $Property(base.number),
  "DeltaTime": $Property(base.number)
}));

const canvasPath = $Class("Rotoris.LuaCanvas.CanvasPath", (self) => ({
  "reset": $Method(
    [
      $Parameter("self", self)]),
  "move_to": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number)]),
  "line_to": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number)]),
  "quad_to": $Method(
    [
      $Parameter("self", self),
      $Parameter("x1", base.number),
      $Parameter("y1", base.number),
      $Parameter("x2", base.number),
      $Parameter("y2", base.number)]),
  "cubic_to": $Method(
    [
      $Parameter("self", self),
      $Parameter("x1", base.number),
      $Parameter("y1", base.number),
      $Parameter("x2", base.number),
      $Parameter("y2", base.number),
      $Parameter("x3", base.number),
      $Parameter("y3", base.number)]),
  "close": $Method(
    [
      $Parameter("self", self)]),
  "add_rect": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("width", base.number),
      $Parameter("height", base.number),
      $Parameter("direction", SkiaSharp_SKPathDirection, true)]),
  "add_circle": $Method(
    [
      $Parameter("self", self),
      $Parameter("cx", base.number),
      $Parameter("cy", base.number),
      $Parameter("radius", base.number),
      $Parameter("direction", SkiaSharp_SKPathDirection, true)]),
  "add_arc": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("width", base.number),
      $Parameter("height", base.number),
      $Parameter("startAngle", base.number),
      $Parameter("sweepAngle", base.number),
      $Parameter("forceMoveTo", base.boolean, true)]),
  "add_oval": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("width", base.number),
      $Parameter("height", base.number),
      $Parameter("direction", SkiaSharp_SKPathDirection, true)]),
  "add_path": $Method(
    [
      $Parameter("self", self),
      $Parameter("anotherPath", "Rotoris.LuaCanvas.CanvasPath"),
      $Parameter("x", base.number, true),
      $Parameter("y", base.number, true)]),
  "add_path_reverse": $Method(
    [
      $Parameter("self", self),
      $Parameter("anotherPath", "Rotoris.LuaCanvas.CanvasPath")]),
  "add_poly": $Method(
    [
      $Parameter("self", self),
      $Parameter("points", "SkiaSharp.SKPoint[]"),
      $Parameter("close", base.boolean, true)]),
  "add_round_rect": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("width", base.number),
      $Parameter("height", base.number),
      $Parameter("rx", base.number),
      $Parameter("ry", base.number),
      $Parameter("direction", SkiaSharp_SKPathDirection, true)]),
  "arc_to": $Method(
    [
      $Parameter("self", self),
      $Parameter("p1X", base.number),
      $Parameter("p1Y", base.number),
      $Parameter("p2X", base.number),
      $Parameter("p2Y", base.number),
      $Parameter("radius", base.number)]),
  "set_fill_type": $Method(
    [
      $Parameter("self", self),
      $Parameter("fillType", SkiaSharp_SKPathFillType)]),
  "get_fill_type": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", SkiaSharp_SKPathFillType)),
  "get": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", SkiaSharp_SKPath))
}));

const fontCache = $Class("Rotoris.LuaCanvas.FontCache", (self) => ({
  "load_from_file": $Method(
    [
      $Parameter("self", self),
      $Parameter("familyName", base.string),
      $Parameter("filePath", base.string)]),
  "get": $Method(
    [
      $Parameter("self", self),
      $Parameter("familyName", base.string),
      $Parameter("fontSize", base.number)
    ],
    $Parameter("return", SkiaSharp_SKFont)),
  "dispose": $Method(
    [
      $Parameter("self", self),
      $Parameter("familyName", base.string),
      $Parameter("fontSize", base.number)
    ],
    $Parameter("return", base.boolean)),
  "dispose_by_family": $Method(
    [
      $Parameter("self", self),
      $Parameter("familyName", base.string)
    ],
    $Parameter("return", base.boolean))
}));

const imageCache = $Class("Rotoris.LuaCanvas.ImageCache", (self) => ({
  "load_bytes": $Method(
    [
      $Parameter("self", self),
      $Parameter("imageId", base.string),
      $Parameter("imageData", Bytes)
    ],
    $Parameter("return", SkiaSharp_SKBitmap)),
  "get": $Method(
    [
      $Parameter("self", self),
      $Parameter("imagePath", base.string)
    ],
    $Parameter("return", SkiaSharp_SKBitmap)),
  "load": $Method(
    [
      $Parameter("self", self),
      $Parameter("filePath", base.string)
    ],
    $Parameter("return", SkiaSharp_SKImageInfo)),
  "dispose": $Method(
    [
      $Parameter("self", self),
      $Parameter("filePath", base.string)
    ],
    $Parameter("return", base.boolean))
}));

const paintCacheEditor = $Class("Rotoris.LuaCanvas.PaintCache.Editor", (self) => ({
  "set_color": $Method(
    [
      $Parameter("self", self),
      $Parameter("hexColor", base.string)]),
  "set_style": $Method(
    [
      $Parameter("self", self),
      $Parameter("style", SkiaSharp_SKPaintStyle)]),
  "set_blend_mode": $Method(
    [
      $Parameter("self", self),
      $Parameter("mode", SkiaSharp_SKBlendMode)]),
  "set_antialias": $Method(
    [
      $Parameter("self", self),
      $Parameter("enabled", base.boolean)]),
  "set_dither": $Method(
    [
      $Parameter("self", self),
      $Parameter("enabled", base.boolean)]),
  "set_stroke_width": $Method(
    [
      $Parameter("self", self),
      $Parameter("width", base.number)]),
  "set_stroke_cap": $Method(
    [
      $Parameter("self", self),
      $Parameter("cap", SkiaSharp_SKStrokeCap)]),
  "set_stroke_join": $Method(
    [
      $Parameter("self", self),
      $Parameter("join", SkiaSharp_SKStrokeJoin)]),
  "set_stroke_miter": $Method(
    [
      $Parameter("self", self),
      $Parameter("miter", base.number)]),
  "set_path_effect": $Method(
    [
      $Parameter("self", self),
      $Parameter("effect", SkiaSharp_SKPathEffect, true)]),
  "set_shader": $Method(
    [
      $Parameter("self", self),
      $Parameter("shader", SkiaSharp_SKShader, true)])
}));

const paintCache = $Class("Rotoris.LuaCanvas.PaintCache", (self) => ({
  "create": $Method(
    [
      $Parameter("self", self),
      $Parameter("name", base.string),
      $Parameter("table", "table")
    ],
    $Parameter("return", SkiaSharp_SKPaint)),
  "set": $Method(
    [
      $Parameter("self", self),
      $Parameter("paint_or_name", [SkiaSharp_SKPaint, base.string])]),
  "update": $Method(
    [
      $Parameter("self", self),
      $Parameter("name_or_action", [base.string, base.function]),
      $Parameter("action", base.function, true)
    ],
    $Parameter("return", base.boolean)),
  "get": $Method(
    [
      $Parameter("self", self),
      $Parameter("name", base.string, true)
    ],
    $Parameter("return", SkiaSharp_SKPaint)),
  "dispose": $Method(
    [
      $Parameter("self", self),
      $Parameter("name", base.string, true)])
}));

const canvasContextDrawRectOptions = [
  $Class("Rotoris.LuaCanvas.CanvasContext.RectangleRadiusOptions",
    (self) => ({
      "Radius": $Property(base.number)
    })
  ),
  $Class("Rotoris.LuaCanvas.CanvasContext.RectangleRadiiOptions",
    (self) => ({
      "RadiusX": $Property(base.number),
      "RadiusY": $Property(base.number),
    })
  )
];

const canvasContext = $Class("Rotoris.LuaCanvas.CanvasContext", (self) => ({
  "paints": $Property(paintCache),
  "images": $Property(imageCache),
  "fonts": $Property(fontCache),
  "clear": $Method(
    [
      $Parameter("self", self),
      $Parameter("hexColor", base.string, true)]),
  "set_paint": $Method(
    [
      $Parameter("self", self),
      $Parameter("name", base.string)]),
  "draw_line": $Method(
    [
      $Parameter("self", self),
      $Parameter("x1", base.number),
      $Parameter("y1", base.number),
      $Parameter("x2", base.number),
      $Parameter("y2", base.number)]),
  "draw_oval": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("rx", base.number),
      $Parameter("ry", base.number)]),
  "draw_rect": $Method(
    [
      $Parameter("self", self),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("width", base.number),
      $Parameter("height", base.number),
      $Parameter("options", canvasContextDrawRectOptions, true)]),
  "draw_circle": $Method(
    [
      $Parameter("self", self),
      $Parameter("cx", base.number),
      $Parameter("cy", base.number),
      $Parameter("radius", base.number)]),
  "draw_text": $Method(
    [
      $Parameter("self", self),
      $Parameter("text", base.string),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("fontSize", base.number, true),
      $Parameter("familyName", base.string, true)]),
  "measure_text": $Method(
    [
      $Parameter("self", self),
      $Parameter("text", base.string),
      $Parameter("fontSize", base.number, true),
      $Parameter("familyName", base.string, true)
    ],
    $Parameter("return", SkiaSharp_SKRect)),
  "draw_image": $Method(
    [
      $Parameter("self", self),
      $Parameter("filePath", base.string),
      $Parameter("x", base.number),
      $Parameter("y", base.number),
      $Parameter("width", base.number, true),
      $Parameter("height", base.number, true)]),
  "create_path": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", canvasPath)),
  "draw_path": $Method(
    [
      $Parameter("self", self),
      $Parameter("action", base.function)]),
  "translate": $Method(
    [
      $Parameter("self", self),
      $Parameter("dx", base.number),
      $Parameter("dy", base.number)]),
  "scale": $Method(
    [
      $Parameter("self", self),
      $Parameter("sx", base.number),
      $Parameter("sy", base.number)]),
  "rotate": $Method(
    [
      $Parameter("self", self),
      $Parameter("degrees", base.number)]),
  "save": $Method(
    [
      $Parameter("self", self)]),
  "restore": $Method(
    [
      $Parameter("self", self)]),
  "delay": $Method(
    [
      $Parameter("self", self),
      $Parameter("milliseconds", base.number)]),
  "done": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", base.boolean))
}));

const canvasContextDrawState = $Class("Rotoris.LuaCanvas.DrawBlueprint.State");

const drawBlueprint = $Class("Rotoris.LuaCanvas.DrawBlueprint", (self) => ({
  "OnInit": $Method(
    [
      $Parameter("ctx", canvasContext),
      $Parameter("args", canvasContextArgs)
    ],
    $Parameter("return", canvasContextDrawState),
    true
  ),
  "OnUpdate": $Method(
    [
      $Parameter("ctx", canvasContext),
      $Parameter("args", canvasContextArgs),
      $Parameter("state", canvasContextDrawState)
    ],
    undefined,
    true
  )
}),
  undefined,
  false,
  `
local Skia = require('SkiaSharp')
local MIN_FRAME_DELAY_SEC = 1
local POINT_RADIUS = 10;
canvas:draw({
    OnInit = function(ctx, args)
        ctx.paints:create("redPoint", {
            Color = "#a00",
            IsAntialias = true,
            Style = Skia.PaintStyle.Fill
        })

        return {
            count = 0,
            x = args.CanvasWidth / 2,
            y = args.CanvasHeight / 2
        }
    end,
    OnUpdate = function(ctx, args, state)
        ctx:set_paint("redPoint")
        ctx:draw_circle(state.x, state.y, POINT_RADIUS)

        if state.count >= 10 then
            ctx:done()
            return
        end

        state.count = state.count + 1
        state.x = state.x + 15

        local required_delay_sec = MIN_FRAME_DELAY_SEC - args.DeltaTime
        local delay_ms = required_delay_sec * 1000
        if delay_ms > 0 then
            ctx:delay(delay_ms)
        end
    end
})
`
);

const canvas = $Class("Rotoris.LuaCanvas", (self) => ({
  "draw": $Method(
    [
      $Parameter("self", self),
      $Parameter("table", drawBlueprint)]),
  "set_size": $Method(
    [
      $Parameter("self", self),
      $Parameter("width", base.number, true),
      $Parameter("height", base.number, true)]),
  "clear": $Method(
    [
      $Parameter("self", self)])
}));

const fs = $Class("Rotoris.LuaFileSystem", (self) => ({
  "read_file": $Method(
    [
      $Parameter("self", self),
      $Parameter("path", base.string)
    ],
    $Parameter("return", base.string)),
  "read_file_bytes": $Method(
    [
      $Parameter("self", self),
      $Parameter("path", base.string)
    ],
    $Parameter("return", Bytes)),
  "write_file": $Method(
    [
      $Parameter("self", self),
      $Parameter("path", base.string),
      $Parameter("content", base.string)]),
  "write_file_bytes": $Method(
    [
      $Parameter("self", self),
      $Parameter("path", base.string),
      $Parameter("content", Bytes)]),
  "file_exists": $Method(
    [
      $Parameter("self", self),
      $Parameter("path", base.string)
    ],
    $Parameter("return", base.boolean)),
  "file_delete": $Method(
    [
      $Parameter("self", self),
      $Parameter("path", base.string)])
}));

const hid = $Class("Rotoris.LuaHID", (self) => ({
  "key_down": $Method(
    [
      $Parameter("self", self),
      $Parameter("virtualKey", base.number)]),
  "key_up": $Method(
    [
      $Parameter("self", self),
      $Parameter("virtualKey", base.number)]),
  "key_press": $Method(
    [
      $Parameter("self", self),
      $Parameter("virtualKey", base.number),
      $Parameter("delayMs", base.number, true)]),
  "mouse_move": $Method(
    [
      $Parameter("self", self),
      $Parameter("dx", base.number),
      $Parameter("dy", base.number)]),
  "scroll_mouse_wheel": $Method(
    [
      $Parameter("self", self),
      $Parameter("scrollAmount", base.number),
      $Parameter("horizontal", base.boolean, true)]),
  "mouse_click": $Method(
    [
      $Parameter("self", self),
      $Parameter("flags", base.number)])
}));

const log = $Class("Rotoris.LuaLog", (self) => ({
  "print": $Method(
    [
      $Parameter("self", self),
      $Parameter("text", base.string)]),
  "println": $Method(
    [
      $Parameter("self", self),
      $Parameter("text", base.string)])
}));

const mediaTimeline = $Class("Rotoris.MediaTimeline", (self) => ({
  "StartTime": $Property(base.number),
  "EndTime": $Property(base.number),
  "Position": $Property(base.number)
}));

const mediaImage = $Class("Rotoris.MediaImage", (self) => ({
  "ContentType": $Property(base.string),
  "ImageData": $Property(Bytes)
}));

const mediaProperties = $Class("Rotoris.MediaProperties", (self) => ({
  "Title": $Property(base.string),
  "Subtitle": $Property(base.string),
  "Artist": $Property(base.string),
  "AlbumArtist": $Property(base.string),
  "AlbumTitle": $Property(base.string),
  "TrackNumber": $Property(base.number),
  "AlbumTrackCount": $Property(base.number),
  "Thumbnail": $Property(mediaImage)
}));

const media = $Class("Rotoris.LuaMedia", (self) => ({
  "get_session_ids": $Method(
    [
      $Parameter("self", self),
      $Parameter("emptyTable", EmptyTable)
    ],
    $Parameter("return", base.stringList)),
  "get_current_session_id": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", base.string)),
  "get_media_timeline": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", mediaTimeline)),
  "get_playback_status": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", base.string)),
  "is_playing": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", base.boolean)),
  "get_media_properties_async": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true),
      $Parameter("needThumbnail", base.boolean)
    ],
    $Parameter("return", Async([mediaProperties, base.nil]))
  ),
  "play_async": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", Async([base.boolean, base.nil]))
  ),
  "pause_async": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", Async([base.boolean, base.nil]))
  ),
  "play_or_pause_async": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", Async([base.boolean]))
  ),
  "skip_next_async": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", Async([base.boolean, base.nil]))
  ),
  "skip_previous_async": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string, true)
    ],
    $Parameter("return", Async([base.boolean, base.nil])))
}));

const menuEditor = $Class("Rotoris.LuaMenu.MenuEditor", (self) => ({
  "add_option": $Method(
    [
      $Parameter("self", self),
      $Parameter("id", base.string),
      $Parameter("action_id", base.string, true),
      $Parameter("icon_path", base.string, true)
    ],
    $Parameter("return", self)),
  "to_json": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", base.string))
}));

const temporaryScroll = $Class("Rotoris.LuaMenu.TemporaryScroll", (self) => ({
  "IdleTimeout": $Property(base.integer),
  "ClockwiseModuleName": $Property(base.string),
  "CounterclockwiseModuleName": $Property(base.string),
  "ClickModuleName": $Property(base.string),
  "TimeoutModuleName": $Property(base.string)
}));

const menu = $Class("Rotoris.LuaMenu", (self) => ({
  "open_menu": $Method(
    [
      $Parameter("self", self),
      $Parameter("name", base.string)]),
  "set_options": $Method(
    [
      $Parameter("self", self),
      $Parameter("optionsJson", base.string),
      $Parameter("idleTimeout", base.integer, true)]),
  "close_menu": $Method(
    [
      $Parameter("self", self)]),
  "set_size": $Method(
    [
      $Parameter("self", self),
      $Parameter("size", base.integer)]),
  "set_temporary_scroll": $Method(
    [
      $Parameter("self", self),
      $Parameter("clockwiseModuleName", base.string),
      $Parameter("counterclockwiseModuleName", base.string)
    ],
    $Parameter("return", base.boolean)),
  "set_temporary_scroll.": $Method(
    [
      $Parameter("self", self),
      $Parameter("table", temporaryScroll)
    ],
    $Parameter("return", base.boolean)),
  "print_message": $Method(
    [
      $Parameter("self", self),
      $Parameter("message", base.string),
      $Parameter("durationSeconds", base.integer, true)]),
  "create_menu": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", menuEditor))
}));

const processContext = $Class("Rotoris.LuaSystem.Process.Context",
  (self) => ({
    "get_pid": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("return", base.integer)
    ),
    "has_exited": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("return", base.boolean)
    ),
    "get_exit_code": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("return", base.integer)
    ),
    "is_running": $Method(
      [
        $Parameter("self", self)
      ],
      $Parameter("return", base.boolean)
    ),
    "write": $Method(
      [
        $Parameter("self", self),
        $Parameter("input", base.string)
      ]
    ),
    "write_line": $Method(
      [
        $Parameter("self", self),
        $Parameter("input", base.string)
      ]
    ),
    "standard_input_close": $Method(
      [
        $Parameter("self", self)
      ]
    ),
    "done": $Method(
      [
        $Parameter("self", self)
      ]
    ),
  })
);

const processOptions = $Class("Rotoris.LuaSystem.Process.Options",
  (self) => ({
    "TimeoutMs": $Property(base.number),
    "OnStrart": $Method(
      [
        $Parameter("ctx", processContext)
      ]
    ),
    "OnOutput": $Method(
      [
        $Parameter("ctx", processContext)
      ]
    ),
    "OnError": $Method(
      [
        $Parameter("ctx", processContext)
      ]
    ),
    "OnTimeout": $Method([]),
  })
);

const process = $Class("Rotoris.LuaSystem.Process",
  (self) => ({
    "start": $Method(
      [
        $Parameter("self", self),
        $Parameter("options", processOptions, true)
      ],
    ),
    "cancel": $Method(
      [
        $Parameter("self", self)
      ],
    ),
  })
);

const sys = $Class("Rotoris.LuaSystem", (self) => ({
  "exec": $Method(
    [
      $Parameter("self", self),
      $Parameter("command", base.string),
      $Parameter("...args", base.stringList)
    ],
    $Parameter("return", process)),
  "open": $Method(
    [
      $Parameter("self", self),
      $Parameter("targetPath", base.string)])
}));

const windowSize = $Class("Rotoris.LuaWindows.WindowSize", (self) => ({
  "Top": $Property(base.number),
  "Left": $Property(base.number),
  "Right": $Property(base.number),
  "Bottom": $Property(base.number),
  "Width": $Property(base.number),
  "Height": $Property(base.number)
}));

const windows = $Class("Rotoris.LuaWindows", (self) => ({
  "focus_window": $Method(
    [
      $Parameter("self", self),
      $Parameter("windowTitle", base.string)
    ],
    $Parameter("return", Result([base.boolean]))
  ),
  "get_active_window_title": $Method(
    [
      $Parameter("self", self)
    ],
    $Parameter("return", Result([base.string, base.nil]))
  ),
  "maximize_window": $Method(
    [
      $Parameter("self", self),
      $Parameter("windowTitle", base.string)
    ],
    $Parameter("return", Result([base.boolean]))
  ),
  "minimize_window": $Method(
    [
      $Parameter("self", self),
      $Parameter("windowTitle", base.string)
    ],
    $Parameter("return", Result([base.boolean]))
  ),
  "get_window_size": $Method(
    [
      $Parameter("self", self),
      $Parameter("windowTitle", base.string),
      $Parameter("emptyTable", EmptyTable)
    ],
    $Parameter("return", Result([windowSize]))
  ),
  "get_windows": $Method(
    [
      $Parameter("self", self),
      $Parameter("emptyTable", EmptyTable)
    ],
    $Parameter("return", Result([base.stringList]))
  ),
  "find_window_regex": $Method(
    [
      $Parameter("self", self),
      $Parameter("pattern", base.string)
    ],
    $Parameter("return", Result([base.string, base.nil]))
  )
}));

export var apiModules: ApiModule[] = [];
apiModules = [
  {
    "name": "env",
    "type": env
  },
  {
    "name": "audio",
    "type": audio,
    "subcategory": [
      {
        "name": "ProcessAudioSession",
        "type": processAudioSession
      },
      {
        "name": "MicrophoneSession",
        "type": microphoneSession
      }
    ]
  },
  {
    "name": "cache",
    "type": cache
  },
  {
    "name": "canvas",
    "type": canvas,
    "subcategory": [
      {
        "name": "DrawBlueprint",
        "type": drawBlueprint
      },
      {
        "name": "CanvasContext",
        "type": canvasContext
      },
      {
        "name": "CanvasPath",
        "type": canvasPath
      },
      {
        "name": "FontCache",
        "type": fontCache
      },
      {
        "name": "ImageCache",
        "type": imageCache
      },
      {
        "name": "PaintCache",
        "type": paintCache
      },
      {
        "name": "PaintCache.Editor",
        "type": paintCacheEditor
      }
    ]
  },
  {
    "name": "fs",
    "type": fs
  },
  {
    "name": "hid",
    "type": hid
  },
  {
    "name": "log",
    "type": log
  },
  {
    "name": "media",
    "type": media,
    "subcategory": [
      {
        "name": "MediaTimeline",
        "type": mediaTimeline
      },
      {
        "name": "MediaProperties",
        "type": mediaProperties
      },
      {
        "name": "MediaImage",
        "type": mediaImage
      }
    ]
  },
  {
    "name": "menu",
    "type": menu,
    "subcategory": [
      {
        "name": "MenuEditor",
        "type": menuEditor
      },
      {
        "name": "TemporaryScroll",
        "type": temporaryScroll
      }
    ]
  },
  {
    "name": "sys",
    "type": sys,
    "subcategory": [
      {
        "name": "Process",
        "type": process
      },
      {
        "name": "ProcessOptions",
        "type": processOptions
      },
      {
        "name": "ProcessContext",
        "type": processContext
      }
    ]
  },
  {
    "name": "windows",
    "type": windows,
    "subcategory": [
      {
        "name": "WindowSize",
        "type": windowSize
      }
    ]
  }
]
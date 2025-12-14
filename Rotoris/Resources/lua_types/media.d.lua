--- Provides access to media session information and control functionalities.
--- @class Rotoris.LuaMedia
--- @field get_session_ids fun(self: Rotoris.LuaMedia, emptyTable: table): string[] Gets a list of media session IDs.
--- @field get_current_session_id fun(self: Rotoris.LuaMedia): string Gets the current active media session ID.
--- @field get_media_timeline fun(self: Rotoris.LuaMedia, id: string?): Rotoris.LuaMedia.MediaTimeline? Gets the media timeline for the specified session ID or the current session if no ID is provided.
--- @field get_playback_status fun(self: Rotoris.LuaMedia, id: string?): string? Gets the playback status for the specified session ID or the current session if no ID is provided.
--- @field is_playing fun(self: Rotoris.LuaMedia, id: string?): boolean? Checks if the media is playing for the specified session ID or the current session if no ID is provided.
--- @field get_media_properties_async fun(self: Rotoris.LuaMedia, id: string?, needThumbnail: boolean?): Rotoris.Async<Rotoris.LuaMedia.MediaProperties?> Asynchronously gets the media properties for the specified session ID or the current session if no ID is provided.
--- @field play_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously plays the media for the specified session ID or the current session if no ID is provided.
--- @field pause_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously pauses the media for the specified session ID or the current session if no ID is provided.
--- @field play_or_pause_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean> Asynchronously toggles play/pause for the specified session ID or the current session if no ID is provided.
--- @field skip_next_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously skips to the next track for the specified session ID or the current session if no ID is provided.
--- @field skip_previous_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously skips to the previous track for the specified session ID or the current session if no ID is provided.

--- @class Rotoris.LuaMedia.MediaTimeline
--- @field StartTime number The start time of the media in milliseconds.
--- @field EndTime number The end time of the media in milliseconds.    
--- @field Position number The current position of the media in milliseconds.

--- @class Rotoris.LuaMedia.MediaProperties
--- @field Title string The title of the media.
--- @field Subtitle string The subtitle of the media.
--- @field Artist string The artist of the media.
--- @field AlbumArtist string The album artist of the media.
--- @field AlbumTitle string The album title of the media.
--- @field TrackNumber number The track number of the media.
--- @field AlbumTrackCount number The total number of tracks in the album.
--- @field Thumbnail Rotoris.LuaMedia.MediaImage? The thumbnail image of the media, if available.

--- @class Rotoris.LuaMedia.MediaImage
--- @field ContentType string The content type of the image (e.g., "image/png").
--- @field ImageData Bytes The binary data of the image as a byte array.


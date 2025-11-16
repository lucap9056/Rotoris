using NLua;
using Rotoris.LuaModules.LuaUtils;
using Rotoris.Logger;
using System.IO;
using Windows.Foundation;
using Windows.Media.Control;

namespace Rotoris.LuaModules.LuaMedia
{
    /*
--- Provides access to media session information and control functionalities.
--- @class Rotoris.LuaMedia
--- @field get_session_ids fun(self: Rotoris.LuaMedia, luaTable: LuaTable): LuaTable Gets a list of media session IDs.
--- @field get_current_session_id fun(self: Rotoris.LuaMedia): string Gets the current active media session ID.
--- @field get_media_timeline fun(self: Rotoris.LuaMedia, id: string?): Rotoris.MediaTimeline? Gets the media timeline for the specified session ID or the current session if no ID is provided.
--- @field get_playback_status fun(self: Rotoris.LuaMedia, id: string?): string? Gets the playback status for the specified session ID or the current session if no ID is provided.
--- @field is_playing fun(self: Rotoris.LuaMedia, id: string?): boolean? Checks if the media is playing for the specified session ID or the current session if no ID is provided.
--- @field get_media_properties_async fun(self: Rotoris.LuaMedia, id: string?, needThumbnail: boolean): Rotoris.Async<Rotoris.MediaProperties?> Asynchronously gets the media properties for the specified session ID or the current session if no ID is provided.
--- @field play_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously plays the media for the specified session ID or the current session if no ID is provided.
--- @field pause_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously pauses the media for the specified session ID or the current session if no ID is provided.
--- @field play_or_pause_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean> Asynchronously toggles play/pause for the specified session ID or the current session if no ID is provided.
--- @field skip_next_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously skips to the next track for the specified session ID or the current session if no ID is provided.
--- @field skip_previous_async fun(self: Rotoris.LuaMedia, id: string?): Rotoris.Async<boolean?> Asynchronously skips to the previous track for the specified session ID or the current session if no ID is provided.

     */
    public class LuaMedia(MediaSessionsManager sessionsManager)
    {
        /*
--- @class Rotoris.MediaTimeline
--- @field StartTime number The start time of the media in milliseconds.
--- @field EndTime number The end time of the media in milliseconds.    
--- @field Position number The current position of the media in milliseconds.
         */
        public struct MediaTimeline
        {
            public double StartTime { set; get; }
            public double EndTime { set; get; }
            public double Position { set; get; }
        }

        /*
--- @class Rotoris.MediaProperties
--- @field Title string The title of the media.
--- @field Subtitle string The subtitle of the media.
--- @field Artist string The artist of the media.
--- @field AlbumArtist string The album artist of the media.
--- @field AlbumTitle string The album title of the media.
--- @field TrackNumber number The track number of the media.
--- @field AlbumTrackCount number The total number of tracks in the album.
--- @field Thumbnail Rotoris.MediaImage? The thumbnail image of the media, if available.
         */
        public struct MediaProperties
        {
            public string Title { set; get; }
            public string Subtitle { set; get; }
            public string Artist { set; get; }
            public string AlbumArtist { set; get; }
            public string AlbumTitle { set; get; }
            public int TrackNumber { set; get; }
            public int AlbumTrackCount { set; get; }
            public MediaImage? Thumbnail { set; get; }
        }

        /*
--- @class Rotoris.MediaImage
--- @field ContentType string The content type of the image (e.g., "image/png").
--- @field ImageData byte[] The binary data of the image as a byte array.
         */
        public struct MediaImage
        {
            public string ContentType { set; get; }
            public byte[] ImageData { set; get; }
        }
        public LuaTable get_session_ids(LuaTable luaTable)
        {
            return sessionsManager.BorrowSessions((data) =>
            {
                if (data.Sessions == null || data.Sessions.Count == 0)
                {
                    return luaTable;
                }

                for (int i = 0; i < data.Sessions.Count; i++)
                {
                    luaTable[i + 1] = data.Sessions[i].SourceAppUserModelId;
                }
                return luaTable;
            });
        }
        public string get_current_session_id()
        {
            return sessionsManager.BorrowSessions(data =>
            {
                if (data.CurrentSession == null)
                {
                    throw new InvalidOperationException("Failed to get current session ID: No active media session found.");
                }

                return data.CurrentSession.SourceAppUserModelId;
            });
        }
        public MediaTimeline? get_media_timeline(string? id = null)
        {
            try
            {
                var session = sessionsManager.FindSession(id);
                if (session == null)
                {
                    return null;
                }

                var properties = session.GetTimelineProperties();
                var timeline = new MediaTimeline
                {
                    StartTime = properties.StartTime.TotalMilliseconds,
                    EndTime = properties.EndTime.TotalMilliseconds,
                    Position = properties.Position.TotalMilliseconds,
                };
                return timeline;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while getting media timeline for session ID '{id}': {ex.Message}", ex);
            }
        }

        public string? get_playback_status(string? id = null)
        {
            try
            {
                var session = sessionsManager.FindSession(id);
                if (session == null)
                {
                    return null;
                }

                var status = session.GetPlaybackInfo()?.PlaybackStatus.ToString();
                if (string.IsNullOrEmpty(status))
                {
                    throw new InvalidOperationException($"Playback information not available for session ID '{id}'.");
                }

                return status;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while getting playback status for session ID '{id}': {ex.Message}", ex);
            }
        }

        public bool? is_playing(string? id = null)
        {
            try
            {
                var session = sessionsManager.FindSession(id);
                if (session == null)
                {
                    return null;
                }

                var playbackInfo = session.GetPlaybackInfo();
                return playbackInfo != null && playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while determining if session ID '{id}' is playing: {ex.Message}", ex);
            }
        }
        private static async Task<MediaImage?> GetMediaThumbnailAsync(GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties)
        {
            if (mediaProperties.Thumbnail == null)
            {
                return null;
            }

            var thumbnail = await mediaProperties.Thumbnail.OpenReadAsync();

            if (!thumbnail.CanRead)
            {
                throw new IOException("The media thumbnail stream does not support reading.");
            }

            using var thumbnailStream = thumbnail.AsStreamForRead();

            if (thumbnailStream.Length <= 0)
            {
                throw new IOException("The media thumbnail stream is empty.");
            }

            try
            {
                if (thumbnailStream.Length > int.MaxValue)
                {
                    throw new IOException($"Thumbnail stream length ({thumbnailStream.Length} bytes) exceeds maximum supported buffer size ({int.MaxValue} bytes).");
                }

                var bufferSize = (int)thumbnailStream.Length;
                byte[] imageBuffer = new byte[bufferSize];

                int actualBytesRead = await thumbnailStream.ReadAsync(imageBuffer);

                if (actualBytesRead == bufferSize)
                {
                    return new MediaImage
                    {
                        ContentType = thumbnail.ContentType,
                        ImageData = imageBuffer
                    };
                }
                else
                {
                    throw new InvalidOperationException($"Mismatch between expected thumbnail size ({bufferSize} bytes) and actual bytes read ({actualBytesRead} bytes). Incomplete read operation.");
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read media thumbnail stream data.", ex);
            }
        }
        private async Task<MediaProperties?> GetMediaPropertiesAsync(string? id = null, bool needThumbnail = false)
        {
            var session = sessionsManager.FindSession(id);
            if (session == null)
            {
                return null;
            }

            var mediaProperties = await session.TryGetMediaPropertiesAsync() ??
                throw new InvalidOperationException($"Failed to retrieve media properties for session ID '{session.SourceAppUserModelId}'.");

            MediaImage? thumbnail = null;
            if (needThumbnail)
            {
                try
                {
                    thumbnail = await GetMediaThumbnailAsync(mediaProperties);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to retrieve media thumbnail for session ID '{session.SourceAppUserModelId}'. Error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Log.Error($"Details: {ex.InnerException.Message}");
                    }
                }
            }

            var properties = new MediaProperties
            {
                Title = mediaProperties.Title,
                Subtitle = mediaProperties.Subtitle,
                Artist = mediaProperties.Artist,
                AlbumArtist = mediaProperties.AlbumArtist,
                AlbumTitle = mediaProperties.AlbumTitle,
                TrackNumber = mediaProperties.TrackNumber,
                AlbumTrackCount = mediaProperties.AlbumTrackCount,
                Thumbnail = thumbnail,
            };

            return properties;
        }

        public Async<MediaProperties?> get_media_properties_async(string? id = null, bool needThumbnail = false)
        {
            var task = GetMediaPropertiesAsync(id, needThumbnail);
            return new Async<MediaProperties?>(task);
        }

        public Async<bool?> play_async(string? id = null)
        {
            var task = PerformSessionAction(session => session.TryPlayAsync(), id, "play");
            return new Async<bool?>(task);
        }

        public Async<bool?> pause_async(string? id = null)
        {
            var task = PerformSessionAction(session => session.TryPauseAsync(), id, "pause");
            return new Async<bool?>(task);
        }

        private async Task<bool> PlayOrPauseAsync(string? id = null)
        {
            try
            {
                var session = sessionsManager.FindSession(id);
                if (session == null)
                {
                    return false;
                }

                var status = session.GetPlaybackInfo()?.PlaybackStatus;
                if (status == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                {
                    bool result = await session.TryPauseAsync();
                    if (!result)
                    {
                        throw new InvalidOperationException("Pause operation failed.");
                    }
                    return true;
                }
                else if (status == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused)
                {
                    bool result = await session.TryPlayAsync();
                    if (!result)
                    {
                        throw new InvalidOperationException("Play operation failed.");
                    }
                    return true;
                }
                else
                {
                    throw new InvalidOperationException($"Cannot perform play/pause operation. Current playback status is: {status}.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while toggling play/pause for session ID '{id}': {ex.Message}", ex);
            }
        }
        public Async<bool> play_or_pause_async(string? id = null)
        {
            var task = PlayOrPauseAsync(id);
            return new Async<bool>(task);
        }

        public Async<bool?> skip_next_async(string? id = null)
        {
            var task = PerformSessionAction(session => session.TrySkipNextAsync(), id, "skip to next track");
            return new Async<bool?>(task);
        }

        public Async<bool?> skip_previous_async(string? id = null)
        {
            var task = PerformSessionAction(session => session.TrySkipPreviousAsync(), id, "skip to previous track");
            return new Async<bool?>(task);
        }

        private async Task<bool?> PerformSessionAction(Func<GlobalSystemMediaTransportControlsSession, IAsyncOperation<bool>> action, string? id = null, string operationName = "")
        {
            var session = sessionsManager.FindSession(id);
            if (session == null)
            {
                return null;
            }

            return await action(session);
        }
    }

}

using Rotoris.Logger;
using Windows.Media.Control;

namespace Rotoris.LuaModules.LuaMedia
{
    public class MediaSessionsManager
    {
        public readonly struct SessionData(List<GlobalSystemMediaTransportControlsSession>? sessions, GlobalSystemMediaTransportControlsSession? currentSession, DateTimeOffset lastUpdated)
        {
            public List<GlobalSystemMediaTransportControlsSession>? Sessions { get; } = sessions;
            public GlobalSystemMediaTransportControlsSession? CurrentSession { get; } = currentSession;
            public DateTimeOffset LastUpdated { get; } = lastUpdated;
        }
        private GlobalSystemMediaTransportControlsSessionManager? sessionManager;
        private List<GlobalSystemMediaTransportControlsSession>? Sessions;
        private GlobalSystemMediaTransportControlsSession? CurrentSession;
        private DateTimeOffset LastUpdated = DateTimeOffset.MinValue;
        private readonly Lock SessionsLock = new();
        private bool IsInitialized = false;
        public async Task Initialize()
        {
            Log.Info("Initializing LuaMedia...");
            try
            {
                sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                if (sessionManager == null)
                {
                    Log.Error("Failed to get session manager. Initialization aborted.");
                    return;
                }

                sessionManager.SessionsChanged += OnSessionsChanged;
                sessionManager.CurrentSessionChanged += OnCurrentSessionChanged;

                lock (SessionsLock)
                {
                    Sessions = [.. sessionManager.GetSessions()];
                    CurrentSession = sessionManager.GetCurrentSession();
                    LastUpdated = DateTimeOffset.Now;
                }

                IsInitialized = true;
                Log.Info("LuaMedia initialization completed.");
                if (CurrentSession != null)
                {
                    Log.Info($"[Initial current session: App ID - {CurrentSession.SourceAppUserModelId}");
                }
                else
                {
                    Log.Warning("No active media session found during initialization.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred during initialization: {ex.Message}");
            }
        }

        private void OnSessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            lock (SessionsLock)
            {
                Sessions = [.. sender.GetSessions()];
                LastUpdated = DateTimeOffset.Now;
                Log.Info("Media session list has been updated.");
            }
        }

        private void OnCurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        {
            lock (SessionsLock)
            {
                CurrentSession = sender.GetCurrentSession();
                LastUpdated = DateTimeOffset.Now;
                if (CurrentSession != null)
                {
                    Log.Info($"Current session updated: App ID - {CurrentSession.SourceAppUserModelId}");
                }
                else
                {
                    Log.Warning("No active media session found.");
                }
            }
        }
        /*
         * 
         * 
         * 
         */
        public GlobalSystemMediaTransportControlsSession? FindSession(string? sessionId)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("LuaMedia is not initialized yet. Call Initialize() first.");
            }

            lock (SessionsLock)
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    if (CurrentSession == null)
                    {
                        return null;
                    }
                    return CurrentSession;
                }

                var session = Sessions?.FirstOrDefault(s => s.SourceAppUserModelId == sessionId);
                return session;
            }
        }

        public T BorrowSessions<T>(Func<SessionData, T> callback)
        {
            lock (SessionsLock)
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("MediaSessionsManager is not initialized yet. Call Initialize() first.");
                }

                var data = new SessionData(Sessions, CurrentSession, LastUpdated);
                T result = callback(data);

                return result;
            }
        }
    }
}

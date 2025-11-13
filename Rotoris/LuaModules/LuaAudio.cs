using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using Rotoris.LuaModules.LuaUtils;
using System.Diagnostics;
using Rotoris.Logger;
using System.Text;
using NLua;

namespace Rotoris.LuaModules
{
    /*
--- Provides functionality to manage and control audio sessions and microphone devices on the system.
--- @class Rotoris.LuaAudio
--- @field get_sessions fun(self: Rotoris.LuaAudio, emptyTable: table): Rotoris.Async<table> Retrieves a list of current audio sessions.
--- @field increase_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<number|nil> Increases the volume of the specified audio session or the default device.
--- @field set_volume fun(self: Rotoris.LuaAudio, id: string, volume: number): Rotoris.Async<boolean> Sets the volume of the specified audio session or the default device.
--- @field decrease_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<number|nil> Decreases the volume of the specified audio session or the default device.
--- @field get_current_volume fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<number> Gets the current volume of the specified audio session or the default device.
--- @field toggle_mute fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Toggles the mute state of the specified audio session or the default device.
--- @field is_muted fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Checks if the specified audio session or the default device is muted.
--- @field get_mic_sessions fun(self: Rotoris.LuaAudio, emptyTable: table): Rotoris.Async<table> Retrieves a list of current microphone devices.
--- @field increase_mic_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<boolean> Increases the volume of the specified microphone device or the default microphone.
--- @field set_mic_volume fun(self: Rotoris.LuaAudio, id: string, volume: number): Rotoris.Async<boolean> Sets the volume of the specified microphone device or the default microphone.
--- @field decrease_mic_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<boolean> Decreases the volume of the specified microphone device or the default microphone.
--- @field get_mic_volume fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<number> Gets the current volume of the specified microphone device or the default microphone.
--- @field toggle_mic_mute fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Toggles the mute state of the specified microphone device or the default microphone.
--- @field is_mic_muted fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Checks if the specified microphone device or the default microphone is muted.
     */
    public class LuaAudio : IMMNotificationClient
    {
        /*
--- @class Rotoris.LuaAudio.ProcessAudioSession
--- @field SessionId string Unique identifier for the audio session.
--- @field ProcessName string Name of the process associated with the audio session.
         */
        public struct ProcessAudioSession
        {
            public string SessionId { get; set; }
            public string ProcessName { get; set; }
            public ProcessAudioSession(AudioSessionControl session)
            {
                ProcessName = Process.GetProcessById((int)session.GetProcessID).ProcessName;
                byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(session.GetSessionInstanceIdentifier));
                StringBuilder sb = new();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                SessionId = sb.ToString();
            }
        }

        /*
--- @class Rotoris.LuaAudio.MicrophoneSession
--- @field SessionId string Unique identifier for the microphone device.
--- @field FriendlyName string User-friendly name of the microphone device.
         */
        public struct MicrophoneSession(MMDevice device)
        {
            public string SessionId { get; set; } = device.ID;
            public string FriendlyName { get; set; } = device.DeviceFriendlyName;
        }
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }
        private readonly BlockingCollection<Action> taskQueue = [];
        private readonly Thread thread;
        private readonly Lock deviceLock = new();
        private MMDeviceEnumerator? enumerator;
        private MMDevice? currentDevice;
        private MMDevice? currentMicDevice;
        private List<MMDevice> micDevices = [];
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string pwstrDeviceId)
        {
            if ((flow == DataFlow.Render && role == Role.Multimedia) || (flow == DataFlow.Capture && role == Role.Communications))
            {
                RunOnCorrectThread(Load);
            }
        }
        public void OnDeviceAdded(string pwstrDeviceId)
        {
            RunOnCorrectThread(Load);
        }
        public void OnDeviceRemoved(string pwstrDeviceId)
        {
            RunOnCorrectThread(Load);
        }
        public void OnDeviceStateChanged(string pwstrDeviceId, DeviceState dwNewState)
        {
            if (dwNewState == DeviceState.Active || dwNewState == DeviceState.Disabled)
            {
                RunOnCorrectThread(Load);
            }
        }

        public LuaAudio()
        {
            thread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = "LuaAudioWorker"
            };
            thread.Start();
        }
        private void WorkerLoop()
        {
            try
            {
                enumerator = new MMDeviceEnumerator();
                enumerator.RegisterEndpointNotificationCallback(this);
                LoadMicphones();
                Load();

                foreach (var task in taskQueue.GetConsumingEnumerable())
                {
                    task();
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                enumerator?.UnregisterEndpointNotificationCallback(this);
                enumerator?.Dispose();
            }
        }
        private Async<T> RunOnCorrectThread<T>(Func<T> action)
        {
            var tcs = new TaskCompletionSource<T>();
            taskQueue.Add(() =>
            {
                try
                {
                    var result = action();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return new Async<T>(tcs.Task);
        }
        private bool Load()
        {
            if (enumerator == null) return false;
            try
            {
                lock (deviceLock)
                {
                    currentDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    currentMicDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get new default device information: {ex.Message}");
            }
            return false;
        }
        private void LoadMicphones()
        {
            if (enumerator == null) return;
            lock (deviceLock)
            {
                micDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
            }
        }
        public void Dispose()
        {
            taskQueue.CompleteAdding();
            thread.Join();
        }
        private AudioSessionControl? FindSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) || currentDevice == null)
            {
                return null;
            }
            lock (deviceLock)
            {
                var sessions = currentDevice.AudioSessionManager.Sessions;

                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    try
                    {
                        ProcessAudioSession sessionData = new(sessions[i]);
                        if (sessionData.SessionId == sessionId)
                        {
                            return session;
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Log.Error($"Invalid regex pattern: {ex.Message}");
                    }
                }
            }
            return null;
        }
        public Async<LuaTable> get_sessions(LuaTable luaTable)
        {
            return RunOnCorrectThread(() =>
            {
                if (currentDevice == null)
                {
                    return luaTable;
                }

                lock (deviceLock)
                {
                    var sessions = currentDevice.AudioSessionManager.Sessions;

                    for (int i = 0; i < sessions.Count; i++)
                    {
                        luaTable[i + 1] = new ProcessAudioSession(sessions[i]);
                    }

                }
                return luaTable;
            });
        }
        public Async<double?> increase_volume(string? id = null, double volumeChange = 0.01)
        {
            return RunOnCorrectThread<double?>(() =>
            {
                if (currentDevice == null)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(id))
                {
                    if (currentDevice.AudioEndpointVolume.Mute)
                    {
                        currentDevice.AudioEndpointVolume.Mute = false;
                    }
                    double currentVolume = currentDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                    double newVolume = currentVolume + volumeChange;

                    double roundedVolume = Math.Round(newVolume, 2);
                    double resultVolume = Math.Clamp(roundedVolume, 0.0, 1.0);
                    currentDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)resultVolume;
                    return resultVolume;
                }
                else
                {
                    var session = FindSession(id);
                    if (session != null)
                    {
                        if (session.SimpleAudioVolume.Mute)
                        {
                            session.SimpleAudioVolume.Mute = false;
                        }
                        double currentVolume = session.SimpleAudioVolume.Volume;
                        double newVolume = currentVolume + volumeChange;

                        double roundedVolume = Math.Round(newVolume, 2);
                        double resultVolume = Math.Clamp(roundedVolume, 0.0, 1.0);
                        session.SimpleAudioVolume.Volume = (float)resultVolume;
                        return resultVolume;
                    }
                }
                return null;
            });
        }
        public Async<bool> set_volume(string id, double volume)
        {
            return RunOnCorrectThread(() =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    if (currentDevice == null)
                    {
                        return false;
                    }

                    currentDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)Math.Clamp(volume, 0.0, 1.0);
                    return true;
                }
                else
                {
                    var session = FindSession(id);
                    if (session != null)
                    {
                        session.SimpleAudioVolume.Volume = (float)Math.Clamp(volume, 0.0, 1.0);
                        return true;
                    }
                }

                return false;
            });

        }
        public Async<double?> decrease_volume(string? id = null, double volumeChange = 0.01)
        {
            return RunOnCorrectThread<double?>(() =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    if (currentDevice == null)
                    {
                        return null;
                    }
                    double currentVolume = currentDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                    double newVolume = currentVolume - volumeChange;

                    double roundedVolume = Math.Round(newVolume, 2);
                    double resultVolume = Math.Clamp(roundedVolume, 0.0, 1.0);
                    currentDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)resultVolume;
                    return resultVolume;
                }
                else
                {
                    var session = FindSession(id);
                    if (session != null)
                    {
                        double currentVolume = session.SimpleAudioVolume.Volume;
                        double newVolume = currentVolume - volumeChange;

                        double roundedVolume = Math.Round(newVolume, 2);
                        double resultVolume = Math.Clamp(roundedVolume, 0.0, 1.0);
                        session.SimpleAudioVolume.Volume = (float)resultVolume;
                        return resultVolume;
                    }
                }
                return null;
            });
        }

        public Async<float> get_current_volume(string? id = null)
        {
            return RunOnCorrectThread(() =>
            {
                float result = 0.0f;
                if (string.IsNullOrEmpty(id))
                {
                    if (currentDevice == null)
                    {
                        return result;
                    }
                    result = currentDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                }
                else
                {
                    var session = FindSession(id);
                    if (session != null)
                    {
                        result = session.SimpleAudioVolume.Volume;
                    }
                }

                return result;
            });
        }

        public Async<bool> toggle_mute(string? id = null)
        {
            return RunOnCorrectThread(() =>
            {
                if (currentDevice == null) return false;
                if (string.IsNullOrEmpty(id))
                {
                    currentDevice.AudioEndpointVolume.Mute = !currentDevice.AudioEndpointVolume.Mute;
                }
                else
                {
                    var session = FindSession(id);
                    if (session != null)
                    {
                        session.SimpleAudioVolume.Mute = !session.SimpleAudioVolume.Mute;
                    }
                }
                return true;
            });
        }

        public Async<bool> is_muted(string? id = null)
        {
            return RunOnCorrectThread(() =>
            {
                bool result = false;
                if (string.IsNullOrEmpty(id))
                {
                    if (currentDevice == null)
                    {
                        return result;
                    }
                    result = currentDevice.AudioEndpointVolume.Mute;
                }
                else
                {
                    var session = FindSession(id);
                    if (session != null)
                    {
                        result = session.SimpleAudioVolume.Mute;
                    }
                }

                return result;
            });
        }

        private MMDevice? FindMicrophone(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }
            lock (deviceLock)
            {
                foreach (var device in micDevices)
                {
                    if (device.ID == sessionId)
                    {
                        return device;
                    }
                }
            }
            return null;
        }
        public Async<LuaTable> get_mic_sessions(LuaTable luaTable)
        {
            return RunOnCorrectThread(() =>
            {
                if (currentMicDevice == null)
                {
                    return luaTable;
                }

                lock (deviceLock)
                {
                    for (int i = 0; i < micDevices.Count; i++)
                    {
                        luaTable[i + 1] = new MicrophoneSession(micDevices[i]);
                    }

                }
                return luaTable;
            });
        }
        public Async<bool> increase_mic_volume(string? id = null, double volumeChange = 0.01)
        {
            return RunOnCorrectThread(() =>
            {
                MMDevice? targetDevice = string.IsNullOrEmpty(id) ? currentMicDevice : FindMicrophone(id);
                if (targetDevice == null) return false;

                double currentVolume = targetDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                double newVolume = currentVolume + volumeChange;
                targetDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)Math.Clamp(Math.Round(newVolume, 2), 0.0, 1.0);
                return true;
            });
        }
        public Async<bool> set_mic_volume(string id, float volume)
        {
            return RunOnCorrectThread(() =>
            {
                MMDevice? targetDevice = string.IsNullOrEmpty(id) ? currentMicDevice : FindMicrophone(id);
                if (targetDevice == null) return false;

                targetDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)Math.Clamp(Math.Round(volume, 2), 0.0, 1.0);
                return true;
            });
        }
        public Async<bool> decrease_mic_volume(string? id = null, double volumeChange = 0.01)
        {
            return RunOnCorrectThread(() =>
            {
                MMDevice? targetDevice = string.IsNullOrEmpty(id) ? currentMicDevice : FindMicrophone(id);
                if (targetDevice == null) return false;

                double currentVolume = targetDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
                double newVolume = currentVolume - volumeChange;
                targetDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)Math.Clamp(Math.Round(newVolume, 2), 0.0, 1.0);
                return true;
            });
        }
        public Async<float> get_mic_volume(string? id = null)
        {
            return RunOnCorrectThread(() =>
            {
                MMDevice? targetDevice = string.IsNullOrEmpty(id) ? currentMicDevice : FindMicrophone(id);
                if (targetDevice == null) return 0.0f;
                return targetDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
            });
        }

        public Async<bool> toggle_mic_mute(string? id = null)
        {
            return RunOnCorrectThread(() =>
            {
                MMDevice? targetDevice = string.IsNullOrEmpty(id) ? currentMicDevice : FindMicrophone(id);
                if (targetDevice == null) return false;
                targetDevice.AudioEndpointVolume.Mute = !targetDevice.AudioEndpointVolume.Mute;
                return true;
            });
        }

        public Async<bool> is_mic_muted(string? id = null)
        {
            return RunOnCorrectThread(() =>
            {
                MMDevice? targetDevice = string.IsNullOrEmpty(id) ? currentMicDevice : FindMicrophone(id);
                if (targetDevice == null) return false;
                return targetDevice.AudioEndpointVolume.Mute;
            });
        }
    }


}

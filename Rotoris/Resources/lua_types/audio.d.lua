--- Provides functionality to manage and control audio sessions and microphone devices on the system.
--- @class Rotoris.LuaAudio
--- @field get_sessions fun(self: Rotoris.LuaAudio, emptyTable: table): Rotoris.Async<Rotoris.LuaAudio.ProcessAudioSession[]> Retrieves a list of current audio sessions.
--- @field increase_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<number|nil> Increases the volume of the specified audio session or the default device.
--- @field set_volume fun(self: Rotoris.LuaAudio, id: string, volume: number): Rotoris.Async<boolean> Sets the volume of the specified audio session or the default device.
--- @field decrease_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<number|nil> Decreases the volume of the specified audio session or the default device.
--- @field get_current_volume fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<number> Gets the current volume of the specified audio session or the default device.
--- @field toggle_mute fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Toggles the mute state of the specified audio session or the default device.
--- @field is_muted fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Checks if the specified audio session or the default device is muted.
--- @field get_mic_sessions fun(self: Rotoris.LuaAudio, emptyTable: table): Rotoris.Async<Rotoris.LuaAudio.MicrophoneSession[]> Retrieves a list of current microphone devices.
--- @field increase_mic_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<boolean> Increases the volume of the specified microphone device or the default microphone.
--- @field set_mic_volume fun(self: Rotoris.LuaAudio, id: string, volume: number): Rotoris.Async<boolean> Sets the volume of the specified microphone device or the default microphone.
--- @field decrease_mic_volume fun(self: Rotoris.LuaAudio, id: string|nil, volumeChange: number): Rotoris.Async<boolean> Decreases the volume of the specified microphone device or the default microphone.
--- @field get_mic_volume fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<number> Gets the current volume of the specified microphone device or the default microphone.
--- @field toggle_mic_mute fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Toggles the mute state of the specified microphone device or the default microphone.
--- @field is_mic_muted fun(self: Rotoris.LuaAudio, id: string|nil): Rotoris.Async<boolean> Checks if the specified microphone device or the default microphone is muted.

--- @class Rotoris.LuaAudio.ProcessAudioSession
--- @field SessionId string Unique identifier for the audio session.
--- @field ProcessName string Name of the process associated with the audio session.

--- @class Rotoris.LuaAudio.MicrophoneSession
--- @field SessionId string Unique identifier for the microphone device.
--- @field FriendlyName string User-friendly name of the microphone device.


--- @class Rotoris.LuaAudio
local audio = {}

audio:get_sessions({}):match({
    --- @param sessions Rotoris.LuaAudio.ProcessAudioSession[]
    Ok = function(sessions)
        for _, session in ipairs(sessions) do
            print("Session ID: " .. session.SessionId .. ", Process Name: " .. session.ProcessName)
        end
    end,
    Err = function(errorMessage)
        print("Error retrieving audio sessions: " .. errorMessage)
    end
})

audio:increase_volume(nil, 10):match({
    Ok = function(newVolume)
        if newVolume then
            print("New volume: " .. newVolume)
        else
            print("Failed to increase volume.")
        end
    end,
    Err = function(errorMessage)
        print("Error increasing volume: " .. errorMessage)
    end
})

audio:set_volume("session-id-123", 50):match({
    Ok = function(success)
        if success then
            print("Volume set successfully.")
        else
            print("Failed to set volume.")
        end
    end,
    Err = function(errorMessage)
        print("Error setting volume: " .. errorMessage)
    end
})

audio:decrease_volume(nil, 5):match({
    Ok = function(newVolume)
        if newVolume then
            print("New volume: " .. newVolume)
        else
            print("Failed to decrease volume.")
        end
    end,
    Err = function(errorMessage)
        print("Error decreasing volume: " .. errorMessage)
    end
})

audio:get_current_volume(nil):match({
    Ok = function(currentVolume)
        print("Current volume: " .. currentVolume)
    end,
    Err = function(errorMessage)
        print("Error getting current volume: " .. errorMessage)
    end
})

audio:toggle_mute(nil):match({
    Ok = function(newMuteState)
        print("New mute state: " .. tostring(newMuteState))
    end,
    Err = function(errorMessage)
        print("Error toggling mute: " .. errorMessage)
    end
})

audio:is_muted(nil):match({
    Ok = function(isMuted)
        print("Is microphone muted: " .. tostring(isMuted))
    end,
    Err = function(errorMessage)
        print("Error checking microphone mute state: " .. errorMessage)
    end
})

audio:get_mic_sessions({}):match({
    --- @param micSessions Rotoris.LuaAudio.MicrophoneSession[]
    Ok = function(micSessions)
        for _, mic in ipairs(micSessions) do
            print("Mic Session ID: " .. mic.SessionId .. ", Friendly Name: " .. mic.FriendlyName)
        end
    end,
    Err = function(errorMessage)
        print("Error retrieving microphone sessions: " .. errorMessage)
    end
})

audio:increase_mic_volume(nil, 10):match({
    Ok = function(success)
        if success then
            print("Microphone volume increased successfully.")
        else
            print("Failed to increase microphone volume.")
        end
    end,
    Err = function(errorMessage)
        print("Error increasing microphone volume: " .. errorMessage)
    end
})

audio:set_mic_volume("mic-session-id-123", 50):match({
    Ok = function(success)
        if success then
            print("Microphone volume set successfully.")
        else
            print("Failed to set microphone volume.")
        end
    end,
    Err = function(errorMessage)
        print("Error setting microphone volume: " .. errorMessage)
    end
})

audio:decrease_mic_volume(nil, 5):match({
    Ok = function(success)
        if success then
            print("Microphone volume decreased successfully.")
        else
            print("Failed to decrease microphone volume.")
        end
    end,
    Err = function(errorMessage)
        print("Error decreasing microphone volume: " .. errorMessage)
    end
})

audio:get_mic_volume(nil):match({
    Ok = function(currentMicVolume)
        print("Current microphone volume: " .. currentMicVolume)
    end,
    Err = function(errorMessage)
        print("Error getting current microphone volume: " .. errorMessage)
    end
})

audio:toggle_mic_mute(nil):match({
    Ok = function(newMicMuteState)
        print("New microphone mute state: " .. tostring(newMicMuteState))
    end,
    Err = function(errorMessage)
        print("Error toggling microphone mute: " .. errorMessage)
    end
})

audio:is_mic_muted(nil):match({
    Ok = function(isMicMuted)
        print("Is microphone muted: " .. tostring(isMicMuted))
    end,
    Err = function(errorMessage)
        print("Error checking microphone mute state: " .. errorMessage)
    end
})

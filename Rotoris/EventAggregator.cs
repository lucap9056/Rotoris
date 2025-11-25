using RotorisLib;

namespace Rotoris
{
    public static class EventAggregator
    {
        public static event EventHandler<EventArgs>? ExitReceived;
        public static void PublishExit()
        {
            ExitReceived?.Invoke(null, EventArgs.Empty);
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<EventArgs>? ReloadReceived;
        public static void PublishReload()
        {
            ReloadReceived?.Invoke(null, EventArgs.Empty);
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<ShowMenuReceiveEventArgs>? ShowMenuReceived;
        public sealed class ShowMenuReceiveEventArgs : EventArgs
        {
            public required string Name;
        }
        public static bool PublishShowMenu(string name = "")
        {
            ShowMenuReceived?.Invoke(null, new ShowMenuReceiveEventArgs { Name = name });
            return true;
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<EventArgs>? HideMenuReceived;
        public static void PublishHideMenu()
        {
            HideMenuReceived?.Invoke(null, EventArgs.Empty);
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<SetMenuReceiveEventArgs>? SetMenuReceived;
        public sealed class SetMenuReceiveEventArgs : EventArgs
        {
            public int IdleTimeoutInSeconds;
            public required MenuOptionData[] Options;
        }
        public static void PublishSetMenu(MenuOptionData[] options, int idleTimeoutInSeconds = 0)
        {
            SetMenuReceived?.Invoke(null, new SetMenuReceiveEventArgs { IdleTimeoutInSeconds = idleTimeoutInSeconds, Options = options });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<SetTemporaryScrollReceiveEventArgs>? SetTemporaryScrollReceived;
        public sealed class SetTemporaryScrollReceiveEventArgs : EventArgs
        {
            public TemporaryScrollConfig TemporaryScroll;
        }
        public static void PublishSetTemporaryScroll(int idleTimeoutInSeconds, string clockwiseModuleName, string counterclockwiseModuleName, string? clickModuleName = null, string? timeoutModuleName = null)
        {
            if (idleTimeoutInSeconds <= 0)
            {
                clickModuleName = AppConstants.BuiltInActionIds.Close;
            }
            SetTemporaryScrollReceived?.Invoke(null, new SetTemporaryScrollReceiveEventArgs
            {
                TemporaryScroll = new TemporaryScrollConfig
                {
                    Enabled = true,
                    IdleTimeoutInSeconds = idleTimeoutInSeconds,
                    ClockwiseModuleName = clockwiseModuleName,
                    CounterclockwiseModuleName = counterclockwiseModuleName,
                    ClickModuleName = clickModuleName ?? AppConstants.BuiltInActionIds.Close,
                    TimeoutModuleName = timeoutModuleName ?? AppConstants.BuiltInActionIds.Close,
                }
            });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<UILoadConfigurationReceiveEventArgs>? UILoadConfigurationReceived;
        public sealed class UILoadConfigurationReceiveEventArgs(Configuration configuration) : EventArgs
        {
            public Configuration Configuration { get; init; } = configuration;
        }
        public static void PublishUILoadConfiguration(Configuration configuration)
        {
            UILoadConfigurationReceived?.Invoke(null, new UILoadConfigurationReceiveEventArgs(configuration));
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<UISetSizeReceiveEventArgs>? UISetSizeReceived;
        public sealed class UISetSizeReceiveEventArgs : EventArgs
        {
            public double Size;
        }
        public static void PublishUISetSize(double size)
        {
            UISetSizeReceived?.Invoke(null, new UISetSizeReceiveEventArgs { Size = Math.Clamp(size, 200, 1200) });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<UIDrawOptionsReceiveEventArgs>? UIDrawOptionsReceived;
        public sealed class UIDrawOptionsReceiveEventArgs : EventArgs
        {
            public required MenuOptionData[] Options;
        }
        public static void PublishUIDrawOptions(MenuOptionData[] options)
        {
            UIDrawOptionsReceived?.Invoke(null, new UIDrawOptionsReceiveEventArgs { Options = options });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<UIFocusOptionReceiveEventArgs>? UIFocusOptionReceived;
        public sealed class UIFocusOptionReceiveEventArgs : EventArgs
        {
            public int TotalCount;
            public int Index;
        }
        public static void PublishUIFocusOption(int totalCount, int index)
        {
            UIFocusOptionReceived?.Invoke(null, new UIFocusOptionReceiveEventArgs { TotalCount = totalCount, Index = index });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<UIDisplayMessageReceiveEventArgs>? UIDisplayMessageReceived;
        public sealed class UIDisplayMessageReceiveEventArgs : EventArgs
        {
            public required string Content;
        }
        public static void PublishUIDisplayMessage(string content = "")
        {
            UIDisplayMessageReceived?.Invoke(null, new UIDisplayMessageReceiveEventArgs { Content = content });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<DisplayMessageReceiveEventArgs>? DisplayMessageReceived;
        public sealed class DisplayMessageReceiveEventArgs : EventArgs
        {
            public required string Content;
            public required int DurationSeconds;
        }
        public static void PublishDisplayMessage(string content, int durationSeconds)
        {
            DisplayMessageReceived?.Invoke(null, new DisplayMessageReceiveEventArgs { Content = content, DurationSeconds = durationSeconds });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<UIDrawMessageCanvasReceiveEventArgs>? UIDrawMessageCanvasReceived;
        public sealed class UIDrawMessageCanvasReceiveEventArgs : EventArgs
        {
            public required int Width;
            public required int Height;
            public required byte[] Data;
        }
        public static void PublishUIDrawMessageCanvas(int width, int height, byte[] data)
        {
            UIDrawMessageCanvasReceived?.Invoke(null, new UIDrawMessageCanvasReceiveEventArgs { Width = width, Height = height, Data = data });
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<EventArgs>? UIClearMessageCanvasReceived;
        public static void PublishUIClearMessageCanvas()
        {
            UIClearMessageCanvasReceived?.Invoke(null, EventArgs.Empty);
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<KeyboardShouldBlockEventArgs>? NextOptionReceived;
        public static bool PublishNextOption(Action shouldBlock)
        {
            NextOptionReceived?.Invoke(null, new KeyboardShouldBlockEventArgs(shouldBlock));
            return true;
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<KeyboardShouldBlockEventArgs>? PreviousOptionReceived;
        public static bool PublishPreviousOption(Action shouldBlock)
        {
            PreviousOptionReceived?.Invoke(null, new KeyboardShouldBlockEventArgs(shouldBlock));
            return true;
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<KeyboardShouldBlockEventArgs>? ExecuteSelectedOptionReceived;
        public static void PublishExecuteSelectedOption(Action shouldBlock)
        {
            ExecuteSelectedOptionReceived?.Invoke(null, new KeyboardShouldBlockEventArgs(shouldBlock));
        }

        /*
         * 
         * 
         * 
         */

        public sealed class KeyboardShouldBlockEventArgs(Action shouldBlock) : EventArgs
        {
            public Action ShouldBlock { get; init; } = shouldBlock;
        }

        public static event EventHandler<KeyboardShouldBlockEventArgs>? ClockwisePressReceived;
        public static void PublishClockwisePress(Action shouldBlock)
        {
            ClockwisePressReceived?.Invoke(null, new KeyboardShouldBlockEventArgs(shouldBlock));
        }

        public static event EventHandler<KeyboardShouldBlockEventArgs>? CounterclockwisePressReceived;
        public static void PublishCounterclockwisePress(Action shouldBlock)
        {
            CounterclockwisePressReceived?.Invoke(null, new KeyboardShouldBlockEventArgs(shouldBlock));
        }

        /*
         * 
         * 
         * 
         */

        public sealed class ExecuteActionEventArgs(string actionId) : EventArgs
        {
            public string ActionId { get; init; } = actionId;
        }

        public static event EventHandler<ExecuteActionEventArgs>? ExecuteActionReceived;
        public static void PublishExecuteAction(string actionId)
        {
            ExecuteActionReceived?.Invoke(null, new ExecuteActionEventArgs(actionId));
        }

        /*
         * 
         * 
         * 
         */

        public static event EventHandler<EventArgs>? ShowLogsReceived;
        public static void PublishShowLogs()
        {
            ShowLogsReceived?.Invoke(null, EventArgs.Empty);
        }

        public static event EventHandler<EventArgs>? HideLogsReceived;
        public static void PublishHideLogs()
        {
            HideLogsReceived?.Invoke(null, EventArgs.Empty);
        }

        public static event EventHandler<WriteLogsEventArgs>? WriteLogsReceived;

        public sealed class WriteLogsEventArgs(string value) : EventArgs
        {
            public string Value { set; get; } = value;
        }
        public static void PublishWriteLogs(char value)
        {
            WriteLogsReceived?.Invoke(null, new WriteLogsEventArgs(value.ToString()));
        }
        public static void PublishWriteLogs(string value)
        {
            WriteLogsReceived?.Invoke(null, new WriteLogsEventArgs(value));
        }
    }
}

using Xunit;
using RotorisLib;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace RotorisLib.Tests
{
    public class AppConstantsTests
    {
        [Fact]
        public void AppName_IsCorrect()
        {
            Assert.Equal("Rotoris", AppConstants.AppName);
        }

        [Fact]
        public void MenuNames_Root_IsCorrect()
        {
            Assert.Equal("ROOT", AppConstants.MenuNames.Root);
        }

        [Fact]
        public void BuiltInActionIds_Close_IsCorrect()
        {
            Assert.Equal("CLOSE", AppConstants.BuiltInActionIds.Close);
        }

        [Fact]
        public void AppDataDirectory_ContainsAppName()
        {
            string expectedPathPart = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppConstants.AppName);
            Assert.Equal(expectedPathPart, AppConstants.AppDataDirectory);
        }

        [Fact]
        public void AppConfigDirectory_DebugBuild_ContainsDebugSubfolder()
        {
#if DEBUG
            string expectedPathPart = Path.Combine(AppConstants.AppDataDirectory, "DEBUG");
            Assert.Equal(expectedPathPart, AppConstants.AppConfigDirectory);
#else
            Assert.Equal(AppConstants.AppDataDirectory, AppConstants.AppConfigDirectory);
#endif
        }

        [Fact]
        public void AppConfigIniPath_IsCorrect()
        {
            string expectedPath = Path.Combine(AppConstants.AppConfigDirectory, "config.ini");
            Assert.Equal(expectedPath, AppConstants.AppConfigIniPath);
        }

        [Fact]
        public void AppModuleDirectory_IsCorrect()
        {
            string expectedPath = Path.Combine(AppConstants.AppConfigDirectory, "modules");
            Assert.Equal(expectedPath, AppConstants.AppModuleDirectory);
        }

        [Fact]
        public void AppIconCacheDirectory_IsCorrect()
        {
            string expectedPath = Path.Combine(AppConstants.AppDataDirectory, "cache_icons");
            Assert.Equal(expectedPath, AppConstants.AppIconCacheDirectory);
        }

        [Fact]
        public void ActionScriptsMap_IsInitializedAndContainsExpectedEntries()
        {
            Assert.NotNull(AppConstants.ActionScriptsMap);
            Assert.False(AppConstants.ActionScriptsMap.Count == 0);

            Assert.Contains(AppConstants.BuiltInActionIds.Close, AppConstants.ActionScriptsMap.Keys);
            Assert.Equal(AppConstants.BuiltInActionScripts.Close, AppConstants.ActionScriptsMap[AppConstants.BuiltInActionIds.Close]);

            Assert.Contains(AppConstants.BuiltInActionIds.Hello, AppConstants.ActionScriptsMap.Keys);
            Assert.Equal(AppConstants.BuiltInActionScripts.Hello, AppConstants.ActionScriptsMap[AppConstants.BuiltInActionIds.Hello]);
        }

        [Fact]
        public void BuiltInIconPaths_IsInitializedAndContainsExpectedEntries()
        {
            Assert.NotNull(AppConstants.BuiltInIconPaths);
            Assert.False(AppConstants.BuiltInIconPaths.Count == 0);

            Assert.Contains(AppConstants.BuiltInActionIds.Close, AppConstants.BuiltInIconPaths.Keys);
            Assert.EndsWith("x-circle.png", AppConstants.BuiltInIconPaths[AppConstants.BuiltInActionIds.Close]);

            Assert.Contains(AppConstants.BuiltInActionIds.ScreenClip, AppConstants.BuiltInIconPaths.Keys);
            Assert.EndsWith("crop.png", AppConstants.BuiltInIconPaths[AppConstants.BuiltInActionIds.ScreenClip]);
        }

        [Fact]
        public void BuiltInOptionsMap_IsInitializedAndContainsExpectedEntries()
        {
            Assert.NotNull(AppConstants.BuiltInOptionsMap);
            Assert.False(AppConstants.BuiltInOptionsMap.Count == 0);

            Assert.Contains(AppConstants.BuiltInActionIds.Close, AppConstants.BuiltInOptionsMap.Keys);
            var closeOption = AppConstants.BuiltInOptionsMap[AppConstants.BuiltInActionIds.Close];
            Assert.Equal(AppConstants.BuiltInActionIds.Close, closeOption.Id);
            Assert.Equal(AppConstants.BuiltInActionIds.Close, closeOption.ActionId);
            Assert.EndsWith("x-circle.png", closeOption.InternalIconResourcePath);

            Assert.Contains(AppConstants.BuiltInActionIds.Calculator, AppConstants.BuiltInOptionsMap.Keys);
            var calculatorOption = AppConstants.BuiltInOptionsMap[AppConstants.BuiltInActionIds.Calculator];
            Assert.Equal(AppConstants.BuiltInActionIds.Calculator, calculatorOption.Id);
            Assert.Equal(AppConstants.BuiltInActionIds.Calculator, calculatorOption.ActionId);
            Assert.EndsWith("calculator.png", calculatorOption.InternalIconResourcePath);
        }

        [Fact]
        public void TextImageRenderer_IsInitialized()
        {
            Assert.NotNull(AppConstants.TextImageRenderer);
        }
    }
}
namespace RotorisLib.Tests
{
    public class MenuOptionDataTests
    {
        [Fact]
        public void MenuOptionData_DefaultInitialization_PropertiesAreDefault()
        {
            MenuOptionData option = new MenuOptionData();
            Assert.Null(option.Hash);
            Assert.Null(option.Id);
            Assert.Null(option.IconPath);
            Assert.Null(option.InternalIconResourcePath);
            Assert.Null(option.ActionId);
        }

        [Fact]
        public void MenuOptionData_PropertyAssignment_WorksCorrectly()
        {
            MenuOptionData option = new MenuOptionData
            {
                Hash = "testHash",
                Id = "testId",
                IconPath = "testIconPath",
                InternalIconResourcePath = "testInternalIconPath",
                ActionId = "testActionId"
            };

            Assert.Equal("testHash", option.Hash);
            Assert.Equal("testId", option.Id);
            Assert.Equal("testIconPath", option.IconPath);
            Assert.Equal("testInternalIconPath", option.InternalIconResourcePath);
            Assert.Equal("testActionId", option.ActionId);
        }

        [Fact]
        public void ParseOptionsFromJson_EmptyJson_ReturnsEmptyArray()
        {
            var options = MenuOptionData.ParseOptionsFromJson("");
            Assert.NotNull(options);
            Assert.Empty(options);
        }

        [Fact]
        public void ParseOptionsFromJson_ValidJson_ReturnsCorrectData()
        {
            string json = @"[
                {
                    ""Id"": ""id1"",
                    ""IconPath"": ""icon1.png"",
                    ""ActionId"": ""action1""
                },
                {
                    ""Id"": ""id2"",
                    ""IconPath"": ""icon2.png"",
                    ""ActionId"": ""action2""
                }
            ]";

            var options = MenuOptionData.ParseOptionsFromJson(json);

            Assert.NotNull(options);
            Assert.Equal(2, options.Length);

            Assert.Equal("id1", options[0].Id);
            Assert.Equal("icon1.png", options[0].IconPath);
            Assert.Equal("action1", options[0].ActionId);

            Assert.Equal("id2", options[1].Id);
            Assert.Equal("icon2.png", options[1].IconPath);
            Assert.Equal("action2", options[1].ActionId);
        }

        [Fact]
        public void ParseOptionsFromJson_InvalidJson_ThrowsJsonSerializationException()
        {
            string invalidJson = @"[{""Id"": ""id1"", ""IconPath"": ""icon1.png"", ""ActionId"": ""action1"",]";
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => MenuOptionData.ParseOptionsFromJson(invalidJson));
        }


        
        [Fact]
        public void ParseOptionsFromJson_ResolvesBuiltInOption()
        {
            string existingBuiltInId = AppConstants.BuiltInActionIds.Close;
            MenuOptionData expectedBuiltInOption = AppConstants.BuiltInOptionsMap[existingBuiltInId];

            string json = $@"[{{""Id"": ""{existingBuiltInId}""}}]";
            var options = MenuOptionData.ParseOptionsFromJson(json);

            Assert.Single(options);
            Assert.Equal(expectedBuiltInOption.Id, options[0].Id);
            Assert.Equal(expectedBuiltInOption.IconPath, options[0].IconPath);
            Assert.Equal(expectedBuiltInOption.InternalIconResourcePath, options[0].InternalIconResourcePath);
            Assert.Equal(expectedBuiltInOption.ActionId, options[0].ActionId);
        }

        [Fact]
        public void ParseOptionsFromJson_ResolvesActionScript()
        {
            string existingActionId = AppConstants.BuiltInActionIds.Close;
            string expectedScript = AppConstants.ActionScriptsMap[existingActionId];

            string json = $@"[{{""Id"": ""normalId"", ""ActionId"": ""{existingActionId}""}}]";
            var options = MenuOptionData.ParseOptionsFromJson(json);

            Assert.Single(options);
            Assert.Equal("normalId", options[0].Id);
            Assert.Equal(expectedScript, options[0].ActionId);
        }

        [Fact]
        public void ResolveIconPathAndCache_BuiltInOption_ReturnsBuiltInIconPath()
        {
            string existingBuiltInId = AppConstants.BuiltInActionIds.Close;
            MenuOptionData builtInOption = AppConstants.BuiltInOptionsMap[existingBuiltInId];

            MenuOptionData option = new MenuOptionData { Id = existingBuiltInId };
            string resolvedPath = MenuOptionData.ResolveIconPathAndCache(option);

            Assert.Equal(builtInOption.IconPath, resolvedPath);
        }

        [Fact]
        public void ResolveIconPathAndCache_BuiltInIconPath_ReturnsBuiltInPath()
        {
            string existingBuiltInIconKey = AppConstants.BuiltInActionIds.Close;
            string expectedPath = AppConstants.BuiltInIconPaths[existingBuiltInIconKey];

            MenuOptionData option = new MenuOptionData { IconPath = existingBuiltInIconKey };
            string resolvedPath = MenuOptionData.ResolveIconPathAndCache(option);

            Assert.Equal(expectedPath, resolvedPath);
        }

        [Fact]
        public void ResolveIconPathAndCache_ExistingFilePath_ReturnsFilePath()
        {
            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName() + ".png");
            System.IO.File.WriteAllText(tempFilePath, "dummy content");

            try
            {
                MenuOptionData option = new MenuOptionData { IconPath = tempFilePath };
                string resolvedPath = MenuOptionData.ResolveIconPathAndCache(option);
                Assert.Equal(tempFilePath, resolvedPath);
            }
            finally
            {
                System.IO.File.Delete(tempFilePath);
            }
        }
        
        [Fact]
        public void ResolveIconPath_CallsResolveIconPathAndCache()
        {
            string existingBuiltInId = AppConstants.BuiltInActionIds.Hello;
            AppConstants.BuiltInOptionsMap.TryGetValue(existingBuiltInId, out MenuOptionData builtInOption);

            MenuOptionData option = new MenuOptionData { Id = existingBuiltInId };
            option.ResolveIconPath();

            Assert.Equal(builtInOption.IconPath, option.InternalIconResourcePath);
        }

        public MenuOptionDataTests()
        {
        }
    }
}
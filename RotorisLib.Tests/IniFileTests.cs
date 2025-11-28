namespace RotorisLib.Tests
{
    public class IniFileTests
    {
        [Fact]
        public void ReadValue_ShouldCorrectlyParseBasicIniFile()
        {
            var iniContent = @"
[Section1]
Key1 = Value1
Key2 = Another Value

[Section2]
KeyA = ValueA
KeyB = 12345
            ";

            string tempFile = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(tempFile, iniContent);

            try
            {
                var iniFile = new IniFile(tempFile);

                Assert.Equal("Value1", iniFile.ReadValue("Section1", "Key1"));
                Assert.Equal("Another Value", iniFile.ReadValue("Section1", "Key2"));
                Assert.Equal("ValueA", iniFile.ReadValue("Section2", "KeyA"));
                Assert.Equal("12345", iniFile.ReadValue("Section2", "KeyB"));

                Assert.Equal("Value1", iniFile.ReadValue("section1", "key1"));

                Assert.Equal("", iniFile.ReadValue("Section1", "NonExistentKey"));
                Assert.Equal("default", iniFile.ReadValue("Section1", "NonExistentKey", "default"));

                Assert.Equal("", iniFile.ReadValue("NonExistentSection", "Key1"));
            }
            finally
            {
                if (System.IO.File.Exists(tempFile))
                {
                    System.IO.File.Delete(tempFile);
                }
            }
        }
    }
}
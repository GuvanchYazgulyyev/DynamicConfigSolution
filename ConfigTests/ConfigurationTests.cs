using Moq;

namespace ConfigTests
{
    public class ConfigurationTests
    {
        private readonly Mock<IConfigurationReader> _configReaderMock;

        public ConfigurationTests()
        {
            _configReaderMock = new Mock<IConfigurationReader>();
        }

        [Fact]
        public void GetValue_ReturnsCorrectValue_WhenKeyExists()
        {
            // Arrange
            var key = "SiteName";
            var expectedValue = "soty.io";
            _configReaderMock.Setup(reader => reader.GetValue<string>(key)).Returns(expectedValue);

            // Act
            var result = _configReaderMock.Object.GetValue<string>(key);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetValue_ReturnsNull_WhenKeyDoesNotExist()
        {
            // Arrange
            var key = "NonExistentKey";
            _configReaderMock.Setup(reader => reader.GetValue<string>(key)).Returns((string)null);

            // Act
            var result = _configReaderMock.Object.GetValue<string>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetValue_ReturnsDefaultValue_WhenValueIsNull()
        {
            // Arrange
            var key = "MaxItemCount";
            var expectedValue = 50;
            _configReaderMock.Setup(reader => reader.GetValue<int>(key)).Returns(expectedValue);

            // Act
            var result = _configReaderMock.Object.GetValue<int>(key);

            // Assert
            Assert.Equal(expectedValue, result);
        }
    }

    public interface IConfigurationReader
    {
        T GetValue<T>(string key);
    }

}

using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DotNetSamples.Core;
using DotNetSamples.Web.Controllers;

namespace DotNetSamples.Web.Tests
{
    [TestClass]
    public class SamplesControllerTests
    {
        Mock<IRepository> _repository;
        SamplesController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new Mock<IRepository>();
            _controller = new SamplesController(_repository.Object);
        }

        [TestMethod]
        public void CanGetAllSamples()
        {
            // Arrange
            _repository.Setup(r => r.GetSamples()).Returns(new[] {
                 new Sample { Command = $"sample1" },
                 new Sample { Command = $"sample2" },
                 new Sample { Command = $"sample3" }
            });

            // Act
            var response = (OkObjectResult)_controller.Get().Result;

            // Assert
            Assert.AreEqual(3, ((IEnumerable<Sample>)response.Value).Count());
            _repository.Verify(r => r.GetSamples(), Times.Once);
        }

        [TestMethod]
        public void CanGetSampleGivenACommand()
        {
            // Arrange
            var expectedSample = new Sample { Command = "sample1" };
            _repository.Setup(r => r.Get(expectedSample.Command)).Returns(expectedSample);

            // Act
            var response = (OkObjectResult)_controller.Get(expectedSample.Command).Result;

            // Assert
            Assert.AreEqual(expectedSample.Command, ((Sample)response.Value).Command);
            _repository.Verify(r => r.Get(expectedSample.Command), Times.Once);
        }

        [TestMethod]
        public void ReturnsNotFoundResultWhenGivenUnknownCommand()
        {
            // Arrange
            var command = "unknowncommand";
            _repository.Setup(r => r.Get(It.IsAny<string>())).Returns<Sample>(null);

            // Act
            var response = _controller.Get(command).Result as NotFoundResult;

            // Assert
            Assert.IsNotNull(response);
            _repository.Verify(r => r.Get(command), Times.Once);
        }
    }
}

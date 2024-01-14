using System;
using System.Collections.Generic;
using AutoMapper;
using ContentService.Controllers;
using ContentService.Data;
using ContentService.DTO;
using ContentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ContentService.Tests
{
    [TestClass]
    public class ContentsControllerTests
    {
        [TestMethod]
        public void GetContents_ReturnsOkResult()
        {
            // Arrange
            var mockRepo = new Mock<IContentRepo>();
            var mockMapper = new Mock<IMapper>();
            var mockConfig = new Mock<IConfiguration>();

            var controller = new ContentsController(mockConfig.Object, mockRepo.Object, mockMapper.Object);

            // Act
            var result = controller.GetContents();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetContentById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var mockRepo = new Mock<IContentRepo>();
            var mockMapper = new Mock<IMapper>();
            var mockConfig = new Mock<IConfiguration>();

            var controller = new ContentsController(mockConfig.Object, mockRepo.Object, mockMapper.Object);

            int validId = 1;

            // Mock repository to return a valid ContentReadDTO for the given ID
            mockRepo.Setup(r => r.GetContentById(validId))
                .Returns(new Content { Id = validId, Name = "Valid Content" });

            // Act
            var result = controller.GetContentById(validId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void GetContentById_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepo = new Mock<IContentRepo>();
            var mockMapper = new Mock<IMapper>();
            var mockConfig = new Mock<IConfiguration>();

            var controller = new ContentsController(mockConfig.Object, mockRepo.Object, mockMapper.Object);

            int invalidId = 999;

            // Act
            var result = controller.GetContentById(invalidId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}

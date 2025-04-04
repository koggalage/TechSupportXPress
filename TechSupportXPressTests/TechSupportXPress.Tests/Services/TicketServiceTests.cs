using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services;
using TechSupportXPress.Services.Interfaces;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Tests.Services
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepoMock = new();
        private readonly Mock<ICommentRepository> _commentRepoMock = new();
        private readonly Mock<ITicketResolutionRepository> _resolutionRepoMock = new();
        private readonly Mock<ISystemCodeService> _systemCodeServiceMock = new();
        private readonly Mock<IFileService> _fileServiceMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();

        private readonly TicketService _service;

        public TicketServiceTests()
        {
            _service = new TicketService(
                _ticketRepoMock.Object,
                _commentRepoMock.Object,
                _resolutionRepoMock.Object,
                _systemCodeServiceMock.Object,
                _fileServiceMock.Object,
                _userRepoMock.Object
            );
        }

        [Fact]
        public async Task AssignTicketAsync_ShouldAssign_WhenTicketExistsAndStatusFound()
        {
            // Arrange
            int ticketId = 1;
            string assignedToId = "support123";
            string userId = "admin456";

            var ticket = new Ticket { Id = ticketId };

            _ticketRepoMock.Setup(repo => repo.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            _systemCodeServiceMock.Setup(s => s.GetResolutionStatusByCodeAsync("Assigned"))
                .ReturnsAsync(new SystemCodeDetail { Id = 10 });

            // Act
            var result = await _service.AssignTicketAsync(ticketId, assignedToId, userId);

            // Assert
            Assert.True(result);
            _resolutionRepoMock.Verify(r => r.AddAsync(It.Is<TicketResolution>(
                tr => tr.TicketId == ticketId &&
                      tr.CreatedById == userId &&
                      tr.Description == "Ticket Assigned"
            )), Times.Once);

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.Is<Ticket>(
                t => t.Id == ticketId &&
                     t.AssignedToId == assignedToId
            )), Times.Once);
        }

        [Fact]
        public async Task CloseTicketAsync_ShouldClose_WhenTicketExistsAndStatusFound()
        {
            // Arrange
            int ticketId = 5;
            string userId = "admin123";

            var ticket = new Ticket { Id = ticketId };
            var closedStatus = new SystemCodeDetail { Id = 99 };

            _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId)).ReturnsAsync(ticket);
            _systemCodeServiceMock.Setup(s => s.GetResolutionStatusByCodeAsync("Closed")).ReturnsAsync(closedStatus);

            // Act
            var result = await _service.CloseTicketAsync(ticketId, userId);

            // Assert
            Assert.True(result);

            _resolutionRepoMock.Verify(r => r.AddAsync(It.Is<TicketResolution>(
                tr => tr.TicketId == ticketId &&
                      tr.StatusId == closedStatus.Id &&
                      tr.Description == "Ticket Closed" &&
                      tr.CreatedById == userId
            )), Times.Once);

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.Is<Ticket>(
                t => t.StatusId == closedStatus.Id
            )), Times.Once);
        }

        [Fact]
        public async Task ReOpenTicketAsync_ShouldReOpen_WhenTicketExistsAndStatusFound()
        {
            // Arrange
            int ticketId = 10;
            string userId = "user123";

            var ticket = new Ticket { Id = ticketId };
            var reopenedStatus = new SystemCodeDetail { Id = 77 };

            _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId)).ReturnsAsync(ticket);
            _systemCodeServiceMock.Setup(s => s.GetResolutionStatusByCodeAsync("ReOpened")).ReturnsAsync(reopenedStatus);

            // Act
            var result = await _service.ReOpenTicketAsync(ticketId, userId);

            // Assert
            Assert.True(result);

            _resolutionRepoMock.Verify(r => r.AddAsync(It.Is<TicketResolution>(
                tr => tr.TicketId == ticketId &&
                      tr.StatusId == reopenedStatus.Id &&
                      tr.Description == "Ticket Re-Opened" &&
                      tr.CreatedById == userId
            )), Times.Once);

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.Is<Ticket>(
                t => t.StatusId == reopenedStatus.Id
            )), Times.Once);
        }

        [Fact]
        public async Task GetTicketDetailsViewModelAsync_ShouldReturnDetails_WhenTicketExists()
        {
            // Arrange
            int ticketId = 42;

            var ticket = new Ticket { Id = ticketId, Title = "Test Ticket" };
            var comments = new List<Comment>
            {
                new Comment { TicketId = ticketId, Description = "Sample comment" }
            };
            
            var resolutions = new List<TicketResolution>
            {
                new TicketResolution { TicketId = ticketId, Description = "Initial assignment" }
            };

            _ticketRepoMock.Setup(r => r.GetTicketDetailsByIdAsync(ticketId)).ReturnsAsync(ticket);
            _commentRepoMock.Setup(r => r.GetCommentsByTicketIdAsync(ticketId)).ReturnsAsync(comments);
            _resolutionRepoMock.Setup(r => r.GetResolutionsByTicketIdAsync(ticketId)).ReturnsAsync(resolutions);

            // Act
            var result = await _service.GetTicketDetailsViewModelAsync(ticketId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticketId, result.TicketDetails.Id);
            Assert.Single(result.TicketComments);
            Assert.Single(result.TicketResolutions);
        }

        [Fact]
        public async Task UpdateTicketAsync_ShouldUpdateTicket_WhenAttachmentProvided()
        {
            // Arrange
            int ticketId = 100;
            string userId = "admin123";
            string uploadedFileName = "Ticket_20250404_sample.pdf";

            var ticket = new Ticket { Id = ticketId };
            var viewModel = new TicketViewModel
            {
                Id = ticketId,
                Title = "Updated Title",
                Description = "Updated Description",
                PriorityId = 2,
                SubCategoryId = 3
            };

            // Create mock IFormFile
            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns("sample.pdf");

            _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId)).ReturnsAsync(ticket);
            _fileServiceMock.Setup(f => f.SaveAttachmentAsync(It.IsAny<IFormFile>(), "Ticket"))
                .ReturnsAsync(uploadedFileName);

            // Act
            var result = await _service.UpdateTicketAsync(ticketId, viewModel, fileMock.Object, userId);

            // Assert
            Assert.True(result);
            Assert.Equal(uploadedFileName, ticket.Attachment);
            Assert.Equal("Updated Title", ticket.Title);

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.Is<Ticket>(
                t => t.Title == "Updated Title" &&
                     t.Description == "Updated Description" &&
                     t.PriorityId == 2 &&
                     t.SubCategoryId == 3 &&
                     t.ModifiedById == userId &&
                     t.Attachment == uploadedFileName
            )), Times.Once);
        }

        [Fact]
        public async Task CreateTicketAsync_ShouldReturnTicketId_WhenValidInput()
        {
            // Arrange
            var userId = "creator123";
            var attachment = "Ticket_20250404_image.png";
            var expectedTicketId = 99;

            var createVm = new TicketCreateViewModel
            {
                Title = "New Ticket",
                Description = "Test description",
                PriorityId = 1,
                SubCategoryId = 2
            };

            _systemCodeServiceMock.Setup(s => s.GetResolutionStatusByDescriptionAsync("Pending"))
                .ReturnsAsync(new SystemCodeDetail { Id = 5 });

            _ticketRepoMock.Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .ReturnsAsync(expectedTicketId);

            // Act
            var result = await _service.CreateTicketAsync(createVm, userId, attachment);

            // Assert
            Assert.Equal(expectedTicketId, result);

            _ticketRepoMock.Verify(r => r.AddAsync(It.Is<Ticket>(t =>
                t.Title == "New Ticket" &&
                t.Description == "Test description" &&
                t.PriorityId == 1 &&
                t.SubCategoryId == 2 &&
                t.Attachment == attachment &&
                t.CreatedById == userId &&
                t.StatusId == 5
            )), Times.Once);
        }

    }
}

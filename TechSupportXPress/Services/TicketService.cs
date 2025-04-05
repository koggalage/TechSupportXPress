using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechSupportXPress.Brokers;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly ITicketResolutionRepository _resolutionRepo;
        private readonly ISystemCodeService _systemCodeService;
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepository;

        public TicketService(
            ITicketRepository ticketRepo,
            ICommentRepository commentRepo,
            ITicketResolutionRepository resolutionRepo,
            ISystemCodeService systemCodeService,
            IFileService fileService,
            IUserRepository userRepository)
        {
            _ticketRepo = ticketRepo;
            _commentRepo = commentRepo;
            _resolutionRepo = resolutionRepo;
            _systemCodeService = systemCodeService;
            _fileService = fileService;
            _userRepository = userRepository;
        }

        public async Task<TicketViewModel> GetFilteredTicketsAsync(string userId, IList<string> roles, TicketViewModel vm)
        {
            var tickets = _ticketRepo.GetAllTicketsWithIncludes();

            if (roles.Contains("SUPPORT"))
                tickets = tickets.Where(t => t.AssignedToId == userId);
            else if (roles.Contains("USER"))
                tickets = tickets.Where(t => t.CreatedById == userId);

            if (!string.IsNullOrWhiteSpace(vm.Title))
                tickets = tickets.Where(t => t.Title.Contains(vm.Title));

            if (!string.IsNullOrWhiteSpace(vm.CreatedById))
                tickets = tickets.Where(t => t.CreatedById == vm.CreatedById);

            if (vm.StatusId > 0)
                tickets = tickets.Where(t => t.StatusId == vm.StatusId);

            if (vm.PriorityId > 0)
                tickets = tickets.Where(t => t.PriorityId == vm.PriorityId);

            if (vm.CategoryId > 0)
                tickets = tickets.Where(t => t.SubCategory.CategoryId == vm.CategoryId);

            vm.Tickets = await tickets.OrderBy(x => x.CreatedOn).ToListAsync();

            return vm;
        }

        public async Task<TicketViewModel> GetTicketDetailsViewModelAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetTicketDetailsByIdAsync(ticketId);
            if (ticket == null) return null;

            var comments = await _commentRepo.GetCommentsByTicketIdAsync(ticketId);
            var resolutions = await _resolutionRepo.GetResolutionsByTicketIdAsync(ticketId);

            return new TicketViewModel
            {
                TicketDetails = ticket,
                TicketComments = comments,
                TicketResolutions = resolutions
            };
        }

        public async Task<int> CreateTicketAsync(TicketCreateViewModel vm, string userId, string attachmentFileName)
        {
            var pendingStatus = await _systemCodeService.GetResolutionStatusByDescriptionAsync("Pending");

            var ticket = new Ticket
            {
                Title = vm.Title,
                Description = vm.Description,
                StatusId = pendingStatus.Id,
                PriorityId = vm.PriorityId,
                SubCategoryId = vm.SubCategoryId,
                Attachment = attachmentFileName,
                CreatedById = userId,
                CreatedOn = DateTime.Now
            };

            var ticketId = await _ticketRepo.AddAsync(ticket);

            return ticketId;
        }

        public async Task<TicketViewModel> GetTicketEditViewModelAsync(int id)
        {
            var ticket = await _ticketRepo.GetTicketWithSubCategoryAsync(id);
            if (ticket == null)
                return null;

            return new TicketViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                PriorityId = ticket.PriorityId,
                CategoryId = ticket.SubCategory.CategoryId,
                SubCategoryId = ticket.SubCategoryId,
                Attachment = ticket.Attachment
            };
        }


        public async Task<bool> UpdateTicketAsync(int id, TicketViewModel model, IFormFile attachment, string userId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(id);
            if (ticket == null)
                return false;

            if (attachment != null && attachment.Length > 0)
            {
                var fileName = await _fileService.SaveAttachmentAsync(attachment, "Ticket");
                ticket.Attachment = fileName;
            }

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.PriorityId = model.PriorityId;
            ticket.SubCategoryId = model.SubCategoryId;
            ticket.ModifiedOn = DateTime.Now;
            ticket.ModifiedById = userId;

            await _ticketRepo.UpdateAsync(ticket);
            return true;
        }

        public async Task<Ticket> GetTicketForDeleteViewAsync(int id)
        {
            return await _ticketRepo.GetWithCreatedByAsync(id);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepo.GetByIdAsync(id);
            if (ticket == null)
                return false;

            // Delete comments first
            await _commentRepo.DeleteByTicketIdAsync(id);

            await _ticketRepo.DeleteAsync(ticket);
            return true;
        }

        public async Task<TicketViewModel> GetTicketResolveViewModelAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetTicketForResolveAsync(ticketId);
            if (ticket == null) return null;

            var comments = await _commentRepo.GetCommentsByTicketIdAsync(ticketId);
            var resolutions = await _resolutionRepo.GetResolutionsByTicketIdAsync(ticketId);

            string nextStatus = ticket.Status.Code switch
            {
                "Pending" => "Assigned",
                "Assigned" => "Resolved",
                "Resolved" => "Closed",
                "Closed" => "ReOpened",
                "ReOpened" => "Assigned",
                _ => null
            };

            var nextStatusObj = !string.IsNullOrEmpty(nextStatus)
                ? await _systemCodeService.GetResolutionStatusByDescriptionAsync(nextStatus)
                : null;

            return new TicketViewModel
            {
                TicketDetails = ticket,
                TicketComments = comments,
                TicketResolutions = resolutions,
                NextStatus = nextStatusObj?.Description,
                StatusId = nextStatusObj?.Id ?? 0
            };
        }

        public async Task<bool> ResolveTicketAsync(int ticketId, int newStatusId, string description, string userId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            var resolution = new TicketResolution
            {
                TicketId = ticketId,
                StatusId = newStatusId,
                CreatedOn = DateTime.Now,
                CreatedById = userId,
                Description = description
            };
            await _resolutionRepo.AddAsync(resolution);

            ticket.StatusId = newStatusId;
            await _ticketRepo.UpdateAsync(ticket);

            return true;
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            return await _ticketRepo.GetByIdAsync(id);
        }

        public async Task<TicketViewModel> GetTicketCloseViewModelAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetTicketForResolveAsync(ticketId);
            if (ticket == null) return null;

            var comments = await _commentRepo.GetCommentsByTicketIdAsync(ticketId);
            var resolutions = await _resolutionRepo.GetResolutionsByTicketIdAsync(ticketId);

            return new TicketViewModel
            {
                TicketDetails = ticket,
                TicketComments = comments,
                TicketResolutions = resolutions
            };
        }

        public async Task<bool> CloseTicketAsync(int ticketId, string userId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            var closedStatus = await _systemCodeService.GetResolutionStatusByCodeAsync("Closed");
            if (closedStatus == null) return false;

            var resolution = new TicketResolution
            {
                TicketId = ticketId,
                StatusId = closedStatus.Id,
                CreatedOn = DateTime.Now,
                CreatedById = userId,
                Description = "Ticket Closed"
            };

            await _resolutionRepo.AddAsync(resolution);

            ticket.StatusId = closedStatus.Id;
            await _ticketRepo.UpdateAsync(ticket);

            return true;
        }

        public async Task<TicketViewModel> GetTicketReOpenViewModelAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetTicketForResolveAsync(ticketId);
            if (ticket == null) return null;

            var comments = await _commentRepo.GetCommentsByTicketIdAsync(ticketId);
            var resolutions = await _resolutionRepo.GetResolutionsByTicketIdAsync(ticketId);

            return new TicketViewModel
            {
                TicketDetails = ticket,
                TicketComments = comments,
                TicketResolutions = resolutions
            };
        }

        public async Task<bool> ReOpenTicketAsync(int ticketId, string userId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            var reopenedStatus = await _systemCodeService.GetResolutionStatusByCodeAsync("ReOpened");
            if (reopenedStatus == null) return false;

            var resolution = new TicketResolution
            {
                TicketId = ticketId,
                StatusId = reopenedStatus.Id,
                CreatedOn = DateTime.Now,
                CreatedById = userId,
                Description = "Ticket Re-Opened"
            };

            await _resolutionRepo.AddAsync(resolution);

            ticket.StatusId = reopenedStatus.Id;
            await _ticketRepo.UpdateAsync(ticket);

            return true;
        }

        public async Task<TicketViewModel> GetTicketAssignmentViewModelAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetTicketForResolveAsync(ticketId);
            if (ticket == null) return null;

            var comments = await _commentRepo.GetCommentsByTicketIdAsync(ticketId);
            var resolutions = await _resolutionRepo.GetResolutionsByTicketIdAsync(ticketId);

            return new TicketViewModel
            {
                TicketDetails = ticket,
                TicketComments = comments,
                TicketResolutions = resolutions
            };
        }

        public async Task<bool> AssignTicketAsync(int ticketId, string assignedToId, string userId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            var assignedStatus = await _systemCodeService.GetResolutionStatusByCodeAsync("Assigned");
            if (assignedStatus == null) return false;

            var resolution = new TicketResolution
            {
                TicketId = ticketId,
                StatusId = assignedStatus.Id,
                CreatedOn = DateTime.Now,
                CreatedById = userId,
                Description = "Ticket Assigned"
            };

            await _resolutionRepo.AddAsync(resolution);

            ticket.StatusId = assignedStatus.Id;
            ticket.AssignedToId = assignedToId;
            ticket.AssignedOn = DateTime.Now;

            await _ticketRepo.UpdateAsync(ticket);

            return true;
        }

        public async Task<TicketViewModel> GetAssignedTicketsAsync(TicketViewModel filterVm, ApplicationUser currentUser)
        {
            var userRoles = await _userRepository.GetUserRolesAsync(currentUser);

            var statuses = await _systemCodeService.GetResolutionStatusesByCodesAsync("Assigned", "ReOpened");

            var statusIds = statuses.Select(x => x.Id).ToList();
            var tickets = await _ticketRepo.GetTicketsByStatusIdsAsync(statusIds);
            var result = tickets.AsQueryable();

            // Role-based filtering
            if (userRoles.Contains("SUPPORT"))
            {
                result = result.Where(t => t.AssignedToId == currentUser.Id);
            }
            else if (userRoles.Contains("USER"))
            {
                result = result.Where(t => t.CreatedById == currentUser.Id);
            }

            // Apply filters
            if (filterVm != null)
            {
                if (!string.IsNullOrEmpty(filterVm.Title))
                    result = result.Where(t => t.Title == filterVm.Title);

                if (!string.IsNullOrEmpty(filterVm.CreatedById))
                    result = result.Where(t => t.CreatedById == filterVm.CreatedById);

                if (filterVm.StatusId > 0)
                    result = result.Where(t => t.StatusId == filterVm.StatusId);

                if (filterVm.PriorityId > 0)
                    result = result.Where(t => t.PriorityId == filterVm.PriorityId);

                if (filterVm.CategoryId > 0)
                    result = result.Where(t => t.SubCategory.CategoryId == filterVm.CategoryId);
            }

            return new TicketViewModel
            {
                Tickets = result.OrderBy(t => t.CreatedOn).ToList()
            };
        }

        public async Task<TicketViewModel> GetClosedTicketsAsync(TicketViewModel filterVm, ApplicationUser currentUser)
        {
            var userRoles = await _userRepository.GetUserRolesAsync(currentUser);

            var closedStatuses = await _systemCodeService.GetResolutionStatusesByCodesAsync("Closed");
            var statusIds = closedStatuses.Select(x => x.Id).ToList();

            var tickets = await _ticketRepo.GetTicketsByStatusIdsAsync(statusIds);
            var result = tickets.AsQueryable();

            // Role-based access
            if (userRoles.Contains("SUPPORT"))
                result = result.Where(t => t.AssignedToId == currentUser.Id);
            else if (userRoles.Contains("USER"))
                result = result.Where(t => t.CreatedById == currentUser.Id);

            // Apply form filters
            if (!string.IsNullOrEmpty(filterVm.Title))
                result = result.Where(t => t.Title == filterVm.Title);

            if (!string.IsNullOrEmpty(filterVm.CreatedById))
                result = result.Where(t => t.CreatedById == filterVm.CreatedById);

            if (filterVm.StatusId > 0)
                result = result.Where(t => t.StatusId == filterVm.StatusId);

            if (filterVm.PriorityId > 0)
                result = result.Where(t => t.PriorityId == filterVm.PriorityId);

            if (filterVm.CategoryId > 0)
                result = result.Where(t => t.SubCategory.CategoryId == filterVm.CategoryId);

            return new TicketViewModel
            {
                Tickets = result.OrderBy(t => t.CreatedOn).ToList()
            };
        }

        public async Task<TicketViewModel> GetResolvedTicketsAsync(TicketViewModel filterVm, ApplicationUser currentUser)
        {
            var userRoles = await _userRepository.GetUserRolesAsync(currentUser);

            var resolvedStatuses = await _systemCodeService.GetResolutionStatusesByCodesAsync("Resolved");
            var statusIds = resolvedStatuses.Select(x => x.Id).ToList();

            var tickets = await _ticketRepo.GetTicketsByStatusIdsAsync(statusIds);
            var result = tickets.AsQueryable();

            if (userRoles.Contains("SUPPORT"))
                result = result.Where(t => t.AssignedToId == currentUser.Id);
            else if (userRoles.Contains("USER"))
                result = result.Where(t => t.CreatedById == currentUser.Id);

            if (!string.IsNullOrEmpty(filterVm.Title))
                result = result.Where(t => t.Title == filterVm.Title);

            if (!string.IsNullOrEmpty(filterVm.CreatedById))
                result = result.Where(t => t.CreatedById == filterVm.CreatedById);

            if (filterVm.StatusId > 0)
                result = result.Where(t => t.StatusId == filterVm.StatusId);

            if (filterVm.PriorityId > 0)
                result = result.Where(t => t.PriorityId == filterVm.PriorityId);

            if (filterVm.CategoryId > 0)
                result = result.Where(t => t.SubCategory.CategoryId == filterVm.CategoryId);

            return new TicketViewModel
            {
                Tickets = result.OrderBy(t => t.CreatedOn).ToList()
            };
        }


    }
}

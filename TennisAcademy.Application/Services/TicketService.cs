using BuildingBlocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.DTOs.Ticket;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;
using TennisAcademy.Domain.Enums;
using System.Linq;

namespace TennisAcademy.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly ICoachMediaRepository _mediaRepo;


        public TicketService(ITicketRepository ticketRepo, ICoachMediaRepository mediaRepo)
        {
            _ticketRepo = ticketRepo;
            _mediaRepo = mediaRepo; // 👈 این خط مهمه
        }

        public async Task<List<Ticket>> GetAllAsync(AdminTicketFilterDto filter)
        {
            var tickets = await _ticketRepo.GetAllAsync();
            if (tickets == null || !tickets.Any())
                throw new NotFoundException("No ticket found.");

            var query = tickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                query = query.Where(t =>
                    (t.User.FirstName + " " + t.User.LastName).Contains(filter.Query, StringComparison.OrdinalIgnoreCase)
                    || t.User.Email.Contains(filter.Query, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (!string.IsNullOrWhiteSpace(filter.Subject))
                query = query.Where(t => t.Subject.Contains(filter.Subject));

            if (filter.CoachId.HasValue)
                query = query.Where(t => t.CoachId == filter.CoachId.Value);

            if (filter.FromCreated.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.FromCreated.Value);

            if (filter.ToCreated.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.ToCreated.Value);

            if (filter.FromAnswered.HasValue)
                query = query.Where(t => t.AnsweredAt.HasValue && t.AnsweredAt.Value >= filter.FromAnswered.Value);

            if (filter.ToAnswered.HasValue)
                query = query.Where(t => t.AnsweredAt.HasValue && t.AnsweredAt.Value <= filter.ToAnswered.Value);

            return query.OrderByDescending(t => t.CreatedAt).ToList();
        }

        public async Task<TicketStatisticsDto> GetStatisticsAsync()
        {
            var tickets = await _ticketRepo.GetAllAsync();
            var stats = new TicketStatisticsDto
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status != TicketStatus.Closed),
                ClosedTickets = tickets.Count(t => t.Status == TicketStatus.Closed)
            };
            return stats;
        }


        public async Task AddTicketAsync(Ticket ticket)
        {
            await _ticketRepo.AddAsync(ticket);
            await _ticketRepo.SaveChangesAsync();
        }

        public async Task<List<Ticket>> GetByUserAsync(Guid userId)
        {
           var ticket = await _ticketRepo.GetByUserIdAsync(userId);
            if (ticket == null)
                throw new NotFoundException("No ticket found.");

            return ticket;
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            var ticket =await _ticketRepo.GetByIdAsync(id);
            if (ticket == null)
                throw new NotFoundException("Ticket not found.");
            return ticket;
        }

        public async Task AnswerTicketAsync(AnswerTicketDto dto)
        {
            var ticket = await _ticketRepo.GetByIdAsync(dto.TicketId);
            if (ticket == null)
                throw new NotFoundException("Ticket not found.");

            ticket.CoachReply = dto.CoachReply;
            ticket.AnsweredAt = DateTime.UtcNow;
            ticket.Status = TicketStatus.Answered;

            // ۱. اگر فایل جدید آپلود شده:
            if (!string.IsNullOrEmpty(dto.CoachReplyVoiceUrl))
                ticket.CoachReplyVoiceUrl = dto.CoachReplyVoiceUrl;

            if (!string.IsNullOrEmpty(dto.CoachReplyVideoUrl))
                ticket.CoachReplyVideoUrl = dto.CoachReplyVideoUrl;

            // ۲. اگر از فایل آماده انتخاب شده:
            if (dto.CoachVoiceMediaId.HasValue)
            {
                var voice = await _mediaRepo.GetByIdAsync(dto.CoachVoiceMediaId.Value);
                if (voice != null && voice.Type == MediaType.Voice)
                    ticket.CoachReplyVoiceUrl = voice.FileUrl;
            }

            if (dto.CoachVideoMediaId.HasValue)
            {
                var video = await _mediaRepo.GetByIdAsync(dto.CoachVideoMediaId.Value);
                if (video != null && video.Type == MediaType.Video)
                    ticket.CoachReplyVideoUrl = video.FileUrl;
            }

            _ticketRepo.Update(ticket);
            await _ticketRepo.SaveChangesAsync();
        }

        public async Task CloseTicketAsync(Guid ticketId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new NotFoundException("Ticket not found.");

            ticket.Status = TicketStatus.Closed;
            _ticketRepo.Update(ticket);
            await _ticketRepo.SaveChangesAsync();
        }


    }
}

using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper imapper)
        {
            _context = context;
            _mapper = imapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.Include(p => p.Sender).Include(p => p.Recipient).SingleOrDefaultAsync(v => v.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(n => n.DateSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(q => q.Recipient.UserName == messageParams.Username && q.RecipientDeleted == false),
                "Outbox" => query.Where(q => q.Sender.UserName == messageParams.Username && q.SenderDeleted == false),
                _ => query.Where(q => q.Recipient.UserName == messageParams.Username && q.RecipientDeleted == false && q.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages.Include(p => p.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(u => u.Photos)
                .Where(r => r.Recipient.UserName == currentUsername && r.RecipientDeleted == false && r.Sender.UserName == recipientUsername
                || r.Recipient.UserName == recipientUsername && r.SenderDeleted == false && r.Sender.UserName == currentUsername)
                .OrderBy(m => m.DateSent)
                .ToListAsync();

            var unreadMessage = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessage.Any())
            {
                foreach (var message in unreadMessage)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

﻿using API.DTOs;
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
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.AppUsers.Where(v=>v.UserName==username).ProjectTo<MemberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.AppUsers.AsQueryable();
            query = query.Where(k => k.UserName != userParams.CurrentUsername);
            query = query.Where(m => m.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(d => d.DateOfBirth >= minDob && d.DateOfBirth <= maxDob);

            query = userParams.OrderBy
                switch
            {
                "created" => query.OrderByDescending(o => o.Created),
                _ => query.OrderByDescending(p => p.LasrActive),
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.AppUsers.Include(b => b.Photos).SingleOrDefaultAsync(m => m.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.AppUsers.Include(x => x.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}

﻿using Microsoft.EntityFrameworkCore;
using TelegramBot_OpenAI.Data.DB.Interfaces;
using TelegramBot_OpenAI.Models;

namespace TelegramBot_OpenAI.Data.DB.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TelegramUser?> GetById(long id, bool tracking)
        {
            TelegramUser? user;

            if (tracking)
                user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == id);
            else user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.TelegramId == id);

            return user;
        }

        public async Task<bool> Add(TelegramUser user)
        {
            await _context.Users.AddAsync(user);

            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved > 0;
        }

        public async Task<bool> Update(TelegramUser user)
        {
            _context.Users.Update(user);

            return await Save();
        }
    }
}
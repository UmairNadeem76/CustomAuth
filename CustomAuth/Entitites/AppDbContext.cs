﻿using Microsoft.EntityFrameworkCore;

namespace CustomAuth.Entitites
{
	public class AppDbContext : DbContext
	{
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options) 
        {

        }

        public DbSet<UserAccount> UserAccounts { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
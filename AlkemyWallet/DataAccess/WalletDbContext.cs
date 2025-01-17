﻿using AlkemyWallet.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace AlkemyWallet.DataAccess;

public class WalletDbContext : IdentityDbContext<ApplicationUser>
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }

    public new DbSet<Role>? Roles { get; set; }
    public new DbSet<User>? Users { get; set; }
    public DbSet<Account>? Accounts { get; set; }
    public DbSet<Transaction>? Transactions { get; set; }
    public DbSet<FixedTermDeposit>? FixedTermDeposits { get; set; }
    public DbSet<Catalogue>? Catalogues { get; set; }


    //en caso de usar CodeFirst a Sql
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        foreach (var foreingKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            foreingKey.DeleteBehavior = DeleteBehavior.Restrict;
        base.OnModelCreating(builder);
        new DbInitializer(builder).Seed();

        //en construccion
        ///////////////////////
        builder.Entity<Role>().ToTable("Role");
        builder.Entity<User>().ToTable("User");
        builder.Entity<Account>().ToTable("Account");
        builder.Entity<Transaction>().ToTable("Transaction");
        builder.Entity<FixedTermDeposit>().ToTable("FixedTermDeposit");
        builder.Entity<Catalogue>().ToTable("Catalogue");

        builder.SetQueryFilterOnAllEntities<ISoftDelete>(e => !e.IsDeleted);

        base.OnModelCreating(builder);
    }

    static void SetQueryFilter<TEntity>(ModelBuilder builder) where TEntity : class
    {
        builder.Entity<TEntity>().HasQueryFilter(m => !(EF.Property<bool>(m, "IsDeleted")));
    }

    public override int SaveChanges()
    {
        UpdateSoftDeleteStatuses();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
        UpdateSoftDeleteStatuses();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateSoftDeleteStatuses()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.Entity.GetType().GetInterfaces().Contains(typeof(ISoftDelete)))
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["IsDeleted"] = false;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsDeleted"] = true;
                        entry.CurrentValues["DeletedDate"] = DateTime.Now;
                        break;
                }
        }
    }
}
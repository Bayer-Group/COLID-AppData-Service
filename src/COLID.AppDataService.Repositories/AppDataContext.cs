using System;
using System.Threading;
using System.Threading.Tasks;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Repositories.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace COLID.AppDataService.Repositories.Context
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
        {
            //this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ConsumerGroup> ConsumerGroups { get; set; }
        public virtual DbSet<SearchFilterDataMarketplace> SearchFilterDataMarketplace { get; set; }
        public virtual DbSet<SearchFilterEditor> SearchFiltersEditor { get; set; }
        public virtual DbSet<StoredQuery> StoredQueries { get; set; }
        public virtual DbSet<ColidEntrySubscription> ColidEntrySubscriptions { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageTemplate> MessageTemplates { get; set; }
        public virtual DbSet<MessageConfig> MessageConfigs { get; set; }
        public virtual DbSet<WelcomeMessage> WelcomeMessages { get; set; }
        public virtual DbSet<FavoritesList> FavoritesLists { get; set; }
        public virtual DbSet<FavoritesListEntry> FavoritesListEntries { get; set; }
        public virtual DbSet<ApplicationVersion> ApplicationVersions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Required function to set the fields CreatedAt and ModifiedAt on all functions, that doesn't use the .asNoTracked() (readOnly) chaining on Queries.
        /// </summary>
        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;

                if (entry.Entity is IEntity entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entity.ModifiedAt = now;
                            break;

                        case EntityState.Added:
                            entity.CreatedAt = now;
                            entity.ModifiedAt = now;
                            break;
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add several configurations to map Json-Objects (JObject) properly
            modelBuilder.ApplyConfiguration(new ConsumerGroupConfiguration());
            modelBuilder.ApplyConfiguration(new SearchFilterEditorConfiguration());
            modelBuilder.ApplyConfiguration(new SearchFilterDataMarketplaceConfiguration());
            modelBuilder.ApplyConfiguration(new StoredQueryConfiguration());
            modelBuilder.ApplyConfiguration(new MessageTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new WelcomeMessageConfiguration());
            modelBuilder.ApplyConfiguration(new FavoritesListConfiguration());

            // User configuration for cascade deletion
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            //Foreign key config for Application Version
            modelBuilder.ApplyConfiguration(new ApplicationVersionConfiguration());

            // Enable snake_case for tables, columns, keys, constraints and indexes
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetName(index.GetName().ToSnakeCase());
                }
            }
        }
    }
}

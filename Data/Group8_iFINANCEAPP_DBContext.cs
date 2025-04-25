using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Models;

namespace Group8_iFINANCE_APP.Data
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the iFinance application,
    /// exposing DbSet properties and configuring entity relationships.
    /// </summary>
    public class Group8_iFINANCEAPP_DBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Group8_iFINANCEAPP_DBContext"/> class
        /// with the specified options.
        /// </summary>
        /// <param name="options">The options to configure the context (e.g., connection string).</param>
        public Group8_iFINANCEAPP_DBContext(DbContextOptions<Group8_iFINANCEAPP_DBContext> options)
            : base(options)
        { }

        // DbSet properties expose each entity as a table in the database
        public DbSet<AccountCategory>   AccountCategories   { get; set; }
        public DbSet<UserPassword>      UserPasswords       { get; set; }
        public DbSet<Administrator>     Administrators      { get; set; }
        public DbSet<NonAdminUser>      NonAdminUsers       { get; set; }
        public DbSet<Group>             Groups              { get; set; }
        public DbSet<MasterAccount>     MasterAccounts      { get; set; }
        public DbSet<Transaction>       Transactions        { get; set; }
        public DbSet<TransactionLine>   TransactionLines    { get; set; }

        /// <summary>
        /// Configures entity relationships, keys, and database behaviors using the Fluent API.
        /// </summary>
        /// <param name="builder">The model builder used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure one-to-one relationship: Administrator ↔ UserPassword
            builder.Entity<Administrator>()
                .HasOne(a => a.UserPassword)
                .WithOne(up => up.Administrator)
                .HasForeignKey<Administrator>(a => a.UserPassword_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-one relationship: NonAdminUser ↔ UserPassword
            builder.Entity<NonAdminUser>()
                .HasOne(nu => nu.UserPassword)
                .WithOne(up => up.NonAdminUser)
                .HasForeignKey<NonAdminUser>(nu => nu.UserPassword_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-one: NonAdminUser → Administrator
            builder.Entity<NonAdminUser>()
                .HasOne(nu => nu.Administrator)
                .WithMany()  // no navigation property on Administrator
                .HasForeignKey(nu => nu.Administrator_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many: AccountCategory → Group
            builder.Entity<Group>()
                .HasOne(g => g.AccountCategory)
                .WithMany(ac => ac.Groups)
                .HasForeignKey(g => g.AccountCategory_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure self-referencing one-to-many: Group.ParentGroup ↔ Group.Children
            builder.Entity<Group>()
                .HasOne(g => g.ParentGroup)
                .WithMany(g => g.Children)
                .HasForeignKey(g => g.parent_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many: Group → MasterAccount
            builder.Entity<MasterAccount>()
                .HasOne(ma => ma.Group)
                .WithMany(g => g.MasterAccounts)
                .HasForeignKey(ma => ma.Group_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many: NonAdminUser → MasterAccount
            builder.Entity<MasterAccount>()
                .HasOne(ma => ma.NonAdminUser)
                .WithMany(u => u.MasterAccounts)
                .HasForeignKey(ma => ma.NonAdminUser_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many: Transaction → NonAdminUser
            builder.Entity<Transaction>()
                .HasOne(t => t.NonAdminUser)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.NonAdminUser_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TransactionLine table and relationships
            builder.Entity<TransactionLine>(e =>
            {
                e.ToTable("TransactionLines");
                e.HasKey(tl => tl.ID);

                // Debit side mapping
                e.Property(tl => tl.MasterAccounts_ID)
                    .HasColumnName("MasterAccounts_ID");
                e.HasOne(tl => tl.MasterAccount)
                    .WithMany(ma => ma.TransactionLines)
                    .HasForeignKey(tl => tl.MasterAccounts_ID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Credit side mapping (no back-navigation)
                e.Property(tl => tl.MasterAccounts1_ID)
                    .HasColumnName("MasterAccounts1_ID");
                e.HasOne(tl => tl.MasterAccount1)
                    .WithMany()
                    .HasForeignKey(tl => tl.MasterAccounts1_ID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Map simple properties to column names
                e.Property(tl => tl.DebitedAmount).HasColumnName("debitedAmount");
                e.Property(tl => tl.CreditedAmount).HasColumnName("creditedAmount");
                e.Property(tl => tl.Comments).HasColumnName("comment");

                // Transaction foreign key mapping
                e.Property(tl => tl.Transaction_ID)
                    .HasColumnName("Transaction_ID");
                e.HasOne(tl => tl.Transaction)
                    .WithMany(t => t.TransactionLines)
                    .HasForeignKey(tl => tl.Transaction_ID)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

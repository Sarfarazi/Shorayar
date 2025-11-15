using Council.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Council.Data.Contexts
{
    public class MainContext : DbContext
    {
        public MainContext()
            : base(Council.Core.Values.Values.MainConnectionString)
        {
            //base.Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MainContext, Council.Data.MainMigrations.Configuration>());
           // Database.SetInitializer<MainContext>(null);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<CouncilPeriod> Periods { get; set; }
        public DbSet<Letter> Letters { get; set; }
        public DbSet<LetterRefrences> LetterRefrences { get; set; }
        public DbSet<LetterStatuses> LetterStatuses { get; set; }
        public DbSet<OutLetter> OutLetterSpecs { get; set; }
        public DbSet<UniqueNumber> UniqueNumbers { get; set; }
        public DbSet<DefaultStatement> DefaultStatements { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Organ> Organs { get; set; }
        public DbSet<MeetingHeader> MeetingHeader { get; set; }
        public DbSet<Draft> Drafts { get; set; }
        public DbSet<SMSDeliversLog> SMSDeliversLogs { get; set; }
        public DbSet<SMSErrorCode> SMSErrorCodes { get; set; }
        public DbSet<SMSToken> SMSTokens { get; set; }
        public DbSet<EmailInfo> EmailsInfo { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<CopyToLetter> CopyToLetters { get; set; }       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commission>()
            .HasMany(c => c.Members)
            .WithMany(u => u.Commissions)
            .Map(model =>
            {
                model.ToTable("Commission_Users");
                model.MapLeftKey("Commission_ID");
                model.MapRightKey("User_ID");
            });
            modelBuilder.Configurations.Add(new LetterConfig());

            /**ارتباط بین جدول کاربر و رول**/
            modelBuilder.Entity<UserRole>()
                .HasKey(c => new { c.UserID, c.RoleID });

            //modelBuilder.Entity<User>()
            //    .HasMany(c => c.UserRole)
            //    .WithRequired()
            //    .HasForeignKey(c => c.UserID);

            //modelBuilder.Entity<Role>()
            //    .HasMany(c => c.UserRole)
            //    .WithRequired()
            //    .HasForeignKey(c => c.RoleID);
        }
    }
}

//Enable-Migrations -ContextTypeName Council.Data.Contexts.MainContext  -Migrations:MainMigrations
//add-migration -configuration  Council.Data.MainMigrations.Configuration initial
//Update-Database -configuration  Council.Data.MainMigrations.Configuration -verbose
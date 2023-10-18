using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace G3.Models
{
    public partial class SWPContext : DbContext
    {
        public SWPContext()
        {
        }

        public SWPContext(DbContextOptions<SWPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assignee> Assignees { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Classsetting> Classsettings { get; set; } = null!;
        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; } = null!;
        public virtual DbSet<Gitlabuser> Gitlabusers { get; set; } = null!;
        public virtual DbSet<Issue> Issues { get; set; } = null!;
        public virtual DbSet<Milestone> Milestones { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<Subjectsetting> Subjectsettings { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("server=localhost;uid=root;pwd=123456789;database=SWP");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignee>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("assignee");

                entity.HasIndex(e => e.GitLabUserId, "Assignee_GitLabUserId_fkey");

                entity.HasIndex(e => new { e.IssueId, e.GitLabUserId }, "Assignee_IssueId_GitLabUserId_key")
                    .IsUnique();

                entity.HasOne(d => d.GitLabUser)
                    .WithMany()
                    .HasForeignKey(d => d.GitLabUserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Assignee_GitLabUserId_fkey");

                entity.HasOne(d => d.Issue)
                    .WithMany()
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Assignee_IssueId_fkey");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("assignment");

                entity.HasIndex(e => e.Id, "Assignment_Id_idx");

                entity.HasIndex(e => e.SubjectId, "Assignment_SubjectId_fkey");

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.Title).HasMaxLength(191);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Assignment_SubjectId_fkey");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("class");

                entity.HasIndex(e => e.SubjectId, "Class_SubjectId_fkey");

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Class_SubjectId_fkey");
            });

            modelBuilder.Entity<Classsetting>(entity =>
            {
                entity.HasKey(e => e.SettingId)
                    .HasName("PRIMARY");

                entity.ToTable("classsetting");

                entity.HasIndex(e => e.ClassId, "ClassSetting_classId_fkey");

                entity.Property(e => e.ClassId).HasColumnName("classId");

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Type).HasMaxLength(191);

                entity.Property(e => e.Value).HasMaxLength(191);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Classsettings)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("ClassSetting_classId_fkey");
            });

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });

            modelBuilder.Entity<Gitlabuser>(entity =>
            {
                entity.ToTable("gitlabuser");

                entity.HasIndex(e => e.UserId, "GitLabUser_UserId_key")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "GitLabUser_Username_key")
                    .IsUnique();

                entity.Property(e => e.AvatarUrl).HasMaxLength(191);

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.State).HasMaxLength(191);

                entity.Property(e => e.Username).HasMaxLength(191);

                entity.Property(e => e.WebUrl).HasMaxLength(191);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Gitlabuser)
                    .HasForeignKey<Gitlabuser>(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("GitLabUser_UserId_fkey");
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.ToTable("issue");

                entity.HasIndex(e => e.AssigneeId, "Issue_AssigneeId_fkey");

                entity.HasIndex(e => e.AuthorId, "Issue_AuthorId_fkey");

                entity.HasIndex(e => e.ClosedById, "Issue_ClosedById_fkey");

                entity.HasIndex(e => e.MilestoneId, "Issue_MilestoneId_fkey");

                entity.Property(e => e.ClosedAt).HasColumnType("datetime(3)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime(3)")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Status).HasColumnType("enum('closed','opened')");

                entity.Property(e => e.Title).HasMaxLength(191);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime(3)");

                entity.HasOne(d => d.Assignee)
                    .WithMany(p => p.IssueAssignees)
                    .HasForeignKey(d => d.AssigneeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Issue_AssigneeId_fkey");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.IssueAuthors)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Issue_AuthorId_fkey");

                entity.HasOne(d => d.ClosedBy)
                    .WithMany(p => p.IssueClosedBies)
                    .HasForeignKey(d => d.ClosedById)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Issue_ClosedById_fkey");

                entity.HasOne(d => d.Milestone)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.MilestoneId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Issue_MilestoneId_fkey");
            });

            modelBuilder.Entity<Milestone>(entity =>
            {
                entity.ToTable("milestone");

                entity.HasIndex(e => e.ClassId, "Milestone_ClassId_fkey");

                entity.HasIndex(e => e.ProjectId, "Milestone_ProjectId_fkey");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime(3)")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.DueDate).HasColumnType("datetime(3)");

                entity.Property(e => e.StartDate).HasColumnType("datetime(3)");

                entity.Property(e => e.State).HasMaxLength(191);

                entity.Property(e => e.Title).HasMaxLength(191);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime(3)");

                entity.Property(e => e.WebUrl).HasMaxLength(191);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Milestones)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Milestone_ClassId_fkey");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Milestones)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Milestone_ProjectId_fkey");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("project");

                entity.HasIndex(e => e.ClassId, "Project_ClassId_fkey");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Project_ClassId_fkey");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("setting");

                entity.HasIndex(e => e.SettingId, "Setting_SettingId_idx");

                entity.HasIndex(e => new { e.Type, e.Value }, "Setting_Type_Value_idx");

                entity.HasIndex(e => new { e.Type, e.Value }, "Setting_Type_Value_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Type).HasMaxLength(191);

                entity.Property(e => e.Value).HasMaxLength(191);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subject");

                entity.HasIndex(e => e.Id, "Subject_Id_idx");

                entity.HasIndex(e => e.MentorId, "Subject_MentorId_fkey");

                entity.HasIndex(e => e.SubjectCode, "Subject_SubjectCode_idx");

                entity.HasIndex(e => e.SubjectCode, "Subject_SubjectCode_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.SubjectCode).HasMaxLength(191);

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Subject_MentorId_fkey");
            });

            modelBuilder.Entity<Subjectsetting>(entity =>
            {
                entity.ToTable("subjectsetting");

                entity.HasIndex(e => e.Id, "SubjectSetting_Id_idx");

                entity.HasIndex(e => e.SubjectId, "SubjectSetting_SubjectId_fkey");

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.Value).HasMaxLength(191);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Subjectsettings)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("SubjectSetting_SubjectId_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.ConfirmToken, "User_ConfirmToken_idx");

                entity.HasIndex(e => e.ConfirmToken, "User_ConfirmToken_key")
                    .IsUnique();

                entity.HasIndex(e => e.DomainSettingId, "User_DomainSettingId_fkey");

                entity.HasIndex(e => e.Email, "User_Email_idx");

                entity.HasIndex(e => e.Email, "User_Email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "User_Id_idx");

                entity.HasIndex(e => e.ResetPassToken, "User_ResetPassToken_idx");

                entity.HasIndex(e => e.ResetPassToken, "User_ResetPassToken_key")
                    .IsUnique();

                entity.HasIndex(e => e.RoleSettingId, "User_RoleSettingId_fkey");

                entity.Property(e => e.Address).HasMaxLength(191);

                entity.Property(e => e.Avatar).HasMaxLength(191);

                entity.Property(e => e.ConfirmToken).HasMaxLength(191);

                entity.Property(e => e.ConfirmTokenVerifyAt).HasColumnType("datetime(3)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime(3)")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime(3)");

                entity.Property(e => e.Description).HasMaxLength(191);

                entity.Property(e => e.Email).HasMaxLength(191);

                entity.Property(e => e.Hash).HasMaxLength(191);

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Phone).HasMaxLength(191);

                entity.Property(e => e.ResetPassToken).HasMaxLength(191);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime(3)");

                entity.HasOne(d => d.DomainSetting)
                    .WithMany(p => p.UserDomainSettings)
                    .HasForeignKey(d => d.DomainSettingId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("User_DomainSettingId_fkey");

                entity.HasOne(d => d.RoleSetting)
                    .WithMany(p => p.UserRoleSettings)
                    .HasForeignKey(d => d.RoleSettingId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("User_RoleSettingId_fkey");

                entity.HasMany(d => d.Classes)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "Userclass",
                        l => l.HasOne<Class>().WithMany().HasForeignKey("ClassId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("userclass_ibfk_2"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("userclass_ibfk_1"),
                        j =>
                        {
                            j.HasKey("UserId", "ClassId").HasName("PRIMARY");

                            j.ToTable("userclass");

                            j.HasIndex(new[] { "ClassId" }, "ClassID");

                            j.IndexerProperty<int>("UserId").HasColumnName("UserID");

                            j.IndexerProperty<int>("ClassId").HasColumnName("ClassID");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

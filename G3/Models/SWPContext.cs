﻿using System;
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
        public virtual DbSet<ClassAssignment> ClassAssignments { get; set; } = null!;
        public virtual DbSet<ClassSetting> ClassSettings { get; set; } = null!;
        public virtual DbSet<ClassStudentProject> ClassStudentProjects { get; set; } = null!;
        public virtual DbSet<GitLabUser> GitLabUsers { get; set; } = null!;
        public virtual DbSet<Issue> Issues { get; set; } = null!;
        public virtual DbSet<Milestone> Milestones { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<SubjectSetting> SubjectSettings { get; set; } = null!;
        public virtual DbSet<Submit> Submits { get; set; } = null!;
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

                entity.ToTable("Assignee", "SWP");

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
                entity.ToTable("Assignment", "SWP");

                entity.HasIndex(e => e.Id, "Assignment_Id_idx");

                entity.HasIndex(e => e.SubjectId, "Assignment_SubjectId_fkey");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Title).HasMaxLength(191);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Assignment_SubjectId_fkey");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Class", "SWP");

                entity.HasIndex(e => e.GitLabGroupId, "Class_GitLabGroupId_idx");

                entity.HasIndex(e => e.GitLabGroupId, "Class_GitLabGroupId_key")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "Class_Name_idx");

                entity.HasIndex(e => e.Name, "Class_Name_key")
                    .IsUnique();

                entity.HasIndex(e => e.SubjectId, "Class_SubjectId_fkey");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.GitLabGroupId).IsRequired();

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Class_SubjectId_fkey");
            });

            modelBuilder.Entity<ClassAssignment>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PRIMARY");

                entity.ToTable("ClassAssignment", "SWP");

                entity.HasIndex(e => new { e.AssignmentId, e.ClassId }, "ClassAssignment_AssignmentId_ClassId_idx");

                entity.HasIndex(e => new { e.AssignmentId, e.ClassId }, "ClassAssignment_AssignmentId_ClassId_key")
                    .IsUnique();

                entity.HasIndex(e => e.ClassId, "ClassAssignment_ClassId_fkey");

                entity.HasIndex(e => e.Key, "ClassAssignment_Key_idx");

                entity.HasIndex(e => e.MilestoneId, "ClassAssignment_MilestoneId_fkey");

                entity.Property(e => e.Key).HasMaxLength(191);

                entity.Property(e => e.EndDate).HasColumnType("datetime(3)");

                entity.Property(e => e.StartDate).HasColumnType("datetime(3)");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.ClassAssignments)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("ClassAssignment_AssignmentId_fkey");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassAssignments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("ClassAssignment_ClassId_fkey");

                entity.HasOne(d => d.Milestone)
                    .WithMany(p => p.ClassAssignments)
                    .HasForeignKey(d => d.MilestoneId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("ClassAssignment_MilestoneId_fkey");
            });

            modelBuilder.Entity<ClassSetting>(entity =>
            {
                entity.HasKey(e => e.SettingId)
                    .HasName("PRIMARY");

                entity.ToTable("ClassSetting", "SWP");

                entity.HasIndex(e => e.ClassId, "ClassSetting_classId_fkey");

                entity.Property(e => e.ClassId).HasColumnName("classId");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Type).HasMaxLength(191);

                entity.Property(e => e.Value).HasMaxLength(191);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassSettings)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("ClassSetting_classId_fkey");
            });

            modelBuilder.Entity<ClassStudentProject>(entity =>
            {
                entity.ToTable("ClassStudentProject", "SWP");

                entity.HasIndex(e => e.ClassId, "ClassStudentProject_ClassId_fkey");

                entity.HasIndex(e => e.ProjectId, "ClassStudentProject_ProjectId_fkey");

                entity.HasIndex(e => e.UserId, "ClassStudentProject_UserId_fkey");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassStudentProjects)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("ClassStudentProject_ClassId_fkey");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ClassStudentProjects)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("ClassStudentProject_ProjectId_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ClassStudentProjects)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("ClassStudentProject_UserId_fkey");
            });

            modelBuilder.Entity<GitLabUser>(entity =>
            {
                entity.ToTable("GitLabUser", "SWP");

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
                    .WithOne(p => p.GitLabUser)
                    .HasForeignKey<GitLabUser>(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("GitLabUser_UserId_fkey");
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.ToTable("Issue", "SWP");

                entity.HasIndex(e => e.AssigneeId, "Issue_AssigneeId_fkey");

                entity.HasIndex(e => e.AuthorId, "Issue_AuthorId_fkey");

                entity.HasIndex(e => e.ClosedById, "Issue_ClosedById_fkey");

                entity.HasIndex(e => e.MilestoneId, "Issue_MilestoneId_fkey");

                entity.HasIndex(e => e.ProjectId, "Issue_ProjectId_fkey");

                entity.Property(e => e.ClosedAt).HasColumnType("datetime(3)");

                entity.Property(e => e.Complexity).HasColumnType("enum('Complex','Medium','Simple')");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime(3)")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Quality).HasColumnType("enum('High','Medium','Low')");

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

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Issue_ProjectId_fkey");
            });

            modelBuilder.Entity<Milestone>(entity =>
            {
                entity.ToTable("Milestone", "SWP");

                entity.HasIndex(e => e.GroupId, "Milestone_GroupId_fkey");

                entity.HasIndex(e => e.Id, "Milestone_Id_key")
                    .IsUnique();

                entity.HasIndex(e => e.ProjectId, "Milestone_ProjectId_fkey");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime(3)")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DueDate).HasColumnType("datetime(3)");

                entity.Property(e => e.StartDate).HasColumnType("datetime(3)");

                entity.Property(e => e.State).HasMaxLength(191);

                entity.Property(e => e.Title).HasMaxLength(191);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime(3)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Milestones)
                    .HasPrincipalKey(p => p.GitLabGroupId)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Milestone_GroupId_fkey");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Milestones)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Milestone_ProjectId_fkey");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project", "SWP");

                entity.HasIndex(e => e.ClassId, "Project_ClassId_fkey");

                entity.HasIndex(e => e.MentorId, "Project_MentorId_fkey");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.EnglishName).HasMaxLength(191);

                entity.Property(e => e.GroupName).HasMaxLength(191);

                entity.Property(e => e.ProjectCode).HasMaxLength(191);

                entity.Property(e => e.Status)
                    .HasColumnType("enum('PENDING','ACTIVE','INACTIVE')")
                    .HasDefaultValueSql("'INACTIVE'");

                entity.Property(e => e.VietNameseName).HasMaxLength(191);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Projects)
                    .HasPrincipalKey(p => p.GitLabGroupId)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Project_ClassId_fkey");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Project_MentorId_fkey");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting", "SWP");

                entity.HasIndex(e => e.SettingId, "Setting_SettingId_idx");

                entity.HasIndex(e => new { e.Type, e.Value }, "Setting_Type_Value_idx");

                entity.HasIndex(e => new { e.Type, e.Value }, "Setting_Type_Value_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Type).HasMaxLength(191);

                entity.Property(e => e.Value).HasMaxLength(191);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject", "SWP");

                entity.HasIndex(e => e.Id, "Subject_Id_idx");

                entity.HasIndex(e => e.MentorId, "Subject_MentorId_fkey");

                entity.HasIndex(e => e.SubjectCode, "Subject_SubjectCode_idx");

                entity.HasIndex(e => e.SubjectCode, "Subject_SubjectCode_key")
                    .IsUnique();

                entity.Property(e => e.Description).HasColumnType("text");

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

            modelBuilder.Entity<SubjectSetting>(entity =>
            {
                entity.ToTable("SubjectSetting", "SWP");

                entity.HasIndex(e => e.Id, "SubjectSetting_Id_idx");

                entity.HasIndex(e => e.SubjectId, "SubjectSetting_SubjectId_fkey");

                entity.HasIndex(e => new { e.Type, e.Value }, "SubjectSetting_Type_Value_key")
                    .IsUnique();

                entity.Property(e => e.Name).HasColumnType("text");

                entity.Property(e => e.Type).HasMaxLength(191);

                entity.Property(e => e.Value).HasMaxLength(191);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.SubjectSettings)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("SubjectSetting_SubjectId_fkey");
            });

            modelBuilder.Entity<Submit>(entity =>
            {
                entity.ToTable("Submit", "SWP");

                entity.HasIndex(e => e.ClassAssignmentId, "Submit_ClassAssignmentId_fkey");

                entity.HasIndex(e => e.ProjectId, "Submit_projectId_fkey");

                entity.Property(e => e.ClassAssignmentId).HasMaxLength(191);

                entity.Property(e => e.Comment).HasColumnType("text");

                entity.Property(e => e.CommentTime).HasColumnType("datetime(3)");

                entity.Property(e => e.FileUrl).HasMaxLength(191);

                entity.Property(e => e.Grade).HasPrecision(65, 30);

                entity.Property(e => e.ProjectId).HasColumnName("projectId");

                entity.Property(e => e.StudentComment).HasColumnType("text");

                entity.Property(e => e.StudentCommentTime).HasColumnType("datetime(3)");

                entity.Property(e => e.SubmitTime).HasColumnType("datetime(3)");

                entity.HasOne(d => d.ClassAssignment)
                    .WithMany(p => p.Submits)
                    .HasForeignKey(d => d.ClassAssignmentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Submit_ClassAssignmentId_fkey");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Submits)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Submit_projectId_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "SWP");

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

                entity.Property(e => e.Description).HasColumnType("text");

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
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

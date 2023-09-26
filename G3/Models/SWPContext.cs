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

        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<SubjectSetting> SubjectSettings { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("server=localhost;uid=root;pwd=123456789;database=SWP");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting", "SWP");

                entity.HasIndex(e => e.SettingId, "Setting_SettingId_idx");

                entity.HasIndex(e => new { e.Type, e.Value }, "Setting_Type_Value_idx");

                entity.HasIndex(e => new { e.Type, e.Value }, "Setting_Type_Value_key")
                    .IsUnique();

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

                entity.HasIndex(e => e.MentorId, "Subject_MentorId_fkey");

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.SubjectCode).HasMaxLength(191);

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.MentorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Subject_MentorId_fkey");
            });

            modelBuilder.Entity<SubjectSetting>(entity =>
            {
                entity.ToTable("SubjectSetting", "SWP");

                entity.HasIndex(e => e.SubjectId, "SubjectSetting_subjectId_fkey");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.SubjectId).HasColumnName("subjectId");

                entity.Property(e => e.Type).HasMaxLength(191);

                entity.Property(e => e.Value).HasMaxLength(191);

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.SubjectSettings)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("SubjectSetting_subjectId_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "SWP");

                entity.HasIndex(e => e.DomainSettingId, "User_DomainSettingId_fkey");

                entity.HasIndex(e => e.Email, "User_Email_idx");

                entity.HasIndex(e => e.Email, "User_Email_key")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "User_Id_idx");

                entity.HasIndex(e => e.RoleSettingId, "User_RoleSettingId_fkey");

                entity.Property(e => e.Address).HasMaxLength(191);

                entity.Property(e => e.Avatar).HasMaxLength(191);

                entity.Property(e => e.ConfirmToken).HasMaxLength(191);

                entity.Property(e => e.ConfirmTokenVerifyAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime(3)")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP(3)'");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime(3)");

                entity.Property(e => e.Email).HasMaxLength(191);

                entity.Property(e => e.Hash).HasMaxLength(191);

                entity.Property(e => e.Name).HasMaxLength(191);

                entity.Property(e => e.Phone).HasMaxLength(15);

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

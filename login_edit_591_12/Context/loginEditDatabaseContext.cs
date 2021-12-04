using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace login_edit_591_12
{
    public partial class loginEditDatabaseContext : DbContext
    {
        public loginEditDatabaseContext()
        {
        }

        public loginEditDatabaseContext(DbContextOptions<loginEditDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<GenderInfo> GenderInfos { get; set; }
        public virtual DbSet<QuestionInfo> QuestionInfos { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-IBE7AS5\\SQLEXPRESS;Database=loginEditDatabase;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<GenderInfo>(entity =>
            {
                entity.HasKey(e => e.IdGender)
                    .HasName("PK__gender_i__E527768D408E0024");

                entity.ToTable("gender_info");

                entity.Property(e => e.IdGender)
                    .ValueGeneratedNever()
                    .HasColumnName("Id_Gender");

                entity.Property(e => e.GenderValue)
                    .IsRequired()
                    .HasMaxLength(1)
                    .HasColumnName("Gender_Value")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<QuestionInfo>(entity =>
            {
                entity.HasKey(e => e.IdQuestion)
                    .HasName("PK__question__B4902467D43AD875");

                entity.ToTable("question_info");

                entity.Property(e => e.IdQuestion)
                    .ValueGeneratedNever()
                    .HasColumnName("Id_Question");

                entity.Property(e => e.QuestionValue)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("Question_Value");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("PK__user_inf__D03DEDCB8228D71F");

                entity.ToTable("user_info");

                entity.HasIndex(e => e.Phone, "UQ__user_inf__5C7E359E5B47D3DC")
                    .IsUnique();

                entity.HasIndex(e => e.Nickname, "UQ__user_inf__CC6CD17EA60BC225")
                    .IsUnique();

                entity.Property(e => e.IdUser).HasColumnName("Id_User");

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.City).HasMaxLength(40);

                entity.Property(e => e.Country).HasMaxLength(40);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(30)
                    .HasColumnName("First_Name");

                entity.Property(e => e.IdGender).HasColumnName("Id_Gender");

                entity.Property(e => e.IdQuestion).HasColumnName("Id_Question");

                entity.Property(e => e.Nickname)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.SecondName)
                    .HasMaxLength(30)
                    .HasColumnName("Second_Name");

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("User_Password");

                entity.HasOne(d => d.IdGenderNavigation)
                    .WithMany(p => p.UserInfos)
                    .HasForeignKey(d => d.IdGender)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_user_info_gender_info");

                entity.HasOne(d => d.IdQuestionNavigation)
                    .WithMany(p => p.UserInfos)
                    .HasForeignKey(d => d.IdQuestion)
                    .HasConstraintName("FK_user_info_question_info");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

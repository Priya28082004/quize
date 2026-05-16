using Microsoft.EntityFrameworkCore;
using QuizApp.API.Models;

namespace QuizApp.API.Data;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<UserAnswer> UserAnswers => Set<UserAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.Email).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasDefaultValue("User");
        });

        // Quiz
        modelBuilder.Entity<Quiz>(e =>
        {
            e.HasKey(q => q.Id);
            e.Property(q => q.Title).HasMaxLength(200).IsRequired();
            e.Property(q => q.Category).HasMaxLength(100).HasDefaultValue("General");
        });

        // Question
        modelBuilder.Entity<Question>(e =>
        {
            e.HasKey(q => q.Id);
            e.Property(q => q.Text).IsRequired();
            e.HasOne(q => q.Quiz)
             .WithMany(quiz => quiz.Questions)
             .HasForeignKey(q => q.QuizId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Option
        modelBuilder.Entity<Option>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Text).IsRequired();
            e.HasOne(o => o.Question)
             .WithMany(q => q.Options)
             .HasForeignKey(o => o.QuestionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // QuizAttempt
        modelBuilder.Entity<QuizAttempt>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.User)
             .WithMany(u => u.Attempts)
             .HasForeignKey(a => a.UserId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Quiz)
             .WithMany(q => q.Attempts)
             .HasForeignKey(a => a.QuizId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // UserAnswer
        modelBuilder.Entity<UserAnswer>(e =>
        {
            e.HasKey(ua => ua.Id);
            e.HasOne(ua => ua.Attempt)
             .WithMany(a => a.UserAnswers)
             .HasForeignKey(ua => ua.AttemptId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ua => ua.Question)
             .WithMany(q => q.UserAnswers)
             .HasForeignKey(ua => ua.QuestionId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(ua => ua.SelectedOption)
             .WithMany()
             .HasForeignKey(ua => ua.SelectedOptionId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        });

        // Seed admin user (password: Admin@123)
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@quizapp.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed sample quiz
        modelBuilder.Entity<Quiz>().HasData(new Quiz
        {
            Id = 1,
            Title = "General Knowledge",
            Description = "Test your general knowledge with these 6 questions!",
            Category = "General",
            TimeLimitSeconds = 120,
            IsActive = true,
            CreatedBy = 1,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed questions
        modelBuilder.Entity<Question>().HasData(
            new Question { Id = 1, QuizId = 1, Text = "What is the capital of France?", Order = 1, Points = 1 },
            new Question { Id = 2, QuizId = 1, Text = "Which planet is known as the Red Planet?", Order = 2, Points = 1 },
            new Question { Id = 3, QuizId = 1, Text = "How many sides does a hexagon have?", Order = 3, Points = 1 },
            new Question { Id = 4, QuizId = 1, Text = "Who painted the Mona Lisa?", Order = 4, Points = 1 },
            new Question { Id = 5, QuizId = 1, Text = "What is the chemical symbol for water?", Order = 5, Points = 1 },
            new Question { Id = 6, QuizId = 1, Text = "What is the capital of India?", Order = 6, Points = 1 }
        );

        // Seed options
        modelBuilder.Entity<Option>().HasData(
            // Q1
            new Option { Id = 1, QuestionId = 1, Text = "London", IsCorrect = false },
            new Option { Id = 2, QuestionId = 1, Text = "Paris", IsCorrect = true },
            new Option { Id = 3, QuestionId = 1, Text = "Berlin", IsCorrect = false },
            new Option { Id = 4, QuestionId = 1, Text = "Rome", IsCorrect = false },
            // Q2
            new Option { Id = 5, QuestionId = 2, Text = "Venus", IsCorrect = false },
            new Option { Id = 6, QuestionId = 2, Text = "Jupiter", IsCorrect = false },
            new Option { Id = 7, QuestionId = 2, Text = "Mars", IsCorrect = true },
            new Option { Id = 8, QuestionId = 2, Text = "Saturn", IsCorrect = false },
            // Q3
            new Option { Id = 9, QuestionId = 3, Text = "5", IsCorrect = false },
            new Option { Id = 10, QuestionId = 3, Text = "6", IsCorrect = true },
            new Option { Id = 11, QuestionId = 3, Text = "7", IsCorrect = false },
            new Option { Id = 12, QuestionId = 3, Text = "8", IsCorrect = false },
            // Q4
            new Option { Id = 13, QuestionId = 4, Text = "Van Gogh", IsCorrect = false },
            new Option { Id = 14, QuestionId = 4, Text = "Picasso", IsCorrect = false },
            new Option { Id = 15, QuestionId = 4, Text = "Da Vinci", IsCorrect = true },
            new Option { Id = 16, QuestionId = 4, Text = "Rembrandt", IsCorrect = false },
            // Q5
            new Option { Id = 17, QuestionId = 5, Text = "O2", IsCorrect = false },
            new Option { Id = 18, QuestionId = 5, Text = "CO2", IsCorrect = false },
            new Option { Id = 19, QuestionId = 5, Text = "H2O", IsCorrect = true },
            new Option { Id = 20, QuestionId = 5, Text = "NaCl", IsCorrect = false },
            // Q6
            new Option { Id = 21, QuestionId = 6, Text = "Mumbai", IsCorrect = false },
            new Option { Id = 22, QuestionId = 6, Text = "New Delhi", IsCorrect = true },
            new Option { Id = 23, QuestionId = 6, Text = "Kolkata", IsCorrect = false },
            new Option { Id = 24, QuestionId = 6, Text = "Chennai", IsCorrect = false }
        );
    }
}

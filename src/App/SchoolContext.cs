using App.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    public sealed class SchoolContext : DbContext
    {
        private static readonly Type[] EnumerationTypes = { typeof(Suffix), typeof(Course) };

        private readonly string _connectionString;
        private readonly bool _useConsoleLogger;
        private readonly EventDispacher _eventDispacher;

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        public SchoolContext(string connectionString, bool useConsoleLogger, EventDispacher eventDispacher)
        {
            _connectionString = connectionString;
            _useConsoleLogger = useConsoleLogger;
            _eventDispacher = eventDispacher;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                    .AddConsole();
            });

            optionsBuilder
                .UseSqlServer(_connectionString)
                .UseLazyLoadingProxies();

            if (_useConsoleLogger)
            {
                optionsBuilder
                    .UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging();
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(x =>
            {
                x.ToTable("Student").HasKey(k => k.Id);
                x.Property(p => p.Id).HasColumnName("StudentID");
                x.Property(p => p.Email)
                    //convertion to and from value objectS
                    .HasConversion(p => p.Value, p => Email.Create(p).Value);

                //x.Property(p => p.Name);
                //them mapping multiple culumns to single Value object
                x.OwnsOne(p => p.Name, p =>
                  {
                      p.Property<long?>("NameSufixID").HasColumnName("NameSuffixID"); //shadow property
                      p.Property(pp => pp.First).HasColumnName("FirstName");
                      p.Property(pp => pp.Last).HasColumnName("LastName");
                      p.HasOne(pp => pp.Suffix).WithMany().HasForeignKey("NameSufixID").IsRequired(false);
                  });

                x.HasOne(p => p.FavoriteCourse).WithMany();
                x.HasMany(p => p.Enrollments).WithOne(p => p.Student)
                    .OnDelete(DeleteBehavior.Cascade)
                    //this is allready done by EF core(automaticly binded to backing field)
                    //it is just to demonstrate how it is done explisitly in case i need
                    .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
            });
            modelBuilder.Entity<Course>(x =>
            {
                x.ToTable("Course").HasKey(k => k.Id);
                x.Property(p => p.Id).HasColumnName("CourseID");
                x.Property(p => p.Name)
                    //for all props one of the way to ignore ceshed objects as entity state modified but better overide savechanges
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<Suffix>(x =>
            {
                x.ToTable("Suffix").HasKey(k => k.Id);
                x.Property(p => p.Id).HasColumnName("SuffixID");
                x.Property(p => p.Name);
            });

            modelBuilder.Entity<Enrollment>(x =>
            {
                x.ToTable("Enrollment").HasKey(k => k.Id);
                x.Property(p => p.Id).HasColumnName("EnrollmentID");
                x.Property(p => p.Grade);
                x.HasOne(x => x.Student).WithMany(p => p.Enrollments);
                x.HasOne(x => x.Course).WithMany();
            });
        }

        public override int SaveChanges()
        {
            IEnumerable<EntityEntry> enumerationEntries = ChangeTracker.Entries()
                .Where(x => EnumerationTypes.Contains(x.Entity.GetType()));

            foreach (var enumerationEntry in enumerationEntries)
            {
                enumerationEntry.State = EntityState.Unchanged;
            }

            var entities = ChangeTracker.Entries()
                 .Where(x => x.Entity is Entity)
                 .Select(x => (Entity)x.Entity)
                 .ToList();

            var result = base.SaveChanges();

            foreach (Entity entity in entities)
            {
                _eventDispacher.Dispatch(entity.DomainEvents);
                entity.ClearDomainEvents();
            }

            return result;
        }
    }
}

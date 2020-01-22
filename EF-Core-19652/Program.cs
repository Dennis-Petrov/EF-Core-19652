using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EF_Core_19652
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var (documentId, eventId) = await SeedAsync();
                var document = await FetchDocumentAsync(documentId);

                await TestAsync(document, eventId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static async Task<(Guid, Guid)> SeedAsync()
        {
            using (var context = CreateContext())
            {
                var documentId = Guid.NewGuid();
                var eventId = Guid.NewGuid();

                context.Documents.Add(new Document
                {
                    Id = documentId,
                    Number = $"Invoice #{documentId}",
                    Events =
                    {
                        new DocumentEvent
                        {
                            Id = eventId,
                            StatusCode = "S"
                        }
                    }
                });

                await context.SaveChangesAsync();

                return (documentId, eventId);
            }
        }

        static async Task<Document> FetchDocumentAsync(Guid documentId)
        {
            using (var context = CreateContext())
            {
                return await context.Documents
                    .SingleAsync(_ => _.Id == documentId);
            }
        }

        static async Task TestAsync(Document document, Guid eventId)
        {
            var comboStatus = new ComboStatus
            {
                DocumentId = document.Id,
                EventId = eventId
            };

            using (var context = CreateContext())
            {
                context.Documents.Attach(document);

                document.ComboStatuses.Add(comboStatus);

                // context.Entry(comboStatus).State = EntityState.Added;

                // This raises exception since comboStatus is in Detached state, if previous line is commented.
                await context.Entry(comboStatus)
                    .Reference(_ => _.Event)
                    .LoadAsync();

                Console.WriteLine($"Event {comboStatus.Event.Id} status code is {comboStatus.Event.StatusCode}");

                await context.SaveChangesAsync();
            }
        }

        static SampleContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SampleContext>()
                .UseSqlServer("Server=.;Database=EF-Core-19652;Trusted_Connection=True")
                .Options;

            return new SampleContext(options);
        }
    }

    #region Data model

    public sealed class Document
    {
        private IList<ComboStatus> comboStatuses;
        private IList<DocumentEvent> events;

        public Guid Id { get; set; }

        public string Number { get; set; }

        public IList<ComboStatus> ComboStatuses => comboStatuses ?? (comboStatuses = new List<ComboStatus>());

        public IList<DocumentEvent> Events => events ?? (events = new List<DocumentEvent>());
    }

    public sealed class DocumentEvent
    {
        private IList<ComboStatus> comboStatuses;

        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }

        public Document Document { get; set; }

        public string StatusCode { get; set; }

        public IList<ComboStatus> ComboStatuses => comboStatuses ?? (comboStatuses = new List<ComboStatus>());
    }

    public sealed class ComboStatus
    {
        public int Id { get; set; }

        public Guid DocumentId { get; set; }

        public Document Document { get; set; }

        public Guid EventId { get; set; }

        public DocumentEvent Event { get; set; }
    }

    #endregion

    #region Configuration

    public sealed class DocumentConfig : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.Id)
                .ValueGeneratedNever();

            builder.Property(_ => _.Number)
                .HasMaxLength(50)
                .IsRequired();

            builder.ToTable("Documents");
        }
    }

    public sealed class DocumentEventConfig : IEntityTypeConfiguration<DocumentEvent>
    {
        public void Configure(EntityTypeBuilder<DocumentEvent> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.Id)
                .ValueGeneratedNever();

            builder.Property(_ => _.DocumentId);

            builder.HasOne(_ => _.Document)
                .WithMany(_ => _.Events)
                .HasForeignKey(_ => _.DocumentId);

            builder.Property(_ => _.StatusCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.ToTable("DocumentEvents");
        }
    }

    public sealed class ComboStatusConfig : IEntityTypeConfiguration<ComboStatus>
    {
        public void Configure(EntityTypeBuilder<ComboStatus> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.Id)
                .ValueGeneratedOnAdd();

            builder.Property(_ => _.DocumentId);

            builder.HasOne(_ => _.Document)
                .WithMany(_ => _.ComboStatuses)
                .HasForeignKey(_ => _.DocumentId);

            builder.Property(_ => _.EventId);

            builder.HasOne(_ => _.Event)
                .WithMany(_ => _.ComboStatuses)
                .HasForeignKey(_ => _.EventId);

            builder.ToTable("ComboStatuses");
        }
    }

    #endregion

    #region DbContext

    public sealed class SampleContext : DbContext
    {
        public SampleContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }

        public DbSet<DocumentEvent> Events { get; set; }

        public DbSet<ComboStatus> ComboStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new DocumentConfig());
            modelBuilder.ApplyConfiguration(new DocumentEventConfig());
            modelBuilder.ApplyConfiguration(new ComboStatusConfig());
        }
    }

    #endregion

}

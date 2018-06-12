using System.Data.Entity;


namespace ASA.Core.Infrastructure
{
    public partial class ASAEntitiesContext : DbContext
    {
        public ASAEntitiesContext() : base("ASAEntities")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ASAEntitiesContext, Migrations.Configuration>("ASAEntities"));
            Database.SetInitializer<ASAEntitiesContext>(new DropCreateDatabaseIfModelChanges<ASAEntitiesContext>());

            // Database.SetInitializer<ASAEntitiesContext>(new CreateDatabaseIfNotExists<ASAEntitiesContext>());
            //Database.SetInitializer<ASAEntitiesContext>(new DropCreateDatabaseAlways<ASAEntitiesContext>());
            //Database.SetInitializer<ASAEntitiesContext>(new ASAEntitiesInitializer());
            base.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Business> Business { get; set; }
        public DbSet<BusinessAddress> BusinessAddress { get; set; }
        public DbSet<Sender> Sender { get; set; }
        public DbSet<PeriodData> PeriodData { get; set; }

        public DbSet<DataRequestStatus> DataRequestStatus { get; set; }
        public DbSet<HMRCResponse> HMRCResponse { get; set; }

        public DbSet<SubmissionError> SubmissionError { get; set; }
        public DbSet<VAT100> VAT100 { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientAddress> ClientAddress { get; set; }
        public DbSet<InvoiceItem> InvoiceItem { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetail { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        //public DbSet<ResponseDenormView> ResponseDenormView { get; set; }

        //public DbSet<SuccessResponseIRmarkReceipt> SuccessResponseIRmarkReceipt { get; set; }
        //public DbSet<VAT100> VAT101 { get; set; }

        protected void onModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Business>().ToTable("Business");
            modelBuilder.Entity<BusinessAddress>().ToTable("BusinessAddress");
            modelBuilder.Entity<PeriodData>().ToTable("PeriodInfo");
            modelBuilder.Entity<VAT100>().ToTable("VAT100Info");
            //modelBuilder.Entity<VAT101>().ToTable("VAT101Info");
            modelBuilder.Entity<Sender>().ToTable("ReturnsSenderInfo");
            modelBuilder.Entity<HMRCResponse>().ToTable("Response");
            modelBuilder.Entity<Client>().ToTable("Clients");
            modelBuilder.Entity<ClientAddress>().ToTable("ClientAddress");
            modelBuilder.Entity<InvoiceItem>().ToTable("InvoiceItems");
            modelBuilder.Entity<InvoiceDetail>().ToTable("InvoiceDetails");
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
                //.HasOptional(c=>c.Client).WithOptionalDependent()
                //.WillCascadeOnDelete(false);              
                
                //.HasMany(c => c.Client).WithRequired().WillCascadeOnDelete(false);       
            base.OnModelCreating(modelBuilder);
        }

    }
}

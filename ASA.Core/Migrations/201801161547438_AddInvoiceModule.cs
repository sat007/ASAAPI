namespace ASA.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInvoiceModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        InvoiceId = c.Guid(nullable: false),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InvoiceId)
                .ForeignKey("dbo.Clients", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.InvoiceDetails", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);

            CreateTable(
                "dbo.InvoiceDetails",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    No = c.String(),
                    IssueDate = c.DateTime(nullable: false),
                    DueDate = c.DateTime(nullable: false),
                    Ref = c.String(),
                    OrderNo = c.String(),
                    Discount = c.String(),
                    Note = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(),
                        Quantity = c.String(),
                        Price = c.String(),
                        VATRate = c.String(),
                        VAT = c.String(),
                        Total = c.String(),
                        SubTotal = c.String(),
                        Invoice_InvoiceId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.Invoice_InvoiceId)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceItems", "Invoice_InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "Id", "dbo.InvoiceDetails");
            DropForeignKey("dbo.Invoices", "ClientId", "dbo.Clients");

            DropIndex("dbo.InvoiceItems", new[] { "Invoice_InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "Id" });
            DropTable("dbo.InvoiceItems");
            DropTable("dbo.InvoiceDetails");
            DropTable("dbo.Invoices");
        }
    }
}

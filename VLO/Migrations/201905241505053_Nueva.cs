namespace VLO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nueva : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Facturas", "FechaFactura", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Facturas", "FechaFactura", c => c.String());
        }
    }
}

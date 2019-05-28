namespace VLO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fecha : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Prestamos", "FechaDev", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Prestamos", "FechaDev", c => c.DateTime(nullable: false));
        }
    }
}

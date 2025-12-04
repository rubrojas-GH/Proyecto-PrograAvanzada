namespace ProyectoPrograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductoTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Productoes",
                c => new
                    {
                        idProducto = c.Int(nullable: false, identity: true),
                        nombreProducto = c.String(),
                        precioProducto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        stock = c.Int(nullable: false),
                        estadoProducto = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.idProducto);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Productoes");
        }
    }
}

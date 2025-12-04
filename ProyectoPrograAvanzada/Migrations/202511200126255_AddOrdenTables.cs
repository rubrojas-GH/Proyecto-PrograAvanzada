namespace ProyectoPrograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrdenTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DETALLES_ORDEN",
                c => new
                    {
                        idOrden = c.Int(nullable: false),
                        idProducto = c.Int(nullable: false),
                        cantidad = c.Int(nullable: false),
                        precioUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        calcularSubtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.idOrden, t.idProducto })
                .ForeignKey("dbo.ORDENES", t => t.idOrden, cascadeDelete: true)
                .ForeignKey("dbo.Productoes", t => t.idProducto, cascadeDelete: true)
                .Index(t => t.idOrden)
                .Index(t => t.idProducto);
            
            CreateTable(
                "dbo.ORDENES",
                c => new
                    {
                        idOrden = c.Int(nullable: false, identity: true),
                        fecha = c.DateTime(nullable: false),
                        total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        idUsuario = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idOrden)
                .ForeignKey("dbo.Usuarios", t => t.idUsuario, cascadeDelete: true)
                .Index(t => t.idUsuario);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DETALLES_ORDEN", "idProducto", "dbo.Productoes");
            DropForeignKey("dbo.DETALLES_ORDEN", "idOrden", "dbo.ORDENES");
            DropForeignKey("dbo.ORDENES", "idUsuario", "dbo.Usuarios");
            DropIndex("dbo.ORDENES", new[] { "idUsuario" });
            DropIndex("dbo.DETALLES_ORDEN", new[] { "idProducto" });
            DropIndex("dbo.DETALLES_ORDEN", new[] { "idOrden" });
            DropTable("dbo.ORDENES");
            DropTable("dbo.DETALLES_ORDEN");
        }
    }
}

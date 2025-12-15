namespace MvcTienda.Infraestructura.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreation : DbMigration
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
                        subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
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
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        idUsuario = c.Int(nullable: false, identity: true),
                        nombre = c.String(nullable: false, maxLength: 100),
                        email = c.String(nullable: false, maxLength: 100),
                        contrasena = c.String(nullable: false, maxLength: 255),
                        estado = c.Boolean(nullable: false),
                        ultimaConexion = c.DateTime(nullable: false),
                        idRol = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idUsuario)
                .ForeignKey("dbo.Rols", t => t.idRol, cascadeDelete: true)
                .Index(t => t.idRol);
            
            CreateTable(
                "dbo.Resenas",
                c => new
                    {
                        idResena = c.Int(nullable: false, identity: true),
                        contenido = c.String(nullable: false, maxLength: 500),
                        calificacion = c.Int(nullable: false),
                        fecha = c.DateTime(nullable: false),
                        estado = c.String(nullable: false, maxLength: 20),
                        idUsuario = c.Int(nullable: false),
                        idProducto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idResena)
                .ForeignKey("dbo.Productoes", t => t.idProducto, cascadeDelete: true)
                .ForeignKey("dbo.Usuarios", t => t.idUsuario, cascadeDelete: true)
                .Index(t => t.idUsuario)
                .Index(t => t.idProducto);
            
            CreateTable(
                "dbo.Productoes",
                c => new
                    {
                        idProducto = c.Int(nullable: false, identity: true),
                        nombreProducto = c.String(nullable: false, maxLength: 100),
                        descripcion = c.String(maxLength: 500),
                        precioProducto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        stock = c.Int(nullable: false),
                        estadoProducto = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.idProducto);
            
            CreateTable(
                "dbo.ImagenProductoes",
                c => new
                    {
                        idImagen = c.Int(nullable: false, identity: true),
                        urlImagen = c.String(nullable: false, maxLength: 500),
                        idProducto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idImagen)
                .ForeignKey("dbo.Productoes", t => t.idProducto, cascadeDelete: true)
                .Index(t => t.idProducto);
            
            CreateTable(
                "dbo.Rols",
                c => new
                    {
                        idRol = c.Int(nullable: false, identity: true),
                        nombreRol = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.idRol);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DETALLES_ORDEN", "idProducto", "dbo.Productoes");
            DropForeignKey("dbo.DETALLES_ORDEN", "idOrden", "dbo.ORDENES");
            DropForeignKey("dbo.ORDENES", "idUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Usuarios", "idRol", "dbo.Rols");
            DropForeignKey("dbo.Resenas", "idUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Resenas", "idProducto", "dbo.Productoes");
            DropForeignKey("dbo.ImagenProductoes", "idProducto", "dbo.Productoes");
            DropIndex("dbo.ImagenProductoes", new[] { "idProducto" });
            DropIndex("dbo.Resenas", new[] { "idProducto" });
            DropIndex("dbo.Resenas", new[] { "idUsuario" });
            DropIndex("dbo.Usuarios", new[] { "idRol" });
            DropIndex("dbo.ORDENES", new[] { "idUsuario" });
            DropIndex("dbo.DETALLES_ORDEN", new[] { "idProducto" });
            DropIndex("dbo.DETALLES_ORDEN", new[] { "idOrden" });
            DropTable("dbo.Rols");
            DropTable("dbo.ImagenProductoes");
            DropTable("dbo.Productoes");
            DropTable("dbo.Resenas");
            DropTable("dbo.Usuarios");
            DropTable("dbo.ORDENES");
            DropTable("dbo.DETALLES_ORDEN");
        }
    }
}

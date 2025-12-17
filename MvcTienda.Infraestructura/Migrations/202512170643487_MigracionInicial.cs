namespace MvcTienda.Infraestructura.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigracionInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CATEGORIAS",
                c => new
                    {
                        idCategoria = c.Int(nullable: false, identity: true),
                        nombreCategoria = c.String(nullable: false, maxLength: 100),
                        descripcionCategoria = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.idCategoria);
            
            CreateTable(
                "dbo.PRODUCTOS",
                c => new
                    {
                        idProducto = c.Int(nullable: false, identity: true),
                        nombreProducto = c.String(nullable: false, maxLength: 100),
                        descripcion = c.String(maxLength: 500),
                        precioProducto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        stock = c.Int(nullable: false),
                        estadoProducto = c.Boolean(nullable: false),
                        idCategoria = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idProducto)
                .ForeignKey("dbo.CATEGORIAS", t => t.idCategoria, cascadeDelete: true)
                .Index(t => t.idCategoria);
            
            CreateTable(
                "dbo.IMAGENES_PRODUCTO",
                c => new
                    {
                        idImagen = c.Int(nullable: false, identity: true),
                        urlImagen = c.String(nullable: false, maxLength: 500),
                        idProducto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idImagen)
                .ForeignKey("dbo.PRODUCTOS", t => t.idProducto, cascadeDelete: true)
                .Index(t => t.idProducto);
            
            CreateTable(
                "dbo.RESENAS",
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
                .ForeignKey("dbo.PRODUCTOS", t => t.idProducto, cascadeDelete: true)
                .ForeignKey("dbo.USUARIOS", t => t.idUsuario, cascadeDelete: true)
                .Index(t => t.idUsuario)
                .Index(t => t.idProducto);
            
            CreateTable(
                "dbo.USUARIOS",
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
                .ForeignKey("dbo.ROLES", t => t.idRol, cascadeDelete: true)
                .Index(t => t.idRol);
            
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
                .ForeignKey("dbo.USUARIOS", t => t.idUsuario, cascadeDelete: true)
                .Index(t => t.idUsuario);
            
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
                .ForeignKey("dbo.PRODUCTOS", t => t.idProducto, cascadeDelete: true)
                .Index(t => t.idOrden)
                .Index(t => t.idProducto);
            
            CreateTable(
                "dbo.ROLES",
                c => new
                    {
                        idRol = c.Int(nullable: false, identity: true),
                        nombreRol = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.idRol);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RESENAS", "idUsuario", "dbo.USUARIOS");
            DropForeignKey("dbo.USUARIOS", "idRol", "dbo.ROLES");
            DropForeignKey("dbo.ORDENES", "idUsuario", "dbo.USUARIOS");
            DropForeignKey("dbo.DETALLES_ORDEN", "idProducto", "dbo.PRODUCTOS");
            DropForeignKey("dbo.DETALLES_ORDEN", "idOrden", "dbo.ORDENES");
            DropForeignKey("dbo.RESENAS", "idProducto", "dbo.PRODUCTOS");
            DropForeignKey("dbo.IMAGENES_PRODUCTO", "idProducto", "dbo.PRODUCTOS");
            DropForeignKey("dbo.PRODUCTOS", "idCategoria", "dbo.CATEGORIAS");
            DropIndex("dbo.DETALLES_ORDEN", new[] { "idProducto" });
            DropIndex("dbo.DETALLES_ORDEN", new[] { "idOrden" });
            DropIndex("dbo.ORDENES", new[] { "idUsuario" });
            DropIndex("dbo.USUARIOS", new[] { "idRol" });
            DropIndex("dbo.RESENAS", new[] { "idProducto" });
            DropIndex("dbo.RESENAS", new[] { "idUsuario" });
            DropIndex("dbo.IMAGENES_PRODUCTO", new[] { "idProducto" });
            DropIndex("dbo.PRODUCTOS", new[] { "idCategoria" });
            DropTable("dbo.ROLES");
            DropTable("dbo.DETALLES_ORDEN");
            DropTable("dbo.ORDENES");
            DropTable("dbo.USUARIOS");
            DropTable("dbo.RESENAS");
            DropTable("dbo.IMAGENES_PRODUCTO");
            DropTable("dbo.PRODUCTOS");
            DropTable("dbo.CATEGORIAS");
        }
    }
}

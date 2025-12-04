namespace ProyectoPrograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReseñaEImagenTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImagenProductoes",
                c => new
                    {
                        idimagen = c.Int(nullable: false, identity: true),
                        urlImagen = c.String(),
                        idProducto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idimagen)
                .ForeignKey("dbo.Productoes", t => t.idProducto, cascadeDelete: true)
                .Index(t => t.idProducto);
            
            CreateTable(
                "dbo.Reseña",
                c => new
                    {
                        idReseña = c.Int(nullable: false, identity: true),
                        contenido = c.String(),
                        fecha = c.DateTime(nullable: false),
                        estado = c.String(),
                        idUsuario = c.Int(nullable: false),
                        idProducto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idReseña)
                .ForeignKey("dbo.Productoes", t => t.idProducto, cascadeDelete: true)
                .ForeignKey("dbo.Usuarios", t => t.idUsuario, cascadeDelete: true)
                .Index(t => t.idUsuario)
                .Index(t => t.idProducto);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImagenProductoes", "idProducto", "dbo.Productoes");
            DropForeignKey("dbo.Reseña", "idUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Reseña", "idProducto", "dbo.Productoes");
            DropIndex("dbo.Reseña", new[] { "idProducto" });
            DropIndex("dbo.Reseña", new[] { "idUsuario" });
            DropIndex("dbo.ImagenProductoes", new[] { "idProducto" });
            DropTable("dbo.Reseña");
            DropTable("dbo.ImagenProductoes");
        }
    }
}

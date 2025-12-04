namespace ProyectoPrograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActualizarModelos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productoes", "descripcion", c => c.String(maxLength: 500));
            AddColumn("dbo.Reseña", "calificacion", c => c.Int(nullable: false));
            AlterColumn("dbo.Usuarios", "nombre", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Usuarios", "email", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Usuarios", "contrasena", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Rols", "nombreRol", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Productoes", "nombreProducto", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.ImagenProductoes", "urlImagen", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Reseña", "contenido", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Reseña", "estado", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reseña", "estado", c => c.String());
            AlterColumn("dbo.Reseña", "contenido", c => c.String());
            AlterColumn("dbo.ImagenProductoes", "urlImagen", c => c.String());
            AlterColumn("dbo.Productoes", "nombreProducto", c => c.String());
            AlterColumn("dbo.Rols", "nombreRol", c => c.String());
            AlterColumn("dbo.Usuarios", "contrasena", c => c.String());
            AlterColumn("dbo.Usuarios", "email", c => c.String());
            AlterColumn("dbo.Usuarios", "nombre", c => c.String());
            DropColumn("dbo.Reseña", "calificacion");
            DropColumn("dbo.Productoes", "descripcion");
        }
    }
}

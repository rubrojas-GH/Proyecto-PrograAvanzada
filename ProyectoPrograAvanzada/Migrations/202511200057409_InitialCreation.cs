namespace ProyectoPrograAvanzada.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rols",
                c => new
                    {
                        idRol = c.Int(nullable: false, identity: true),
                        nombreRol = c.String(),
                    })
                .PrimaryKey(t => t.idRol);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        idUsuario = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                        email = c.String(),
                        contrasena = c.String(),
                        estado = c.Boolean(nullable: false),
                        ultimaConexion = c.DateTime(nullable: false),
                        idRol = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idUsuario)
                .ForeignKey("dbo.Rols", t => t.idRol, cascadeDelete: true)
                .Index(t => t.idRol);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Usuarios", "idRol", "dbo.Rols");
            DropIndex("dbo.Usuarios", new[] { "idRol" });
            DropTable("dbo.Usuarios");
            DropTable("dbo.Rols");
        }
    }
}

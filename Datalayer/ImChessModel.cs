using Datalayer.Entities;
using System.Data.Entity;

namespace Datalayer
{
    public class ImChessModel : DbContext
    {
        private static ImChessModel _instance;

        // Your context has been configured to use a 'Model' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Datalayer.Model' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model' 
        // connection string in the application configuration file.
        public ImChessModel() : base("name=ImChessModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Engine> Engines { get; set; }
        public virtual DbSet<EngineOption> EngineOptions { get; set; }
        public virtual DbSet<GameHistory> Games { get; set; }
        public virtual DbSet<ApplicationSetting> ApplicationSettings { get; set; } 

        public static ImChessModel Instance { get { return _instance ?? (_instance = new ImChessModel()); } }
    }
}
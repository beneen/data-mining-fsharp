module DataMiningInFSharp.Database.Bootstrap

open DataMiningInFSharp.Database.Schema

open System.Data.SqlServerCe;
open System.Data.Linq
open System.IO

let bootstrapDatabase() = 
    if File.Exists Configuration.databaseFile then
        printfn "Database already exists at '%s'" Configuration.databaseFile
    else
        printfn "Creating database at '%s'" Configuration.databaseFile
        use database = new Schema(new SqlCeConnection(Configuration.databaseConnection))
        database.CreateDatabase()
module DataMiningInFSharp.Database.Main

open DataMiningInFSharp.Database.Bootstrap
open DataMiningInFSharp.Database.Importer

open System
open System.IO

[<EntryPoint>]
let main args =
    bootstrapDatabase()
    importTickers()

    printfn "Press the enter key to exit..."
    let unused = Console.ReadLine() // Keep console open
    // Return 0. This indicates success.
    0

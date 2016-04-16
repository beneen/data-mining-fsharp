module DataMiningInFSharp.Database.Main

open DataMiningInFSharp.Database.Bootstrap
open DataMiningInFSharp.Database.Importer

open System
open System.IO

[<EntryPoint>]
let main args =
    bootstrapDatabase()
    importTickers()

    printfn "Press any key to exit..."
    Console.Read() |> ignore
    0 // Return 0. This indicates success.

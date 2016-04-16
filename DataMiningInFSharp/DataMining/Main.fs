module DataMiningInFSharp.DataMining.Main

open FSharp.Data
open System
open DataMiningInFSharp.DataMining.AverageDifference

[<EntryPoint>]
let main argv =
   
    let diff = averagePriceDifference "EBAY" "MSFT"
    printfn "Average price difference was %f" diff

    printfn "Press any key to exit..."
    Console.Read() |> ignore
    0 // Return 0. This indicates success.

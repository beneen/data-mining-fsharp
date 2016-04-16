module DataMiningInFSharp.DataMining.Main

open FSharp.Data
open System
open DataMiningInFSharp.DataMining.AverageDifference

[<EntryPoint>] // this tells the F# compiler that this is the main method
let main argv =  // this is the standard main function declaration (a function called main with a single parameter argv)
   
    let diff = averagePriceDifference "EBAY" "MSFT"
    printfn "Average price difference was %f" diff

    // keep the window open
    printfn "Press the enter key to exit..."
    Console.Read() |> ignore
    0 // return an integer exit code

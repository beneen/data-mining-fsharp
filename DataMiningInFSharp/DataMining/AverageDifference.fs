module DataMiningInFSharp.DataMining.AverageDifference

open FSharp.Data
open System
open DataMiningInFSharp.DataMining.Configuration
open DataMiningInFSharp.DataMining.StockLoader

let startDate (file:StockHistory) =
    let firstRow = file.Entries |> Seq.head
    firstRow.Date

let validateStartDates (fileA:StockHistory) (fileB:StockHistory) =
    let startDateA = startDate fileA
    let startDateB = startDate fileB
    if startDateA <> startDateB then failwith ("Start dates for " + fileA.ToString() + " and " + fileB.ToString() + " do not match!")

let length (file:StockHistory) = file.Entries |> Seq.length

let minimumLength (fileA:StockHistory) (fileB:StockHistory) = min (length fileA) (length fileB)

let takeNPrices n (file:StockHistory) = 
    file.Entries 
        |> Seq.take n 
        |> Seq.map (fun row -> row.ClosingPrice)

let averagePriceDifference stockA stockB =
    let fileA = loadFile stockA
    let fileB = loadFile stockB
    validateStartDates fileA fileB
    let minimumLength = minimumLength fileA fileB
    let pricesA = takeNPrices minimumLength fileA
    let pricesB = takeNPrices minimumLength fileB
    Seq.zip pricesA pricesB 
        |> Seq.map (fun (priceA, priceB) -> priceA - priceB)
        |> Seq.average
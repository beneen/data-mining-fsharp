module DataMiningInFSharp.DataMining.AverageDifference

open FSharp.Data
open System
open DataMiningInFSharp.DataMining.Configuration

let startDate (file:CsvFile) =
    let firstRow = file.Rows |> Seq.head
    firstRow.GetColumn("Date")

let validateStartDates (fileA:CsvFile) (fileB:CsvFile) =
    let startDateA = startDate fileA
    let startDateB = startDate fileB
    if startDateA <> startDateB then failwith ("Start dates for " + fileA.ToString() + " and " + fileB.ToString() + " do not match!")

let loadFile stock = CsvFile.Load(Configuration.stockPath + stock + ".csv")

let length (file:CsvFile) = file.Rows |> Seq.length

let minimumLength (fileA:CsvFile) (fileB:CsvFile) = min (length fileA) (length fileB)

let takeNPrices n (file:CsvFile) = 
    file.Rows 
        |> Seq.take n 
        |> Seq.map (fun row -> row.GetColumn "Close")
        |> Seq.map Double.Parse

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
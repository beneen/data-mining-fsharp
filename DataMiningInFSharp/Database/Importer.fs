module DataMiningInFSharp.Database.Importer

open DataMiningInFSharp.Database.Schema

open System
open System.IO
open System.Data.SqlServerCe;
open System.Data.Linq
open System.Data.Linq.Mapping
open FSharp.Data

let priceHistoryEntry ticker (csvRow: CsvRow) = 
    let tradingDate = csvRow.GetColumn("Date") |> DateTime.Parse
    let closingPrice = csvRow.GetColumn("Close") |> Double.Parse
    PriceHistory(ticker, tradingDate, closingPrice)

let loadPriceHistory (ticker: Ticker) = 
    printfn "Loading history for ticker '%s'" ticker.TickerName 
    use file = CsvFile.Load(Configuration.tickerFile ticker.TickerName)
    file.Rows |> Seq.map (fun csvRow -> priceHistoryEntry ticker csvRow) |> Seq.toList

let tickerNames() = 
    DirectoryInfo(Configuration.tickerPath).GetFiles()
        |> Seq.map (fun tickerFile -> tickerFile.Name)
        |> Seq.map (fun tickerName -> Path.GetFileNameWithoutExtension tickerName)
        
let importTickers() =
    printfn "Importing tickers that were downloaded to'%s' into database '%s' ..." Configuration.tickerPath Configuration.databaseFile
    
    use database = new Schema(new SqlCeConnection(Configuration.databaseConnection))
    let tickers = database.GetTable<Ticker>()
    let priceHistory = database.GetTable<PriceHistory>()

    printfn "Importing ticker names..."
    tickerNames() |> Seq.map Ticker |> Seq.iter tickers.InsertOnSubmit
    database.SubmitChanges()

    printfn "Importing stock history..."
    let insertHistory history = priceHistory.InsertAllOnSubmit(history) ; database.SubmitChanges()
    tickers |> Seq.map loadPriceHistory |> Seq.iter insertHistory

    printfn "Finished importing"

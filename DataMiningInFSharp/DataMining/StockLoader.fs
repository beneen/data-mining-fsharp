module DataMiningInFSharp.DataMining.StockLoader

open System
open FSharp.Data
open NUnit.Framework
open FsUnit

type StockHistoryEntry(csvRow: CsvRow) = class
    member x.Date = csvRow.GetColumn("Date") |> DateTime.Parse
    member x.ClosingPrice:double = csvRow.GetColumn("Close") |> Double.Parse
end

type StockHistory(ticker: string, entries: seq<StockHistoryEntry>) = class
    member x.Entries = entries
    member x.Ticker = ticker
    member x.Length = x.Entries |> Seq.length
    member x.MinDate = (x.Entries |> Seq.last).Date
    member x.MaxDate = (x.Entries |> Seq.head).Date
    member x.ClampToWindow (firstDate: DateTime) (lastDate: DateTime) =
        let overlappingEntries = x.Entries |> Seq.filter (fun entry -> entry.Date >= firstDate && entry.Date <= lastDate)
        StockHistory(x.Ticker, overlappingEntries)

    member x.PriceDifferences =
        x.Entries 
            |> Seq.map (fun stock -> stock.ClosingPrice) 
            |> Seq.pairwise 
            |> Seq.map (fun (x, y) -> (y - x) / x)
            |> Seq.toArray

    static member minimumLength (stocks: seq<StockHistory>) = stocks|> Seq.map (fun stock -> stock.Length) |> Seq.min
end

let loadFile stock = 
    let file = CsvFile.Load(Configuration.stockPath + stock + ".csv")
    let entries = file.Rows |> Seq.map StockHistoryEntry
    StockHistory(stock, entries)

[<TestFixture>] 
type ``given a stock file that has been downloaded`` ()=
    [<Test>] member test.
     ``when I load the CSV`` ()=
            (loadFile "EBAY").Length |> should be (greaterThan 0)
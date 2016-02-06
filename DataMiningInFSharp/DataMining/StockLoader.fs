module DataMiningInFSharp.DataMining.StockLoader

open System
open FSharp.Data
open NUnit.Framework
open FsUnit

type StockHistoryEntry(csvRow: CsvRow) = class
    member x.Date = csvRow.GetColumn("Date") |> DateTime.Parse
    member x.ClosingPrice = csvRow.GetColumn("Close") |> Double.Parse
end

type StockHistory(ticker: string, csvFile: CsvFile) = class
    member x.Entries = csvFile.Rows |> Seq.map StockHistoryEntry
    member x.Ticker = ticker;
end

let loadFile stock = 
    let file = CsvFile.Load(Configuration.stockPath + stock + ".csv")
    StockHistory(stock, file)

[<TestFixture>] 
type ``given a stock file that has been downloaded`` ()=
    [<Test>] member test.
     ``when I load the CSV`` ()=
            (loadFile "EBAY").Entries |> Seq.length |> should be (greaterThan 0)
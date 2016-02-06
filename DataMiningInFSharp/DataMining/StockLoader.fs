module DataMiningInFSharp.DataMining.StockLoader

open FSharp.Data
open NUnit.Framework
open FsUnit

let loadFile stock = CsvFile.Load(Configuration.stockPath + stock + ".csv")

[<TestFixture>] 
type ``given a stock file that has been downloaded`` ()=
    [<Test>] member test.
     ``when I load the CSV`` ()=
            (loadFile "EBAY").Rows |> Seq.length |> should be (greaterThan 0)
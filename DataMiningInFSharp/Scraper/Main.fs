module DataMiningInFSharp.Scraper.Main

open System
open System.IO
open DataMiningInFSharp.Scraper.StockAnalyzer

[<EntryPoint>]
let main args =
    File.ReadLines(Configuration.indexesPath + "tickers.txt") |> Seq.iter Downloader.loadPrices 

    let analysers = StockAnalyzer.StockAnalyzer.GetAnalyzers([|"msft"; "orcl"; "ebay"|], 20)
    for analyser in analysers do
        Console.WriteLine("Ret:{0}\tStdDev:{1}", analyser.Return, analyser.StdDev)
    printfn "Arguments passed to function : %A" args
    let unused = Console.ReadLine()
        // Return 0. This indicates success.
    0

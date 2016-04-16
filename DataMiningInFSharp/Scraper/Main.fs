module DataMiningInFSharp.Scraper.Main

open System
open System.IO

[<EntryPoint>]
let main args =
    printfn "Scraping tickers that are listed in '%s'..." Configuration.indexesFile
    File.ReadLines(Configuration.indexesFile) 
        |> Seq.iter Downloader.loadPrices

    printfn "Finished scraping. Press any key to exit."
    let unused = Console.ReadLine() // Keep console open
    // Return 0. This indicates success.
    0

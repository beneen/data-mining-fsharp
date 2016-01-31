module DataMiningInFSharp.Scraper.StockAnalyzer

open System.Net
open System.IO
open FSharp.Charting
open Microsoft.FSharp.Control.WebExtensions
open DataMiningInFSharp.Scraper.Scraper

type StockAnalyzer (lprices, days) = 
    let prices  = 
        lprices 
        |> Seq.map snd // snd returns the second element of a tuple
        |> Seq.take days 
    static member GetAnalyzers (tickers, days) =  
        tickers
        |> Seq.map loadPrices
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Seq.map (fun prices -> new StockAnalyzer(prices, days))
    //factory method, you pass it a bunch of tickers and returns a bunch of stock analysers
     member s.Return = 
        let lastPrice = prices |> Seq.item 0
        let startPrice = prices |> Seq.item (days - 1)
        lastPrice / startPrice - 1.
     member s.StdDev = 
        let logRets = 
            prices
            |> Seq.pairwise 
            // pairwise takes a sequence of things, returns the 
            //first and the second and creates a tuple of those two, then the second and the third and creates another tuple
            // so that in the end you have a sequence that creates all the possible pairs   
            |> Seq.map (fun (x, y) -> log (x / y))
        let mean = logRets |> Seq.average
        let sqr x = x * x
        let var = logRets |> Seq.averageBy (fun r -> sqr (r - mean))
        sqrt var



    //DONE put these in different functions

    // prices is not a sequence of array of strings where each string is one price
    // fun is the way to define a function on the spot
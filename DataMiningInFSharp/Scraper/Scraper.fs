module DataMiningInFSharp.Scraper.Scraper

open System.Net
open System.IO
open FSharp.Charting
open Microsoft.FSharp.Control.WebExtensions
//f# is declarative, easily parralellisable



let internal loadPrices ticker = async {
    let url = "http://real-chart.finance.yahoo.com/table.csv?s=" + ticker + "&d=9&e=9&f=2015&g=d&a=2&b=13&c=1986&ignore=.csv"

    let req = WebRequest.Create(url)
    let! resp = req.AsyncGetResponse()
    // at this point in the code we are just keeping the thread, we aren't doing anything, we're waiting for the website to send us a response.
    // idea is to parallelise and asynchronous so that when we get to these points in the code they release the thread to the web pool
    //this is why we have used async up there ^ (let! and Async myMember)
    let stream = resp.GetResponseStream()
    let reader = new StreamReader(stream)
    let! csv = reader.AsyncReadToEnd() // installed F# powerpack but not opened it 

//parsing of the data to take out all the prices we need

    let prices = 
        csv.Split([|'\n'|])
        |> Seq.skip 1
        |> Seq.map(fun line -> line.Split([|','|]))
        |> Seq.filter(fun values -> values |> Seq.length = 7)
        |> Seq.map(fun values -> 
            System.DateTime.Parse(values.[0]), 
            float values.[6])
    return prices }


// stock analyser - this is just a class // use it as a library so we can use it with c#

//type StockAnalyzer (lprices, days) = 
//    let prices  = 
//        lprices 
//        |> Seq.map snd // snd returns the second element of a tuple
//        |> Seq.take days 
//    static member GetAnalyzers (tickers, days) =  
//        tickers
//        |> Seq.map loadPrices
//        |> Async.Parallel
//        |> Async.RunSynchronously
//        |> Seq.map (fun prices -> new StockAnalyzer(prices, days))
//    //factory method, you pass it a bunch of tickers and returns a bunch of stock analysers
//     member s.Return = 
//        let lastPrice = prices |> Seq.item 0
//        let startPrice = prices |> Seq.item (days - 1)
//        lastPrice / startPrice - 1.
//     member s.StdDev = 
//        let logRets = 
//            prices
//            |> Seq.pairwise 
//            // pairwise takes a sequence of things, returns the 
//            //first and the second and creates a tuple of those two, then the second and the third and creates another tuple
//            // so that in the end you have a sequence that creates all the possible pairs   
//            |> Seq.map (fun (x, y) -> log (x / y))
//        let mean = logRets |> Seq.average
//        let sqr x = x * x
//        let var = logRets |> Seq.averageBy (fun r -> sqr (r - mean))
//        sqrt var



    //put these in different functions

    // prices is not a sequence of array of strings where each string is one price
    // fun is the way to define a function on the spot
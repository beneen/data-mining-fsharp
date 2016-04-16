module DataMiningInFSharp.Scraper.Downloader

open System.Net
open System.IO
open NUnit.Framework
open FsUnit

let downloadTickerFile (ticker: string) (url: string) (file: string) =
    let webClient = new WebClient()
    try 
        webClient.DownloadFile(url, file)
    with
        | :? System.Net.WebException as ex when (ex.Response :?> HttpWebResponse).StatusCode = HttpStatusCode.NotFound -> printfn "Ticker '%s' not found on Yahoo" ticker
        | _ as ex -> printfn "Problem downloading ticker '%s' from Yahoo:\n%s" ticker (string ex)

let loadPrices ticker =
    let url = "http://real-chart.finance.yahoo.com/table.csv?s=" + ticker + "&d=9&e=9&f=2015&g=d&a=2&b=13&c=1986&ignore=.csv"
    let file = Configuration.tickerFile ticker
    if File.Exists(file) then 
        let dateDownloaded = (File.GetLastWriteTime file).ToString("dd-MM-yyyy")
        printfn "Already downloaded ticker '%s' on '%s'" ticker dateDownloaded
    else 
        downloadTickerFile ticker url file

[<TestFixture>] 
type ``given a ticker that does not exist on Yahoo`` ()=
    [<Test>] member test.
     ``when I try to download the CSV`` ()=
            loadPrices "badticker"; File.Exists (Configuration.basePath + "data/badticker.csv") |> should be False
    
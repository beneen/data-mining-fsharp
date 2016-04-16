module DataMiningInFSharp.Scraper.Configuration

open System.IO

let basePath = Path.GetFullPath "../../../../"
let dataPath = basePath + "data/"
let indexesPath = dataPath + "indexes/"
let indexesFile = indexesPath + "tickers.txt"
let tickerPath = dataPath + "stocks/"
let tickerFile ticker = tickerPath + ticker + ".csv" 

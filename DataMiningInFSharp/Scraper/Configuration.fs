module DataMiningInFSharp.Scraper.Configuration

let basePath = "../../../../"
let dataPath = basePath + "data/"
let indexesPath = dataPath + "indexes/"
let indexesFile = indexesPath + "tickers.txt"
let tickerPath = dataPath + "stocks/"
let tickerFile ticker = tickerPath + ticker + ".csv" 

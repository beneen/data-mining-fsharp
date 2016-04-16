module DataMiningInFSharp.Database.Configuration

open System.IO

let basePath = Path.GetFullPath "../../../../"
let dataPath = basePath + "data/"
let tickerPath = dataPath + "stocks/"
let tickerFile ticker = tickerPath + ticker + ".csv" 
let databasePath = dataPath + "database/"
let databaseFile = databasePath + "tickers.sdf"
let databaseConnection = "Data Source=" + databaseFile + ";Persist Security Info=False;"
module DataMiningInFSharp.DataMining.DataSetConverter

open Accord.MachineLearning
open System.Data
open Accord.Math
open DataMiningInFSharp.DataMining.StockLoader
open NUnit.Framework
open FsUnit

type DataSet(trainingSet: float[][], classifications: int[]) = class
    member x.TrainingSet = trainingSet
    member x.Classifications = classifications
end

let entriesOverlapping (targetStockHistory: StockHistory) (otherStockHistories: seq<StockHistory>) =
    let firstDate = targetStockHistory.MinDate
    let lastDate = targetStockHistory.MaxDate
    otherStockHistories 
        |> Seq.map (fun stockHistory -> stockHistory.ClampToWindow firstDate lastDate)
        |> Seq.filter (fun stockHistory -> stockHistory.Length = targetStockHistory.Length)

let convertToDataSet (targetStockHistory: StockHistory) (otherStockHistories: seq<StockHistory>) =
    let entriesOverlapping = entriesOverlapping targetStockHistory otherStockHistories
    let otherStockPriceDifferences = entriesOverlapping |> Seq.map (fun stockHistory -> stockHistory.PriceDifferences) |> Seq.toArray
    let trainingSet = Matrix.Transpose<float>(otherStockPriceDifferences)
    let classifications = targetStockHistory.PriceDifferences |> Seq.map (fun difference -> if difference >= 0.0 then 1 else 0)|> Seq.toArray
    DataSet(trainingSet, classifications)

[<TestFixture>] 
type ``given stock files that have been downloaded`` ()=
    let targetStock = loadFile "EBAY"
    let otherStocks = [| "MSFT"; "JPM" |] |> Seq.map loadFile
    [<Test>] member test.
     ``when I convert to a data set`` ()=
            (convertToDataSet targetStock otherStocks).TrainingSet.Length |> should be (greaterThan 0)
module DataMiningInFSharp.DataMining.DataSetConverter

open Accord.MachineLearning
open Accord.Statistics.Analysis
open System.Data
open Accord.Math
open DataMiningInFSharp.DataMining.StockLoader
open NUnit.Framework
open FsUnit

type DataPoint = double[]
type Classification = int
type Classifier = DataPoint -> Classification

type DataSet(dataPoints: DataPoint[], classifications: Classification[]) = class
    member x.DataPoints = dataPoints
    member x.Classifications = classifications

    member x.SubSet (indices: int[]): DataSet =
        DataSet(x.DataPoints.Submatrix(indices), x.Classifications.Submatrix(indices))

    member x.Predict (classifier: Classifier): ConfusionMatrix =
        let predictions = x.DataPoints |> Seq.map classifier |> Seq.toArray
        ConfusionMatrix(predictions, x.Classifications)
end

let entriesOverlapping (targetStockHistory: StockHistory) (otherStockHistories: List<StockHistory>) =
    let predictForward = 1.0
    let firstDate = targetStockHistory.MinDate.AddDays(-predictForward)
    let lastDate = targetStockHistory.MaxDate.AddDays(-predictForward)
    otherStockHistories 
        |> Seq.map (fun stockHistory -> stockHistory.ClampToWindow firstDate lastDate)
        |> Seq.filter (fun stockHistory -> stockHistory.Length = targetStockHistory.Length)

let convertToDataSet (targetStockHistory: StockHistory) (otherStockHistories: List<StockHistory>) =
    let entriesOverlapping = entriesOverlapping targetStockHistory otherStockHistories
    let otherStockPriceDifferences = entriesOverlapping |> Seq.map (fun stockHistory -> stockHistory.PriceDifferences) |> Seq.toArray
    let dataPoints = Matrix.Transpose<double>(otherStockPriceDifferences)
    let classifications = targetStockHistory.PriceDifferences |> Seq.map (fun difference -> if difference >= 0.0 then 1 else 0)|> Seq.toArray
    DataSet(dataPoints, classifications)

[<TestFixture>] 
type ``given stock files that have been downloaded`` ()=
    let targetStock = loadFile "EBAY"
    let otherStocks = [| "MSFT"; "JPM" |] |> Seq.map loadFile |> Seq.toList
    [<Test>] member test.
     ``when I convert to a data set`` ()=
            (convertToDataSet targetStock otherStocks).DataPoints.Length |> should be (greaterThan 0)
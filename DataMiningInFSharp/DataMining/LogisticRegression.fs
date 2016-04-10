module DataMiningInFSharp.DataMining.LogisticRegression

open DataMiningInFSharp.DataMining.StockLoader
open DataMiningInFSharp.DataMining.CrossValidation
open DataMiningInFSharp.DataMining.DataSetConverter

open Accord.Statistics.Models.Regression.Fitting
open Accord.Statistics.Models.Regression

open System.IO
open NUnit.Framework
open FsUnit

let tolerance = 0.001

let rec iterate f = seq { 
    yield f()
    yield! iterate f 
}

let trainLogisticRegression (dataSet: DataSet) (logisticRegression: LogisticRegression): Unit = 
    let teacher = IterativeReweightedLeastSquares(logisticRegression)
    let runIteration = fun delta -> teacher.Run(dataSet.DataPoints, dataSet.Classifications)
    let finalDelta = iterate runIteration |> Seq.takeWhile (fun delta -> delta > tolerance) |> Seq.last
    printfn "Final delta: %f" finalDelta
    ()

let logisticRegressionFactory (dataSet: DataSet): Classifier =
    let numberOfVariables = dataSet.DataPoints.[0].Length
    let logisticRegression = LogisticRegression(numberOfVariables)
    trainLogisticRegression dataSet logisticRegression

    let probabilityToClassification probability = if probability > 0.5 then 1 else 0
    logisticRegression.Compute >> probabilityToClassification

[<TestFixture>] 
type ``given stock files that have been downloaded`` ()=
    let targetStock = loadFile "EBAY"
    let otherStocks = [| "MSFT"; "JPM"; "GS"; "GOOGL" |] |> Seq.map loadFile
   // let otherStocks = 
   //     DirectoryInfo(Configuration.stockPath).GetFiles() 
   //         |> Seq.map (fun file -> file.Name.Substring(0, file.Name.Length - 4))
   //         |> Seq.map loadFile
   //         |> Seq.take 10

    let dataSet = convertToDataSet targetStock otherStocks
    [<Test>] member test.
     ``when I run the Logistic Regression classifier`` ()=
            performCrossValidation dataSet logisticRegressionFactory |> should equal 0.5
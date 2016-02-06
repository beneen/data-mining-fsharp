module DataMiningInFSharp.DataMining.KNN
// input is the training set of examples where the examples have a certain number of columns or features. 
// for that example we have the classification. 
// given a new example it will tell us the new classification based on its best guess from the training set.

// in our case - is the stock gonna go up or down - one stock only. looking at price changes over the past n days.
// each example is a vector of the price changes of the stocks over the past n days. 
// we can have as many examples as we have days of data to work with
// if we have 20 days worth of data and we are looking back 2 days then we have 18 possible data points
// the classification is whether the target stock has goes up or down after the next day.

// to begin with pick 10 stocks and choose one to be the target stock 
open DataMiningInFSharp.DataMining.Configuration
open DataMiningInFSharp.DataMining.StockLoader
open DataMiningInFSharp.DataMining.DataSetConverter
open Accord.MachineLearning
open Accord.Math
open Accord.Statistics.Analysis

open System.IO
open NUnit.Framework
open FsUnit

type Accord.MachineLearning.CrossValidationResult<'TModel when 'TModel : not struct> with
    member this.ConfusionMatrices = this.Models |> Seq.map (fun model -> model.Tag :?> ConfusionMatrix)

let doKnn (dataSet: DataSet) =
    let crossValidation = CrossValidation<KNearestNeighbors>(dataSet.TrainingSet.Length)
    let fitting (k:int) (indicesTrain: int[]) (indicesValidation: int[]) = 
        let trainingInputs = dataSet.TrainingSet.Submatrix(indicesTrain)
        let trainingOutputs = dataSet.Classifications.Submatrix(indicesTrain)

        let validationInputs = dataSet.TrainingSet.Submatrix(indicesValidation)
        let validationOutputs = dataSet.Classifications.Submatrix(indicesValidation)

        let knn = KNearestNeighbors(11, trainingInputs, trainingOutputs)

        let trainingPredictions = trainingInputs |> Seq.map (fun input -> knn.Compute input) |> Seq.toArray
        let trainingConfusion = ConfusionMatrix(trainingPredictions, trainingOutputs)

        let validationPredictions = validationInputs |> Seq.map (fun input -> knn.Compute input) |> Seq.toArray
        let validationConfusion = ConfusionMatrix(validationPredictions, validationOutputs)
        
        let result = CrossValidationValues<KNearestNeighbors>(knn, trainingConfusion.Accuracy, validationConfusion.Accuracy)
        result.Tag <- validationConfusion
        result

    crossValidation.Fitting <- CrossValidationFittingFunction(fitting)

    let result = crossValidation.Compute()
    printfn "Average training accuracy: %f" result.Training.Mean
    printfn "Average validation accuracy: %f" result.Validation.Mean
    result.ConfusionMatrices
    
[<TestFixture>] 
type ``given stock files that have been downloaded`` ()=
    let targetStock = loadFile "EBAY"
    let otherStocks = [| "MSFT"; "JPM"; "GS"; "GOOGL" |] |> Seq.map loadFile
    //let otherStocks = DirectoryInfo(Configuration.stockPath).GetFiles() |> Seq.map (fun file -> file.Name.Substring(0, file.Name.Length - 4)) |> Seq.map loadFile
    let dataSet = convertToDataSet targetStock otherStocks
    [<Test>] member test.
     ``when I convert to a data set`` ()=
            doKnn dataSet |> should equal 0.5
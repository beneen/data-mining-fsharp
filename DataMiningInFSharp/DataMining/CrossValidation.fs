module DataMiningInFSharp.DataMining.CrossValidation

open DataMiningInFSharp.DataMining.Configuration
open DataMiningInFSharp.DataMining.StockLoader
open DataMiningInFSharp.DataMining.DataSetConverter
open Accord.MachineLearning
open Accord.Math
open Accord.Statistics.Analysis

open System.IO
open NUnit.Framework
open FsUnit

let numberOfFolds = 10

type Accord.MachineLearning.CrossValidationResult<'TModel when 'TModel : not struct> with
    member this.ConfusionMatrices = this.Models |> Seq.map (fun model -> model.Tag :?> ConfusionMatrix)

let performCrossValidation (dataSet: DataSet) (classifierFactory: DataSet -> Classifier) =
    let crossValidation = CrossValidation<Classifier>(dataSet.DataPoints.Length, numberOfFolds)
    let fitting _ (indicesTrain: int[]) (indicesValidation: int[]) = 
        let trainingSet = dataSet.SubSet(indicesTrain)
        let validationSet = dataSet.SubSet(indicesValidation)

        let classifier = classifierFactory(trainingSet)
        let trainingConfusion = trainingSet.Predict(classifier)
        let validationConfusion = validationSet.Predict(classifier)
        
        let result = CrossValidationValues<Classifier>(classifier, trainingConfusion.Accuracy, validationConfusion.Accuracy)
        result.Tag <- validationConfusion
        result

    crossValidation.Fitting <- CrossValidationFittingFunction(fitting)

    let result = crossValidation.Compute()
    printfn "Average training accuracy: %f" result.Training.Mean
    printfn "Average validation accuracy: %f" result.Validation.Mean
    result.ConfusionMatrices

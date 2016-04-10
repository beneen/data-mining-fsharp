module DataMiningInFSharp.DataMining.NaiveBayes

open DataMiningInFSharp.DataMining.StockLoader
open DataMiningInFSharp.DataMining.CrossValidation
open DataMiningInFSharp.DataMining.DataSetConverter

open Accord.Math
open Accord.MachineLearning.Bayes

open System.IO
open NUnit.Framework
open FsUnit

let numberOfBins = 10

type BinLocation = double
type BinLocations = BinLocation[]
type BinnedDataPoint = int[]

type FeatureData = double[]

let computeBinLocation (sortedFeatureData: FeatureData) (binIndex: int): BinLocation = 
    let percentileWidth = sortedFeatureData.Length / numberOfBins
    let locationIndex = (binIndex + 1) * percentileWidth - 1
    sortedFeatureData.[locationIndex]

let computeBinLocations (featureData: FeatureData): BinLocations =
    let sortedFeatureData = Array.sort featureData
    let computeBinLocationUsingSortedFeatureData = computeBinLocation sortedFeatureData
    [| 0 .. numberOfBins - 2|] |> Seq.map computeBinLocationUsingSortedFeatureData |> Seq.toArray

let binDataPointFeature (binLocations: BinLocations) (feature: double): int =
    let binIndex = Array.tryFindIndex (fun location -> location >= feature) binLocations
    if binIndex.IsSome then binIndex.Value else binLocations.Length

let binDataPoint (binLocations: BinLocations[]) (dataPoint: DataPoint): BinnedDataPoint =
    [| 0 .. dataPoint.Length - 1|] |> Seq.map (fun featureIndex -> binDataPointFeature binLocations.[featureIndex] dataPoint.[featureIndex]) |> Seq.toArray

let naiveBayesFactory (dataSet: DataSet): Classifier =
    let numberOfVariables = dataSet.DataPoints.[0].Length
    let symbols = Array.create numberOfVariables numberOfBins
    let naiveBayes = NaiveBayes(2, symbols)

    let featureDatas = Matrix.Transpose<double> dataSet.DataPoints

    let binLocations = featureDatas |> Seq.map computeBinLocations |> Seq.toArray
    let binDataPointUsingBinLocations = binDataPoint binLocations

    let binnedFeatureData = dataSet.DataPoints |> Seq.map binDataPointUsingBinLocations |> Seq.toArray 
    let modelError = naiveBayes.Estimate(binnedFeatureData, dataSet.Classifications)
    printfn "Model error: %f" modelError

    binDataPointUsingBinLocations >> naiveBayes.Compute 

[<TestFixture>] 
type ``given stock files that have been downloaded`` ()=
    let targetStock = loadFile "EBAY"
    let otherStocks = [| "MSFT"; "JPM"; "GS"; "GOOGL" |] |> Seq.map loadFile
    //let otherStocks = 
    //    DirectoryInfo(Configuration.stockPath).GetFiles() 
    //        |> Seq.map (fun file -> file.Name.Substring(0, file.Name.Length - 4))
    //        |> Seq.map loadFile
    //        |> Seq.take 10

    let dataSet = convertToDataSet targetStock otherStocks
    [<Test>] member test.
     ``when I run the Naive Bayes classifier`` ()=
            performCrossValidation dataSet naiveBayesFactory |> should equal 0.5

[<TestFixture>] 
type ``given the bins have been computed`` ()=
    let binLocations = [| 
        [| 10.0 ; 20.0 |] ;
        [| 1.0 ; 2.0 |]
     |]

    static member BinningExamples = seq {
       yield TestCaseData([ 5.0  ; 0.5 ], [ 0 ; 0 ])
       yield TestCaseData([ 10.0 ; 1.0 ], [ 0 ; 0 ])
       yield TestCaseData([ 15.0 ; 1.5 ], [ 1 ; 1 ])
       yield TestCaseData([ 20.0 ; 2.0 ], [ 1 ; 1 ])
       yield TestCaseData([ 25.0 ; 2.5 ], [ 2 ; 2 ])
    }
        
    [<Test>]
    [<TestCaseSource("BinningExamples")>]
    member test.
     ``when I bin the data point`` (dataPoint: List<double>, binnedDataPoint: List<int>) =
            binDataPoint binLocations (List.toArray dataPoint) |> should equal (List.toArray binnedDataPoint)
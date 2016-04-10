module DataMiningInFSharp.DataMining.KNN

open DataMiningInFSharp.DataMining.StockLoader
open DataMiningInFSharp.DataMining.CrossValidation
open DataMiningInFSharp.DataMining.DataSetConverter

open Accord.MachineLearning

open NUnit.Framework
open FsUnit

let knnFactory (dataSet: DataSet) = 
    KNearestNeighbors(5, dataSet.DataPoints, dataSet.Classifications).Compute

[<TestFixture>] 
type ``given stock files that have been downloaded`` ()=
    let targetStock = loadFile "test/PERFECTB"
    let otherStocks = [| "test/PERFECTA" |] |> Seq.map loadFile |> Seq.toList

    //let targetStock = loadFile "EBAY"
    //let otherStocks = [| "MSFT"; "JPM"; "GS"; "GOOGL" |] |> Seq.map loadFile |> Seq.toList
   // let otherStocks = 
   //     DirectoryInfo(Configuration.stockPath).GetFiles() 
   //         |> Seq.map (fun file -> file.Name.Substring(0, file.Name.Length - 4))
   //         |> Seq.map loadFile
   //         |> Seq.take 10
   //         |> Seq.toList

    let dataSet = convertToDataSet targetStock otherStocks
    [<Test>] member test.
     ``when I run the KNN classifier`` ()=
            performCrossValidation dataSet knnFactory |> should equal 0.5
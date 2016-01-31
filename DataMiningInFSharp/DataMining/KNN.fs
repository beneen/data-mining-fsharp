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



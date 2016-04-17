module DataMiningInFSharp.Database.Schema

open System
open System.Data.Linq
open System.Data.Linq.Mapping
open System.Data.Common

[<Table>]
type Ticker (tickerName: string) =
    new() = Ticker(null)

    [<Column(IsPrimaryKey=true, IsDbGenerated=true, CanBeNull=false)>]
    member val TickerId = -1 with get, set
        
    [<Column(CanBeNull=false)>]
    member val TickerName = tickerName with get, set

[<Table>]
type PriceHistory (ticker: Ticker, tradingDate: DateTime, closingPrice: double) =
    new() = PriceHistory(Ticker(null), DateTime.MinValue, -1.0)

    [<Column(IsPrimaryKey=true, CanBeNull=false)>]
    member val TickerId = ticker.TickerId with get, set

    [<Association(ThisKey="TickerId", OtherKey="TickerId", IsForeignKey=true)>]
    member val Ticker = ticker with get, set

    [<Column(IsPrimaryKey=true, CanBeNull=false)>]
    member val TradingDate = tradingDate with get, set
        
    [<Column(CanBeNull=false)>]
    member val ClosingPrice = closingPrice with get, set

type Schema(connection: DbConnection) = 
    inherit DataContext(connection)
    member x.Tickers = x.GetTable<Ticker>()
    member x.PriceHistory = x.GetTable<PriceHistory>()

module DataMiningInFSharp.Database.Schema

open System
open System.Data.Linq
open System.Data.Linq.Mapping
open System.Data.Common

[<Table>]
type Ticker (tickerName: string) =
    new() = Ticker(null)

    [<Column(IsPrimaryKey=true, IsDbGenerated=true)>]
    member val TickerId = -1 with get, set
        
    [<Column>]
    member val TickerName = tickerName with get, set

[<Table>]
type PriceHistory (tickerId: int, tradingDate: DateTime, closingPrice: double) =
    new() = PriceHistory(-1, DateTime.MinValue, -1.0)

    [<Column(IsPrimaryKey=true)>]
    member val TickerId = tickerId with get, set

    [<Column(IsPrimaryKey=true)>]
    member val TradingDate = tradingDate with get, set
        
    [<Column>]
    member val ClosingPrice = closingPrice with get, set

type Schema(connection: DbConnection) = 
    inherit DataContext(connection)
    member x.Tickers = x.GetTable<Ticker>()
    member x.PriceHistory = x.GetTable<PriceHistory>()

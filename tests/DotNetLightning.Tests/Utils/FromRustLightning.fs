namespace DotNetLightning.Tests.Utils
open DotNetLightning.Chain
open Expecto.Logging
open Microsoft.Extensions.Logging

type Node = {
    ChainMonitor: ChainWatchInterfaceUtil
}

type TestLogger = {
    Level: LogLevel
    Id: string
}
    with
        interface ILogger with
            member this.BeginScope(state: 'TState): System.IDisposable = 
                failwith "Not Implemented"
            member this.IsEnabled(logLevel: LogLevel): bool = 
                true
            member this.Log(logLevel: LogLevel, eventId: EventId, state: 'TState, ``exception``: exn, formatter: System.Func<'TState,exn,string>): unit = 
                printf "[%O]: %s" logLevel (formatter.Invoke(state, ``exception``))


        static member Zero =
            TestLogger.Create("")

        static member Create(id) =
            {
                Level = LogLevel.Debug
                Id = id
            }

        member this.Enable(level) = { this with Level = level }


[<AutoOpen>]
module FromRustLN =
    let createNetwork (nodeCount: uint32) =
        ()
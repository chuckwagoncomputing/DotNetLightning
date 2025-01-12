namespace DotNetLightning.Utils
open NBitcoin
open Aether


type ChannelHandshakeConfig = {
    /// Confirmations we will wait for before considering the channel locked in.
    /// Applied only for inbound channels (see `ChannelHandshakeLimits.MaxMinimumDepth` for the
    /// equivalent limit applied to outbound channel)
    MinimumDepth: BlockHeight
 }
    with

    static member Zero =
        {
            MinimumDepth = BlockHeight 6u
        }

/// Optional Channel limits which are applied during channel creation.
/// These limits are only applied to our counterparty's limits, not our own
type ChannelHandshakeLimits = {
    MinFundingSatoshis: Money
    MaxHTLCMinimumMSat: LNMoney
    /// The remote node sets a limit on the maximum value of pending HTLCs to them at any given time
    /// to limit their funds exposure to HTLCs. This allows you to set a minimum such value.
    MinMaxHTLCValueInFlightMSat: LNMoney
    /// The remote node will require we keep a certain amount in direct payment to ourselves at all
    /// time, ensuring that we are able to be punished if we broadcast an old state. This allows to
    /// you limit the amount which we will have to keep to ourselves (and cannot use for HTLCs).
    MaxChannelReserveSatoshis: Money
    /// The remote node sets a limit on the maximum number of pending HTLCs to them at any given
    /// time. This allows you to set a minimum such value.
    MinMaxAcceptedHTLCs: uint16
    /// HTLCs below this amount plus HTLC transaction fees are not enforceable on-chain.
    /// This settings allows you to set a minimum dust limit for their commitment TXs,
    /// Defaults to 546 , or the current dust limit on the Bitcoin network.
    MinDustLimitSatoshis: Money

    /// Maximum allowed threshold above which outputs will not be generated in their commitment
    /// Transactions.
    /// HTLCs below this amount plus HTLC tx fees are not enforceable on-chain.
    MaxDustLimitSatoshis: Money
    /// before a channel is usable the funding TX will need to be confirmed by at least a certain number
    /// of blocks, specified by the node which is not the funder (as the funder can assume they aren't
    /// going to double-spend themselves).
    /// This config allows you to set a limit on the maximum amount of time to wait. Defaults to 144
    /// blocks or roughly one day and only applies to outbound channels.
    MaxMinimumDepth: BlockHeight
    /// Set to force the incoming channel to match our announced channel preference in ChannelConfig.
    /// Defaults to true to make the default that no announced channels are possible (which is
    /// appropriate for any nodes which are not online very reliably)
    ForceAnnounceChannelPreference: bool

    /// We don't exchange more than this many signatures when negotiating the closing fee
    MaxNegotiationIterations: int32
 }

    with
    static member Zero =
        {
            MinFundingSatoshis = Money.Satoshis(1000m)
            MaxHTLCMinimumMSat = LNMoney.Coins(21_000_000m)
            MinMaxHTLCValueInFlightMSat = LNMoney.Zero
            MaxChannelReserveSatoshis = Money.Zero
            MinMaxAcceptedHTLCs = 0us
            MinDustLimitSatoshis = Money.Satoshis(546m)
            MaxDustLimitSatoshis = Money.Coins(21_000_000m)
            MaxMinimumDepth = 144u |> BlockHeight
            ForceAnnounceChannelPreference = true
            MaxNegotiationIterations = 20
        }


/// Configuration containing all information used by Channel
type ChannelConfig = {
    ChannelHandshakeConfig: ChannelHandshakeConfig
    PeerChannelConfigLimits: ChannelHandshakeLimits
    ChannelOptions: ChannelOptions
 }

    with
    static member Zero =
        {
            ChannelHandshakeConfig = ChannelHandshakeConfig.Zero
            PeerChannelConfigLimits = ChannelHandshakeLimits.Zero
            ChannelOptions = ChannelOptions.Zero
        }

    static member PeerChannelConfigLimits_: Lens<_, _> =
        (fun uc -> uc.PeerChannelConfigLimits),
        (fun v uc -> { uc with PeerChannelConfigLimits = v })

    static member ChannelOptions_: Lens<_, _> =
        (fun uc -> uc.ChannelOptions),
        (fun v uc -> { uc with ChannelOptions = v })

and ChannelOptions = {
    MaxFeeRateMismatchRatio: float
    // Amount (in millionth of a satoshi) the channel will charge per transferred satoshi.
    // This may be allowed to change at runtime in a later update, however doing so must result in
    // update messages sent to notify all nodes of our updated relay fee.
    FeeProportionalMillionths: uint32
    // Set to announce the channel publicly and notify all nodes that they can route via this channel.
    // This should only be set to true for nodes which expect to be online reliably.
    // As the node which funds a channel picks this value this will only apply for new outbound channels unless
    // `ChannelHandshakeLimits.ForceAnnouncedChannelPreferences` is set.
    AnnounceChannel: bool
 }
    with

    static member Zero =
        {
            FeeProportionalMillionths = 0u
            AnnounceChannel = false
            MaxFeeRateMismatchRatio = 0.
        }

    static member FeeProportionalMillionths_: Lens<_, _> =
        (fun cc -> cc.FeeProportionalMillionths),
        (fun v cc -> { cc with FeeProportionalMillionths = v })

    static member AnnouncedChannel_: Lens<_, _> =
        (fun cc -> cc.AnnounceChannel),
        (fun v cc -> { cc with AnnounceChannel = v })

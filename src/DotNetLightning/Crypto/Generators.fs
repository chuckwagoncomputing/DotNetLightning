namespace DotNetLightning.Crypto
open NBitcoin
open NBitcoin.Crypto

module Generators =
    let derivePrivKey (perCommitmentPoint: PubKey) (baseSecret: byte[])=
        Array.append (perCommitmentPoint.ToBytes()) (baseSecret)
        |> Hashes.SHA256
        |> Key

    let derivePubKey (perCommitmentPoint: PubKey) (basePoint: PubKey) =
        derivePrivKey perCommitmentPoint (basePoint.ToBytes())
        |> fun k -> k.PubKey.Add(basePoint)
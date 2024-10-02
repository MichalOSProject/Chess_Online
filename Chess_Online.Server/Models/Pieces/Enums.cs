using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chess_Online.Server.Models.Pieces
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TeamEnum
    {
        White,
        Black,
        NoMansLand
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PieceTypeEnum
    {
        Nomad,
        Bishop,
        King,
        Knight,
        Pawn,
        Queen,
        Rook
    }
    public enum CheckmateStatusEnum
    {
        Safe,
        Endangered,
        Defeated
    }

}

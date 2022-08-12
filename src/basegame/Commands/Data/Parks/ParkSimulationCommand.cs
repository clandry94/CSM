using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Parks
{
    /// <summary>
    ///     Called when the park simulation updates.
    /// </summary>
    /// Sent by:
    /// - ParkHandler
    [ProtoContract]
    public class ParkSimulationCommand : CommandBase
    {
        /// <summary>
        ///     The park district id
        /// </summary>
        [ProtoMember(1)]
        public byte TargetParkId { get; set; }
        
        /// <summary>
        ///     The entertainment
        /// </summary>
        [ProtoMember(2)]
        public ushort TargetEntertainmentAccumulation { get; set; }

        /// <summary>
        ///     The attractiveness
        /// </summary>
        [ProtoMember(3)]
        public ushort TargetAttractivenessAccumulation { get; set; }
    }
}

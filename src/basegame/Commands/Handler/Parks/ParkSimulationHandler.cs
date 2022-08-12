using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Parks;

namespace CSM.BaseGame.Commands.Handler.Parks
{
    // public class ParkSimulationHandler : CommandHandler<ParkSimulationCommand>
    // {
    //     protected override void Handle(ParkSimulationCommand command)
    //     {
    //         if (Command.CurrentRole == MultiplayerRole.Server)
    //             return;
    //
    //         IgnoreHelper.Instance.StartIgnore();
    //
    //         DistrictPark park = DistrictManager.instance.m_parks.m_buffer[command.TargetParkId];
    //
    //         park.m_finalEntertainmentAccumulation = command.TargetEntertainmentAccumulation;
    //         park.m_finalAttractivenessAccumulation = command.TargetAttractivenessAccumulation;
    //
    //         DistrictManager.instance.m_parks.m_buffer[command.TargetParkId] = park;
    //         
    //         IgnoreHelper.Instance.EndIgnore();
    //     }
    // }
}
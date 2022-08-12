using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictParkUpdateHandler : CommandHandler<DistrictParkUpdateCommand>
    {
        protected override void Handle(DistrictParkUpdateCommand command)
        {
            if (Command.CurrentRole == MultiplayerRole.Server)
                return;

            IgnoreHelper.Instance.StartIgnore();

            var sourcePark = command.Park;
            var targetPark = DistrictManager.instance.m_parks.m_buffer[command.ParkID];

            targetPark.m_totalVisitorCount = sourcePark.m_totalVisitorCount;
            targetPark.m_lastVisitorCount = sourcePark.m_lastVisitorCount;
            targetPark.m_finalTicketIncome = sourcePark.m_finalTicketIncome;
            targetPark.m_condition = sourcePark.m_condition;
            targetPark.m_ticketPrice = sourcePark.m_ticketPrice;
            targetPark.m_finalResidentCount = sourcePark.m_finalResidentCount;
            targetPark.m_finalTouristCount = sourcePark.m_finalTouristCount;
            targetPark.m_finalVisitorCapacity = sourcePark.m_finalVisitorCapacity;
            targetPark.m_finalMainCapacity = sourcePark.m_finalMainCapacity;
            targetPark.m_finalWorkerCount = sourcePark.m_finalWorkerCount;
            targetPark.m_parkLevel = sourcePark.m_parkLevel;
            targetPark.m_finalWorkEfficiencyDelta = sourcePark.m_finalWorkEfficiencyDelta;
            targetPark.m_finalStorageDelta = sourcePark.m_finalStorageDelta;
            targetPark.m_studentCount = sourcePark.m_studentCount;
            targetPark.m_academicStaffCount = sourcePark.m_academicStaffCount;
            targetPark.m_academicStaffAccumulation = sourcePark.m_academicStaffAccumulation;
            targetPark.m_previousYearProgress = sourcePark.m_previousYearProgress;
            targetPark.m_winProbability = sourcePark.m_winProbability;
            targetPark.m_dynamicVarsityAttractivenessModifier = sourcePark.m_dynamicVarsityAttractivenessModifier;
            targetPark.m_totalIncomingPassengerCount = sourcePark.m_totalIncomingPassengerCount;
            targetPark.m_totalOutgoingPassengerCount = sourcePark.m_totalOutgoingPassengerCount;
            targetPark.m_finalWeeklyPassengerCapacity = sourcePark.m_finalWeeklyPassengerCapacity;
            targetPark.m_finalIncomingPassengers = sourcePark.m_finalIncomingPassengers;

            //DistrictPark park = DistrictManager.instance.m_parks.m_buffer[command.TargetParkId];

            //park.m_finalEntertainmentAccumulation = command.TargetEntertainmentAccumulation;
            //park.m_finalAttractivenessAccumulation = command.TargetAttractivenessAccumulation;
            Log.Info($"Target park id={command.ParkID} updating");

            DistrictManager.instance.m_parks.m_buffer[command.ParkID] = targetPark;
            
            Log.Info($"Target park id={command.ParkID} updated");

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
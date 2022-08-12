using System;
using System.IO;
using System.Xml.Serialization;
using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Districts
{
    /// <summary>
    ///     Called when we want to update a district in some way
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class DistrictParkUpdateCommand : CommandBase
    {
        /// <summary>
        ///     The target park id id.
        /// </summary>
        [ProtoMember(1)]
        public byte ParkID { get; set; }

        [ProtoMember(2)] public SerializableDistrictPark Park { get; set; }
    }

    [ProtoContract]
    public class SerializableDistrictPark : IEquatable<SerializableDistrictPark>
    {
        [ProtoMember(1)] public uint m_totalVisitorCount;

        [ProtoMember(2)] public uint m_lastVisitorCount;

        [ProtoMember(3)] public uint m_finalTicketIncome;

        [ProtoMember(4)] public ushort m_condition;

        [ProtoMember(5)] public ushort m_ticketPrice;

        [ProtoMember(6)] public ushort m_finalResidentCount;

        [ProtoMember(7)] public ushort m_finalTouristCount;

        [ProtoMember(8)] public ushort m_finalVisitorCapacity;

        [ProtoMember(9)] public ushort m_finalMainCapacity;

        [ProtoMember(10)] public ushort m_finalWorkerCount;

        [ProtoMember(11)] public DistrictPark.ParkLevel m_parkLevel;

        [ProtoMember(12)] public byte m_finalWorkEfficiencyDelta;

        [ProtoMember(13)] public byte m_finalStorageDelta;

        [ProtoMember(14)] public uint m_studentCount;

        [ProtoMember(15)] public byte m_academicStaffCount;

        [ProtoMember(16)] public float m_academicStaffAccumulation;

        [ProtoMember(17)] public float m_previousYearProgress;

        [ProtoMember(18)] public byte m_winProbability;

        [ProtoMember(19)] public int m_dynamicVarsityAttractivenessModifier;

        [ProtoMember(20)] public uint m_totalIncomingPassengerCount;

        [ProtoMember(21)] public uint m_totalOutgoingPassengerCount;

        [ProtoMember(22)] public uint m_finalWeeklyPassengerCapacity;

        [ProtoMember(23)] public uint m_finalIncomingPassengers;

        public SerializableDistrictPark()
        {
            
        }
        
        public SerializableDistrictPark(DistrictPark park)
        {
            m_totalVisitorCount = park.m_totalVisitorCount;
            m_lastVisitorCount = park.m_lastVisitorCount;
            m_finalTicketIncome = park.m_finalTicketIncome;
            m_condition = park.m_condition;
            m_ticketPrice = park.m_ticketPrice;
            m_finalResidentCount = park.m_finalResidentCount;
            m_finalTouristCount = park.m_finalTouristCount;
            m_finalVisitorCapacity = park.m_finalVisitorCapacity;
            m_finalMainCapacity = park.m_finalMainCapacity;
            m_finalWorkerCount = park.m_finalWorkerCount;
            m_parkLevel = park.m_parkLevel;
            m_finalWorkEfficiencyDelta = park.m_finalWorkEfficiencyDelta;
            m_finalStorageDelta = park.m_finalStorageDelta;
            m_studentCount = park.m_studentCount;
            m_academicStaffCount = park.m_academicStaffCount;
            m_academicStaffAccumulation = park.m_academicStaffAccumulation;
            m_previousYearProgress = park.m_previousYearProgress;
            m_winProbability = park.m_winProbability;
            m_dynamicVarsityAttractivenessModifier = park.m_dynamicVarsityAttractivenessModifier;
            m_totalIncomingPassengerCount = park.m_totalIncomingPassengerCount;
            m_totalOutgoingPassengerCount = park.m_totalOutgoingPassengerCount;
            m_finalWeeklyPassengerCapacity = park.m_finalWeeklyPassengerCapacity;
            m_finalIncomingPassengers = park.m_finalIncomingPassengers;
        }

        // public DistrictAreaResourceData m_grainData;
        // public DistrictAreaResourceData m_logsData;
        // public DistrictAreaResourceData m_oreData;
        // public DistrictAreaResourceData m_oilData;
        // public DistrictAreaResourceData m_animalProductsData;
        // public DistrictAreaResourceData m_floursData;
        // public DistrictAreaResourceData m_paperData;
        // public DistrictAreaResourceData m_planedTimberData;
        // public DistrictAreaResourceData m_petroleumData;
        // public DistrictAreaResourceData m_plasticsData;
        // public DistrictAreaResourceData m_glassData;
        // public DistrictAreaResourceData m_metalsData;
        // public DistrictAreaResourceData m_luxuryProductsData;
        //public AcademicWorksData m_academicWorksData;
        //public DistrictYearReportData m_currentYearReportData;
        //public DistrictYearReportData m_lastYearReportData;
        //public DistrictYearReportData m_secondLastYearReportData;
        //public DistrictYearReportLedger m_ledger;
        // public static string m_airlineName;
        // public bool m_hasTerminal;
        // public bool m_hasAircraftStand;
        // public bool m_hasRunway;
        // public bool m_hadCheerleadingBudget;
        // public bool[] m_awayMatchesDone;
        // public Dictionary<DistrictPark.SportIndex, FastList<ushort>> m_arenas;
        // public Dictionary<DistrictPark.SportIndex, FastList<ushort>> m_activeArenas;
        // public ushort m_partyVenue;
        // public byte m_goingToParty;
        // public byte m_arrivedAtParty;
        public bool Equals(SerializableDistrictPark other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return m_totalVisitorCount == other.m_totalVisitorCount &&
                   m_lastVisitorCount == other.m_lastVisitorCount &&
                   m_finalTicketIncome == other.m_finalTicketIncome && m_condition == other.m_condition &&
                   m_ticketPrice == other.m_ticketPrice && m_finalResidentCount == other.m_finalResidentCount &&
                   m_finalTouristCount == other.m_finalTouristCount &&
                   m_finalVisitorCapacity == other.m_finalVisitorCapacity &&
                   m_finalMainCapacity == other.m_finalMainCapacity &&
                   m_finalWorkerCount == other.m_finalWorkerCount && m_parkLevel == other.m_parkLevel &&
                   m_finalWorkEfficiencyDelta == other.m_finalWorkEfficiencyDelta &&
                   m_finalStorageDelta == other.m_finalStorageDelta && m_studentCount == other.m_studentCount &&
                   m_academicStaffCount == other.m_academicStaffCount &&
                   m_academicStaffAccumulation.Equals(other.m_academicStaffAccumulation) &&
                   m_previousYearProgress.Equals(other.m_previousYearProgress) &&
                   m_winProbability == other.m_winProbability &&
                   m_dynamicVarsityAttractivenessModifier == other.m_dynamicVarsityAttractivenessModifier &&
                   m_totalIncomingPassengerCount == other.m_totalIncomingPassengerCount &&
                   m_totalOutgoingPassengerCount == other.m_totalOutgoingPassengerCount &&
                   m_finalWeeklyPassengerCapacity == other.m_finalWeeklyPassengerCapacity &&
                   m_finalIncomingPassengers == other.m_finalIncomingPassengers;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SerializableDistrictPark)obj);
        }
        
        public string stringSerialize()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(GetType());

            using(StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)m_totalVisitorCount;
                hashCode = (hashCode * 397) ^ (int)m_lastVisitorCount;
                hashCode = (hashCode * 397) ^ (int)m_finalTicketIncome;
                hashCode = (hashCode * 397) ^ m_condition.GetHashCode();
                hashCode = (hashCode * 397) ^ m_ticketPrice.GetHashCode();
                hashCode = (hashCode * 397) ^ m_finalResidentCount.GetHashCode();
                hashCode = (hashCode * 397) ^ m_finalTouristCount.GetHashCode();
                hashCode = (hashCode * 397) ^ m_finalVisitorCapacity.GetHashCode();
                hashCode = (hashCode * 397) ^ m_finalMainCapacity.GetHashCode();
                hashCode = (hashCode * 397) ^ m_finalWorkerCount.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)m_parkLevel;
                hashCode = (hashCode * 397) ^ m_finalWorkEfficiencyDelta.GetHashCode();
                hashCode = (hashCode * 397) ^ m_finalStorageDelta.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)m_studentCount;
                hashCode = (hashCode * 397) ^ m_academicStaffCount.GetHashCode();
                hashCode = (hashCode * 397) ^ m_academicStaffAccumulation.GetHashCode();
                hashCode = (hashCode * 397) ^ m_previousYearProgress.GetHashCode();
                hashCode = (hashCode * 397) ^ m_winProbability.GetHashCode();
                hashCode = (hashCode * 397) ^ m_dynamicVarsityAttractivenessModifier;
                hashCode = (hashCode * 397) ^ (int)m_totalIncomingPassengerCount;
                hashCode = (hashCode * 397) ^ (int)m_totalOutgoingPassengerCount;
                hashCode = (hashCode * 397) ^ (int)m_finalWeeklyPassengerCapacity;
                hashCode = (hashCode * 397) ^ (int)m_finalIncomingPassengers;
                return hashCode;
            }
        }
    }
}


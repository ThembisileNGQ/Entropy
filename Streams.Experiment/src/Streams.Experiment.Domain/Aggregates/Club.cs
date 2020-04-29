using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Actor;
using Akka.Event;

namespace Streams.Experiment.Domain.Aggregates
{
    public class Club : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        public ImmutableList<Guid> GoldMemberUserIds { get; private set; }
        public ImmutableList<Guid> PlatinumMemberUserIds { get; private set; }
        public ImmutableList<Guid> BlackMemberUserIds { get; private set; }

        public Club()
        {
            _logger = Context.GetLogger();
            
            GoldMemberUserIds = ImmutableList<Guid>.Empty;
            PlatinumMemberUserIds = ImmutableList<Guid>.Empty;
            BlackMemberUserIds = ImmutableList<Guid>.Empty;

            Receive<AddGoldMember>(Handle);
            Receive<AddPlatinumMember>(Handle);
            Receive<AddBlackMember>(Handle);
            Receive<GetClubMembers>(Handle);
        }

        public bool Handle(AddGoldMember command)
        {
            GoldMemberUserIds = GoldMemberUserIds.Add(command.UserId);
            
            _logger.Info("Added user to Gold Club for Id={0}", command.UserId);
            return true;
        }
        public bool Handle(AddPlatinumMember command)
        {
            PlatinumMemberUserIds = PlatinumMemberUserIds.Add(command.UserId);
            
            _logger.Info("Added user to Platinum Club for Id={0}", command.UserId);
            return true;
        }
        public bool Handle(AddBlackMember command)
        {
            BlackMemberUserIds = BlackMemberUserIds.Add(command.UserId);
            
            _logger.Info("Added user to Black Club for Id={0}", command.UserId);
            return true;
        }

        public bool Handle(GetClubMembers query)
        {
            var result = new ClubMembers(GoldMemberUserIds.ToList(), PlatinumMemberUserIds.ToList(), BlackMemberUserIds.ToList());
            
            Sender.Tell(result);
            
            _logger.Info("Query for Club members returned.");
            return true;
        }
    }


    public class AddGoldMember : AddMember
    {
        public AddGoldMember(
            Guid userId) 
            : base(userId)
        {
        }
    }
    
    public class AddPlatinumMember : AddMember
    {
        public AddPlatinumMember(
            Guid userId) 
            : base(userId)
        {
        }
    }
    
    public class AddBlackMember : AddMember
    {
        public AddBlackMember(
            Guid userId) 
            : base(userId)
        {
        }
    }
    public abstract class AddMember 
    {
        public Guid UserId { get; }
        
        public AddMember(
            Guid userId)
        {
            UserId = userId;
        }
    }

    public class ClubMembers
    {
        public List<Guid> GoldMembers { get; }
        public List<Guid> PlatinumMembers { get; }
        public List<Guid> BlackMembers { get; }

        public ClubMembers(
            List<Guid> goldMembers,
            List<Guid> platinumMembers,
            List<Guid> blackMembers)
        {
            GoldMembers = goldMembers;
            PlatinumMembers = platinumMembers;
            BlackMembers = blackMembers;
        }
    }
    public class GetClubMembers{}
}
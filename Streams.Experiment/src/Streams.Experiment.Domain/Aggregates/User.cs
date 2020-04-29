using System;
using Akka.Actor;
using Akka.Event;

namespace Streams.Experiment.Domain.Aggregates
{
    public class User : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private UserState _state;
        
        public User()
        {
            _state = null;
            _logger = Context.GetLogger();
            
            Receive<RegisterUser>(Handle);
            Receive<GetUserState>(Handle);
        }

        public bool Handle(RegisterUser command)
        {
            _state = new UserState(
                command.Id,
                command.Name,
                command.Email,
                command.Nationality,
                command.PersonNummer);
            
            _logger.Info("Registered User for Id={0}", command.Id);
            return true;
        }
        
        private bool Handle(GetUserState query)
        {
            var result = new UserState(
                _state.Id,
                _state.Name,
                _state.Email,
                _state.Nationality,
                _state.PersonNummer);
            
            Sender.Tell(result);

            _logger.Info("Query for User State returned for Id={0}", query.Id);
            return true;
        }
    }

    public class UserState
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; }
        public string Nationality { get; }
        public string PersonNummer { get; }

        public UserState(
            Guid id,
            string name,
            string email,
            string nationality,
            string personNummer)
        {
            Id = id;
            Name = name;
            Email = email;
            Nationality = nationality;
            PersonNummer = personNummer;
        }
    }

    public class RegisterUser : UserMessage
    {
        public string Name { get; }
        public string Email { get; }
        public string Nationality { get; }
        public string PersonNummer { get; }
        
        public RegisterUser(
            Guid id,
            string name,
            string email,
            string nationality,
            string personNummer)
        : base(id)
        {
            Name = name;
            Email = email;
            Nationality = nationality;
            PersonNummer = personNummer;
        }
    }

    public class GetUserState : UserMessage
    {
        public GetUserState(Guid id) 
            : base(id)
        {
        }
    }
    
    public abstract class UserMessage : AggregateMessage
    {
        protected UserMessage(Guid id) 
            : base(id)
        {
        }
    }
}
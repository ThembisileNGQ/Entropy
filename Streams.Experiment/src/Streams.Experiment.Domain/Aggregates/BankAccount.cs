using System;
using Akka.Actor;
using Akka.Event;

namespace Streams.Experiment.Domain.Aggregates
{
    public class BankAccount : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private BankAccountState _state;
        public BankAccount()
        {
            _state = null;
            _logger = Context.GetLogger();
            
            Receive<OpenBankAccount>(Handle);
            Receive<GetBankAccountState>(Handle);
        }

        private bool Handle(OpenBankAccount command)
        {
            _state = new BankAccountState(
                command.Id,
                command.Savings,
                command.Credit);
            
            _logger.Info("Opened Bank Account for Id={0}", command.Id);
            
            return true;
        }

        private bool Handle(GetBankAccountState query)
        {
            var result = new BankAccountState(
                _state.UserId,
                _state.Savings,
                _state.Credit);
            
            Sender.Tell(result);

            _logger.Info("Query for Bank Account State returned for Id={0}", query.Id);
            return true;
        }
    }

    public class BankAccountState
    {
        public Guid UserId { get; }
        public int Savings { get; }
        public int Credit { get; }

        public BankAccountState(
            Guid userId,
            int savings,
            int credit)
        {
            UserId = userId;
            Savings = savings;
            Credit = credit;
        }
    }

    public class OpenBankAccount : BankAccountMessage
    {
        public int Savings { get; }
        public int Credit { get; }

        public OpenBankAccount(
            Guid id,
            int savings,
            int credit)
        : base(id)
        {
            Savings = savings;
            Credit = credit;
        }
    }
    
    public class GetBankAccountState : BankAccountMessage
    {
        public GetBankAccountState(Guid id)
            : base(id)
        {
        }
    }

    public abstract class BankAccountMessage : AggregateMessage
    {
        protected BankAccountMessage(Guid id) 
            : base(id)
        {
        }
    }
}
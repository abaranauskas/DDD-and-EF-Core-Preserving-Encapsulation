using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public interface IDomainEvent //marker interface
    {
    }

    public sealed class StudentEmailChangedEvent : IDomainEvent
    {
        public StudentEmailChangedEvent(long srudentId, Email newEmail)
        {
            SrudentId = srudentId;
            NewEmail = newEmail;
        }

        public long SrudentId { get; }
        public Email NewEmail { get; }
    }


    public interface IBus
    {
        void Send(string message);
    }

    public class Bus : IBus
    {
        public void Send(string message)
        {
            //put message on the bus instead
            Console.WriteLine("Message sent" + message);
        }
    }

    public class MessageBus
    {
        private readonly IBus _bus;

        public MessageBus(IBus bus)
        {
            _bus = bus;
        }

        public void SendEmailChangedMessage(long studentId, string newEmail) //responsible for composing messages
        {
            _bus.Send("Type: Student_Email_Changed; " +
                $"id: {studentId};" +
                $"Email: {newEmail}"); //in real would be json
        }
    }


    public class EventDispacher
    {
        private readonly MessageBus _messageBus;

        public EventDispacher(MessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public void Dispatch(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var @event in domainEvents)
            {
                Dispatch(@event);
            }
        }

        private void Dispatch(IDomainEvent @event)
        {
            switch (@event)
            {
                case StudentEmailChangedEvent emailChangedEvent:
                    _messageBus.SendEmailChangedMessage(emailChangedEvent.SrudentId,
                        emailChangedEvent.NewEmail.Value);
                    break;

                    //future domain events

                default:
                    //catched all unhandled events
                    throw new Exception($"Unknown domain event type: '{@event.GetType()}'");
            }
        }
    }
}

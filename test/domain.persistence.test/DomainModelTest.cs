using domain.model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace domain.persistence.test
{
    public class DomainModelTest
    {
        private readonly DomainModel model;

        public DomainModelTest()
        {
            this.model = new DomainModel();
        }

        public class Observer : IObserver<DomainEvent>
        {
            public List<DomainEvent> Events { get; } = new List<DomainEvent>();

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(DomainEvent value) => this.Events.Add(value);
        }

        [Fact]
        public void DomainModel_sends_event()
        {
            // ARRANGE

            var observer = new Observer();
            using var subscription = this.model.DomainEvents.Subscribe(observer);

            var entity = new DomainEntity();

            // ACT

            this.model.Entities.Add(entity);

            // ASSERT

            Assert.Single(observer.Events);
            Assert.Equal("Added", observer.Events.Single().Event);
            Assert.Equal(entity, observer.Events.Single().Data);
        }
    }
}
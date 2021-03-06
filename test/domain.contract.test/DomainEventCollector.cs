﻿using System;
using System.Collections.Generic;

namespace Domain.Contract.Test
{
    public class DomainEventCollector : IObserver<DomainEntityEvent>
    {
        public List<DomainEntityEvent> Collected { get; } = new List<DomainEntityEvent>();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(DomainEntityEvent value) => this.Collected.Add(value);
    }
}
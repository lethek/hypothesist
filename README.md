[![nuget](https://img.shields.io/nuget/v/Hypothesist.svg)](https://www.nuget.org/packages/Hypothesist/)
[![codecov](https://codecov.io/gh/riezebosch/hypothesist/branch/main/graph/badge.svg)](https://codecov.io/gh/riezebosch/hypothesist)
[![stryker](https://img.shields.io/endpoint?style=flat&label=stryker&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Friezebosch%2Fhypothesist%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/riezebosch/hypothesist/main)
[![build status](https://ci.appveyor.com/api/projects/status/21ssw4pgaxpcppp2/branch/main?svg=true)](https://ci.appveyor.com/project/riezebosch/hypothesist)

# Hypothesist

> The "future" assertion library for .NET, promise!

This library is there to help you do assertions on events that are about to happen in the near future.
For example, when building integration tests for a subscriber on a service bus.

![schema](https://raw.githubusercontent.com/riezebosch/hypothesist/main/docs/img/hypothesize.svg)

### Define

Define your hypothesis with an _experiment_, _time constraint_ and then _test_ with samples:

```c#
var hypothesis = Hypothesis
    .For<Data>()
    .Any(x => x.Value == 1234);
```

### Test

You _test_ your _hypothesis_ by providing samples:

```c#
await hypothesis.Test(sample);
```

For example with an injected stub:

```c#
var service = Substitute.For<IDemoService>();
service
    .When(x => x.Demo(Arg.Any<Data>()))
    .Do(x => hypothesis.Test(x.Arg<Data>()));
```

or with a hand-rolled implementation:

```c#
class TestService : IDemoService
{
    private readonly IHypothesis<Data> _hypothesis;

    public TestService(IHypothesis<Data> hypothesis) => 
        _hypothesis = hypothesis;

    public Task Demo(Data data) =>
        _hypothesis.Test(data);
}

var service = new TestService(hypothesis);
```

or with the consumer factory [Hypothesist.MassTransit](MassTransit) when using [MassTransit](https://masstransit-project.com):

```c#
cfg.ReceiveEndpoint("...", x => x.Consumer(hypothesis.AsConsumer));
```

or with the handler factory [Hypothesist.Rebus](Rebus) when using [Rebus](https://github.com/rebus-org/):

```c#
using var activator = new BuiltinHandlerActivator()
    .Register(hypothesis.AsHandler);
```

Just checkout the available [adapters](#adapters) for more information!

### Validate

You _validate_ if your _hypothesis_ holds true for the supplied _samples_ during the specified _time window_.

```c#
await hypothesis.Validate(15.Seconds()); // <-- using TimeSpan extension from FluentAssertions
```

But somewhere in between you've fired off the eventing mechanism that ultimately invokes the injected service.

## Experiments

The two parts of the hypothesis are the experiment and a time constraint.

### Any

Validates that _at least one_ item matches the assertion, meaning the experiment stops when this item is observed.

### All

Validates that _all_ items that are observed during the experiment matches the assertion.

Remark: having no items observed during the time window also means the hypothesis holds true;

### First

Validates that _the first_ item that is observed matches the assertion.

### Single

Validates that _exactly one_ item is observed that matches the assertion.

Remark: having _other_ items _not matching_ the assert means the hypothesis still holds true.

### Exactly

Validates that _exactly_ the given number of _occurrences_ is observed that matches the assertion within the given timeframe.

### AtLeast

Validates that _at least_ the given number of _occurrences_ is observed that matches the assertion.

## Adapters

Some adapters wrapping around the hypothesis to make invocation convenient:

* [Rebus](Rebus)
* [MassTransit](MassTransit)
* [Azure ServiceBus](ServiceBus)
* [ASPNET Core](AspNet)
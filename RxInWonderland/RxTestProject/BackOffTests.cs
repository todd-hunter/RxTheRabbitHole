using FluentAssertions;
using NUnit.Framework;
using Rx.Experimental;
using Tests.Common;
using TestStack.BDDfy;

namespace Tests
{

    [TestFixture]
    public class GetsExceptionWhenNotUsingBackOffScenario : RxTest<int>
    {
        [SetUp]
        public void Setup()
        {
            Init();
        }

        [Test]
        public void Execute()
        {
            this.BDDfy();
        }

        public void GivenAStreamThatWillYieldASingleError()
        {
            Sut = TestStreams.HasOneError();
            TimeAdvances();
        }

        public void AndGivenThereAreNoRecorderErrors()
        {
            Error.Should().BeNull();
        }

        public void WhenSubscribing()
        {
            Subscribe(Schedulers.Dispatcher);
            TimeAdvances();
        }

        public void ThenAnErrorOccured()
        {
            Error.Should().NotBeNull();
            Error.Message.Should().Contain("UNIT TEST");
        }

        public void AndThenTheResultIsNotReturned()
        {
            Result.Should().Be(0);
            HasCompleted.Should().BeFalse();
        }

        public void AndThenDisposesOk()
        {
            CleanUp();
        }
    }

    [TestFixture]
    public class CanBackOffOkWhenGettingOneExceptionScenario : RxTest<int>
    {
        [SetUp]
        public void Setup()
        {
            Init();
        }

        [Test]
        public void Execute()
        {
            this.BDDfy();
        }

        public void GivenAStreamWithSingleErrorAndBackOff()
        {
            Sut = TestStreams.HasOneError().BackOff(Schedulers);
            TimeAdvances();
        }

        public void AndGivenThereAreNoRecorderErrors()
        {
            Error.Should().BeNull();
        }

        public void WhenSubscribing()
        {
            Subscribe(Schedulers.Dispatcher);
            TimeAdvances();
        }

        public void ThenNoErrorOccured()
        {
            Error.Should().BeNull();
        }

        public void AndThenTheResultIsReturnedOk()
        {
            Result.Should().Be(42);
            HasCompleted.Should().BeTrue();
        }

        public void AndThenDisposesOk()
        {
            CleanUp();
        }
    }

    [TestFixture]
    public class CanBackOffOnManyExceptionsAndStillGetResultScenario : RxTest<int>
    {
        [SetUp]
        public void Setup()
        {
            Init();
        }

        [Test]
        public void Execute()
        {
            this.BDDfy();
        }

        public void GivenAStreamWithLotsOfErrorsAndBackOff()
        {
            Sut = TestStreams.ErrorsUntil(10000).BackOff(Schedulers, 10001);
            TimeAdvances();
        }

        public void AndGivenThereAreNoRecorderErrors()
        {
            Error.Should().BeNull();
        }

        public void WhenSubscribing()
        {
            Subscribe(Schedulers.Dispatcher);
            TimeAdvances(int.MaxValue);
        }

        public void ThenNoErrorOccured()
        {
            Error.Should().BeNull();
        }

        public void AndThenTheResultIsReturned()
        {
            Result.Should().Be(42);
            HasCompleted.Should().BeTrue();
        }

        public void AndThenDisposesOk()
        {
            CleanUp();
        }
    }

    [TestFixture]
    public class BackOffRunsOutOfRetrysAfterTooManyExceptionsScenario : RxTest<int>
    {
        [SetUp]
        public void Setup()
        {
            Init();
        }

        [Test]
        public void Execute()
        {
            this.BDDfy();
        }

        public void GivenAStreamWithMoreErrorsThanTheBackOffAllows()
        {
            Sut = TestStreams.ErrorsUntil(10000).BackOff(Schedulers, 9999);
            TimeAdvances();
        }

        public void AndGivenThereAreNoRecorderErrors()
        {
            Error.Should().BeNull();
        }

        public void WhenSubscribing()
        {
            Subscribe(Schedulers.Dispatcher);
            TimeAdvances(int.MaxValue);
        }

        public void ThenAnErrorOccured()
        {
            Error.Should().NotBeNull();
            Error.Message.Should().Contain("UNIT TEST");
        }

        public void AndThenNoResultIsReturned()
        {
            Result.Should().Be(0);
            HasCompleted.Should().BeFalse();
        }

        public void AndThenDisposesOk()
        {
            CleanUp();
        }
    }
}
namespace MockPaymentGateway.Api.Enums
{
    public enum SimulationMode
    {
        Random,
        AlwaysSucceed,
        AlwaysFail,
        AlwaysFailFatal,
        FailNTimesThenSucceed,
        SlowResponse
    }
}

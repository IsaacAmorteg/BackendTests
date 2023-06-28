using TechTalk.SpecFlow;

namespace Task9.Steps
{
    [Binding]
    public sealed class Hooks1
    {

        [BeforeScenario(Order = 1)]
        public void FirstBeforeScenario()
        {

        }

        [AfterScenario]
        public void AfterScenario()
        {
            
        }
    }
}
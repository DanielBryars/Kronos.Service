using System.Threading.Tasks;

namespace Kronos.Service
{
    public interface IHomeAssistantControl
    {
        double GetCurrentHouseTemperature();
        double GetSetTemperature();
        Task SwitchHeatingAsync(bool state);
        Task SwitchHotwaterAsync(bool state);
    }
}
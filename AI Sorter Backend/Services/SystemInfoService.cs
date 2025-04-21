using Hardware.Info;

namespace AI_Sorter_Backend.Services;

public class SystemInfoService
{
	private readonly HardwareInfo _hardwareInfo;

	public SystemInfoService()
	{
		_hardwareInfo = new HardwareInfo();
		_hardwareInfo.RefreshAll();
	}

	public object GetSystemLoad()
	{
		return new
		{
			CpuName = _hardwareInfo.CpuList.FirstOrDefault()?.Name,
			Load = _hardwareInfo.CpuList.FirstOrDefault()?.CurrentClockSpeed,
			MemoryAvailable = _hardwareInfo.MemoryStatus.AvailablePhysical,
			MemoryTotal = _hardwareInfo.MemoryStatus.TotalPhysical
		};
	}
}

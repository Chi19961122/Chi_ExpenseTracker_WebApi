using AutoMapper;
using Chi.ExpenseTracker.Services.Mapping;
using Microsoft.Extensions.Logging.Abstractions;

namespace Chi.ExpenseTracker.Tests.TestHelpers;

/// <summary>
/// 測試用 AutoMapper 工廠 — 載入 Service 層的對應設定。
/// </summary>
public static class MapperFactory
{
    /// <summary>
    /// 建立含 ServiceMappingProfile 的 IMapper。
    /// </summary>
    public static IMapper Create()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<ServiceMappingProfile>(),
            NullLoggerFactory.Instance);
        return config.CreateMapper();
    }
}

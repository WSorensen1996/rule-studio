using DataverseLogic;
using DataverseLogic.RuleStudio.Promotion;

namespace DataverseRegistration.RuleStudio;

/// <summary>Maps the generated Custom API requests and responses onto <see cref="ExportService"/> and <see cref="ImportService"/>.</summary>
public class PromotionApi(ExportService exportService, ImportService importService)
{
    private readonly ExportService exportService = exportService;
    private readonly ImportService importService = importService;

    public ExportRulesResponse Export(ExportRulesRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        return new ExportRulesResponse(exportService.Export(request.RuleKeys));
    }

    public ImportRulesResponse Import(ImportRulesRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        return new ImportRulesResponse(importService.Import(request.Bundle ?? string.Empty, request.Mode ?? "check"));
    }
}

using TradingPlatform.Decision;
using TradingPlatform.Execution;
using TradingPlatform.FeatureIntelligence;
using TradingPlatform.Market;
using TradingPlatform.ModelManagement;
using TradingPlatform.Operations;
using TradingPlatform.Reconciliation;
using TradingPlatform.RiskGuard;
using TradingPlatform.TradeManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services
    .AddMarketModule()
    .AddFeatureIntelligenceModule()
    .AddDecisionModule()
    .AddRiskGuardModule()
    .AddTradeManagementModule()
    .AddExecutionModule()
    .AddReconciliationModule()
    .AddModelManagementModule()
    .AddOperationsModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;

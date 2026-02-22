namespace The_Garden_Group.ViewModels;

public sealed class DashboardVm
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }

    public double OpenPct => Total == 0 ? 0 : (Open * 100.0 / Total);
    public double ResolvedPct => Total == 0 ? 0 : (Resolved * 100.0 / Total);
    public double ClosedPct => Total == 0 ? 0 : (Closed * 100.0 / Total);
}
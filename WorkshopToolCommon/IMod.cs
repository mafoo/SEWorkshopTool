namespace Phoenix.WorkshopTool
{
    internal interface IMod
    {
        string Title { get; }
        ulong ModId { get; }
        string ModPath { get; }
    }
}
using TableCloth3.Spork.Contracts;

namespace TableCloth3.Spork.Services
{
    public sealed class TaskBarProgressService : ITaskBarProgressService
    {
        public ITaskBarProgressHost? ProgressHost { get; set; }
    }
}

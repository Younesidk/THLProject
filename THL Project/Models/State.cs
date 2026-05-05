namespace THL_Project.Models
{
    public class State
    {
        public string Name { get; private set; }

        public bool IsInitial { get; private set; }
        public bool IsEnd { get; private set; }

        public State(string name, bool isInitial, bool isEnd)
        {
            Name = name;
            IsInitial = isInitial;
            IsEnd = isEnd;
        }

        public State(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}

namespace THL_Project.Models
{
    public class State
    {
        public string Name { get; private set; }

        public bool IsInitial { get; private set; }
        public bool IsEnd { get; private set; }

        public State(string Name,bool isInitial,bool isEnd)
        {
            this.Name = Name;
            IsInitial = isInitial;
            IsEnd = isEnd;
        }

        public State(string Name)
        {
            this.Name = Name;
        }

        public override string ToString() =>
            Name;
    }
}

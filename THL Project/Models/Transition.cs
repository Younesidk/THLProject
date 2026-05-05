namespace THL_Project.Models;

public class Transition
{
    public State? From { get; set; }
    public State? To { get; set; }
    public char Symbol { get; set; }

    public Transition(State from, State to,char symbol)
    {
        Symbol = symbol;
        From = from;
        To = to;
    }

    public Transition(State from, char symbol)
    {
        Symbol = symbol;
        From = from;
    }
}
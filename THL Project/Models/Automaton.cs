using System.Diagnostics;

namespace THL_Project.Models;

public class Automaton
{
    private List<State>[,] transitionMatrix;

    private Dictionary<string, int> StateIndexMap;
    private Dictionary<string, int> SymbolIndexMap;

    private List<State> states;


    private void RebuiltMatrix()
    {
        int stateCount = StateIndexMap.Count;
        int symbolCount = SymbolIndexMap.Count;

        int oldRows = transitionMatrix.GetLength(0);
        int oldCols = transitionMatrix.GetLength(1);

        var NewMatrix = new List<State>[stateCount, symbolCount];

        foreach (var state in StateIndexMap)
        {
            foreach (var symbol in SymbolIndexMap)
            {
                if (state.Value < oldRows && symbol.Value < oldCols)
                    if (transitionMatrix[state.Value, symbol.Value] != null)
                        NewMatrix[state.Value, symbol.Value] = new List<State>(transitionMatrix[state.Value, symbol.Value]);
            }
        }

        transitionMatrix = NewMatrix;
    }

    public Automaton()
    {
        transitionMatrix = new List<State>[0, 0];
        states = new();
        StateIndexMap = new();
        SymbolIndexMap = new();
    }
        
    public void AddState(State state)
    {
        StateIndexMap.Add(state.Name, StateIndexMap.Count);
        states.Add(state);

        RebuiltMatrix();
    }

    public void AddSymbol(char symbol)
    {
        SymbolIndexMap.Add(symbol.ToString(), SymbolIndexMap.Count);

        RebuiltMatrix();
    }

    public void AddTransition(Transition transition)
    {
        int rowNumber;
        if (!StateIndexMap.TryGetValue(transition.From.Name, out rowNumber))
        {
            return;
        }

        int colNumber;
        if (!SymbolIndexMap.TryGetValue(transition.Symbol.ToString(), out colNumber))
        {
            return;
        }

        if (transitionMatrix[rowNumber, colNumber] is null)
            transitionMatrix[rowNumber, colNumber] = new();

        transitionMatrix[rowNumber, colNumber].Add(transition.To);
    }

    public List<State?>? GetTransition(State fromState,char symbol)
    {
        if (!StateIndexMap.TryGetValue(fromState.Name, out var rowNumber))
        {
            return null;
        }

        if (!SymbolIndexMap.TryGetValue(symbol.ToString(), out var colNumber))
        {
            return null;
        }

        return transitionMatrix[rowNumber, colNumber];
    }

    public bool IsDeterministic()
    {
        foreach (var state in StateIndexMap)
        {
            foreach (var symbol in SymbolIndexMap)
            {
                var cell = transitionMatrix[state.Value, symbol.Value];
                if (cell is not null)
                    if (cell.Count > 1)
                        return false;
            }
        }
        return true;
    }

    public bool IsValidSymbol(char symbol) =>
        SymbolIndexMap.ContainsKey(symbol.ToString());

    public List<State> GetStates() => new(states);

    public List<State> GetFinalStates() => 
        states.Where(s => s.IsEnd).ToList();

    public State? GetInitialState() =>
        states.FirstOrDefault(s => s.IsInitial);

    public List<State>[,]? GetMatrixDisplay() => transitionMatrix;

    public List<string> GetStateNames() =>
        StateIndexMap.OrderBy(s => s.Value).Select(s => s.Key).ToList();

    public List<string> GetSymbolNames() =>
        SymbolIndexMap.OrderBy(s => s.Value).Select(s => s.Key).ToList();

    public bool InitExist() =>
        states.Any(s => s.IsInitial);
}
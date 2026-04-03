using THL_Project.Models;

namespace THL_Project.Logic;

public static class AutomatonDeterminizer
{
    public static Automaton? NFAtoDFA(Automaton NFA)
    {
        if (NFA.IsDeterministic())
            return NFA;

        var initialState = NFA.GetInitialState();
        if (initialState is null)
            return null;

        var DFA = new Automaton();
        var alpha = NFA.GetSymbolNames();

        // Add all symbols to DFA
        foreach (var symbol in alpha)
            DFA.AddSymbol(char.Parse(symbol));

        // Key: sorted comma-joined state names -> DFA State
        // e.g "q0,q1" -> State("q0,q1", false, true)
        var stateMap = new Dictionary<string, State>();

        // Queue of NFA state-sets still needing to be processed
        var queue = new Queue<List<State>>();

        // --- Initialize with the NFA initial state ---
        var initialSet = new List<State> { initialState };
        var initialDFAState = new State(GetKey(initialSet), true, initialState.IsEnd);

        DFA.AddState(initialDFAState);
        stateMap[GetKey(initialSet)] = initialDFAState;
        queue.Enqueue(initialSet);

        // --- Process each subset ---
        while (queue.Count > 0)
        {
            var currentSet = queue.Dequeue();
            var currentKey = GetKey(currentSet);
            var currentDFAState = stateMap[currentKey];

            foreach (var symbol in alpha)
            {
                var c = char.Parse(symbol);

                // Union of all transitions from each NFA state in currentSet
                var nextSet = new List<State>();

                foreach (var nfaState in currentSet)
                {
                    var reachable = NFA.GetTransition(nfaState, c);

                    if (reachable is null)
                        continue;

                    foreach (var s in reachable)
                    {
                        // s could be null since GetTransition returns List<State?>?
                        if (s is null)
                            continue;

                        // Avoid duplicates
                        if (!nextSet.Any(x => x.Name == s.Name))
                            nextSet.Add(s);
                    }
                }

                // No reachable states = dead transition, skip
                if (nextSet.Count == 0)
                    continue;

                var nextKey = GetKey(nextSet);

                // New subset never seen before -> create new DFA state
                if (!stateMap.ContainsKey(nextKey))
                {
                    // IsEnd = true if ANY NFA state in the set is an end state
                    bool isEnd = nextSet.Any(s => s.IsEnd);
                    var nextDFAState = new State(nextKey, false, isEnd);

                    DFA.AddState(nextDFAState);
                    stateMap[nextKey] = nextDFAState;

                    queue.Enqueue(nextSet);
                }

                // Add transition: currentDFAState --c--> nextDFAState
                DFA.AddTransition(new Transition(currentDFAState, stateMap[nextKey], c));
            }
        }

        return DFA;
    }

    // Sorted so that {q1,q0} and {q0,q1} produce the same key
    private static string GetKey(List<State> states)
    {
        var sorted = states
            .Select(s => s.Name)
            .OrderBy(n => n);

        return string.Join(',', sorted);
    }
}
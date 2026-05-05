using THL_Project.Models;

namespace THL_Project.Logic;

public static class AutomatonDeterminizer
{
    public static Automaton? NFAtoDFA(Automaton NFA)
    {
        // If the NFA is already a DFA (every state has exactly one transition per symbol),
        // no conversion needed — just return it as-is
        if (NFA.IsDeterministic())
            return NFA;

        // Find the starting state of the NFA (e.g. q0)
        var initialState = NFA.GetInitialState();

        // If there's no initial state, the automaton is broken — return null
        if (initialState is null)
            return null;

        // Create a brand new empty DFA that we'll build from scratch
        var DFA = new Automaton();

        // Get all the symbols in the alphabet (e.g. ["a", "b"])
        // We'll use this to iterate over every possible input symbol
        var alpha = NFA.GetSymbolNames();

        // Copy every symbol from the NFA into the DFA
        // The alphabet doesn't change, only the states and transitions do
        foreach (var symbol in alpha)
            DFA.AddSymbol(char.Parse(symbol));

        // This dictionary maps a "super state key" (e.g. "q0,q1") to its actual DFA State object
        // It serves two purposes:
        //   1. Lets us check if we've already created a DFA state for a given NFA subset
        //   2. Lets us look up the DFA State object by key when adding transitions
        var stateMap = new Dictionary<string, State>();

        // This queue holds NFA state-sets that still need to be processed
        // We discover new sets as we go, so we need somewhere to store pending work
        var queue = new Queue<List<State>>();

        // -----------------------------------------------------------------------
        // INITIALIZATION: set up the starting super state
        // -----------------------------------------------------------------------

        // Wrap the NFA's initial state in a list — this is our first "super state"
        // e.g. if NFA starts at q0, our first DFA super state is {q0}
        var initialSet = new List<State> { initialState };

        // Create the DFA state for this initial super state
        // Name = "q0" (the key), IsInitial = true, IsEnd = same as the NFA's initial state
        var initialDFAState = new State(GetKey(initialSet), true, initialState.IsEnd);

        // Register this state in the DFA
        DFA.AddState(initialDFAState);

        // Also register it in our lookup dictionary so we can find it later by key
        stateMap[GetKey(initialSet)] = initialDFAState;

        // Push the initial set into the queue — it's the first thing we need to process
        queue.Enqueue(initialSet);

        // -----------------------------------------------------------------------
        // MAIN LOOP: process each super state until none are left
        // -----------------------------------------------------------------------

        while (queue.Count > 0)
        {
            // Take the next unprocessed NFA state-set from the front of the queue
            var currentSet = queue.Dequeue();

            // Compute its string key (e.g. "q0,q1") for dictionary lookups
            var currentKey = GetKey(currentSet);

            // Retrieve the DFA State object that corresponds to this set
            var currentDFAState = stateMap[currentKey];

            // Now process every symbol in the alphabet for this super state
            foreach (var symbol in alpha)
            {
                // Convert the symbol string (e.g. "a") to a char for transition lookup
                var c = char.Parse(symbol);

                // This will collect all NFA states reachable from currentSet on symbol c
                // It's the "union" of transitions — we ask every NFA state in the set
                // where it can go on c, then merge all answers together
                var nextSet = new List<State>();

                // Ask each NFA state in the current set: "where can you go on symbol c?"
                foreach (var nfaState in currentSet)
                {
                    // Get all states reachable from this single NFA state on symbol c
                    // Returns null if there are no transitions at all (not even an empty list)
                    var reachable = NFA.GetTransition(nfaState, c);

                    // This NFA state has no transition on c — skip it
                    if (reachable is null)
                        continue;

                    // Loop through each reachable state
                    foreach (var s in reachable)
                    {
                        // GetTransition returns List<State?>? so individual states can be null
                        // Skip null entries defensively
                        if (s is null)
                            continue;

                        // Only add this state to nextSet if it's not already in there
                        // This prevents duplicates (e.g. two NFA states both going to q2)
                        if (!nextSet.Any(x => x.Name == s.Name))
                            nextSet.Add(s);
                    }
                }

                // If no NFA states are reachable on this symbol, there's no transition to add
                // In a full DFA you'd add a "dead state" here, but this implementation skips it
                if (nextSet.Count == 0)
                    continue;

                // Compute the key for this new super state (e.g. "q1,q2")
                var nextKey = GetKey(nextSet);

                // Check if we've seen this super state before
                if (!stateMap.ContainsKey(nextKey))
                {
                    // This is a brand new super state we've never encountered before

                    // It's an accepting state if ANY of the NFA states inside it is accepting
                    // (because if the NFA *could* end in an accepting state, so can the DFA)
                    bool isEnd = nextSet.Any(s => s.IsEnd);

                    // Create the new DFA state
                    // IsInitial = false because only the first state is initial
                    var nextDFAState = new State(nextKey, false, isEnd);

                    // Register it in the DFA
                    DFA.AddState(nextDFAState);

                    // Register it in our lookup dictionary
                    stateMap[nextKey] = nextDFAState;

                    // Add it to the queue so we process its outgoing transitions later
                    queue.Enqueue(nextSet);
                }

                // Add the transition: currentDFAState --c--> the DFA state for nextKey
                // We look up nextKey in stateMap because it's guaranteed to exist by now
                // (either it already existed, or we just created it above)
                DFA.AddTransition(new Transition(currentDFAState, stateMap[nextKey], c));
            }
        }

        // All super states have been processed — the DFA is complete
        return DFA;
    }

    // Produces a canonical string key for a set of NFA states
    // Sorting is critical: {q1,q0} and {q0,q1} must produce the SAME key "q0,q1"
    // Otherwise we'd create duplicate DFA states for the same set
    private static string GetKey(List<State> states)
    {
        var sorted = states
            .Select(s => s.Name)   // grab just the name string of each state
            .OrderBy(n => n);      // sort alphabetically so order doesn't matter

        // Join them with commas: ["q0", "q1"] -> "q0,q1"
        return string.Join(',', sorted);
    }
}
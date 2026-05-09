using System;
using System.Collections.Generic;
using System.Linq;
using THL_Project.Models;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;

namespace THL_Project.Logic
{
    public class AutomatonController
    {
        private Automaton automaton;

        // MSAGL graph styling constants
        private static readonly Microsoft.Msagl.Drawing.Color GraphNodeFill = new Microsoft.Msagl.Drawing.Color(255, 24, 24, 31);
        private static readonly Microsoft.Msagl.Drawing.Color GraphNodeBorder = Microsoft.Msagl.Drawing.Color.White;
        private static readonly Microsoft.Msagl.Drawing.Color GraphNodeText = Microsoft.Msagl.Drawing.Color.White;
        private static readonly Microsoft.Msagl.Drawing.Color GraphInitialState = Microsoft.Msagl.Drawing.Color.DarkGreen;
        private static readonly Microsoft.Msagl.Drawing.Color GraphFinalState = Microsoft.Msagl.Drawing.Color.Cyan;
        private static readonly Microsoft.Msagl.Drawing.Color GraphEdgeLine = Microsoft.Msagl.Drawing.Color.Black;
        private static readonly Microsoft.Msagl.Drawing.Color GraphEdgeText = Microsoft.Msagl.Drawing.Color.Black;

        public AutomatonController()
        {
            automaton = new Automaton();
        }

        /// <summary>
        /// Attempts to add a symbol to the alphabet. Returns (success, errorMessage).
        /// </summary>
        public (bool Success, string Error) AddSymbol(char symbol)
        {
            if (automaton.GetSymbolNames().Contains(symbol.ToString()))
                return (false, $"'{symbol}' is already in the alphabet!");

            automaton.AddSymbol(symbol);
            return (true, string.Empty);
        }

        /// <summary>
        /// Attempts to add a new state. Returns (success, errorMessage).
        /// </summary>
        public (bool Success, string Error) AddState(string name, bool isInitial, bool isFinal)
        {
            if (automaton.GetStateNames().Contains(name))
                return (false, $"\"{name}\" is already in the Automaton");

            if (isInitial && automaton.InitExist())
                return (false, "There already exists an Initial State");

            automaton.AddState(new State(name, isInitial, isFinal));
            return (true, string.Empty);
        }

        /// <summary>
        /// Adds a transition from one state to another on a symbol. Returns (success, errorMessage).
        /// </summary>
        /// FirstOrDefault returns the 1st element that satisfies the condition 
        public (bool Success, string Error) AddTransition(string fromName, char symbol, string toName)
        {
            var from = automaton.GetStates().FirstOrDefault(s => s.Name == fromName);
            var to = automaton.GetStates().FirstOrDefault(s => s.Name == toName);

            if (from == null || to == null)
                return (false, "Error locating states.");

            automaton.AddTransition(new Transition(from, to, symbol));
            return (true, string.Empty);
        }

        /// <summary>
        /// Returns all alphabet symbols as strings.
        /// </summary>
        public List<string> GetSymbolNames() => automaton.GetSymbolNames().ToList();

        /// <summary>
        /// Returns all state names.
        /// </summary>
        public List<string> GetStateNames() => automaton.GetStateNames().ToList();

        /// <summary>
        /// Returns all states (for UI iteration).
        /// </summary>
        public IEnumerable<State> GetStates() => automaton.GetStates();

        /// <summary>
        /// Returns the target state names for a given state and symbol.
        /// </summary>
        public List<string> GetTargetStateNames(string fromStateName, char symbol)
        {
            var from = automaton.GetStates().FirstOrDefault(s => s.Name == fromStateName);
            if (from == null) return new List<string>();
            var targets = automaton.GetTransition(from, symbol);
            return targets?.Select(s => s.Name).ToList() ?? new List<string>();
        }

        /// <summary>
        /// Checks whether the automaton is deterministic.
        /// </summary>
        public bool IsDeterministic() => automaton.IsDeterministic();

        /// <summary>
        /// Converts the automaton to DFA using the subset construction.
        /// </summary>
        public void ConvertToDFA()
        {
            automaton = AutomatonDeterminizer.NFAtoDFA(automaton);
        }

        /// <summary>
        /// Recognizes a word. Returns (accepted, rejectionReason).
        /// </summary>
        public (bool Accepted, string Reason) RecognizeWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                // Check if empty string is accepted
                var start = automaton.GetStates().FirstOrDefault(s => s.IsInitial);
                if (start != null && start.IsEnd)
                    return (true, "Empty Word is Accepted");
                else
                    return (false, "Empty Word is Rejected");
            }

            var recognizer = new AutomatonRecognizer(automaton);
            var result = recognizer.RecognizeWord(word);

            if (result.Accepted)
                return (true, "Word is Accepted");
            else
                return (false, result.RejectionReason);
        }

        /// <summary>
        /// Resets the automaton to an empty state.
        /// </summary>
        public void Reset()
        {
            automaton = new Automaton();
        }

        /// <summary>
        /// Builds and returns an MSAGL (Microsoft Automatic Graph Layout) Graph object
        /// that visually represents the current finite automaton, including all its
        /// states (nodes) and transitions (edges/arrows between nodes).
        /// 
        /// This graph can then be rendered in a UI control (like GViewer) to display
        /// the automaton diagram to the user.
        /// </summary>
        public Graph BuildGraph()
        {
            // =========================================================================
            // STEP 1: CREATE THE GRAPH CONTAINER
            // =========================================================================
            // Create a new empty MSAGL Graph object with a descriptive title.
            // Think of this as creating a blank canvas where we will draw all the
            // automaton's states and transitions.
            Graph graph = new Graph("Automaton Graph");

            // =========================================================================
            // STEP 2: CONFIGURE THE LAYOUT ALGORITHM
            // =========================================================================
            // MSAGL uses layout algorithms to automatically position nodes and route
            // edges on the canvas. Here we use "Sugiyama" layout, which is a
            // hierarchical layout algorithm — it arranges nodes in layers/levels,
            // which works well for automata diagrams.
            var layoutSettings = new SugiyamaLayoutSettings();

            // --- Edge Routing Mode ---
            // By default, MSAGL might draw edges as straight lines. The problem with
            // straight lines is that when two states have transitions going in BOTH
            // directions (e.g., State A → State B AND State B → State A), the two
            // arrows would overlap and become impossible to distinguish.
            //
            // Setting the routing mode to "Spline" forces edges to be drawn as smooth
            // curved lines, which means bidirectional arrows will curve away from each
            // other and both remain clearly visible.
            layoutSettings.EdgeRoutingSettings.EdgeRoutingMode =
                Microsoft.Msagl.Core.Routing.EdgeRoutingMode.Spline;

            // Apply these layout settings to our graph so they take effect when the
            // graph is rendered.
            graph.LayoutAlgorithmSettings = layoutSettings;

            // =========================================================================
            // STEP 3: ADD NODES (STATES) TO THE GRAPH
            // =========================================================================
            // Every state in the automaton becomes a "node" (a circle) in the graph.
            // We iterate over all states and create a corresponding visual node for each.
            foreach (var state in automaton.GetStates())
            {
                // --- Create the Node ---
                // graph.AddNode() creates a new node with the given label (the state's name).
                // If a node with this name already exists, MSAGL returns the existing one.
                // The state's Name (e.g., "q0", "q1") will be displayed inside the circle.
                var node = graph.AddNode(state.Name);

                // --- Default Visual Styling ---
                // Apply the default fill (background) color inside the node circle.
                // GraphNodeFill is a predefined color constant for regular/neutral states.
                node.Attr.FillColor = GraphNodeFill;

                // Set the border/outline color of the node circle.
                // GraphNodeBorder is a predefined color for the circle's outline stroke.
                node.Attr.Color = GraphNodeBorder;

                // Set the color of the text label displayed inside the node.
                // GraphNodeText is a predefined color for the state name text.
                node.Label.FontColor = GraphNodeText;

                // Set the default shape for all nodes to a simple circle.
                // In automata theory, states are conventionally represented as circles.
                node.Attr.Shape = Shape.Circle;

                // --- Special Styling: Final/Accepting States ---
                // In automata theory, "accepting" or "final" states are visually represented
                // with a DOUBLE circle (a circle inside another circle).
                // If this state is marked as an end/accepting state, we override the shape.
                if (state.IsEnd)
                {
                    // Switch to DoubleCircle shape to follow the standard automata notation
                    // for final/accepting states (the inner circle indicates acceptance).
                    node.Attr.Shape = Shape.DoubleCircle;

                    // Also change the border color to a special color to make final states
                    // even more visually distinct from regular states.
                    // GraphFinalState is a predefined color for final state borders.
                    node.Attr.Color = GraphFinalState;
                }

                // --- Special Styling: Initial/Start State ---
                // In automata theory, the start state is the one where processing begins.
                // We highlight it with a different FILL color to make it immediately
                // recognizable as the entry point of the automaton.
                // Note: Conventionally, an arrow pointing to the start state is also drawn,
                // but here we use a distinct fill color as an alternative visual cue.
                if (state.IsInitial)
                {
                    // Override the fill color with the special "initial state" color.
                    // GraphInitialState is a predefined color constant for the start state.
                    node.Attr.FillColor = GraphInitialState;
                }

                // NOTE: A state can be BOTH initial AND final at the same time (e.g., in
                // an automaton that accepts the empty string). In that case, both the
                // DoubleCircle shape AND the initial fill color will be applied together.
            }

            // =========================================================================
            // STEP 4: ADD EDGES (TRANSITIONS) TO THE GRAPH
            // =========================================================================
            // Every transition in the automaton becomes an "edge" (an arrow) in the graph.
            // A transition means: "When in state X and reading symbol Y, move to state Z."
            // We iterate over all states and all symbols to find every possible transition.
            foreach (var fromState in automaton.GetStates())
            {
                // --- Iterate Over Every Symbol in the Alphabet ---
                // The automaton's alphabet is the set of all valid input symbols (e.g., 'a', 'b').
                // GetSymbolNames() returns them as strings (e.g., "a", "b", "0", "1").
                foreach (var symbolStr in automaton.GetSymbolNames())
                {
                    // --- Extract the Transition Symbol as a char ---
                    // The symbol is stored as a string (e.g., "a"), but GetTransition()
                    // expects a char, so we take the first character of the string.
                    // This assumes each symbol is a single character (standard for simple automata).
                    char symbol = symbolStr[0];

                    // --- Look Up the Transition ---
                    // GetTransition() checks the automaton's transition function (delta):
                    //   δ(fromState, symbol) → set of target states
                    //
                    // For a DFA (Deterministic Finite Automaton), this returns at most one state.
                    // For an NFA (Non-deterministic Finite Automaton), this can return multiple states.
                    // If there is NO transition defined for this (state, symbol) pair, it returns null.
                    var targetStates = automaton.GetTransition(fromState, symbol);

                    // --- Only Draw an Edge if the Transition Exists ---
                    // If targetStates is null, there is no transition for this symbol from this state,
                    // so we skip it (no arrow to draw — the automaton simply rejects if this input
                    // is encountered in this state).
                    if (targetStates != null)
                    {
                        // --- Handle Multiple Target States (NFA Support) ---
                        // In an NFA, one transition can lead to multiple states simultaneously.
                        // Each target state gets its own separate arrow/edge drawn on the graph.
                        foreach (var toState in targetStates)
                        {
                            // --- Create the Edge (Arrow) ---
                            // graph.AddEdge() draws a directed arrow from fromState to toState,
                            // with symbolStr displayed as the label on the arrow.
                            //
                            // Parameters:
                            //   fromState.Name → the label of the source node (e.g., "q0")
                            //   symbolStr      → the transition symbol displayed on the arrow (e.g., "a")
                            //   toState.Name   → the label of the destination node (e.g., "q1")
                            //
                            // This represents the transition: δ(fromState, symbol) = toState
                            var edge = graph.AddEdge(fromState.Name, symbolStr, toState.Name);

                            // --- Style the Edge Arrow Line ---
                            // Set the color of the arrow line itself.
                            // GraphEdgeLine is a predefined color constant for transition arrows.
                            edge.Attr.Color = GraphEdgeLine;

                            // --- Style the Edge Label Text ---
                            // Set the color of the symbol label displayed on/near the arrow.
                            // GraphEdgeText is a predefined color constant for transition labels.
                            edge.Label.FontColor = GraphEdgeText;
                        }
                    }
                }
            }

            // =========================================================================
            // STEP 5: RETURN THE COMPLETED GRAPH
            // =========================================================================
            // At this point, the graph object contains:
            //   ✔ All states as styled nodes (circles / double-circles)
            //   ✔ All transitions as styled directed edges (curved arrows with labels)
            //   ✔ Layout settings for proper automatic positioning
            //
            // The caller can pass this graph object to an MSAGL viewer control
            // (e.g., GViewer.Graph = BuildGraph()) to display the automaton diagram.
            return graph;
        }
    }
}
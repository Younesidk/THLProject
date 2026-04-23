using THL_Project.Models;

namespace THL_Project.Logic;

public class AutomatonRecognizer
{
    private readonly Automaton automaton;

    public AutomatonRecognizer(Automaton automaton)
    {
        this.automaton = automaton;
    }

    public RecognitionResult RecognizeWord(string word)
    {
        var stepLog = new List<string>();

        try
        {
            if (word is null)
            {
                return BuildRejectedResult("Word cannot be null", stepLog);
            }

            var currentState = automaton.GetInitialState();
            if (currentState is null)
            {
                return BuildRejectedResult("No initial state defined", stepLog);
            }

            for (int i = 0; i < word.Length; i++)
            {
                char symbol = word[i];
                if (!automaton.IsValidSymbol(symbol))
                {
                    return BuildRejectedResult($"Invalid symbol: {symbol}", stepLog);
                }

                var nextStates = automaton.GetTransition(currentState, symbol);
                if (nextStates is null || nextStates.Count == 0 || nextStates[0] is null)
                {
                    return BuildRejectedResult($"Blocked at state {currentState.Name} on symbol {symbol}", stepLog);
                }

                var nextState = nextStates[0]!;
                stepLog.Add($"{currentState.Name} --{symbol}--> {nextState.Name}");
                currentState = nextState;
            }

            var finalStates = automaton.GetFinalStates();
            bool isFinal = false;
            for (int i = 0; i < finalStates.Count; i++)
            {
                if (finalStates[i].Name == currentState.Name)
                {
                    isFinal = true;
                    break;
                }
            }

            if (isFinal)
            {
                return new RecognitionResult
                {
                    Accepted = true,
                    RejectionReason = null,
                    StepLog = stepLog
                };
            }

            return BuildRejectedResult($"Ended in non-final state {currentState.Name}", stepLog);
        }
        catch (Exception ex)
        {
            return BuildRejectedResult(ex.Message, stepLog);
        }
    }

    private static RecognitionResult BuildRejectedResult(string reason, List<string> stepLog)
    {
        return new RecognitionResult
        {
            Accepted = false,
            RejectionReason = reason,
            StepLog = stepLog
        };
    }
}

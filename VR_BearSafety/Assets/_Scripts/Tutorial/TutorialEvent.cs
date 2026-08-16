using System;

public static class TutorialEvent 
{
    public static event Action<TutorialStep> ActionPerformed;

    public static void ReportAction(TutorialStep action)
    {
        ActionPerformed?.Invoke(action);
    }
}

namespace System.Threading
{
  public static class SynchronisationContextExtensions
  {
    /// <summary>
    /// Post an action to the <see cref="SynchronizationContext"/>.
    /// </summary>
    /// <param name="syncContext"></param>
    /// <param name="callback"></param>
    public static void BeginInvoke(this SynchronizationContext syncContext, Action callback)
    {
      syncContext.Post(o => { callback?.Invoke(); }, null);
    }

    /// <summary>
    /// Post a delegate callback with parameters to the <see cref="SynchronizationContext"/>.
    /// </summary>
    /// <param name="syncContext"></param>
    /// <param name="callback"></param>
    /// <param name="args"></param>
    public static void BeginInvoke(this SynchronizationContext syncContext, Delegate callback, params object[] args)
    {
      syncContext.Post(o => { callback?.DynamicInvoke(args); }, args);
    }

    /// <summary>
    /// Check if the current context is the same as the <see cref="SynchronizationContext"/> provided.
    /// True if same context, else false if an invoke is required.
    /// </summary>
    /// <param name="syncContext"></param>
    /// <returns></returns>
    public static bool CheckAccess(this SynchronizationContext syncContext)
    {
      return (SynchronizationContext.Current == syncContext);
    }

    /// <summary>
    /// Send an action to a <see cref="SynchronizationContext"/> to invoke immediately.
    /// </summary>
    /// <param name="syncContext"></param>
    /// <param name="callback"></param>
    public static void Invoke(this SynchronizationContext syncContext, Action callback)
    {
      if (callback != null)
      {
        if (syncContext.CheckAccess())
        {
          callback();
        }
        else
        {
          syncContext.Send(o => { callback(); }, null);
        }
      }
    }

    /// <summary>
    /// Send a delegate to a <see cref="SynchronizationContext"/> to invoke immediately.
    /// </summary>
    /// <param name="syncContext"></param>
    /// <param name="callback"></param>
    /// <param name="args"></param>
    public static void Invoke(this SynchronizationContext syncContext, Delegate callback, params object[] args)
    {
      if (callback != null)
      {
        if (syncContext.CheckAccess())
        {
          callback.DynamicInvoke(args);
        }
        else
        {
          syncContext.Send(o => { callback.DynamicInvoke(args); }, args);
        }
      }
    }

    /// <summary>
    /// Send a function with a return type to a <see cref="SynchronizationContext"/> to
    /// invoke immediately and return the result.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="syncContext"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static TResult Invoke<TResult>(this SynchronizationContext syncContext, Func<TResult> callback)
    {
      TResult retVal = default(TResult);
      if (callback != null)
      {
        if (syncContext.CheckAccess())
        {
          callback();
        }
        else
        {
          syncContext.Send(o => { retVal = callback(); }, retVal);
        }
      }
      return retVal;
    }
  }
}
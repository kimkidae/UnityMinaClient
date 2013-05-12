using UnityEngine;
using System.Collections;
using log4net.Layout;
using log4net.Core;
using log4net.Appender;
using System;

public class ValionLogAppender : MemoryAppender
{
  private int maximumEntries;

  public ValionLogAppender()
  {
    maximumEntries = 600;
  }

  public int MaximumEntries
  {
    get
    {
      return this.maximumEntries;
    }
    set
    {
      this.maximumEntries = Math.Max (0, value);
    }
  }

  protected override void Append (LoggingEvent loggingEvent)
  {
    try
    {
      base.Append (loggingEvent);
      if (maximumEntries == 0)
      {
        return;
      }

      lock (m_eventsList.SyncRoot)
      {
        int elementsToRemove = m_eventsList.Count - maximumEntries;
        if (elementsToRemove > 0)
        {
          UnityEngine.Debug.Log ("Removing " + elementsToRemove + " elements.");
          m_eventsList.RemoveRange (0, elementsToRemove);
        }
      }
    }
    catch(Exception e)
    {
      UnityEngine.Debug.LogError(e);
    }
  }
}

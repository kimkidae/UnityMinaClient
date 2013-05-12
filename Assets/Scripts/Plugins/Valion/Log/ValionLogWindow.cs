using System;
using System.Text;
using UnityEngine;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

public class ValionLogWindow: MonoBehaviour
{
  private static ILog logger = LogManager.GetLogger (typeof(ValionLogWindow));
  public Rect logWindowPosition;
  public bool visible;
  private ValionLogAppender logAppender;
  private StringBuilder buffer;
  private Vector2 scrollPosition;

  public ValionLogWindow ()
  {
    buffer = new StringBuilder ();
  }

  public void Awake ()
  {
    if (logWindowPosition.width < 20 || logWindowPosition.height < 20)
    {
      logWindowPosition = new Rect (0, 0, 500, 400);
    }
  }

  public void Update ()
  {
    if (Input.GetKeyUp (KeyCode.F12))
    {
      visible = !visible;
      if (logger.IsDebugEnabled)
      {
        logger.Debug ("Log Window is visible: " + visible);
      }
    }
  }

  public ValionLogAppender GetAppender ()
  {
    if (logAppender == null)
    {
      Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository ();
      IAppender[] appenders = hierarchy.GetAppenders ();
      foreach (IAppender appender in appenders)
      {
        if (appender is ValionLogAppender)
        {
          logAppender = (ValionLogAppender)appender;
          break;
        }
      }
    }
    return logAppender;
  }

  public void OnGUI ()
  {
    if (visible)
    {
      GUI.Window (9999, logWindowPosition, DrawLogWindow, "Log Window");
    }
  }

  public void DrawLogWindow (int windowId)
  {
    buffer.Remove (0, buffer.Length);

    ValionLogAppender logAppender = GetAppender ();
    LoggingEvent[] events = (logAppender == null) ? new LoggingEvent[0] : logAppender.GetEvents ();
    foreach (LoggingEvent ev in events)
    {
      buffer.Append (ev.RenderedMessage);
      buffer.Append(ev.GetExceptionString());
      buffer.AppendLine ();
    }

    float textHeight = Math.Max(logWindowPosition.height - 25, events.Length * 20);
    float textWidth = logWindowPosition.width - 15;

    scrollPosition = GUI.BeginScrollView (new Rect (10, 20, textWidth, logWindowPosition.height - 25), scrollPosition, new Rect (0, 0, textWidth, textHeight));
    GUI.TextArea (new Rect (0, 20, textWidth, textHeight), buffer.ToString ());
    GUI.EndScrollView();

    GUI.DragWindow ();

  }
}


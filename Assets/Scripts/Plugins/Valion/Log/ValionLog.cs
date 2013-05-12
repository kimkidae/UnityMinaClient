using System;
using System.IO;
using log4net.Config;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

using UnityEngine;
using log4net;

[AddComponentMenu("Valion/Log Manager")]
public class ValionLog : MonoBehaviour
{
  private static ValionLog instance = null;

  public static ValionLog Instance
  {
    get
    {
      if (instance == null)
      {
        GameObject go = new GameObject ();
        go.name = "Log Manager";
        instance = go.AddComponent<ValionLog> ();
        instance.Configure();
      }
      
      return instance;
    }
  }

  public string ApplicationName;
  public bool configureFromFile;

  private bool configured;
  private ILog logger;

  public ValionLog ()
  {
    ApplicationName = "Valion";
  }

  public void Debug (string text)
  {
    if (logger == null)
    {
      logger = LogManager.GetLogger (typeof(ValionLog));
    }
    logger.Debug (text);
  }

  public void Debug (string text, Exception e)
  {
    if (logger == null)
    {
      logger = LogManager.GetLogger(typeof (ValionLog));
    }
    logger.Debug (text, e);
  }

  public void Awake ()
  {
    instance = this;
    Configure();
  }

  public void Configure()
  {
    if (configured)
      return;

    if (configureFromFile)
    {
      try
      {
        TextAsset textAsset = (TextAsset) Resources.Load("logging", typeof(TextAsset));
        if( textAsset != null )
        {
          MemoryStream stream = new MemoryStream(textAsset.bytes);

          XmlConfigurator.Configure(stream);
          configured = true;
          return;
        }

        FileInfo fileInfo = new FileInfo("./logging.xml");

        if (fileInfo.Exists)
        {
          XmlConfigurator.Configure(fileInfo);
          configured = true;
          return;
        }
      }
      catch (Exception e)
      {
        UnityEngine.Debug.LogError("Failed to configure logging from file:" + e.Message);
      }
    }

    UnityEngine.Debug.Log ("Initializing Log4Net");

    configured = true;

    Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository ();
    hierarchy.Shutdown();
    hierarchy.ResetConfiguration();
    hierarchy.Root.Level = Level.Debug;

    ConfigureAppenders();

    ILog log = LogManager.GetLogger (System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType);
    log.Info ("Logging system started");
    log.Info ("Operating-System: " + SystemInfo.operatingSystem);
    log.Info ("     System Spec: " + SystemInfo.processorType + " - #" + SystemInfo.processorCount + " (" + SystemInfo.systemMemorySize + ")");
    log.Info ("     Device Spec: " + SystemInfo.deviceName + " " + SystemInfo.deviceModel + " [" + SystemInfo.deviceUniqueIdentifier + "]");
    log.Info ("    Graphic Spec: " + SystemInfo.graphicsDeviceID + ":" + SystemInfo.graphicsDeviceName + " - "
     + SystemInfo.graphicsDeviceVendorID + ":" + SystemInfo.graphicsDeviceVendor + " (" + SystemInfo.graphicsMemorySize + "MB, Shader Level " + SystemInfo.graphicsShaderLevel + ")");
  }



  private void ConfigureAppenders ()
  {
    PatternLayout layout = new PatternLayout();
    layout.ConversionPattern = "%date [%thread] %-5level %logger - %message%n";
    layout.ActivateOptions();

    ValionLogAppender logAppender = new ValionLogAppender ();
    logAppender.MaximumEntries = 600;
    logAppender.Threshold = Level.Debug;
    logAppender.Layout = layout;
    logAppender.ActivateOptions();
    
    OSType osType = GetOperatingSystem ();
    if (osType == OSType.WebPlayer)
    {
      UnityEngine.Debug.Log ("Detected Web-Player, only using appended logger");
      BasicConfigurator.Configure(logAppender);
      return;
    }

    ConsoleAppender consoleAppener = new ConsoleAppender();
    consoleAppener.Layout = layout;
    consoleAppener.Threshold = Level.Debug;
    consoleAppener.Target = ConsoleAppender.ConsoleOut; 
    consoleAppener.ActivateOptions();

#if UNITY_WEBPLAYER
    BasicConfigurator.Configure (logAppender, consoleAppener);
#else
    PatternLayout fileLayout = new PatternLayout ();
    fileLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%n";
    fileLayout.ActivateOptions ();

    RollingFileAppender fileAppender = new RollingFileAppender ();
    fileAppender.AppendToFile = true;
    fileAppender.MaxFileSize = 2048 * 1024;
    fileAppender.MaxSizeRollBackups = 10;
    fileAppender.RollingStyle = RollingFileAppender.RollingMode.Composite;
    fileAppender.Threshold = Level.Debug;
    fileAppender.File = GetLogDirectory(osType) + "/log.txt";
    fileAppender.Layout = fileLayout;
    fileAppender.PreserveLogFileNameExtension = true;
    fileAppender.ImmediateFlush = true;
    fileAppender.ActivateOptions();

    UnityEngine.Debug.Log ("Detected " + osType + ", using file Logging: " + fileAppender.File);
    BasicConfigurator.Configure (fileAppender, logAppender, consoleAppener);
#endif
  }

#region GuessLogDirectory
  private string GetLogDirectory (OSType type)
  {
    string homeDirectory = Environment.GetEnvironmentVariable("HOME");
    if (String.IsNullOrEmpty(homeDirectory))
    {
      homeDirectory = Environment.GetEnvironmentVariable("TMP");
      if (String.IsNullOrEmpty(homeDirectory))
      {
        homeDirectory = ".";
      }
    }
    if (String.IsNullOrEmpty(ApplicationName))
    {
      ApplicationName = "Unity3D-Project";
    }

    switch(type)
    {
      case OSType.MacOS:
          {
            return homeDirectory + "/Library/Logs/" +  ApplicationName;
          }
      case OSType.WindowsXP:
          {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + ApplicationName + "/Logs";
          }
      case OSType.WindowsVista7:
          {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/" + ApplicationName + "/Logs";
          }
    }

    // Unix and all others ...
    return homeDirectory + "/." + ApplicationName;
  }

  public enum OSType
  {
    WebPlayer,
    WindowsXP,
    WindowsVista7,
    MacOS,
    Unix
  }

  public OSType GetOperatingSystem ()
  {
    OperatingSystem os = Environment.OSVersion;
    PlatformID pid = os.Platform;
    switch (pid)
    {
      case PlatformID.Win32NT:
      case PlatformID.Win32S:
      case PlatformID.Win32Windows:
      case PlatformID.WinCE:
        break;
    
      case PlatformID.Unix:
      {
        // Mono is broken here ...
        string osTypeVar = SystemInfo.operatingSystem;
        if (osTypeVar.StartsWith("Mac OS"))
        {
          return OSType.MacOS;
        }
        return OSType.Unix;
      }
      case PlatformID.MacOSX:
        return OSType.MacOS;
      case PlatformID.Xbox:
        return OSType.WebPlayer;
      default:
        return OSType.WebPlayer;
    }
    
    if (os.Version.Major >= 6)
      return OSType.WindowsVista7;
    
    return OSType.WindowsXP;
  }

#endregion

  public void OnApplicationQuit()
  {
    Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository ();
    hierarchy.Shutdown ();
    UnityEngine.Debug.Log("On Application Quit Called");
  }

  public void OnDestroy()
  {
    Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository ();
    hierarchy.Shutdown ();
    UnityEngine.Debug.Log ("On Destroy Called");
  }
}



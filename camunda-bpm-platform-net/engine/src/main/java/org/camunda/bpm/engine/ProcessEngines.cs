using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine
{
	using ProcessEngineInfoImpl = org.camunda.bpm.engine.impl.ProcessEngineInfoImpl;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;



	/// <summary>
	/// Helper for initializing and closing process engines in server environments.
	/// <br>
	/// All created <seealso cref="ProcessEngine"/>s will be registered with this class.
	/// <br>
	/// The activiti-webapp-init webapp will
	/// call the <seealso cref="init()"/> method when the webapp is deployed and it will call the
	/// <seealso cref="destroy()"/> method when the webapp is destroyed, using a context-listener
	/// (<code>org.camunda.bpm.engine.test.impl.servlet.listener.ProcessEnginesServletContextListener</code>).  That way,
	/// all applications can just use the <seealso cref="getProcessEngines()"/> to
	/// obtain pre-initialized and cached process engines. <br>
	/// <br>
	/// Please note that there is <b>no lazy initialization</b> of process engines, so make sure the
	/// context-listener is configured or <seealso cref="ProcessEngine"/>s are already created so they were registered
	/// on this class.<br>
	/// <br>
	/// The <seealso cref="init()"/> method will try to build one <seealso cref="ProcessEngine"/> for
	/// each camunda.cfg.xml file found on the classpath.  If you have more then one,
	/// make sure you specify different process.engine.name values.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public abstract class ProcessEngines
	{

	  private static readonly ProcessEngineLogger LOG = ProcessEngineLogger.INSTANCE;

	  public const string NAME_DEFAULT = "default";

	  protected internal static bool isInitialized = false;
	  protected internal static IDictionary<string, ProcessEngine> processEngines = new Dictionary<string, ProcessEngine>();
	  protected internal static IDictionary<string, ProcessEngineInfo> processEngineInfosByName = new Dictionary<string, ProcessEngineInfo>();
	  protected internal static IDictionary<string, ProcessEngineInfo> processEngineInfosByResourceUrl = new Dictionary<string, ProcessEngineInfo>();
	  protected internal static IList<ProcessEngineInfo> processEngineInfos = new List<ProcessEngineInfo>();

	  public static void init()
	  {
		  lock (typeof(ProcessEngines))
		  {
			init(true);
		  }
	  }

	  /// <summary>
	  /// Initializes all process engines that can be found on the classpath for
	  /// resources <code>camunda.cfg.xml</code> (plain Activiti style configuration)
	  /// and for resources <code>activiti-context.xml</code> (Spring style configuration). 
	  /// </summary>
	  public static void init(bool forceCreate)
	  {
		  lock (typeof(ProcessEngines))
		  {
			if (!isInitialized)
			{
			  if (processEngines == null)
			  {
				// Create new map to store process-engines if current map is null
				processEngines = new Dictionary<string, ProcessEngine>();
			  }
			  ClassLoader classLoader = ReflectUtil.ClassLoader;
			  IEnumerator<URL> resources = null;
			  try
			  {
				resources = classLoader.getResources("camunda.cfg.xml");
			  }
			  catch (IOException)
			  {
				try
				{
				  resources = classLoader.getResources("activiti.cfg.xml");
				}
				catch (IOException ex)
				{
				  if (forceCreate)
				  {
					throw new ProcessEngineException("problem retrieving camunda.cfg.xml and activiti.cfg.xml resources on the classpath: " + System.getProperty("java.class.path"), ex);
				  }
				  else
				  {
					return;
				  }
				}
			  }
        
			  // Remove duplicated configuration URL's using set. Some classloaders may return identical URL's twice, causing duplicate startups
			  ISet<URL> configUrls = new HashSet<URL>();
			  while (resources.MoveNext())
			  {
				configUrls.Add(resources.Current);
			  }
			  for (IEnumerator<URL> iterator = configUrls.GetEnumerator(); iterator.MoveNext();)
			  {
				URL resource = iterator.Current;
				initProcessEngineFromResource(resource);
			  }
        
			  try
			  {
				resources = classLoader.getResources("activiti-context.xml");
			  }
			  catch (IOException e)
			  {
				if (forceCreate)
				{
				  throw new ProcessEngineException("problem retrieving activiti-context.xml resources on the classpath: " + System.getProperty("java.class.path"), e);
				}
				else
				{
				  return;
				}
			  }
			  while (resources.MoveNext())
			  {
				URL resource = resources.Current;
				initProcessEngineFromSpringResource(resource);
			  }
        
			  isInitialized = true;
			}
			else
			{
			  LOG.processEngineAlreadyInitialized();
			}
		  }
	  }

	  protected internal static void initProcessEngineFromSpringResource(URL resource)
	  {
		try
		{
		  Type springConfigurationHelperClass = ReflectUtil.loadClass("org.camunda.bpm.engine.test.spring.SpringConfigurationHelper");
		  System.Reflection.MethodInfo method = springConfigurationHelperClass.GetMethod("buildProcessEngine", new Type[]{typeof(URL)});
		  ProcessEngine processEngine = (ProcessEngine) method.invoke(null, new object[]{resource});

		  string processEngineName = processEngine.Name;
		  ProcessEngineInfo processEngineInfo = new ProcessEngineInfoImpl(processEngineName, resource.ToString(), null);
		  processEngineInfosByName[processEngineName] = processEngineInfo;
		  processEngineInfosByResourceUrl[resource.ToString()] = processEngineInfo;

		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("couldn't initialize process engine from spring configuration resource " + resource.ToString() + ": " + e.Message, e);
		}
	  }

	  /// <summary>
	  /// Registers the given process engine. No <seealso cref="ProcessEngineInfo"/> will be
	  /// available for this process engine. An engine that is registered will be closed
	  /// when the <seealso cref="ProcessEngines.destroy()"/> is called.
	  /// </summary>
	  public static void registerProcessEngine(ProcessEngine processEngine)
	  {
		processEngines[processEngine.Name] = processEngine;
	  }

	  /// <summary>
	  /// Unregisters the given process engine.
	  /// </summary>
	  public static void unregister(ProcessEngine processEngine)
	  {
		processEngines.Remove(processEngine.Name);
	  }

	  private static ProcessEngineInfo initProcessEngineFromResource(URL resourceUrl)
	  {
		ProcessEngineInfo processEngineInfo = processEngineInfosByResourceUrl[resourceUrl];
		// if there is an existing process engine info
		if (processEngineInfo != null)
		{
		  // remove that process engine from the member fields
		  processEngineInfos.Remove(processEngineInfo);
		  if (string.ReferenceEquals(processEngineInfo.Exception, null))
		  {
			string processEngineName = processEngineInfo.Name;
			processEngines.Remove(processEngineName);
			processEngineInfosByName.Remove(processEngineName);
		  }
		  processEngineInfosByResourceUrl.Remove(processEngineInfo.ResourceUrl);
		}

		string resourceUrlString = resourceUrl.ToString();
		try
		{
		  LOG.initializingProcessEngineForResource(resourceUrl);
		  ProcessEngine processEngine = buildProcessEngine(resourceUrl);
		  string processEngineName = processEngine.Name;
		  LOG.initializingProcessEngine(processEngine.Name);
		  processEngineInfo = new ProcessEngineInfoImpl(processEngineName, resourceUrlString, null);
		  processEngines[processEngineName] = processEngine;
		  processEngineInfosByName[processEngineName] = processEngineInfo;
		}
		catch (Exception e)
		{
		  LOG.exceptionWhileInitializingProcessengine(e);
		  processEngineInfo = new ProcessEngineInfoImpl(null, resourceUrlString, getExceptionString(e));
		}
		processEngineInfosByResourceUrl[resourceUrlString] = processEngineInfo;
		processEngineInfos.Add(processEngineInfo);
		return processEngineInfo;
	  }

	  private static string getExceptionString(Exception e)
	  {
		StringWriter sw = new StringWriter();
		PrintWriter pw = new PrintWriter(sw);
		e.printStackTrace(pw);
		return sw.ToString();
	  }

	  private static ProcessEngine buildProcessEngine(URL resource)
	  {
		Stream inputStream = null;
		try
		{
		  inputStream = resource.openStream();
		  ProcessEngineConfiguration processEngineConfiguration = ProcessEngineConfiguration.createProcessEngineConfigurationFromInputStream(inputStream);
		  return processEngineConfiguration.buildProcessEngine();

		}
		catch (IOException e)
		{
		  throw new ProcessEngineException("couldn't open resource stream: " + e.Message, e);
		}
		finally
		{
		  IoUtil.closeSilently(inputStream);
		}
	  }

	  /// <summary>
	  /// Get initialization results. </summary>
	  public static IList<ProcessEngineInfo> ProcessEngineInfos
	  {
		  get
		  {
			return processEngineInfos;
		  }
	  }

	  /// <summary>
	  /// Get initialization results. Only info will we available for process engines
	  /// which were added in the <seealso cref="ProcessEngines.init()"/>. No <seealso cref="ProcessEngineInfo"/>
	  /// is available for engines which were registered programatically.
	  /// </summary>
	  public static ProcessEngineInfo getProcessEngineInfo(string processEngineName)
	  {
		return processEngineInfosByName[processEngineName];
	  }

	  public static ProcessEngine DefaultProcessEngine
	  {
		  get
		  {
			return getDefaultProcessEngine(true);
		  }
	  }

	  public static ProcessEngine getDefaultProcessEngine(bool forceCreate)
	  {
		return getProcessEngine(NAME_DEFAULT, forceCreate);
	  }

	  public static ProcessEngine getProcessEngine(string processEngineName)
	  {
		return getProcessEngine(processEngineName, true);
	  }

	  /// <summary>
	  /// obtain a process engine by name. </summary>
	  /// <param name="processEngineName"> is the name of the process engine or null for the default process engine.   </param>
	  public static ProcessEngine getProcessEngine(string processEngineName, bool forceCreate)
	  {
		if (!isInitialized)
		{
		  init(forceCreate);
		}
		return processEngines[processEngineName];
	  }

	  /// <summary>
	  /// retries to initialize a process engine that previously failed.
	  /// </summary>
	  public static ProcessEngineInfo retry(string resourceUrl)
	  {
		try
		{
		  return initProcessEngineFromResource(new URL(resourceUrl));
		}
		catch (MalformedURLException e)
		{
		  throw new ProcessEngineException("invalid url: " + resourceUrl, e);
		}
	  }

	  /// <summary>
	  /// provides access to process engine to application clients in a
	  /// managed server environment.
	  /// </summary>
	  public static IDictionary<string, ProcessEngine> getProcessEngines()
	  {
		return processEngines;
	  }

	  /// <summary>
	  /// closes all process engines.  This method should be called when the server shuts down. </summary>
	  public static void destroy()
	  {
		  lock (typeof(ProcessEngines))
		  {
			if (isInitialized)
			{
			  IDictionary<string, ProcessEngine> engines = new Dictionary<string, ProcessEngine>(processEngines);
			  processEngines = new Dictionary<string, ProcessEngine>();
        
			  foreach (string processEngineName in engines.Keys)
			  {
				ProcessEngine processEngine = engines[processEngineName];
				try
				{
				  processEngine.close();
				}
				catch (Exception e)
				{
				  LOG.exceptionWhileClosingProcessEngine(string.ReferenceEquals(processEngineName, null) ? "the default process engine" : "process engine " + processEngineName, e);
				}
			  }
        
			  processEngineInfosByName.Clear();
			  processEngineInfosByResourceUrl.Clear();
			  processEngineInfos.Clear();
        
			  isInitialized = false;
			}
		  }
	  }

	}

}
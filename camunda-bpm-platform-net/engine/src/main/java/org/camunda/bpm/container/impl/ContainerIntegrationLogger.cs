using System;
using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.container.impl
{


	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using VirtualFile = org.jboss.vfs.VirtualFile;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ContainerIntegrationLogger : ProcessEngineLogger
	{

	  public virtual ProcessEngineException couldNotInstantiateJobExecutorClass(Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("001", "Could not instantiate job executor class"),e);
	  }

	  public virtual ProcessEngineException couldNotLoadJobExecutorClass(Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Could not load job executor class"),e);
	  }

	  public virtual void exceptionWhileStopping(string serviceType, string serviceName, Exception t)
	  {
		logWarn("003", "Exception while stopping {} '{}': {}", serviceType, serviceName, t.Message, t);
	  }

	  public virtual void debugRootPath(string urlPath)
	  {
		logDebug("004", "Rootpath is {}", urlPath);
	  }

	  public virtual ProcessEngineException cannotDecodePathName(UnsupportedEncodingException e)
	  {
		return new ProcessEngineException(exceptionMessage("005", "Could not decode pathname using utf-8 decoder."), e);
	  }

	  public virtual ProcessEngineException exceptionWhileScanning(string file, IOException e)
	  {
		return new ProcessEngineException(exceptionMessage("006", "IOException while scanning archive '{}'.", file), e);
	  }

	  public virtual void debugDiscoveredResource(string resourcePath)
	  {
		logDebug("007", "Discovered resource {}", resourcePath);
	  }

	  public virtual ProcessEngineException cannotOpenFileInputStream(string absolutePath, IOException e)
	  {
		return new ProcessEngineException(exceptionMessage("008", "Cannot not open file for reading: {}.", e.Message), e);
	  }

	  public virtual ProcessEngineException couldNotGetResource(string strippedPaResourceRootPath, ClassLoader cl, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("009", "Could not load resources at '{}' using classloaded '{}'", strippedPaResourceRootPath, cl), e);
	  }

	  public virtual void cannotFindResourcesForPath(string resourceRootPath, ClassLoader classLoader)
	  {
		logWarn("010", "Could not find any resources for process archive resource root path '{}' using classloader '{}'.", resourceRootPath, classLoader);
	  }

	  public virtual ProcessEngineException exceptionWhileGettingVirtualFolder(URL url, URISyntaxException e)
	  {
		return new ProcessEngineException(exceptionMessage("011", "Could not load virtual file for url '{}'", url), e);
	  }

	  public virtual void cannotScanVfsRoot(VirtualFile processArchiveRoot, IOException e)
	  {
		logWarn("012", "Cannot scan process archive root {}", processArchiveRoot, e);
	  }

	  public virtual void cannotReadInputStreamForFile(string resourceName, VirtualFile processArchiveRoot, IOException e)
	  {
		logWarn("013", "Could not read input stream of file '{}' from process archive '{}'.", resourceName, processArchiveRoot, e);
	  }

	  public virtual ProcessEngineException exceptionWhileLoadingCpRoots(string strippedPaResourceRootPath, ClassLoader classLoader, IOException e)
	  {
		return new ProcessEngineException(exceptionMessage("014", "Could not load resources at '{}' using classloaded '{}'", strippedPaResourceRootPath, classLoader), e);
	  }

	  public virtual ProcessEngineException unsuppoertedParameterType(Type parameterType)
	  {
		return new ProcessEngineException(exceptionMessage("015", "Unsupported parametertype {}", parameterType));
	  }

	  public virtual void debugAutoCompleteUrl(string url)
	  {
		logDebug("016", "Autocompleting url : [{}]", url);
	  }

	  public virtual void debugAutoCompletedUrl(string url)
	  {
		logDebug("017", "Autocompleted url : [{}]", url);
	  }

	  public virtual void foundConfigJndi(string jndi, string @string)
	  {
		logInfo("018", "Found camunda bpm platform configuration in JNDI [{}] at {}", jndi, @string);
	  }

	  public virtual void debugExceptionWhileGettingConfigFromJndi(string jndi, NamingException e)
	  {
		logDebug("019", "Failed to look up camunda bpm platform configuration in JNDI [{}]: {}", jndi, e);
	  }

	  public virtual void foundConfigAtLocation(string logStatement, string @string)
	  {
		logInfo("020", "Found camunda bpm platform configuration through {}  at {} ", logStatement, @string);
	  }

	  public virtual void notCreatingPaDeployment(string name)
	  {
		logInfo("021", "Not creating a deployment for process archive '{}': no resources provided.", name);
	  }

	  public virtual System.ArgumentException illegalValueForResumePreviousByProperty(string @string)
	  {
		return new System.ArgumentException(exceptionMessage("022", @string));
	  }

	  public virtual void deploymentSummary(ICollection<string> deploymentResourceNames, string deploymentName)
	  {

		StringBuilder builder = new StringBuilder();
		builder.Append("Deployment summary for process archive '" + deploymentName + "': \n");
		builder.Append("\n");
		foreach (string resourceName in deploymentResourceNames)
		{
		  builder.Append("        " + resourceName);
		  builder.Append("\n");
		}

		logInfo("023", builder.ToString());

	  }

	  public virtual void foundProcessesXmlFile(string @string)
	  {

		logInfo("024", "Found processes.xml file at {}", @string);

	  }

	  public virtual void emptyProcessesXml()
	  {
		logInfo("025", "Detected empty processes.xml file, using default values");
	  }

	  public virtual void noProcessesXmlForPa(string paName)
	  {
		logInfo("026", "No processes.xml file found in process application '{}'", paName);
	  }

	  public virtual ProcessEngineException exceptionWhileReadingProcessesXml(string deploymentDescriptor, IOException e)
	  {
		return new ProcessEngineException(exceptionMessage("027", "Exception while reading {}", deploymentDescriptor), e);
	  }

	  public virtual ProcessEngineException exceptionWhileInvokingPaLifecycleCallback(string methodName, string paName, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("028", "Exception while invoking {} on Process Application {}: {}", methodName, paName, e.Message), e);
	  }

	  public virtual void debugFoundPaLifecycleCallbackMethod(string methodName, string paName)
	  {
		logDebug("029", "Found Process Application lifecycle callback method {} in application {}", methodName, paName);
	  }

	  public virtual void debugPaLifecycleMethodNotFound(string methodName, string paName)
	  {
		logDebug("030", "Process Application lifecycle callback method {} not found in application {}", methodName, paName);
	  }

	  public virtual ProcessEngineException camnnotLoadConfigurationClass(string className, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("031", "Failed to load configuration class '{}': {}", className, e.Message), e);
	  }

	  public virtual ProcessEngineException configurationClassHasWrongType(string className, Type expectedType, System.InvalidCastException e)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return new ProcessEngineException(exceptionMessage("032", "Class '{}' has wrong type. Must extend {}", expectedType.FullName), e);
	  }

	  public virtual void timeoutDuringShutdownOfThreadPool(int i, TimeUnit seconds)
	  {
		logError("033", "Timeout during shutdown of managed thread pool. The current running tasks could not end within {} {} after shutdown operation.", i, seconds);
	  }

	  public virtual void interruptedWhileShuttingDownThreadPool(InterruptedException e)
	  {
		logError("034", "Interrupted while shutting down thread pool", e);
	  }

	  public virtual ProcessEngineException cannotRegisterService(ObjectName serviceName, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("035", "Cannot register service {} with MBean Server: {}", serviceName, e.Message), e);
	  }

	  public virtual ProcessEngineException cannotComposeNameFor(string serviceName, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("036", "Cannot compose name for service {}: {}", serviceName, e.Message), e);
	  }

	  public virtual ProcessEngineException exceptionWhileUnregisteringService(string canonicalName, Exception t)
	  {
		return new ProcessEngineException(exceptionMessage("037", "Exception while unregistering service {} with the MBeanServer: {}", canonicalName, t), t);
	  }

	  public virtual ProcessEngineException unknownExceptionWhileParsingDeploymentDescriptor(Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("038", "Unknown exception while parsing deployment camunda descriptor: {}", e.Message), e);
	  }

	  public virtual ProcessEngineException cannotSetValueForProperty(string key, string canonicalName, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("039", "Cannot set property '{}' on instance of class '{}'", key, canonicalName), e);
	  }

	  public virtual ProcessEngineException cannotFindSetterForProperty(string key, string canonicalName)
	  {
		return new ProcessEngineException(exceptionMessage("040", "Cannot find setter for property '{}' on class '{}'", key, canonicalName));
	  }

	  public virtual void debugPerformOperationStep(string stepName)
	  {
		logDebug("041", "Performing deployment operation step '{}'", stepName);
	  }

	  public virtual void debugSuccessfullyPerformedOperationStep(string stepName)
	  {
		logDebug("041", "Successfully performed deployment operation step '{}'", stepName);
	  }

	  public virtual void exceptionWhileRollingBackOperation(Exception e)
	  {
		logError("042", "Exception while rolling back operation", e);
	  }

	  public virtual ProcessEngineException exceptionWhilePerformingOperationStep(string opName, string stepName, Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("043", "Exception while performing '{}' => '{}': {}", opName, stepName, e.Message), e);
	  }

	  public virtual void exceptionWhilePerformingOperationStep(string name, Exception e)
	  {
		logError("044", "Exception while performing '{}': {}", name, e.Message, e);
	  }

	  public virtual void debugRejectedExecutionException(RejectedExecutionException e)
	  {
		logDebug("045", "RejectedExecutionException while scheduling work", e);
	  }

	  public virtual void foundTomcatDeploymentDescriptor(string bpmPlatformFileLocation, string fileLocation)
	  {
		logInfo("046", "Found camunda bpm platform configuration in CATALINA_BASE/CATALINA_HOME conf directory [{}] at '{}'", bpmPlatformFileLocation, fileLocation);

	  }

	  public virtual ProcessEngineException invalidDeploymentDescriptorLocation(string bpmPlatformFileLocation, MalformedURLException e)
	  {
		throw new ProcessEngineException(exceptionMessage("047", "'{} is not a valid camunda bpm platform configuration resource location.", bpmPlatformFileLocation), e);
	  }

	  public virtual void camundaBpmPlatformSuccessfullyStarted(string serverInfo)
	  {
		logInfo("048", "Camunda BPM platform sucessfully started at '{}'.", serverInfo);
	  }

	  public virtual void camundaBpmPlatformStopped(string serverInfo)
	  {
		logInfo("049", "Camunda BPM platform stopped at '{}'", serverInfo);
	  }

	  public virtual void paDeployed(string name)
	  {
		logInfo("050", "Process application {} successfully deployed", name);
	  }

	  public virtual void paUndeployed(string name)
	  {
		logInfo("051", "Process application {} undeployed", name);
	  }

	}

}
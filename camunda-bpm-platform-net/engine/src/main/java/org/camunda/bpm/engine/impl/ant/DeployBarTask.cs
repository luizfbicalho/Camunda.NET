using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
namespace org.camunda.bpm.engine.impl.ant
{

	using BuildException = org.apache.tools.ant.BuildException;
	using DirectoryScanner = org.apache.tools.ant.DirectoryScanner;
	using Task = org.apache.tools.ant.Task;
	using FileSet = org.apache.tools.ant.types.FileSet;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using LogUtil = org.camunda.bpm.engine.impl.util.LogUtil;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DeployBarTask : Task
	{

	  internal string processEngineName = ProcessEngines.NAME_DEFAULT;
	  internal File file;
	  internal IList<FileSet> fileSets;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute() throws org.apache.tools.ant.BuildException
	  public virtual void execute()
	  {
		IList<File> files = new List<File>();
		if (file != null)
		{
		  files.Add(file);
		}
		if (fileSets != null)
		{
		  foreach (FileSet fileSet in fileSets)
		  {
			DirectoryScanner directoryScanner = fileSet.getDirectoryScanner(Project);
			File baseDir = directoryScanner.Basedir;
			string[] includedFiles = directoryScanner.IncludedFiles;
			string[] excludedFiles = directoryScanner.ExcludedFiles;
			IList<string> excludedFilesList = Arrays.asList(excludedFiles);
			foreach (string includedFile in includedFiles)
			{
			  if (!excludedFilesList.Contains(includedFile))
			  {
				files.Add(new File(baseDir, includedFile));
			  }
			}
		  }
		}

		Thread currentThread = Thread.CurrentThread;
		ClassLoader originalClassLoader = currentThread.ContextClassLoader;
		currentThread.ContextClassLoader = typeof(DeployBarTask).ClassLoader;

		LogUtil.readJavaUtilLoggingConfigFromClasspath();

		try
		{
		  log("Initializing process engine " + processEngineName);
		  ProcessEngines.init();
		  ProcessEngine processEngine = ProcessEngines.getProcessEngine(processEngineName);
		  if (processEngine == null)
		  {
			IList<ProcessEngineInfo> processEngineInfos = ProcessEngines.ProcessEngineInfos;
			if (processEngineInfos != null && processEngineInfos.Count > 0)
			{
			  // Since no engine with the given name is found, we can't be 100% sure which ProcessEngineInfo
			  // is causing the error. We should show ALL errors and process engine names / resource URL's.
			  string message = getErrorMessage(processEngineInfos, processEngineName);
			  throw new ProcessEngineException(message);
			}
			else
			{
				throw new ProcessEngineException("Could not find a process engine with name '" + processEngineName + "', no engines found. " + "Make sure an engine configuration is present on the classpath");
			}
		  }
		  RepositoryService repositoryService = processEngine.RepositoryService;

		  log("Starting to deploy " + files.Count + " files");
		  foreach (File file in files)
		  {
			string path = file.AbsolutePath;
			log("Handling file " + path);
			try
			{
			  FileStream inputStream = new FileStream(file, FileMode.Open, FileAccess.Read);
			  try
			  {
				log("deploying bar " + path);
				repositoryService.createDeployment().name(file.Name).addZipInputStream(new ZipInputStream(inputStream)).deploy();
			  }
			  finally
			  {
				IoUtil.closeSilently(inputStream);
			  }
			}
			catch (Exception e)
			{
			  throw new BuildException("couldn't deploy bar " + path + ": " + e.Message, e);
			}
		  }

		}
		finally
		{
		  currentThread.ContextClassLoader = originalClassLoader;
		}
	  }

	  private string getErrorMessage(IList<ProcessEngineInfo> processEngineInfos, string name)
	  {
		StringBuilder builder = new StringBuilder("Could not find a process engine with name ");
		builder.Append(name).Append(", engines loaded:\n");
		foreach (ProcessEngineInfo engineInfo in processEngineInfos)
		{
		  string engineName = (!string.ReferenceEquals(engineInfo.Name, null)) ? engineInfo.Name : "unknown";
		  builder.Append("Process engine name: ").Append(engineName);
		  builder.Append(" - resource: ").Append(engineInfo.ResourceUrl);
		  builder.Append(" - status: ");

		  if (!string.ReferenceEquals(engineInfo.Exception, null))
		  {
			builder.Append("Error while initializing engine. ");
			if (engineInfo.Exception.IndexOf("driver on UnpooledDataSource", StringComparison.Ordinal) != -1)
			{
			  builder.Append("Exception while initializing process engine! Database or database driver might not have been configured correctly.").Append("Please consult the user guide for supported database environments or build.properties. Stacktrace: ").Append(engineInfo.Exception);
			}
			else
			{
			  builder.Append("Stacktrace: ").Append(engineInfo.Exception);
			}
		  }
		  else
		  {
			// Process engine initialised without exception
			builder.Append("Initialised");
		  }
		  builder.Append("\n");
		}
		return builder.ToString();
	  }

	  public virtual string ProcessEngineName
	  {
		  get
		  {
			return processEngineName;
		  }
		  set
		  {
			this.processEngineName = value;
		  }
	  }
	  public virtual File File
	  {
		  get
		  {
			return file;
		  }
		  set
		  {
			this.file = value;
		  }
	  }
	  public virtual IList<FileSet> FileSets
	  {
		  get
		  {
			return fileSets;
		  }
		  set
		  {
			this.fileSets = value;
		  }
	  }
	}

}
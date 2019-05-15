using System;
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
namespace org.camunda.bpm.qa.performance.engine.junit
{

	using PerfTestException = org.camunda.bpm.qa.performance.engine.framework.PerfTestException;
	using PerfTestResults = org.camunda.bpm.qa.performance.engine.framework.PerfTestResults;
	using JsonUtil = org.camunda.bpm.qa.performance.engine.util.JsonUtil;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;

	/// <summary>
	/// JUnit rule recording the performance test result
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestResultRecorderRule : TestWatcher
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly Logger LOG = Logger.getLogger(typeof(PerfTestResultRecorderRule).FullName);

	  protected internal PerfTestResults results;

	  protected internal override void succeeded(Description description)
	  {
		if (results != null)
		{
		  results.TestName = description.TestClass.SimpleName + "." + description.MethodName;
		  LOG.log(Level.INFO, results.ToString());

		  string resultFileName = formatResultFileName(description);

		  try
		  {
			// create file:
			File directory = new File(formatResultFileDirName());
			if (!directory.exists())
			{
			  directory.mkdir();
			}

			JsonUtil.writeObjectToFile(resultFileName, results);

		  }
		  catch (Exception e)
		  {
			throw new PerfTestException("Could not record results to file " + resultFileName, e);

		  }
		}
	  }

	  protected internal virtual string formatResultFileDirName()
	  {
		return "target" + Path.DirectorySeparatorChar + "results";
	  }

	  protected internal virtual string formatResultFileName(Description description)
	  {
		return formatResultFileDirName() + Path.DirectorySeparatorChar + description.TestClass.SimpleName + "." + description.MethodName + ".json";
	  }

	  public virtual PerfTestResults Results
	  {
		  set
		  {
			this.results = value;
		  }
	  }

	}

}
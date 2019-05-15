using System.Collections.Generic;

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
namespace org.camunda.bpm.qa.performance.engine.framework.aggregate
{

	using JsonUtil = org.camunda.bpm.qa.performance.engine.util.JsonUtil;

	/// <summary>
	/// A result aggregator is used to aggregate the results of a
	/// performance testsuite run as a table.
	/// 
	/// The aggegator needs to be pointed to a directory containing the
	/// result files. It will read the result file by file and delegate the
	/// actual processing to a subclass implementation of this class.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class TabularResultAggregator
	{

	  protected internal File resultDirectory;
	  private bool isSortingEnabled = true;

	  public TabularResultAggregator(string resultsFolderPath)
	  {
		resultDirectory = new File(resultsFolderPath);
		if (!resultDirectory.exists())
		{
		  throw new PerfTestException("Folder " + resultsFolderPath + " does not exist.");
		}
	  }

	  public virtual TabularResultAggregator sortResults(bool isSortingEnabled)
	  {
		this.isSortingEnabled = isSortingEnabled;
		return this;
	  }

	  public virtual TabularResultSet execute()
	  {
		TabularResultSet tabularResultSet = createAggregatedResultsInstance();

		File[] resultFiles = resultDirectory.listFiles();
		foreach (File resultFile in resultFiles)
		{
		  if (resultFile.Name.EndsWith(".json"))
		  {
			processFile(resultFile, tabularResultSet);
		  }
		}

		if (isSortingEnabled)
		{
		  tabularResultSet.Results.Sort(new ComparatorAnonymousInnerClass(this));
		}

		postProcessResultSet(tabularResultSet);

		return tabularResultSet;
	  }

	  private class ComparatorAnonymousInnerClass : IComparer<IList<object>>
	  {
		  private readonly TabularResultAggregator outerInstance;

		  public ComparatorAnonymousInnerClass(TabularResultAggregator outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public int compare(IList<object> o1, IList<object> o2)
		  {
			return o1[0].ToString().CompareTo(o2[0].ToString());
		  }
	  }

	  protected internal virtual void postProcessResultSet(TabularResultSet tabularResultSet)
	  {
		// do nothing
	  }

	  protected internal virtual void processFile(File resultFile, TabularResultSet tabularResultSet)
	  {

		PerfTestResults results = JsonUtil.readObjectFromFile(resultFile.AbsolutePath, typeof(PerfTestResults));
		processResults(results, tabularResultSet);

	  }

	  protected internal abstract TabularResultSet createAggregatedResultsInstance();

	  protected internal abstract void processResults(PerfTestResults results, TabularResultSet tabularResultSet);

	}

}
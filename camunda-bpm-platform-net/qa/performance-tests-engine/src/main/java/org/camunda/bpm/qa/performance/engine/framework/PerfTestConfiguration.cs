using System;
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
namespace org.camunda.bpm.qa.performance.engine.framework
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.framework.activitylog.ActivityPerfTestWatcher.WATCH_ALL_ACTIVITIES;



	/// <summary>
	/// Configuration of a performance test
	/// 
	/// @author Daniel Meyer, Ingo Richtsmeier
	/// 
	/// </summary>
	public class PerfTestConfiguration
	{

	  protected internal int numberOfThreads = 2;
	  protected internal int numberOfRuns = 1000;
	  protected internal string databaseName = "";

	  protected internal string testWatchers = null;
	  protected internal string historyLevel;

	  protected internal IList<string> watchActivities = null;

	  protected internal DateTime startTime;

	  protected internal string platform;

	  public PerfTestConfiguration(Properties properties)
	  {
		numberOfRuns = int.Parse(properties.getProperty("numberOfRuns"));
		numberOfThreads = int.Parse(properties.getProperty("numberOfThreads"));
		testWatchers = properties.getProperty("testWatchers", null);
		databaseName = properties.getProperty("databaseDriver", null);
		historyLevel = properties.getProperty("historyLevel");
		watchActivities = parseWatchActivities(properties.getProperty("watchActivities", null));
	  }

	  public PerfTestConfiguration()
	  {
	  }

	  public virtual int NumberOfThreads
	  {
		  get
		  {
			return numberOfThreads;
		  }
		  set
		  {
			this.numberOfThreads = value;
		  }
	  }


	  public virtual int NumberOfRuns
	  {
		  get
		  {
			return numberOfRuns;
		  }
		  set
		  {
			this.numberOfRuns = value;
		  }
	  }


	  public virtual string TestWatchers
	  {
		  get
		  {
			return testWatchers;
		  }
		  set
		  {
			this.testWatchers = value;
		  }
	  }

	  public virtual string DatabaseName
	  {
		  get
		  {
			return databaseName;
		  }
	  }


	  public virtual string HistoryLevel
	  {
		  get
		  {
			return historyLevel;
		  }
		  set
		  {
			this.historyLevel = value;
		  }
	  }


	  public virtual DateTime StartTime
	  {
		  get
		  {
			return startTime;
		  }
		  set
		  {
			this.startTime = value;
		  }
	  }


	  public virtual string Platform
	  {
		  get
		  {
			return platform;
		  }
		  set
		  {
			this.platform = value;
		  }
	  }


	  public virtual IList<string> WatchActivities
	  {
		  get
		  {
			return watchActivities;
		  }
		  set
		  {
			this.watchActivities = value;
		  }
	  }


	  protected internal virtual IList<string> parseWatchActivities(string watchActivitiesString)
	  {
		if (string.ReferenceEquals(watchActivitiesString, null) || watchActivitiesString.Trim().Length == 0)
		{
		  return null;
		}
		else if (watchActivitiesString.Trim().Equals(WATCH_ALL_ACTIVITIES.get(0), StringComparison.OrdinalIgnoreCase))
		{
		  return WATCH_ALL_ACTIVITIES;
		}
		else
		{
		  IList<string> watchActivities = new List<string>();
		  string[] parts = watchActivitiesString.Split(",", true);
		  foreach (string part in parts)
		  {
			part = part.Trim();
			if (part.Length > 0)
			{
			  watchActivities.Add(part);
			}
		  }
		  return Collections.unmodifiableList(watchActivities);
		}
	  }
	}

}
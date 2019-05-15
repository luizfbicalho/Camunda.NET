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
namespace org.camunda.bpm.qa.performance.engine.framework.activitylog
{

	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using PerfTestProcessEngine = org.camunda.bpm.qa.performance.engine.junit.PerfTestProcessEngine;
	using PerfTestConstants = org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants;

	public class ActivityPerfTestWatcher : PerfTestWatcher
	{

	  public static readonly IList<string> WATCH_ALL_ACTIVITIES = Collections.singletonList("ALL");

	  protected internal IList<string> activityIds;
	  protected internal bool watchAllActivities;

	  public ActivityPerfTestWatcher(IList<string> activityIds)
	  {
		this.activityIds = activityIds;
//JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
//ORIGINAL LINE: watchAllActivities = WATCH_ALL_ACTIVITIES.equals(activityIds);
		watchAllActivities = WATCH_ALL_ACTIVITIES.SequenceEqual(activityIds);
	  }

	  public virtual void beforePass(PerfTestPass pass)
	  {
		// nothing to do
	  }

	  public virtual void beforeRun(PerfTest test, PerfTestRun run)
	  {
		// nothing to do
	  }

	  public virtual void beforeStep(PerfTestStep step, PerfTestRun run)
	  {
		// nothing to do
	  }

	  public virtual void afterStep(PerfTestStep step, PerfTestRun run)
	  {
		// nothing to do
	  }

	  public virtual void afterRun(PerfTest test, PerfTestRun run)
	  {
		// nothing to do
	  }

	  public virtual void afterPass(PerfTestPass pass)
	  {
		ProcessEngine processEngine = PerfTestProcessEngine.Instance;
		HistoryService historyService = processEngine.HistoryService;

		foreach (PerfTestRun run in pass.Runs.Values)
		{
		  logActivityResults(pass, run, historyService);
		}
	  }

	  protected internal virtual void logActivityResults(PerfTestPass pass, PerfTestRun run, HistoryService historyService)
	  {
		string processInstanceId = run.getVariable(PerfTestConstants.PROCESS_INSTANCE_ID);
		IList<ActivityPerfTestResult> activityResults = new List<ActivityPerfTestResult>();

		HistoricProcessInstance processInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).singleResult();
		DateTime startTime = processInstance.StartTime;

		IList<HistoricActivityInstance> activityInstances = historyService.createHistoricActivityInstanceQuery().processInstanceId(processInstanceId).orderByHistoricActivityInstanceStartTime().asc().list();

		foreach (HistoricActivityInstance activityInstance in activityInstances)
		{
		  if (watchAllActivities || activityIds.Contains(activityInstance.ActivityId))
		  {
			ActivityPerfTestResult result = new ActivityPerfTestResult(activityInstance);
			if (activityInstance.ActivityType.Equals("startEvent"))
			{
			  result.StartTime = startTime;
			}
			activityResults.Add(result);
		  }
		}

		pass.logActivityResult(processInstanceId, activityResults);
	  }

	}

}
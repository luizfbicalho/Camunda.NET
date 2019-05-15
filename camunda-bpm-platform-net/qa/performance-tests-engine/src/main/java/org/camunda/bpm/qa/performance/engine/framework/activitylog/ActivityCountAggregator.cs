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

	using TabularResultAggregator = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultAggregator;
	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;
	using SectionedHtmlReportBuilder = org.camunda.bpm.qa.performance.engine.framework.report.SectionedHtmlReportBuilder;
	using TableCell = org.camunda.bpm.qa.performance.engine.framework.report.SectionedHtmlReportBuilder.TableCell;

	public class ActivityCountAggregator : TabularResultAggregator
	{

	  public const long INTERVAL = 1000;
	  public const long TIME_UNIT = 1000;
	  public static readonly long INTERVAL_SECONDS = INTERVAL / 1000;

	  protected internal SectionedHtmlReportBuilder htmlBuilder;

	  public ActivityCountAggregator(string resultsFolderPath, SectionedHtmlReportBuilder htmlBuilder) : base(resultsFolderPath)
	  {
		this.htmlBuilder = htmlBuilder;
		sortResults(false);
	  }

	  protected internal override TabularResultSet createAggregatedResultsInstance()
	  {
		return null;
	  }

	  protected internal override void processResults(PerfTestResults results, TabularResultSet tabularResultSet)
	  {
		PerfTestConfiguration configuration = results.Configuration;
		IList<string> watchActivities = configuration.WatchActivities;

		foreach (PerfTestResult passResult in results.PassResults)
		{
		  string passTitle = getPassTitle(results.TestName, configuration, passResult);
		  TabularResultSet result = processPassResult(watchActivities, passResult);
		  htmlBuilder.addSection(passTitle, result);
		}
	  }

	  protected internal virtual string getPassTitle(string testName, PerfTestConfiguration configuration, PerfTestResult passResult)
	  {
		return testName + " (Runs: " + configuration.NumberOfRuns + ", Threads: " + passResult.NumberOfThreads + ", Duration: " + passResult.Duration + " ms)";
	  }

	  protected internal virtual TabularResultSet processPassResult(IList<string> watchActivities, PerfTestResult passResult)
	  {
		TabularResultSet tabularResultSet = new TabularResultSet();
		addTableHeaders(tabularResultSet, watchActivities);
		addTableBody(tabularResultSet, watchActivities, passResult);
		return tabularResultSet;
	  }

	  protected internal virtual void addTableHeaders(TabularResultSet tabularResultSet, IList<string> watchActivities)
	  {
		IList<object> row1 = new List<object>();
		IList<object> row2 = new List<object>();

		row1.Add(new SectionedHtmlReportBuilder.TableCell("", true));
		row2.Add(new SectionedHtmlReportBuilder.TableCell("seconds", true));
		foreach (string activity in watchActivities)
		{
		  row1.Add(new SectionedHtmlReportBuilder.TableCell(activity, 5, true));
		  row2.Add(new SectionedHtmlReportBuilder.TableCell("started", true));
		  row2.Add(new SectionedHtmlReportBuilder.TableCell("&sum; started", true));
		  row2.Add(new SectionedHtmlReportBuilder.TableCell("ended", true));
		  row2.Add(new SectionedHtmlReportBuilder.TableCell("&sum; ended", true));
		  row2.Add(new SectionedHtmlReportBuilder.TableCell("&Oslash; duration", true));
		}

		row1.Add(new SectionedHtmlReportBuilder.TableCell("", 2, true));
		row2.Add(new SectionedHtmlReportBuilder.TableCell("act/s", true));
		row2.Add(new SectionedHtmlReportBuilder.TableCell("&Oslash; act/s", true));

		tabularResultSet.addResultRow(row1);
		tabularResultSet.addResultRow(row2);
	  }

	  protected internal virtual void addTableBody(TabularResultSet tabularResultSet, IList<string> watchActivities, PerfTestResult passResult)
	  {
		// get first and last timestamp
		DateTime firstStartTime = null;
		DateTime lastEndTime = null;

		foreach (IList<ActivityPerfTestResult> activityResults in passResult.ActivityResults.Values)
		{
		  foreach (ActivityPerfTestResult activityResult in activityResults)
		  {
			if (firstStartTime == null || activityResult.StartTime < firstStartTime)
			{
			  firstStartTime = activityResult.StartTime;
			}

			if (lastEndTime == null || activityResult.EndTime > lastEndTime)
			{
			  lastEndTime = activityResult.EndTime;
			}
		  }
		}

		long firstTimestamp = firstStartTime.Ticks;
		long lastTimestamp = lastEndTime.Ticks;
		IList<IDictionary<string, ActivityCount>> resultTable = new List<IDictionary<string, ActivityCount>>();

		for (long t = firstTimestamp; t <= lastTimestamp + INTERVAL; t += INTERVAL)
		{
		  IDictionary<string, ActivityCount> activitiesMap = new Dictionary<string, ActivityCount>();
		  foreach (string activity in watchActivities)
		  {
			activitiesMap[activity] = new ActivityCount(this);
		  }
		  resultTable.Add(activitiesMap);
		}


		foreach (IList<ActivityPerfTestResult> activityResults in passResult.ActivityResults.Values)
		{
		  foreach (ActivityPerfTestResult activityResult in activityResults)
		  {
			string activityId = activityResult.ActivityId;
			int startSlot = calculateTimeSlot(activityResult.StartTime, firstTimestamp);
			int endSlot = calculateTimeSlot(activityResult.EndTime, firstTimestamp);
			resultTable[startSlot][activityId].incrementStarted();
			resultTable[endSlot][activityId].incrementEnded();
			resultTable[endSlot][activityId].addDuration(activityResult.Duration);
		  }
		}

		List<object> row = null;
		IDictionary<string, ActivityCount> sumMap = new Dictionary<string, ActivityCount>();
		foreach (string activity in watchActivities)
		{
		  sumMap[activity] = new ActivityCount(this);
		}

		long sumActivitiesEnded = 0;
		for (int i = 0; i < resultTable.Count; i++)
		{
		  row = new List<object>();
		  row.Add(i * INTERVAL / TIME_UNIT);
		  long currentActivitiesEnded = 0;
		  foreach (string activity in watchActivities)
		  {
			sumMap[activity].addStarted(resultTable[i][activity].Started);
			sumMap[activity].addEnded(resultTable[i][activity].Ended);
			sumMap[activity].addDuration(resultTable[i][activity].Duration);
			currentActivitiesEnded += resultTable[i][activity].Ended;
		  }
		  foreach (string activity in watchActivities)
		  {
			long started = resultTable[i][activity].Started;
			long ended = resultTable[i][activity].Ended;
			double endedFraction = 0;
			long avgDuration = 0;

			if (sumMap[activity].Ended > 0)
			{
			  avgDuration = sumMap[activity].Duration / sumMap[activity].Ended;
			}

			if (currentActivitiesEnded > 0)
			{
			  endedFraction = ended * 100.0 / currentActivitiesEnded;
			}

			row.Add(started);
			row.Add(sumMap[activity].Started);
			row.Add(string.Format("{0:D} ({1:F1}%)", ended, endedFraction));
			row.Add(sumMap[activity].Ended);
			row.Add(avgDuration + " ms");
		  }
		  sumActivitiesEnded += currentActivitiesEnded;
		  row.Add(currentActivitiesEnded / INTERVAL_SECONDS);
		  row.Add(sumActivitiesEnded / ((i + 1) * INTERVAL_SECONDS));
		  tabularResultSet.addResultRow(row);
		}
	  }

	  protected internal virtual int calculateTimeSlot(DateTime date, long firstTimestamp)
	  {
		return (long)Math.Round((date.Ticks - firstTimestamp) / INTERVAL, MidpointRounding.AwayFromZero);
	  }

	  internal class ActivityCount
	  {
		  private readonly ActivityCountAggregator outerInstance;

		  public ActivityCount(ActivityCountAggregator outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		internal int started = 0;
		internal int ended = 0;
		internal long duration = 0;

		public virtual void incrementStarted()
		{
		  ++started;
		}

		public virtual void addStarted(int started)
		{
		  this.started += started;
		}

		public virtual int Started
		{
			get
			{
			  return started;
			}
		}

		public virtual void incrementEnded()
		{
		  ++ended;
		}

		public virtual void addEnded(int ended)
		{
		  this.ended += ended;
		}

		public virtual int Ended
		{
			get
			{
			  return ended;
			}
		}

		public virtual void addDuration(long? duration)
		{
		  if (duration != null)
		  {
			this.duration += duration.Value;
		  }
		}

		public virtual long Duration
		{
			get
			{
			  return duration;
			}
		}

	  }

	}

}
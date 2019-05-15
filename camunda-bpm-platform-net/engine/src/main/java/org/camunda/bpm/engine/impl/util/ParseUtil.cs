using System;

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
namespace org.camunda.bpm.engine.impl.util
{

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using FailedJobRetryConfiguration = org.camunda.bpm.engine.impl.bpmn.parser.FailedJobRetryConfiguration;
	using DurationHelper = org.camunda.bpm.engine.impl.calendar.DurationHelper;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Expression = org.camunda.bpm.engine.impl.el.Expression;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;

	public class ParseUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  protected internal static readonly Pattern REGEX_TTL_ISO = Pattern.compile("^P(\\d+)D$");

	  /// <summary>
	  /// Parse History Time To Live in ISO-8601 format to integer and set into the given entity </summary>
	  /// <param name="historyTimeToLive"> </param>
	  public static int? parseHistoryTimeToLive(string historyTimeToLive)
	  {
		int? timeToLive = null;

		if (!string.ReferenceEquals(historyTimeToLive, null) && historyTimeToLive.Length > 0)
		{
		  Matcher matISO = REGEX_TTL_ISO.matcher(historyTimeToLive);
		  if (matISO.find())
		  {
			historyTimeToLive = matISO.group(1);
		  }
		  timeToLive = parseIntegerAttribute("historyTimeToLive", historyTimeToLive);
		}

		if (timeToLive != null && timeToLive < 0)
		{
		  throw new NotValidException("Cannot parse historyTimeToLive: negative value is not allowed");
		}

		return timeToLive;
	  }

	  protected internal static int? parseIntegerAttribute(string attributeName, string text)
	  {
		int? result = null;

		if (!string.ReferenceEquals(text, null) && text.Length > 0)
		{
		  try
		  {
			result = int.Parse(text);
		  }
		  catch (System.FormatException e)
		  {
			throw new ProcessEngineException("Cannot parse " + attributeName + ": " + e.Message);
		  }
		}

		return result;
	  }

	  public static FailedJobRetryConfiguration parseRetryIntervals(string retryIntervals)
	  {

		if (!string.ReferenceEquals(retryIntervals, null) && retryIntervals.Length > 0)
		{

		  if (StringUtil.isExpression(retryIntervals))
		  {
			ExpressionManager expressionManager = Context.ProcessEngineConfiguration.ExpressionManager;
			Expression expression = expressionManager.createExpression(retryIntervals);
			return new FailedJobRetryConfiguration(expression);
		  }

		  string[] intervals = StringUtil.Split(retryIntervals, ",");
		  int retries = intervals.Length + 1;

		  if (intervals.Length == 1)
		  {
			try
			{
			  DurationHelper durationHelper = new DurationHelper(intervals[0]);

			  if (durationHelper.Repeat)
			  {
				retries = durationHelper.Times;
			  }
			}
			catch (Exception e)
			{
			  LOG.logParsingRetryIntervals(intervals[0], e);
			  return null;
			}
		  }
		  return new FailedJobRetryConfiguration(retries, Arrays.asList(intervals));
		}
		else
		{
		  return null;
		}
	  }
	}

}
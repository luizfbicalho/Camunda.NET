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
namespace org.camunda.bpm.engine.impl.calendar
{

	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EngineUtilLogger = org.camunda.bpm.engine.impl.util.EngineUtilLogger;

	public class CycleBusinessCalendar : BusinessCalendar
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  public static string NAME = "cycle";

	  public virtual DateTime resolveDuedate(string duedateDescription)
	  {
		return resolveDuedate(duedateDescription, null);
	  }

	  public virtual DateTime resolveDuedate(string duedateDescription, DateTime startDate)
	  {
		try
		{
		  if (duedateDescription.StartsWith("R", StringComparison.Ordinal))
		  {
			return (new DurationHelper(duedateDescription, startDate)).getDateAfter(startDate);
		  }
		  else
		  {
			CronExpression ce = new CronExpression(duedateDescription);
			return ce.getTimeAfter(startDate == null ? ClockUtil.CurrentTime : startDate);
		  }

		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileParsingCronExpresison(duedateDescription, e);
		}

	  }

	}

}
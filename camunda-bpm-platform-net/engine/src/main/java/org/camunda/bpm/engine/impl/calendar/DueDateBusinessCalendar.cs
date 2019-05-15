﻿using System;

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

	using EngineUtilLogger = org.camunda.bpm.engine.impl.util.EngineUtilLogger;
	using DateTime = org.joda.time.DateTime;
	using ISOPeriodFormat = org.joda.time.format.ISOPeriodFormat;


	public class DueDateBusinessCalendar : BusinessCalendar
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  public const string NAME = "dueDate";

	  public virtual DateTime resolveDuedate(string duedate)
	  {
		return resolveDuedate(duedate, null);
	  }

	  public virtual DateTime resolveDuedate(string duedate, DateTime startDate)
	  {
		try
		{
		  if (duedate.StartsWith("P", StringComparison.Ordinal))
		  {
			DateTime start = null;
			if (startDate == null)
			{
			  start = DateTimeUtil.now();
			}
			else
			{
			  start = new DateTime(startDate);
			}
			return start.plus(ISOPeriodFormat.standard().parsePeriod(duedate)).toDate();
		  }

		  return DateTimeUtil.parseDateTime(duedate).toDate();

		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileResolvingDuedate(duedate, e);
		}
	  }
	}

}
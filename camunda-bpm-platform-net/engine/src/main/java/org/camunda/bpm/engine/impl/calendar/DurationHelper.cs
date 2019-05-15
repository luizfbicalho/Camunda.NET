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
namespace org.camunda.bpm.engine.impl.calendar
{


	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using EngineUtilLogger = org.camunda.bpm.engine.impl.util.EngineUtilLogger;

	/// <summary>
	/// helper class for parsing ISO8601 duration format (also recurring) and computing next timer date
	/// </summary>
	public class DurationHelper
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  internal DateTime start;

	  internal DateTime end;

	  internal Duration period;

	  internal bool isRepeat;

	  internal int times;

	  internal DatatypeFactory datatypeFactory;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DurationHelper(String expressions) throws Exception
	  public DurationHelper(string expressions) : this(expressions, null)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DurationHelper(String expressions, java.util.Date startDate) throws Exception
	  public DurationHelper(string expressions, DateTime startDate)
	  {
		IList<string> expression = new List<string>();
		if (!string.ReferenceEquals(expressions, null))
		{
		  expression = Arrays.asList(expressions.Split("/", true));
		}
		datatypeFactory = DatatypeFactory.newInstance();

		if (expression.Count > 3 || expression.Count == 0)
		{
		  throw LOG.cannotParseDuration(expressions);
		}
		if (expression[0].StartsWith("R", StringComparison.Ordinal))
		{
		  isRepeat = true;
		  times = expression[0].Length == 1 ? int.MaxValue : int.Parse(expression[0].Substring(1));
		  expression = expression.subList(1, expression.Count);
		}

		if (isDuration(expression[0]))
		{
		  period = parsePeriod(expression[0]);
		  end = expression.Count == 1 ? null : DateTimeUtil.parseDate(expression[1]);
		}
		else
		{
		  start = DateTimeUtil.parseDate(expression[0]);
		  if (isDuration(expression[1]))
		  {
			period = parsePeriod(expression[1]);
		  }
		  else
		  {
			end = DateTimeUtil.parseDate(expression[1]);
			period = datatypeFactory.newDuration(end.Ticks - start.Ticks);
		  }
		}
		if (start == null && end == null)
		{
		  start = startDate == null ? ClockUtil.CurrentTime : startDate;
		}
	  }

	  public virtual DateTime DateAfter
	  {
		  get
		  {
			return getDateAfter(null);
		  }
	  }

	  public virtual DateTime getDateAfter(DateTime date)
	  {
		if (isRepeat)
		{
		  return getDateAfterRepeat(date == null ? ClockUtil.CurrentTime : date);
		}
		//TODO: is this correct?
		if (end != null)
		{
		  return end;
		}
		return add(start, period);
	  }

	  public virtual int Times
	  {
		  get
		  {
			return times;
		  }
	  }

	  public virtual bool Repeat
	  {
		  get
		  {
			return isRepeat;
		  }
	  }

	  private DateTime getDateAfterRepeat(DateTime date)
	  {
		if (start != null)
		{
		  DateTime cur = start;
		  for (int i = 0;i < times && !cur > date;i++)
		  {
			cur = add(cur, period);
		  }
		  return cur < date ? null : cur;
		}
		DateTime cur = add(end, period.negate());
		DateTime next = end;

		for (int i = 0;i < times && cur > date;i++)
		{
		  next = cur;
		  cur = add(cur, period.negate());
		}
		return next < date ? null : next;
	  }

	  private DateTime add(DateTime date, Duration duration)
	  {
		DateTime calendar = new GregorianCalendar();
		calendar = new DateTime(date);
		duration.addTo(calendar);
		return calendar;
	  }

	  private Duration parsePeriod(string period)
	  {
		return datatypeFactory.newDuration(period);
	  }

	  private bool isDuration(string time)
	  {
		return time.StartsWith("P", StringComparison.Ordinal);
	  }

	}

}
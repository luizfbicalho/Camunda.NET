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


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DefaultBusinessCalendar : BusinessCalendar
	{

	  private static IDictionary<string, int> units = new Dictionary<string, int>();
	  static DefaultBusinessCalendar()
	  {
		units["millis"] = DateTime.MILLISECOND;
		units["seconds"] = DateTime.SECOND;
		units["second"] = DateTime.SECOND;
		units["minute"] = DateTime.MINUTE;
		units["minutes"] = DateTime.MINUTE;
		units["hour"] = DateTime.HOUR;
		units["hours"] = DateTime.HOUR;
		units["day"] = DateTime.DAY_OF_YEAR;
		units["days"] = DateTime.DAY_OF_YEAR;
		units["week"] = DateTime.WEEK_OF_YEAR;
		units["weeks"] = DateTime.WEEK_OF_YEAR;
		units["month"] = DateTime.MONTH;
		units["months"] = DateTime.MONTH;
		units["year"] = DateTime.YEAR;
		units["years"] = DateTime.YEAR;
	  }

	  public virtual DateTime resolveDuedate(string duedate)
	  {
		return resolveDuedate(duedate, null);
	  }

	  public virtual DateTime resolveDuedate(string duedate, DateTime startDate)
	  {
		DateTime resolvedDuedate = startDate == null ? ClockUtil.CurrentTime : startDate;

		string[] tokens = duedate.Split(" and ", true);
		foreach (string token in tokens)
		{
		  resolvedDuedate = addSingleUnitQuantity(resolvedDuedate, token);
		}

		return resolvedDuedate;
	  }

	  protected internal virtual DateTime addSingleUnitQuantity(DateTime startDate, string singleUnitQuantity)
	  {
		int spaceIndex = singleUnitQuantity.IndexOf(" ", StringComparison.Ordinal);
		if (spaceIndex == -1 || singleUnitQuantity.Length < spaceIndex + 1)
		{
		  throw new ProcessEngineException("invalid duedate format: " + singleUnitQuantity);
		}

		string quantityText = singleUnitQuantity.Substring(0, spaceIndex);
		int? quantity = Convert.ToInt32(quantityText);

		string unitText = singleUnitQuantity.Substring(spaceIndex + 1).Trim().ToLower();

		int unit = units[unitText];

		GregorianCalendar calendar = new GregorianCalendar();
		calendar.Time = startDate;
		calendar.add(unit, quantity);

		return calendar.Time;
	  }
	}

}
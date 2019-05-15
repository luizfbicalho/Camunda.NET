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
namespace org.camunda.bpm.engine.rest.util
{

	public abstract class DateTimeUtils
	{

	  public static readonly SimpleDateFormat DATE_FORMAT_WITHOUT_TIMEZONE = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
	  public static readonly SimpleDateFormat DATE_FORMAT_WITH_TIMEZONE = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSSZ");

	  /// <summary>
	  /// Converts date string without timezone to the one with timezone. </summary>
	  /// <param name="dateString">
	  /// @return </param>
	  public static string withTimezone(string dateString)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date parse;
		DateTime parse;
		try
		{
		  parse = DATE_FORMAT_WITHOUT_TIMEZONE.parse(dateString);
		  return withTimezone(parse);
		}
		catch (ParseException e)
		{
		  throw new Exception(e);
		}
	  }

	  public static string withTimezone(DateTime date)
	  {
		  return DATE_FORMAT_WITH_TIMEZONE.format(date);
	  }

	  public static DateTime updateTime(DateTime now, DateTime newTime)
	  {
		DateTime c = new DateTime();
		c = new DateTime(now);
		DateTime newTimeCalendar = new DateTime();
		newTimeCalendar = new DateTime(newTime);
		c.set(DateTime.ZONE_OFFSET, newTimeCalendar.get(DateTime.ZONE_OFFSET));
		c.set(DateTime.DST_OFFSET, newTimeCalendar.get(DateTime.DST_OFFSET));
		c.set(DateTime.HOUR_OF_DAY, newTimeCalendar.Hour);
		c.set(DateTime.MINUTE, newTimeCalendar.Minute);
		c.set(DateTime.SECOND, newTimeCalendar.Second);
		c.set(DateTime.MILLISECOND, newTimeCalendar.Millisecond);
		return c;
	  }

	  public static DateTime addDays(DateTime date, int amount)
	  {
		DateTime c = new DateTime();
		c = new DateTime(date);
		c.AddDays(amount);
		return c;
	  }
	}

}
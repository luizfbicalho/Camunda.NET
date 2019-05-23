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
	using DateTimeUtils = org.joda.time.DateTimeUtils;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ClockUtil
	{

	  /// <summary>
	  /// Freezes the clock to a specified Date that will be returned by
	  /// <seealso cref="now()"/> and <seealso cref="getCurrentTime()"/>
	  /// </summary>
	  /// <param name="currentTime">
	  ///          the Date to freeze the clock at </param>
	  public static DateTime CurrentTime
	  {
		  set
		  {
			DateTimeUtils.CurrentMillisFixed = value.Ticks;
		  }
		  get
		  {
			return now();
		  }
	  }

	  public static void reset()
	  {
		resetClock();
	  }


	  public static DateTime now()
	  {
		return new DateTime(DateTimeUtils.currentTimeMillis());
	  }

	  /// <summary>
	  /// Moves the clock by the given offset and keeps it running from that point
	  /// on.
	  /// </summary>
	  /// <param name="offsetInMillis">
	  ///          the offset to move the clock by </param>
	  /// <returns> the new 'now' </returns>
	  public static DateTime offset(long? offsetInMillis)
	  {
		DateTimeUtils.CurrentMillisOffset = offsetInMillis;
		return new DateTime(DateTimeUtils.currentTimeMillis());
	  }

	  public static DateTime resetClock()
	  {
		DateTimeUtils.setCurrentMillisSystem();
		return new DateTime(DateTimeUtils.currentTimeMillis());
	  }

	}

}
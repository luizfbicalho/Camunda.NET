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
namespace org.camunda.bpm.engine.test.util
{

	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	public sealed class ClockTestUtil
	{

	  /// <summary>
	  /// Increments the current time by the given seconds.
	  /// </summary>
	  /// <param name="seconds"> the seconds to add to the clock </param>
	  /// <returns> the new current time </returns>
	  public static DateTime incrementClock(long seconds)
	  {
		long time = ClockUtil.CurrentTime.Ticks;
		ClockUtil.CurrentTime = new DateTime(time + seconds * 1000);
		return ClockUtil.CurrentTime;
	  }

	  /// <summary>
	  /// Sets the clock to a date without milliseconds. Older mysql
	  /// versions do not support milliseconds. Test which test timestamp
	  /// should avoid timestamps with milliseconds.
	  /// </summary>
	  /// <returns> the new current time </returns>
	  public static DateTime setClockToDateWithoutMilliseconds()
	  {
		ClockUtil.CurrentTime = new DateTime(1363608000000L);
		return ClockUtil.CurrentTime;
	  }

	}

}
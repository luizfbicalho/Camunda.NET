using System;
using System.Threading;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	using AfterClass = org.junit.AfterClass;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	public class ClockUtilTest
	{

	  private const long ONE_SECOND = 1000L;
	  private const long TWO_SECONDS = 2000L;
	  private const long FIVE_SECONDS = 5000L;
	  private const long TWO_DAYS = 172800000L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void resetClock()
	  public static void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void nowShouldReturnCurrentTime()
	  public virtual void nowShouldReturnCurrentTime()
	  {
		assertThat(ClockUtil.now()).isCloseTo(DateTime.Now, ONE_SECOND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getCurrentTimeShouldReturnSameValueAsNow()
	  public virtual void getCurrentTimeShouldReturnSameValueAsNow()
	  {
		assertThat(ClockUtil.CurrentTime).isCloseTo(ClockUtil.now(), ONE_SECOND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void offsetShouldTravelInTime() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void offsetShouldTravelInTime()
	  {
		long duration = TWO_DAYS;
		DateTime target = new DateTime((DateTime.Now).Ticks + duration);

		ClockUtil.offset(duration);

		Thread.Sleep(1100L);

		assertThat(ClockUtil.now()).isCloseTo(target, TWO_SECONDS);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setCurrentTimeShouldFreezeTime() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setCurrentTimeShouldFreezeTime()
	  {
		long duration = TWO_DAYS;
		DateTime target = new DateTime((DateTime.Now).Ticks + duration);

		ClockUtil.CurrentTime = target;

		Thread.Sleep(1100L);

		assertThat(ClockUtil.now()).isCloseTo(target, ONE_SECOND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resetClockShouldResetToCurrentTime()
	  public virtual void resetClockShouldResetToCurrentTime()
	  {
		long duration = TWO_DAYS;
		DateTime target = new DateTime((DateTime.Now).Ticks + duration);

		ClockUtil.offset(duration);

		assertThat(ClockUtil.now()).isCloseTo(target, ONE_SECOND);

		assertThat(ClockUtil.resetClock()).isCloseTo(DateTime.Now, ONE_SECOND);
		assertThat(ClockUtil.CurrentTime).isCloseTo(DateTime.Now, ONE_SECOND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void resetShouldResetToCurrentTime()
	  public virtual void resetShouldResetToCurrentTime()
	  {
		long duration = TWO_DAYS;
		DateTime target = new DateTime((DateTime.Now).Ticks + duration);

		ClockUtil.offset(duration);

		assertThat(ClockUtil.now()).isCloseTo(target, ONE_SECOND);

		ClockUtil.reset();

		assertThat(ClockUtil.now()).isCloseTo(DateTime.Now, ONE_SECOND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void timeShouldMoveOnAfterTravel() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void timeShouldMoveOnAfterTravel()
	  {
		DateTime now = DateTime.Now;
		long duration = TWO_DAYS;
		DateTime target = new DateTime(now.Ticks + duration);

		ClockUtil.offset(duration);

		assertThat(ClockUtil.now()).isCloseTo(target, ONE_SECOND);

		Thread.Sleep(FIVE_SECONDS);

		assertThat(ClockUtil.now()).isCloseTo(new DateTime(target.Ticks + FIVE_SECONDS), ONE_SECOND);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void timeShouldFreezeWithSetCurrentTime() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void timeShouldFreezeWithSetCurrentTime()
	  {
		DateTime now = DateTime.Now;
		long duration = TWO_DAYS;
		DateTime target = new DateTime(now.Ticks + duration);
		ClockUtil.CurrentTime = target;

		Thread.Sleep(FIVE_SECONDS);

		assertThat(ClockUtil.now()).isCloseTo(target, ONE_SECOND);
	  }
	}

}
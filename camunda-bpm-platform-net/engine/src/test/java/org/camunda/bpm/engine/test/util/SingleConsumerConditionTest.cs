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
namespace org.camunda.bpm.engine.test.util
{

	using SingleConsumerCondition = org.camunda.bpm.engine.impl.util.SingleConsumerCondition;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	public class SingleConsumerConditionTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(timeout=10000) public void shouldNotBlockIfSignalAvailable()
	  public virtual void shouldNotBlockIfSignalAvailable()
	  {
		SingleConsumerCondition condition = new SingleConsumerCondition(Thread.CurrentThread);

		// given
		condition.signal();

		// then
		condition.await(100000);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(timeout=10000) public void shouldNotBlockIfSignalAvailableDifferentThread() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void shouldNotBlockIfSignalAvailableDifferentThread()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.util.SingleConsumerCondition condition = new org.camunda.bpm.engine.impl.util.SingleConsumerCondition(Thread.currentThread());
		SingleConsumerCondition condition = new SingleConsumerCondition(Thread.CurrentThread);

		Thread consumer = new Thread(() =>
		{
	condition.signal();
		});

		consumer.Start();
		consumer.Join();

		// then
		condition.await(100000);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotAwaitFromDifferentThread()
	  public virtual void cannotAwaitFromDifferentThread()
	  {
		// given
		SingleConsumerCondition condition = new SingleConsumerCondition(new Thread());

		// when then
		try
		{
		  condition.await(0);
		  Assert.fail("expected exception");
		}
		catch (Exception)
		{
		  // expected
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotCreateWithNull()
	  public virtual void cannotCreateWithNull()
	  {
		try
		{
		  new SingleConsumerCondition(null);
		  Assert.fail("expected exception");
		}
		catch (System.ArgumentException)
		{
		  // expected
		}
	  }

	}

}
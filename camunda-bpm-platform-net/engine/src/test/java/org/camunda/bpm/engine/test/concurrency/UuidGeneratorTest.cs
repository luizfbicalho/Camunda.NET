using System.Collections.Generic;
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
namespace org.camunda.bpm.engine.test.concurrency
{

	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	using EthernetAddress = com.fasterxml.uuid.EthernetAddress;
	using Generators = com.fasterxml.uuid.Generators;
	using TimeBasedGenerator = com.fasterxml.uuid.impl.TimeBasedGenerator;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UuidGeneratorTest
	{

	  private const int THREAD_COUNT = 10;
	  private const int LOOP_COUNT = 10000;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultithreaded() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testMultithreaded()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Thread> threads = new java.util.ArrayList<Thread>();
		IList<Thread> threads = new List<Thread>();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.fasterxml.uuid.impl.TimeBasedGenerator timeBasedGenerator = com.fasterxml.uuid.Generators.timeBasedGenerator(com.fasterxml.uuid.EthernetAddress.fromInterface());
		TimeBasedGenerator timeBasedGenerator = Generators.timeBasedGenerator(EthernetAddress.fromInterface());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.ConcurrentSkipListSet<String> generatedIds = new java.util.concurrent.ConcurrentSkipListSet<String>();
		ConcurrentSkipListSet<string> generatedIds = new ConcurrentSkipListSet<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.concurrent.ConcurrentSkipListSet<String> duplicatedIds = new java.util.concurrent.ConcurrentSkipListSet<String>();
		ConcurrentSkipListSet<string> duplicatedIds = new ConcurrentSkipListSet<string>();

		for (int i = 0; i < THREAD_COUNT; i++)
		{
		  Thread thread = new Thread(() =>
		  {
	  for (int j = 0; j < LOOP_COUNT; j++)
	  {

		string id = timeBasedGenerator.generate().ToString();
		bool wasAdded = generatedIds.add(id);
		if (!wasAdded)
		{
		  duplicatedIds.add(id);
		}
	  }
		  });
		  threads.Add(thread);
		  thread.Start();
		}

		foreach (Thread thread in threads)
		{
		  thread.Join();
		}

		Assert.assertEquals(THREAD_COUNT * LOOP_COUNT, generatedIds.size());
		Assert.assertTrue(duplicatedIds.Empty);
	  }
	}

}
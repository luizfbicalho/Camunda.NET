using System;
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
namespace org.camunda.bpm.engine.test.api.cfg
{

	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;


	public class IdGeneratorDataSourceTest : ResourceProcessEngineTestCase
	{

	  public IdGeneratorDataSourceTest() : base("org/camunda/bpm/engine/test/api/cfg/IdGeneratorDataSourceTest.camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIdGeneratorDataSource()
	  public virtual void testIdGeneratorDataSource()
	  {
		IList<Thread> threads = new List<Thread>();
		for (int i = 0; i < 20; i++)
		{
		  Thread thread = new Thread(() =>
		  {
	  for (int j = 0; j < 5; j++)
	  {
		runtimeService.startProcessInstanceByKey("idGeneratorDataSource");
	  }
		  });
		  thread.Start();
		  threads.Add(thread);
		}

		foreach (Thread thread in threads)
		{
		  try
		  {
			thread.Join();
		  }
		  catch (InterruptedException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }
		}
	  }
	}

}
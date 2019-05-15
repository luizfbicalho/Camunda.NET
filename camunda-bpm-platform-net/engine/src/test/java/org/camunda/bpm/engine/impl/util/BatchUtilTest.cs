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
namespace org.camunda.bpm.engine.impl.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using Mockito = org.mockito.Mockito;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	public class BatchUtilTest
	{

	  private ProcessEngineConfigurationImpl engineConfiguration;
	  private BatchConfiguration batchConfiguration;
	  private IList<string> ids;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Before public void setUp()
	  public virtual void setUp()
	  {
		batchConfiguration = Mockito.mock(typeof(BatchConfiguration));
		engineConfiguration = Mockito.mock(typeof(ProcessEngineConfigurationImpl));
		ids = Mockito.mock(typeof(System.Collections.IList));
		when(batchConfiguration.Ids).thenReturn(ids);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnCorrectSizeUneven()
	  public virtual void shouldReturnCorrectSizeUneven()
	  {
		when(ids.Count).thenReturn(5);
		when(engineConfiguration.InvocationsPerBatchJob).thenReturn(2);
		testConfiguration(engineConfiguration, batchConfiguration, 3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnCorrectSizeZeroBatchSize()
	  public virtual void shouldReturnCorrectSizeZeroBatchSize()
	  {
		when(ids.Count).thenReturn(2);
		when(engineConfiguration.InvocationsPerBatchJob).thenReturn(0);
		testConfiguration(engineConfiguration, batchConfiguration, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnCorrectSizeEven()
	  public virtual void shouldReturnCorrectSizeEven()
	  {
		when(ids.Count).thenReturn(4);
		when(engineConfiguration.InvocationsPerBatchJob).thenReturn(2);
		testConfiguration(engineConfiguration, batchConfiguration, 2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnCorrectSizeZeroInstances()
	  public virtual void shouldReturnCorrectSizeZeroInstances()
	  {
		when(ids.Count).thenReturn(0);
		when(engineConfiguration.InvocationsPerBatchJob).thenReturn(2);
		testConfiguration(engineConfiguration, batchConfiguration, 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnCorrectSizeZeroInstancesZeroBatchSize()
	  public virtual void shouldReturnCorrectSizeZeroInstancesZeroBatchSize()
	  {
		when(ids.Count).thenReturn(0);
		when(engineConfiguration.InvocationsPerBatchJob).thenReturn(0);
		testConfiguration(engineConfiguration, batchConfiguration, 0);
	  }

	  private void testConfiguration(ProcessEngineConfigurationImpl engineConfiguration, BatchConfiguration batchConfiguration, int expectedResult)
	  {
		assertEquals(expectedResult, BatchUtil.calculateBatchSize(engineConfiguration, batchConfiguration));
	  }
	}

}
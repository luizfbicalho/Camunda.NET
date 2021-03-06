﻿/*
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
namespace org.camunda.bpm.engine.cdi.test.impl.context
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;

	using ProcessScopedMessageBean = org.camunda.bpm.engine.cdi.test.impl.beans.ProcessScopedMessageBean;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Test = org.junit.Test;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class ThreadContextAssociationTest : CdiProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testBusinessProcessScopedWithJobExecutor() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testBusinessProcessScopedWithJobExecutor()
	  {
		string pid = runtimeService.startProcessInstanceByKey("processkey").Id;

		waitForJobExecutorToProcessAllJobs(5000l, 25l);

		assertNull(managementService.createJobQuery().singleResult());

		ProcessScopedMessageBean messageBean = (ProcessScopedMessageBean) runtimeService.getVariable(pid, "processScopedMessageBean");
		assertEquals("test", messageBean.Message);

		runtimeService.signal(pid);

	  }

	}

}
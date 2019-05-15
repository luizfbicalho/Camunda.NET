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
namespace org.camunda.bpm.engine.test.api.authorization.externaltask
{
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using Assert = org.junit.Assert;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ExtendLockOnExternalTaskAuthorizationTest extends HandleExternalTaskAuthorizationTest
	public class ExtendLockOnExternalTaskAuthorizationTest : HandleExternalTaskAuthorizationTest
	{
		public override void testExternalTaskApi(LockedExternalTask task)
		{
		engineRule.ExternalTaskService.extendLock(task.Id, "workerId", 2000L);
		}

	  public override void assertExternalTaskResults()
	  {
		ExternalTask taskWithExtendedLock = engineRule.ExternalTaskService.createExternalTaskQuery().locked().singleResult();
		Assert.assertNotNull(taskWithExtendedLock);
	  }
	}

}